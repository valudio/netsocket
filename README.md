# netsocket
Small library to easily deal with websockets using .NET Standard 1.5 or .NET 4.6.2

## How can I consume this?
For the .NET 4.6.2 version you can use the `Nuget Package`:

```
# .NET Core
PM> Install-Package Valudio.NetSocket.NET 
# .NET Standard
PM> Install-Package Valudio.NetSocket.NETStandard
```

## How does it work?
This library comes with a `middleware` that you can use in your `Startup.cs` file:

```cs

public void ConfigureServices(IServiceCollection services)
{
    services.AddRealTimeComm(); // this will inject all the services
}

public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
{
    loggerFactory.AddConsole();
    app.UseRealTimeComm(); // this will start listening (Note that you can pass the ports your application will be listening here)
    if (env.IsDevelopment()) app.UseDeveloperExceptionPage();
}
```

Once you have set this up the next step is to **create your own classes extending `SocketServiceBase`** . You will have 4 methods to override:

```cs 
// this will be executed whenever a new websocket is open.
public virtual void OnClientInitialized(IClient client) { }
// this will be executed whenever the server receives some message.
public virtual void OnMessageReceived(IClient fromClient, string message) { }
// this will be executed whenever the server sents any message to some client
public virtual void OnMessageSent(IClient toClient, string message, IClient fromClient) { }
// this will be executed whenever a websocket is closed.
public virtual void OnClientClosed(IClient client) { }
```

Besides those `virtual methods` you've got some other methods that will be useful to **`send` messages**:

```cs
// just like the method's name says... sends a message to everyone.
Task SendAllClientsAsync(string message, IClient fromClient = null);
// sends a message to all the clients except the one you pass as a parameter.
Task SendAllClientsExceptAsync(IClient except, string message, IClient fromClient = null);
// sends a message to all the clients except the ones you pass as a parameter.
Task SendAllClientsExceptAsync(IEnumerable<IClient> except, string message, IClient fromClient = null);
// sends a message to a specific client.
Task SendClientAsync(IClient toClient, string message, IClient fromClient = null);
```

## Enabling / Disabling SocketServices
Let's say you want to not use one of your `SocketServices` (your classes extending `SocketServiceBase`). You've got the option to mark them with an attribute `SocketServiceAttribute`:

```cs
// setting this attribute to false will make this service not to be used by the middleware.
// this attribute is only necessary to deactivate the service. If not present
[SocketService(false)]
public class HelloTimerService : SocketServiceBase
{
    public HelloTimerService(ISocketManager manager) : base(manager)
    {
        var timer = new System.Threading.Timer(async (o) =>
        {
            await SendAllClientsAsync($"Hello at {DateTime.Now.ToUniversalTime()}");
        }, null, 0, 30000);
        
    }
}
```

## Sending additional parameters to the server

If you want to send additional custom data through the socket connection you can add it to the url as a query string

```js
var url = "ws://domain.com/ws?param1=value&param2=value";
var socket = new WebSocket(url);
```

You will receive that data in the property `AdditionalParameters` as a `Dictionary<string, StringValues>` in the `IClient` passed as a parameter to the `virtual methods` of your `SocketServices`

## How to test this
Run one of the example projects and then use [Simple Web Socket Client for Chrome](https://chrome.google.com/webstore/detail/simple-websocket-client/pfdhoblngboilpfeibdedpjgfnlcodoo), for instance, in order to setup a websocket connection.

Alternatively, if you don't want to run the examples, you can use this url [ws://examplenetsocketnetcore.azurewebsites.net](ws://examplenetsocketnetcore.azurewebsites.net). The website also has a [simple form to test it](http://examplenetsocketnetcore.azurewebsites.net/)

Bear in mind this is a **F1 free Azure Machine** so it will have to start-up first. Stay calm if first request takes longer than it should. ;D
