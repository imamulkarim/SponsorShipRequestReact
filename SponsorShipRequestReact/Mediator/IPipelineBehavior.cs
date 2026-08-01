//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace TechAssessment.Application.Mediator;

///// <summary>
///// Represents an async continuation for the next task to execute in the pipeline
///// </summary>
///// <typeparam name="TResponse">Response type</typeparam>
///// <returns>Awaitable task returning a <typeparamref name="TResponse"/></returns>
//public delegate Task<TResponse> RequestHandlerDelegate<TResponse>(CancellationToken t = default);

//public interface IPipelineBehavior<in TRequest, TResponse> where TRequest : notnull
//{
//    //
//    // Summary:
//    //     Pipeline handler. Perform any additional behavior and await the next delegate
//    //     as necessary
//    //
//    // Parameters:
//    //   request:
//    //     Incoming request
//    //
//    //   next:
//    //     Awaitable delegate for the next action in the pipeline. Eventually this delegate
//    //     represents the handler.
//    //
//    //   cancellationToken:
//    //     Cancellation token
//    //
//    // Returns:
//    //     Awaitable task returning the TResponse
//    Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken);
//}

