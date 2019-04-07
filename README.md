# SimpleMediator [![Build status](https://ci.appveyor.com/api/projects/status/4wbdssddl5qxukk7?svg=true)](https://ci.appveyor.com/project/dasiths/simplemediator) [![NuGet](https://img.shields.io/nuget/v/SimpleMediator.svg)](https://www.nuget.org/packages/SimpleMediator)

A .NET/C# implementation of the mediator pattern with support for queries, commands and events. Has first class support for middleware and mediation contexts.

---
The mediator pattern or the "domain whisperer" as I like to call it has been around for a long time and the main selling point of it is the reduction of coupling and enforcing clear separation of the domain logic from UI/Top layers.

Let's look at the Wikipedia definition.

> Usually a program is made up of a large number of classes. Logic and computation are distributed among these classes. However, as more classes are added to a program, especially during maintenance and/or refactoring, the problem of communication between these classes may become more complex. This makes the program harder to read and maintain. Furthermore, it can become difficult to change the program, since any change may affect code in several other classes.
With the mediator pattern, communication between objects is encapsulated within a mediator object. Objects no longer communicate directly with each other, but instead communicate through the mediator. This reduces the dependencies between communicating objects, thereby reducing coupling.

I believe in learning through examples. So to make it easier to understand let's look at a real life non code related problem. 

*You run a business and few of your customers owe you a lot of money. You don't want to waste your time chasing the money. You don't know the law well enough to take them to court yourself. So you go to a debt collection agency. You know they have lawyers (and enforcers) to 'encourage' people to pay. Of course this service doesn't come free but you have now effectively cut yourself off from having to deal with the debt collection operation process.*

The debt collection agency in that example is the mediator.

## Why Use This Pattern?

I've briefly touched about the advantage of less coupling between components but there are some side benefits as well.

*Consider the scenario below*

 You're developing a web api that needs to talk to the core domain objects to yield results. If you decide to communicate with your domain directly from your controllers, then inevitably you end up coupling your controller to a lot of domain level services. The line between your layers can get blurry here as well. By using the mediator pattern, you offload the heavy lifting to the request handlers. The handlers take a dependency on only the required domain services so things get much easier to understand and maintain. This gives you a clear separation from the controller layer as well.

Because this is effectively a message dispatching pattern with the added benefit of decoupled reusable components, you can transition your project to follow a microservices type architecture later on if you wish to do so. This has been a very nice side benefit of using the pattern in my experience.

This post [here](https://sourcemaking.com/design_patterns/mediator) gives more details about the pattern if you're interested.

If you have used it before and have a story to tell, please leave a comment here. I'm keen to know :)

## Motivation

There are a few .NET implementations around. This [post](https://drusellers.com/posts/greenpipes/) by Dru Sellers pointed me in the right direction. The most popular one of course is [MediatR](https://github.com/jbogard/MediatR) from Jimmy Bogard. It's a very simple library with a `Request` and `Response` model with support for middleware. After investigating the source for `MediatR` (Which heavily inspired my design) and some other libraries I decided to create my own.

My motivations were the following.

- Defined types (of requests) for `Queries`, `Commands` and `Events`. I want the message type to convey intent.
- Support for cancellation tokens.
- Have a `MediationContext`. So when the request handler is called the Request is concise and lightweight. Any context related information is captured in this class.
- Concept of `middleware`, where each request goes through the pipeline and response come back through it.
- Have the ability to dispatch requests over the cloud (Using MassTransit or something similar) to a consumer (and get a response back). Much like how the [Brighter](https://github.com/BrighterCommand/Brighter) framework functions.

## Design

Conceptually I want the Mediator to take in a request, dispatch it to the domain and give the caller the result back. I needed the following core pieces to achieve that.

- Request and Response

    The request is defined as `IRequest<TResponse>` where `TResponse` is the type of Response. This assumes that every request has a matching response.

- Queries, Command and Events

    - Queries are meant to return something. Therefore it is easy to define as `IQuery<TResponse> : IReuqest<TResponse>`
    - Commands don't return anything. To represent nothing I defined a type called `Unit`. Hence the definition for a command becomes `ICommand : IRequest<Unit>`
    - Event is the same. `IEvent : IRequest<Unit>`

- Request Handlers

    There will be a single method with the signature `Task<TResponse> Handle(IRequest<TResponse> request, IMediationContext mediationContext, CancellationToken cancellationToken)`.
    ```csharp
    public interface IRequestHandler<in TRequest, TResponse> where TRequest : IRequest<TResponse> 
    {
        Task<TResponse> HandleAsync(TRequest request, IMediationContext mediationContext,
            CancellationToken cancellationToken);
    }
    ```

    The `IMediationContext` represents the context I alluded to in the motivation section above, and the `CancellationToken` allows for signalling graceful termination (early exit).

    - The equivalent specialised handlers for Queries, Commands and Events are as follows.
        ```csharp
        public interface IQueryHandler<in TQuery, TResponse> : IRequestHandler<TQuery, TResponse>
        where TQuery : IRequest<TResponse>
        {
        }

        public interface ICommandHandler<in TCommand>: IRequestHandler<TCommand, Unit> where TCommand : IRequest<Unit>
        {
        }

        public interface IEventHandler<in TEvent>: IRequestHandler<TEvent, Unit> where TEvent : IRequest<Unit>
        {
        }
        ``` 

    - I created abstract classes from these to make the syntax a bit nicer for the Handle methods. When you inherit from it the method you have to implement conveys the intent better in my opinion. **You can always implement the interface directly if you wish to**.
        ```csharp
        public abstract class QueryHandler<TQuery, TResponse> : IQueryHandler<TQuery, TResponse> where TQuery : IRequest<TResponse>
        {
            public Task<TResponse> HandleAsync(TQuery request, IMediationContext mediationContext,
                CancellationToken cancellationToken)
            {
                return HandleQueryAsync(request, mediationContext, cancellationToken);
            }

            protected abstract Task<TResponse> HandleQueryAsync(TQuery query, IMediationContext mediationContext,
                CancellationToken cancellationToken);
        }

        public abstract class CommandHandler<TCommand> : ICommandHandler<TCommand> where TCommand : IRequest<Unit>
        {
            public async Task<Unit> HandleAsync(TCommand request, IMediationContext mediationContext,
                CancellationToken cancellationToken)
            {
                await HandleCommandAsync(request, mediationContext, cancellationToken);
                return Unit.Result;
            }

            protected abstract Task HandleCommandAsync(TCommand command, IMediationContext mediationContext,
                CancellationToken cancellationToken);
        }

        public abstract class EventHandler<TEvent> : IEventHandler<TEvent> where TEvent : IRequest<Unit>
        {
            public async Task<Unit> HandleAsync(TEvent request, IMediationContext mediationContext,
                CancellationToken cancellationToken)
            {
                await HandleEventAsync(request, mediationContext, cancellationToken);
                return Unit.Result;
            }

            protected abstract Task HandleEventAsync(TEvent @event, IMediationContext mediationContext,
                CancellationToken cancellationToken);
        }
        ```
- Middleware

    If you really think about it, a middleware is another type of request handler. They are like the Russian [Matryoshka Dolls](https://en.wikipedia.org/wiki/Matryoshka_doll) with the very inner most doll being the query/command/event handler.

    This is how I designed it.
    ```csharp
    public delegate Task<TResponse> HandleRequestDelegate<in TRequest, TResponse>(TRequest request, IMediationContext mediationContext, CancellationToken cancellationToken);

    public interface IMiddleware<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        Task<TResponse> RunAsync(TRequest request, IMediationContext mediationContext,
            CancellationToken cancellationToken, HandleRequestDelegate<TRequest, TResponse> next);
    }
    ```

    Notice how the method signature is exactly the same as a request handler except for the `next` argument. That's because a request handler is the last thing in the pipeline. There is no next middleware to run. That's the fundamental difference between a request handler and middleware.

- The Request Processor

    The job of the request processor is to accept a request and pass it through the middleware pipeline. It constructs the pipeline and calls the `RunAsync` methods on the first middleware.

    ```csharp
    public interface IRequestProcessor<in TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        Task<TResponse> HandleAsync(TRequest request, IMediationContext mediationContext,
            CancellationToken cancellationToken);
    }
    ```

    The implementation of this is as follows. The full code is available [here](https://github.com/dasiths/SimpleMediator/blob/master/SimpleMediator/Middleware/RequestProcessor.cs)
    ```csharp
    public class RequestProcessor<TRequest, TResponse> : IRequestProcessor<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        private readonly IEnumerable<IRequestHandler<TRequest, TResponse>> _requestHandlers;
        private readonly IEnumerable<IMiddleware<TRequest, TResponse>> _middlewares;
        // These come from the constructor. See the github repo for full code.
        // https://github.com/dasiths/SimpleMediator/blob/master/SimpleMediator/Middleware/RequestProcessor.cs

        public Task<TResponse> HandleAsync(TRequest request, IMediationContext mediationContext,
            CancellationToken cancellationToken)
        {
            return RunMiddleware(request, HandleRequest, mediationContext, cancellationToken);
        }

        private async Task<TResponse> HandleRequest(TRequest requestObject, IMediationContext mediationContext, CancellationToken cancellationToken)
        {
            // See github repo for full code
        }

        private Task<TResponse> RunMiddleware(TRequest request, HandleRequestDelegate<TRequest, TResponse> handleRequestHandlerCall, 
            IMediationContext mediationContext, CancellationToken cancellationToken)
        {
            HandleRequestDelegate<TRequest, TResponse> next = null;

            next = _middlewares.Reverse().Aggregate(handleRequestHandlerCall, (requestDelegate, middleware) =>
                ((req, ctx, ct) => middleware.RunAsync(req, ctx, ct, requestDelegate)));

            return next.Invoke(request, mediationContext, cancellationToken);
        }
    }
    ```
    The entry point is in `HandleAsync`. This is the method that gets called from the Mediator but the real magic happens in the `RunMiddleware` method. See how it aggregates the middleware to one delegate and then executes that. If you need a refresher on how `Aggregate` works have a read [here](http://www.csharp-examples.net/linq-aggregate/).

- Mediator

    This is the interface you use to dispatch a message. It has one simple method.
    ```csharp
    public interface IMediator
    {
        Task<TResponse> HandleAsync<TResponse>(IRequest<TResponse> request, IMediationContext mediationContext = default(IMediationContext),
            CancellationToken cancellationToken = default(CancellationToken));
    }
    ```

    The implementation for the interface is as follows. `IServiceFactory` is just a service locator we use to help us utilize the IOC container of our choice. We use a **single instance** of the mediator to service all requests. This means we have to request an instance of `IRequestProcessor` per request type from our IOC container. The service locator pattern was used here because of that.
    
    See how the `ServiceFactoryDelegate` (In the `AddRequiredServices()` method) is used in the code sample [linked here](https://github.com/dasiths/SimpleMediator/blob/master/SimpleMediator.Extensions.Microsoft.DependencyInjection/ReflectionUtilities.cs) if you need help understanding how it links up with the IOC container.
    ```csharp
    public class Mediator : IMediator
    {
        private readonly IServiceFactory _serviceFactory;

        public Mediator(IServiceFactory serviceFactory)
        {
            _serviceFactory = serviceFactory;
        }

        public Task<TResponse> HandleAsync<TResponse>(IRequest<TResponse> request,
            IMediationContext mediationContext = default(MediationContext), CancellationToken cancellationToken = default(CancellationToken))
        {
            if (mediationContext == null)
            {
                mediationContext = MediationContext.Default;
            }

            var targetType = request.GetType();
            var targetHandler = typeof(IRequestProcessor<,>).MakeGenericType(targetType, typeof(TResponse));
            var instance = _serviceFactory.GetInstance(targetHandler);

            var result = InvokeInstanceAsync(instance, request, targetHandler, mediationContext, cancellationToken);

            return result;
        }

        private Task<TResponse> InvokeInstanceAsync<TResponse>(object instance, IRequest<TResponse> request, Type targetHandler, 
            IMediationContext mediationContext, CancellationToken cancellationToken)
        {
            var method = instance.GetType()
                .GetTypeInfo()
                .GetMethod(nameof(IRequestProcessor<IRequest<TResponse>, TResponse>.HandleAsync));

            if (method == null)
            {
                throw new ArgumentException($"{instance.GetType().Name} is not a known {targetHandler.Name}",
                    instance.GetType().FullName);
            }

            return (Task<TResponse>) method.Invoke(instance, new object[] {request, mediationContext, cancellationToken});
        }

        // See the full code at https://github.com/dasiths/SimpleMediator/blob/master/SimpleMediator/Core/Mediator.cs
    }
    ```

Those are the major pieces that make up the Mediator. Conceptually it is very simple but there are some implementation concerns that make the code a bit complicated. I am aware of this.

## Using It

1. Define a request and response.
    ```csharp
    public class SimpleQuery: IQuery<SimpleResponse>
    {
    }

    public class SimpleResponse
    {
        public string Message { get; set; }
    }
    ```

2. Define a handler.
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
    - You can find more examples for the concept of middleware [here](https://github.com/dasiths/SimpleMediator/tree/master/SimpleMediator.Samples.ConsoleApp). 

    - I have examples of how to set it up with IOC containers [here](https://github.com/dasiths/SimpleMediator/tree/master/SimpleMediator.Samples.Shared/Helpers). `Autofac` and `Microsoft.Extensions.DependencyInjection` currently have examples but I'll keep adding more as I go. 
    - There is also some code samples (work in progress) on how to integrate it with `MassTransit` to dispatch messages over the wire to consumers. Check it out [here](https://github.com/dasiths/SimpleMediator/tree/master/SimpleMediator.Samples.MassTransit).

    As you can see, the usage is pretty straight forward and simple. The middleware gives you a lot of extensibility options and I've even been able to create constrained middleware that validates only certain types of requests.

## Conclusion 
I hope this post has been helpful in understanding the concepts behind the mediator pattern and how you would go about implementing it. All the code is hosted at https://github.com/dasiths/SimpleMediator under the MIT license. Feel free to have a look and create a PR if you think there are improvements. Please leave any feedback you have here. Thank you.