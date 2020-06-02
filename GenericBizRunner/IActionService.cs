﻿// Original work Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Modified work Copyright (c) 2020 tchpeng, GitHub: tchpeng
// Licensed under MIT license. See License.txt in the project root for license information.

using System;

namespace GenericBizRunner
{
    /// <summary>
    /// This defines the interface for the ActionService that uses the default repository supplied via IRepository
    /// </summary>
    /// <typeparam name="TBizInstance">The instance of the business logic to run</typeparam>
    public interface IActionService<TBizInstance> : IActionService<IRepository, TBizInstance> where TBizInstance : class
    { }

    /// <summary>
    /// This is the primary interface to the sync actions
    /// </summary>
    /// <typeparam name="TRepository"></typeparam>
    /// <typeparam name="TBizInstance"></typeparam>
    public interface IActionService<TRepository, TBizInstance> where TRepository : IRepository where TBizInstance : class
    {
        /// <summary>
        /// This contains the Status after the BizAction is run
        /// </summary>
        IBizActionStatus Status { get; }

        /// <summary>
        /// This will run a business action that takes and input and produces a result
        /// </summary>
        /// <typeparam name="TOut">The type of the result to return. Should either be the Business logic output type or class which inherits fromm GenericActionFromBizDto</typeparam>
        /// <param name="inputData">The input data. It should be Should either be the Business logic input type or class which inherits form GenericActionToBizDto</param>
        /// <returns>The result, or default(TOut) if there is an error</returns>
        TOut RunBizAction<TOut>(object inputData);

        /// <summary>
        /// This will run a business action that does not take an input but does produces a result
        /// </summary>
        /// <typeparam name="TOut">The type of the result to return. Should either be the Business logic output type or class which inherits fromm GenericActionFromBizDto</typeparam>
        /// <returns>The result, or default(TOut) if there is an error</returns>
        TOut RunBizAction<TOut>();

        /// <summary>
        /// This runs a business action which takes an input and returns just a status message
        /// </summary>
        /// <param name="inputData">The input data. It should be Should either be the Business logic input type or class which inherits form GenericActionToBizDto</param>
        /// <returns>status message with no result part</returns>
        void RunBizAction(object inputData);

        /// <summary>
        /// This will return a new class for input. 
        /// If the type is based on a GenericActionsDto it will run SetupSecondaryData on it before hadning it back
        /// </summary>
        /// <typeparam name="TDto"></typeparam>
        /// <param name="runBeforeSetup">An optional action to set something in the new DTO before SetupSecondaryData is called</param>
        /// <returns></returns>
        TDto GetDto<TDto>(Action<TDto>runBeforeSetup = null) where TDto : class, new();

        /// <summary>
        /// This should be called if a model error is found in the input data.
        /// If the provided data is a GenericActions dto it will call SetupSecondaryData 
        /// to reset any data in the dto ready for reshowing the dto to the user.
        /// </summary>
        /// <typeparam name="TDto"></typeparam>
        /// <returns></returns>
        TDto ResetDto<TDto>(TDto dto) where TDto : class;
    }
}