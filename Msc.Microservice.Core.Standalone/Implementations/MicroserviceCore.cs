// -------------------------------------------------------------------------------------------------------------------
// <copyright file="MicroserviceCore.cs" company="Konyshev A.V., Stepanov N.O.">
//   Copyright (c) Konyshev A.V., Stepanov N.O.
// </copyright>
// <summary>
//   Внутренний поток сервиса в котором выполняется основная работа.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Msc.Microservice.Core.Standalone.Exceptions;
using Msc.Microservice.Core.Standalone.Interfaces;
using NLog;
using NLog.Extensions.Logging;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace Msc.Microservice.Core.Standalone.Implementations
{
    /// <summary>
    /// Ядро микросервиса.
    /// </summary>
    public class MicroserviceCore : IMicroserviceCore
    {
        /// <summary>
        /// Внутренний поток сервиса в котором выполняется основная работа.
        /// </summary>
        protected Thread ServiceBackgroundThread;

        /// <summary>
        /// Получить индентификатор слубы.
        /// </summary>
        protected string ServiceIdentifier => $"[SERVICE ID [{ServiceName}] - NAME [{ServiceName}]]";

        private readonly string _configFileName;
        private readonly string _runtimeEnvVariableName;

        /// <summary>
        /// Логгер.
        /// </summary>
        private ILogger<IMicroserviceCore> _logger;

        /// <summary>
        /// Подключение и настройка логгера.
        /// </summary>
        private ILoggerFactory _loggerFactory;

        /// <summary>
        /// Защелка, удерживающая основной поток службы.
        /// </summary>
        private ManualResetEventSlim _event = new ManualResetEventSlim(false);

        /// <summary>
        /// Фабрика токенов.
        /// </summary>
        private CancellationTokenSource _cts;

        /// <summary>
        /// Слои.
        /// </summary>
        private List<IMicroserviceLayer> _layers;

        /// <summary>
        /// Слои, в которых могут быть запущены асинхронные задачи.
        /// </summary>
        private List<IRunnableLayer> _runnableLayers;

        /// <summary>
        /// Microservice configuration root.
        /// </summary>
        private IConfigurationRoot _configurationRoot;

        /// <summary>
        /// Построитель конфигурации.
        /// </summary>
        private IConfigurationBuilder _configurationBuilder;

        private List<Thread> _runnableLayerTasks = new List<Thread>();

        /// <summary>
        /// Идентификатор служы.
        /// </summary>
        public string ServiceId { get; private set; }

        /// <summary>
        /// Имя службы.
        /// </summary>
        public string ServiceName { get; private set; }

        /// <summary>
        /// Di Container
        /// </summary>
        private IServiceCollection _serviceCollection;

        private IServiceProvider _serviceProvider;

        /// <summary>
        /// Конструктор класса по умолчанию.
        /// </summary>
        public MicroserviceCore()
            : this(string.Empty)
        {
            AppDomain.CurrentDomain.ProcessExit += CurrentDomain_OnProcessExit;
        }

        private void CurrentDomain_OnProcessExit(object sender, EventArgs e)
        {
            StopRunnableLayers();
        }

        /// <summary>
        /// Конструктор класса с использованием конфигурации по умолчанию.
        /// </summary>
        /// <param name="configFileName">Имя конфигурационного файла.</param>
        /// <param name="runtimeEnvVariableName">Имя переменной окружения.</param>
        public MicroserviceCore(string configFileName, string runtimeEnvVariableName = null)
            : this(string.Empty, configFileName, runtimeEnvVariableName)
        {
        }

        /// <summary>
        /// Создание службы с заданным именем.
        /// </summary>
        /// <param name="serviceName">Имя службы.</param>
        /// <param name="configFileName">Имя конфигурационного файла.</param>
        /// <param name="runtimeEnvVariableName">Имя переменной окружения для определения типа рантайма приложения.</param>
        public MicroserviceCore(string serviceName, string configFileName, string runtimeEnvVariableName)
        {
            _serviceCollection = new ServiceCollection();
            _configFileName = configFileName;
            _runtimeEnvVariableName = runtimeEnvVariableName;
            ServiceId = Guid.NewGuid().ToString();
            if (string.IsNullOrEmpty(serviceName))
            {
                ServiceName = $"Service - {ServiceId}";
            }

            _runnableLayers = new List<IRunnableLayer>();
            BasicInitialize(ServiceName);
        }

        /// <summary>
        /// Остановить работу службы.
        /// </summary>
        public void Stop()
        {
            try
            {
                if (_serviceProvider == null)
                {
                    _logger.LogWarning($"Сделана попытка остановить службу которая не была запущена, или была запущена с ошибками: {ServiceIdentifier}");
                    return;
                }

                _logger.LogInformation($"Запрошена остановка службы: {ServiceIdentifier}");
                PrepareShutdown?.Invoke(_serviceProvider);
                _cts.Cancel();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ошибка во время останова службы", ex);
                throw ex;
            }
            finally
            {
                _event.Set();
            }
        }

        /// <summary>
        /// ЗАпустить службу асинхронно.
        /// </summary>
        /// <returns>Асинхронный запуск службы.</returns>
        public Task RunAsync()
        {
            return Task.Factory.StartNew(() => Run());
        }

        /// <summary>
        /// Запустить службу на выполнение.
        /// </summary>
        public void Run()
        {
            var ct = _cts.Token;

            if (DoWork == null)
            {
                throw new MicroserviceStartupException($"Запуск {ServiceIdentifier} невозможен т.к. нет подписки на событие запуска процесса работы сервиса");
            }

            _serviceCollection.AddOptions();
            var configurationJsonFile = "N/A";
            if (!string.IsNullOrEmpty(_configFileName))
            {
                var cfgFile = _configFileName ?? "appsettings";
                Configuration(cfgFile);
            }

            void Configuration(string configFile)
            {
                _configurationBuilder.AddJsonFile($"{configFile}.json");
                var configName = Environment.GetEnvironmentVariable(_runtimeEnvVariableName);
                if (!string.IsNullOrEmpty(_runtimeEnvVariableName) && !string.IsNullOrWhiteSpace(configName))
                {
                    _configurationBuilder.AddJsonFile($"{_configFileName}.{configName}.json");
                }
            }

            _logger.LogInformation($"Выполняется запуск              {ServiceIdentifier}");
            _logger.LogInformation($"Слоев:                          [{_layers?.Count ?? 0}]");
            _logger.LogInformation($"Исполяемых слоев:               [{_runnableLayers?.Count ?? 0}]");
            _logger.LogInformation($"JSON конфигурация по умолчанию: [{configurationJsonFile}]");
            _logger.LogDebug(">> Вызов внешних конфигураций");
            RegisterConfigurations?.Invoke(this, _configurationBuilder);
            _logger.LogDebug("<< DONE");
            var errorsList = new List<Exception>();

            // Произвести инициализацию всех слоев.
            if (_layers != null)
            {
                foreach (var layer in _layers)
                {
                    _logger.LogDebug($">> Конфигурации слоя [{layer}]");
                    try
                    {
                        layer.RegisterConfiguration(_configurationBuilder);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Ошибка во время добавления конфигурации слоя [{layer}]");
                        throw;
                    }

                    _logger.LogDebug("<< DONE");
                }

                _logger.LogDebug($">> Построение DI контейнера");
                _configurationRoot = _configurationBuilder.Build();
                _logger.LogDebug("<< DONE");
                foreach (var layer in _layers)
                {
                    _logger.LogDebug($">> Подключение и настройка слоя [{layer}]");
                    try
                    {
                        var errors = layer.RegisterLayer(_configurationRoot, _serviceCollection);

                        if (errors != null)
                        {
                            _logger.LogDebug($"<< Агрегация ошибок подключения слоя [{layer}]");
                            errorsList.AddRange(errors.Select(errm => new Exception($"Ошибка конфигурации объектов слоя {ServiceIdentifier}: [{layer}]: \n {errm}")));
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Необработанное исключение во время регистрации слоя [{layer}] {ServiceIdentifier}");
                    }
                }
            }
            else
            {
                RegisterConfigurations?.Invoke(this, _configurationBuilder);
                _configurationRoot = _configurationBuilder.Build();
            }

            if (errorsList.Any())
            {
                errorsList.ForEach(err => _logger.LogError(err.Message));
                throw new AggregateException(errorsList);
            }

            _logger.LogDebug($"Подготовка к запуску службы");
            PrepareExecution?.Invoke(_serviceCollection, _configurationRoot);
            _serviceProvider = _serviceCollection.BuildServiceProvider();
            var arguments = new DoWorkArguments()
            {
                CancellationToken = ct,
                ServiceProvider = _serviceProvider,
            };

            foreach (var runnableLayer in _runnableLayers)
            {
                try
                {
                    _logger.LogInformation($"Выполнение запускаемого слоя [{runnableLayer}] в {ServiceIdentifier}");
                    var runnableThread = new Thread(() =>
                    {
                        runnableLayer.RunAsync(_serviceProvider);
                    });

                    runnableThread.Start();
                    _runnableLayerTasks.Add(runnableThread);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Ошибка во время запуска Runnable слоя [{runnableLayer}] {ServiceIdentifier}", ex);
                }
            }

            try
            {
                _logger.LogInformation($"Инициализация выполнена, запуск основного процесса службы");
                ServiceBackgroundThread.Start(arguments);
                _logger.LogInformation($"Основной процесс {ServiceIdentifier} запущен");
            }
            catch (Exception ex)
            {
                var errm = $"Ошибка во время запуска одного или нескольких запускаемых слоев \n {ServiceIdentifier} не может быть запущена";
                _logger.LogError(errm, ex);
                throw new Exception(errm, ex);
            }

            _logger.LogInformation($"Запуск службы выполнен");

            // Выполнить инициализацию слоев
            _event.Wait(ct);

            _logger.LogWarning($"Запрошен останов {ServiceIdentifier}");
            StopRunnableLayers();
        }

        /// <summary>
        /// Добавить слой.
        /// </summary>
        /// <param name="layer">Слой.</param>
        public void AddLayer(IMicroserviceLayer layer)
        {
            if (layer != null)
            {
                if (_layers == null)
                {
                    _layers = new List<IMicroserviceLayer>();
                }

                if (layer is IRunnableLayer runnableLayer)
                {
                    _runnableLayers.Add(runnableLayer);
                }

                _layers.Add(layer);
            }
        }

        /// <summary>
        /// Начать выполнение асинхронной работы
        /// </summary>
        public event DoWorkDelegate DoWork;

        /// <summary>
        /// Шаг конфигурирования
        /// </summary>
        public event PrepareExecutionDelegate PrepareExecution;

        /// <summary>
        /// Нотификация о необработанной исключительной ситуации
        /// </summary>
        public event EventHandler<UnhandledExceptionEventArgs> GotUnhandledException;

        /// <summary>
        /// Произвести действия перед остановкой службы
        /// </summary>
        public event PrepareShutdownDelegate PrepareShutdown;

        /// <summary>
        /// Выполнить регистрацию конфигураций
        /// </summary>
        public event EventHandler<IConfigurationBuilder> RegisterConfigurations;

        /// <summary>
        /// Метод выполняет базовую инициализаци службы
        /// в т.ч. добавление логгеров и подписку на необработанные исключения.
        /// </summary>
        /// <param name="serviceName">Имя службы.</param>
        protected void BasicInitialize(string serviceName)
        {
            // GlobalDiagnosticsContext.Set("service", _serviceName);
            GlobalDiagnosticsContext.Set("service", serviceName);
            _loggerFactory = new LoggerFactory();
            _loggerFactory.AddNLog(new NLogProviderOptions
            {
                CaptureMessageTemplates = true,
                CaptureMessageProperties = true,
            });

            _cts = new CancellationTokenSource();
            _logger = _loggerFactory.CreateLogger<IMicroserviceCore>();

            // Добавить построитель конфигурации
            _configurationBuilder = new ConfigurationBuilder();
            ServiceBackgroundThread = new Thread((arguments) =>
            {
                var args = arguments as DoWorkArguments;
                DoWork.Invoke(args.ServiceProvider, args.CancellationToken);
                _logger.LogWarning($"Поток службы {ServiceIdentifier} прекратил работу, выполнение службы остановлено.");
            });

            AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;
            _serviceCollection.AddSingleton((p) => _cts);
            _serviceCollection.AddSingleton(_loggerFactory);
            _serviceCollection.AddSingleton(typeof(ILogger<>), typeof(Logger<>));
            _serviceCollection.AddLogging(x => x.SetMinimumLevel(LogLevel.Trace));
        }

        private void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            _logger.LogCritical(e.ExceptionObject.ToString());
            if (e.IsTerminating)
            {
                _logger.LogCritical("Служба не может продолжать работу и будет остановлена");
            }

            StopRunnableLayers();

            if (GotUnhandledException != null)
            {
                GotUnhandledException.Invoke(sender, e);
            }
        }

        private void StopRunnableLayers()
        {
            foreach (var thread in _runnableLayerTasks)
            {
                thread.Abort();
            }

            foreach (var runnableLayer in _runnableLayers)
            {
                try
                {
                    runnableLayer.Shutdown(_serviceProvider);
                }
                catch (Exception e)
                {
                    _logger.LogError($"Ошибка во время останова работы выолняемого слоя [{runnableLayer}] в {ServiceIdentifier}", e);
                }
            }
        }
    }
}
