// Original work Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Modified work Copyright (c) 2020 tchpeng, GitHub: tchpeng
// Licensed under MIT license. See License.txt in the project root for license information.

using GenericBizRunner.Internal;
using GenericBizRunner.PublicButHidden;
using System;

namespace GenericBizRunner
{
    /// <summary>
    /// This defines the ActionService using the default repository supplied via IRepository
    /// </summary>
    /// <typeparam name="TBizInstance">The instance of the business logic you are linking to</typeparam>
    public class ActionService<TBizInstance> : ActionService<IRepository, TBizInstance>, IActionService<TBizInstance>
        where TBizInstance : class, IBizActionStatus
    {
        /// <inheritdoc />
        public ActionService(IRepository repository, TBizInstance bizInstance, IWrappedBizRunnerConfigAndMappings wrappedConfig)
            : base(repository, bizInstance, wrappedConfig)
        {
        }
    }

    /// <summary>
    /// This defines the ActionService where you supply the type of the repository you want used with the business logic
    /// </summary>
    /// <typeparam name="TRepository">The repository to be used with this business logic</typeparam>
    /// <typeparam name="TBizInstance">The instance of the business logic you are linking to</typeparam>
    public class ActionService<TRepository, TBizInstance> : IActionService<TRepository, TBizInstance>
        where TRepository : class, IRepository
        where TBizInstance : class, IBizActionStatus
    {
        private readonly TBizInstance _bizInstance;
        private readonly IWrappedBizRunnerConfigAndMappings _wrappedConfig;
        private readonly TRepository _repository;
        private readonly bool _turnOffCaching;

        /// <inheritdoc />
        public ActionService(TRepository repository, TBizInstance bizInstance, IWrappedBizRunnerConfigAndMappings wrappedConfig)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _bizInstance = bizInstance ?? throw new ArgumentNullException(nameof(bizInstance));
            _wrappedConfig = wrappedConfig ?? throw new ArgumentNullException(nameof(wrappedConfig));
            _turnOffCaching = _wrappedConfig.Config.TurnOffCaching;
        }

        /// <summary>
        /// This contains the Status after it has been run
        /// </summary>
        public IBizActionStatus Status => _bizInstance;

        /// <summary>
        /// This will run a business action that takes and input and produces a result
        /// </summary>
        /// <typeparam name="TOut">The type of the result to return. Should either be the Business logic output type or class which inherits fromm GenericActionFromBizDto</typeparam>
        /// <param name="inputData">The input data. It should be Should either be the Business logic input type or class which inherits form GenericActionToBizDto</param>
        /// <returns>The returned data after the run, or default value is there was an error</returns>
        public TOut RunBizAction<TOut>(object inputData)
        {
            var decoder = new BizDecoder(typeof(TBizInstance), RequestedInOut.InOut, _turnOffCaching);
            return decoder.BizInfo.GetServiceInstance(_wrappedConfig).RunBizActionDbAndInstance<TOut>(_repository, _bizInstance, inputData);
        }

        /// <summary>
        /// This will run a business action that does not take an input but does produces a result
        /// </summary>
        /// <typeparam name="TOut">The type of the result to return. Should either be the Business logic output type or class which inherits fromm GenericActionFromBizDto</typeparam>
        /// <returns>The returned data after the run, or default value is thewre was an error</returns>
        public TOut RunBizAction<TOut>()
        {
            var decoder = new BizDecoder(typeof(TBizInstance), RequestedInOut.Out, _turnOffCaching);
            return decoder.BizInfo.GetServiceInstance(_wrappedConfig).RunBizActionDbAndInstance<TOut>(_repository, _bizInstance);
        }

        /// <summary>
        /// This runs a business action which takes an input and returns just a status message
        /// </summary>
        /// <param name="inputData">The input data. It should be Should either be the Business logic input type or class which inherits form GenericActionToBizDto</param>
        /// <returns>status message with no result part</returns>
        public void RunBizAction(object inputData)
        {
            var decoder = new BizDecoder(typeof(TBizInstance), RequestedInOut.In, _turnOffCaching);
            decoder.BizInfo.GetServiceInstance(_wrappedConfig)
                .RunBizActionDbAndInstance(_repository, _bizInstance, inputData);
        }

        /// <summary>
        /// This will return a new class for input. 
        /// If the type is based on a GenericActionsDto it will run SetupSecondaryData on it before handing it back
        /// </summary>
        /// <typeparam name="TDto"></typeparam>
        /// <param name="runBeforeSetup">An optional action to set something in the new DTO before SetupSecondaryData is called</param>
        /// <returns></returns>
        public TDto GetDto<TDto>(Action<TDto> runBeforeSetup = null) where TDto : class, new()
        {
            if (!typeof(TDto).IsClass)
                throw new InvalidOperationException("You should only call this on a primitive type. Its only useful for Dtos.");

            var decoder = new BizDecoder(typeof(TBizInstance), RequestedInOut.InOrInOut, _turnOffCaching);
            var toBizCopier = DtoAccessGenerator.BuildCopier(typeof(TDto), decoder.BizInfo.GetBizInType(), 
                true, false, _turnOffCaching);
            return toBizCopier.CreateDataWithPossibleSetup<TDto>(_repository, Status, runBeforeSetup);
        }

        /// <summary>
        /// This should be called if a model error is found in the input data.
        /// If the provided data is a GenericActions dto, or it has ISetupSecondaryData, it will call SetupSecondaryData 
        /// to reset any data in the dto ready for reshowing the dto to the user.
        /// </summary>
        /// <typeparam name="TDto"></typeparam>
        /// <returns></returns>
        public TDto ResetDto<TDto>(TDto dto) where TDto : class
        {
            if (!typeof(TDto).IsClass)
                throw new InvalidOperationException("You should only call this on a primitive type. Its only useful for Dtos.");

            var decoder = new BizDecoder(typeof(TBizInstance), RequestedInOut.InOrInOut, _turnOffCaching);
            var toBizCopier = DtoAccessGenerator.BuildCopier(typeof(TDto), decoder.BizInfo.GetBizInType(), true, false, _turnOffCaching);
            toBizCopier.SetupSecondaryDataIfRequired(_repository, Status, dto);
            return dto;
        }

    }
}