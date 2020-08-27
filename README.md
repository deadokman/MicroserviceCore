# Microservice Core (MSC) .NET Core library for building microservices

This set of the libraries will help you to organize your microservice architecture in most efficient way. 
This project is aimed at placing microservices inside docker's containers, but you also can use it inside self-hosted console applications.

##### Base information
* [Microservice Core](#microservicecore)
* [Lifecycle events](#lifecycle)
* [Default configurations](#configurations)
* [Layers concept](#layersconcept)
* [Runnable layers](#runnablelayers)
##### Predefined layers
* (WIP) RabbitMq layer 
* (WIP) Redis layer 
* (WIP) Postgres layer 
* (WIP) Grpc Server layer 
* (WIP) Grpc client layer 

#### <a name="microservicecore"></a> Microservice Core (MSC)

*MicroserviceCore.cs* - is the main class of all over library. It is provide microservice lifecycle events and layer extension mechanisms. 

Inside this class operates with a default .NetCore DI container, Microsoft configurations extensions and layering system,  all this together provides you arhitecture, that you can use to create and easily extend your microservices.

Note: *At this point, MSC uses NLog as default logger. But this may change in future, to give you possebility to use you favourite logger; Add nlog.config from ./Shared/ folder, to your project file and set "Copy Always" option in file properties => Advanced => Copy to output Directory;*

To create your first microservice, create new .NET Core console application. And then paste this code inside your *Programm.cs* file, Programm class:

```csharp

public static MicroserviceCore Microservice = new MicroserviceCore();
public static void Main()
{
    Microservice.DoWork += Microservice_OnDoWork;
}

private static void Microservice_OnDoWork(IServiceProvider serviceprovider, CancellationToken stopservicetoken)
{
    stopservicetoken.WaitHandle.WaitOne();
}
   
```

Thats it, you can start your application... And it won't do anything. Read next article in order to make you microservice more functional.

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

3) *DoWork* (required) - this is main start service event, it is provide you IServiceProvider interface (which is the builded DI contrainer) and CancellationToken for your service. Inside this event delegate you can retrive your custom classes and start some sync or async process.
   
#### <a name="layersconcept"></a> Layers concept
  
#### <a name="configurations"></a> Default configurations

#### <a name="runnablelayers"></a> Runnable layers