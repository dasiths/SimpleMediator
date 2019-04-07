# SimpleMediator [![Build status](https://ci.appveyor.com/api/projects/status/4wbdssddl5qxukk7?svg=true)](https://ci.appveyor.com/project/dasiths/simplemediator) [![NuGet](https://img.shields.io/nuget/v/SimpleMediator.svg)](https://www.nuget.org/packages/SimpleMediator)

A .NET/C# implementation of the mediator pattern with support for queries, commands and events. Has first class support for middleware and mediation contexts. 

---
## Motivation


My motivations are the following.

- Defined types (of `Messages`) for `Queries`, `Commands` and `Events`. Convery the intent clearly.
- Support for cancellation tokens all the way down to message handlers.
- Have a `MediationContext`. So when the message handler is called, the Message is **concise and lightweight**. Any context related information is captured in the MediationContext instead.
- Concept of `Middleware`, where each message goes through the pipeline and response come back through it.
- Have the ability to dispatch messages over the wire to a consumer (and get a response back). The ambition is **clearly not** to create a full framework that supports sending messages over the wire, but rather to make this library integrate with something like [MassTransit](http://masstransit-project.com/) with minimal effort.

You can find more details about the design at my blog https://dasith.me/2019/01/07/mediator-pattern-implemented-in-net/
## Using It

1. Define a message and response. A message can be a `ICommand`, `IQuery<ResultType>` or `IEvent`
    ```csharp
    public class SimpleQuery: IQuery<SimpleResponse>
    {
    }

    public class SimpleResponse
    {
        public string Message { get; set; }
    }
    ```

2. Define a message handler. A message handler can be `CommandHandler<CommandType>` for commands, `QueryHandler<QueryType, ResultType>` for queries and `EventHandler<EventType>` for events.
    ```csharp
    public class SimpleQueryHandler : QueryHandler<SimpleQuery, SimpleResponse>
    {
        protected override async Task<SimpleResponse> HandleQueryAsync(SimpleQuery query,
            IMediationContext mediationContext, CancellationToken cancellationToken)
        {
            Console.WriteLine("Test query executed");

            return new SimpleResponse()
            {
                Message = "Test query response message"
            };
        }
    }
    
    ```

3. Use it. 
    ```csharp
    public class Program
    {
        public static void Main(string[] args)
        {
            RunSample().ConfigureAwait(false).GetAwaiter().GetResult();
            Console.ReadLine();
        }

        public static async Task RunSample()
        {
            // You can use your favourite DI library to inject mediator and use it
            using (var container = MicrosoftDependencyContainerHelper.CreateServiceCollection())
            {
                var mediator = container.GetService<IMediator>();
                var simpleQuery = new SimpleQuery();

                var result = await mediator.HandleAsync(simpleQuery);
                Console.WriteLine(result.Message);
            }
        }
    }
    ```    
    - You can find more examples for the concept of middleware [here](https://github.com/dasiths/SimpleMediator/tree/master/Samples/SimpleMediator.Samples.ConsoleApp). 

    - I have examples of how to set it up with IOC containers [here](https://github.com/dasiths/SimpleMediator/tree/master/Samples/SimpleMediator.Samples.Shared/Helpers). `Autofac` and `Microsoft.Extensions.DependencyInjection` currently have examples but I'll keep adding more as I go. 
    - There is also some code samples (work in progress) on how to integrate it with `MassTransit` to dispatch messages over the wire to consumers. Check it out [here](https://github.com/dasiths/SimpleMediator/tree/master/Samples/SimpleMediator.Samples.MassTransit).

    As you can see, the usage is pretty straight forward and simple. The middleware gives you a lot of extensibility options and I've even been able to create constrained middleware that validates only certain types of requests.

## Contributing
The project code is shared under the MIT license. Feel free to have a look and create a PR if you think there are improvements. Please raise an issues if you find a bug. Thank you.
