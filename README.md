# Microservice Core (MSC) .NET Core library for building microservices

This set of the libraries will help you to organize your microservice architecture in most efficient way. 
This project is aimed at placing microservices inside docker's containers, but you also can use it inside self-hosted console applications.

##### Base information
* [Microservice Core](#microservicecore)
* [Lifecycle events](#lifecycle)
* [Default configurations](#configurations)
* [Layers](#layersconcept)
* [Runnable layers](#runnablelayers)
##### Predefined layers
* (WIP) RabbitMq layer 
* (WIP) Redis layer 
* (WIP) Postgres layer 
* (WIP) Grpc Server layer 
* (WIP) Grpc client layer 
##### Dealing with the Docker

* (WIP) Cooking container with MSC

#### <a name="microservicecore"></a> Microservice Core (MSC)

*Msc.Microservice.Core.Standalone* - is the main library of architecture. It is provide microservice lifecycle events and layer extension mechanisms. 

Too install library as Nuget package use command:

    dotnet add package Msc.Microservice.Core.Standalone

This library operates with a default .NetCore DI container (may change in a future), Microsoft configurations extensions and custom layering system, all this together provides you arhitecture, that you can use to create and easily extend your microservices.

Note: *At this point, MSC uses NLog as default logger. But this subject may change in future, to give you possebility to use you favourite logger; Add nlog.config from ./Shared/ folder, to your project file and set "Copy Always" option in file properties => Advanced => Copy to output Directory;*

To create your first microservice, create new .NET Core console application. And then paste this code inside your *Programm.cs* file, Programm class:

```csharp

public static MicroserviceCore Microservice = new MicroserviceCore();
public static void Main()
{
    Microservice.DoWork += Microservice_OnDoWork;
}

private static void Microservice_OnDoWork(IServiceProvider serviceprovider, CancellationToken stopservicetoken)
{
    // This wait token holds main service thread
    stopservicetoken.WaitHandle.WaitOne();
}
   
```

Thats it, you can start your application... And it won't do anything :). 

Read next article in order to make you microservice more functional.

#### <a name="lifecycle"></a> Lifecycle events

When you microservice starts, shutdowns and/or throws unhandled exception, the MSC fires it's life cycle evets that you can use to perfome some operations or too extend your application. **Required to subscribe is the DoWork event**. Others events subscription are optional.


All events except unhandled exceptions are raised in a specific order:

**When you service starts** fires next events:

1) *RegisterConfigurations* (optional) - inside this event handler delegate you can register you own configuration files:
```csharp
// Event subscription inside Main() method
Microservice.RegisterConfigurations += Microservice_OnRegisterConfigurations;
// ...
// Event delegate inside Program class.
private static void Microservice_OnRegisterConfigurations(object? sender, IConfigurationBuilder builder)
{
    // Add syou custom configuration file from Microsoft.Extensions.Configuration.Json or anoter default package
    builder.AddJsonFile("./MyConfig.json");

    // Or register your own configuration source
    builder.Add(new CustomConfigurationSource());
}
```

2) *PrepareExecution* (optional) - this event provides you an IServiceCollection instance (which is the implementation of the dependency injection container) and instance of class, that implemets IConfigurationRoot. 
   Note: *At this point, MSC uses .Net core default DI contriner, but it will be optional in future*.
   Inside this event handler delagete you can register your own classes inside DI container (IServiceCollection) and get access to you configuration classes from IConfigurationRoot which is the part of the Microsoft.Configuration namespace.

```csharp
// Event subscription inside Main() method
Microservice.PrepareExecution += Microservice_OnPrepareExecution;
 
 //...

// Event delegate inside Program class.
private static void Microservice_OnPrepareExecution(IServiceCollection servicecollection, IConfigurationRoot configurationRoot)
{
    servicecollection.AddTransient<ISomeInterface, SomeRealization>();
    var someCustomConfig = configuration.Get<SomeConfigClass>();

    // Add IOptions<> from your config file or default config
    sc.Configure<SomeConfigClass>(configuration.GetSection(nameof(SomeConfigClass)));

    servicecollection.AddTransient<ISomeInterface2>((serviceProvider) => new SomeClass(someCustomConfig));
}

```

   3) *DoWork* (subscription is required) - this is main service event, it is provide you IServiceProvider interface (which is the builded DI contrainer) and CancellationToken for your service. Inside this event delegate you can retrive your custom classes and start some sync or async process.

```csharp
// Event subscription inside Main() method
Microservice.DoWork += MicroserviceCoreOnDoWork;
 
 //...

// Event delegate inside Program class.
        /// <summary>
        /// Работа, выполняемая службой
        /// </summary>
        /// <param name="serviceprovider">Провайдер сервисов</param>
        /// <param name="stopservicetoken">Токен ожидания останова службы</param>
        private static void MicroserviceCoreOnDoWork(IServiceProvider serviceprovider, CancellationToken stopservicetoken)
        {
            _logger = serviceprovider.GetService<ILogger<IWorkflowHost>>();
            _eventNotificator = serviceprovider.GetService<IEventNotificator>();
            var workflowHost = serviceprovider.GetService<IWorkflowHost>();
            _wfHost = workflowHost;
            _wfHost.Start();
            workflowHost.OnStepError += WorkflowHost_OnOnStepError;

            // This wait token holds main service thread
            stopservicetoken.WaitHandle.WaitOne();
        }
 ```  

*GotUnhandledException* (optional) - this lifecycle event rises when unhandled application exception occurs. You can handle this error and peform user data storing to prevent information loss.

*PrepareShutDown* (optional) - this lifecycle event rises when your microservice is going to shutdown. You can clear unmanagment resources inside this event handler delegate.
  
#### <a name="configurations"></a> Default configurations

The most important thing of any software is configuration. This library supports configuration with environment variables and configuration files. 

1) Initilalize you MicroserviceCore class like listed below:
```csharp
MicroserviceCore MicroserviceCore = new MicroserviceCore("appsettings", "environment");
 ```  
The first argument here is the settings file name, the second argument specifies the name of the environment variable; the short will contain the postfix of the name of the configuration file. Using this two arguments you can have multiple configurations files inside your microservice project, and you can switch them when running inside  docker or any other system;

2) After that, add *appsettings.development.json* file to your project, and set "Copy always" inside file propeties in "Copy to output folder" option.

The last thing you need to configure is the Environment Variables of you .csproj file. 

3) Open your project file properties, go to Debug tab and add new environment variable in a list, set environment variable name as "environment" and set value as "development", thats it! If you did everything right, configuration file will be automatically attached to service runtime, after you call *MicroserviceCore.Run()* method.


After that, all of your classes can import IOption<> interface inside thier constructor, to get access to configuration file sections.

You can learn more about this pattern by reading this article.

https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/options?view=aspnetcore-3.1


#### <a name="layersconcept"></a> Layers

Layers is the architectural concept of this library. They provides you possebility to quickly integrate some common fuctionalyti to your microservice. MSC have some predefined layers and you can write your own.

You class should implement *IMicroserviceLayer* interface, if you want use it as a layer; After that you can add layer to your microservice like listed below:

```csharp
_microserviceCore.AddLayer(new SomeCustomLayer());
```

This project provides you a bunch of default layers that you can use in your application.

*IMicroserviceLayer* declares two methods:

The first one is the:

    void RegisterConfiguration(IConfigurationBuilder configurationRoot);

Inside implementation of this method, you can register your custom configurations that assosiated with this particular layer. 

And the second:

    IEnumerable<string> RegisterLayer(IConfigurationRoot configurationRoot, IServiceCollection serviceCollection);

Inside implementation of this method you can instantiate, register and configure your custom classes, that should be shared between multiple different microservices (for example DAC layer providers, object erpositories, factories, etc);

This two methods will be called automatically after you run your microservice;

#### <a name="runnablelayers"></a> Runnable layers

The runnable layers are a variation on regular layers. IRunnableLayer interface inherits IMicroserviceLayer and adds two addtitional method declaration.

The first one:

    void RunAsync(IServiceProvider serviceProvider);

This method will be called right before your OnDoWork message handler will be triggered. All operations within this method must be asynchronous, inside it you can run your buisness logic porcess, timers etc...

The second methos is:

    void Shutdown(IServiceProvider serviceProvider);

Inside this method you should stop you buisness login processes and clear your unmanaged resources. This method will be called right before OnShutDown microservice lifecycle event.

