// Original work Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Modified work Copyright (c) 2020 tchpeng, GitHub: tchpeng
// Licensed under MIT license. See License.txt in the project root for license information.

using GenericBizRunner.PublicButHidden;
using System.Threading.Tasks;

namespace GenericBizRunner.Internal.Runners
{
    internal class ActionServiceInOutAsync<TBizInterface, TBizIn, TBizOut> : ActionServiceBase
    {
        public ActionServiceInOutAsync(bool requiresSaveChanges, IWrappedBizRunnerConfigAndMappings wrappedConfig)
            : base(requiresSaveChanges, wrappedConfig)
        {
        }

        public async Task<TOut> RunBizActionDbAndInstanceAsync<TOut>(object repository, TBizInterface bizInstance,
            object inputData)
        {
            var toBizCopier = DtoAccessGenerator.BuildCopier(inputData.GetType(), typeof(TBizIn), true, true, WrappedConfig.Config.TurnOffCaching);
            var fromBizCopier = DtoAccessGenerator.BuildCopier(typeof(TBizOut), typeof(TOut), false, true, WrappedConfig.Config.TurnOffCaching);
            var bizStatus = (IBizActionStatus)bizInstance;

            //The SetupSecondaryData produced errors
            if (bizStatus.HasErrors) return default(TOut);

            var inData = await toBizCopier.DoCopyToBizAsync<TBizIn>(repository, WrappedConfig.ToBizIMapper, inputData).ConfigureAwait(false);

            var result = await ((IGenericActionAsync<TBizIn, TBizOut>)bizInstance).BizActionAsync(inData).ConfigureAwait(false);

            //This handles optional call of save changes. Only be used if type of DbContext is being used and supplied via IRepository.
            await SaveChangedIfRequiredAndNoErrorsAsync(repository, bizStatus).ConfigureAwait(false);
            if (bizStatus.HasErrors) return default(TOut);

            var data = await fromBizCopier.DoCopyFromBizAsync<TOut>(repository, WrappedConfig.FromBizIMapper, result).ConfigureAwait(false);
            return data;
        }
    }
}