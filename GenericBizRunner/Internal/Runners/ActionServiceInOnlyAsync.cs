// Original work Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Modified work Copyright (c) 2020 tchpeng, GitHub: tchpeng
// Licensed under MIT license. See License.txt in the project root for license information.

using GenericBizRunner.PublicButHidden;
using System.Threading.Tasks;

namespace GenericBizRunner.Internal.Runners
{
    internal class ActionServiceInOnlyAsync<TBizInterface, TBizIn> : ActionServiceBase
    {
        public ActionServiceInOnlyAsync(bool requiresSaveChanges, IWrappedBizRunnerConfigAndMappings wrappedConfig)
            : base(requiresSaveChanges, wrappedConfig)
        {
        }

        public async Task RunBizActionDbAndInstanceAsync(object repository, TBizInterface bizInstance, object inputData)
        {
            var toBizCopier = DtoAccessGenerator.BuildCopier(inputData.GetType(), typeof(TBizIn), true, true, WrappedConfig.Config.TurnOffCaching);
            var bizStatus = (IBizActionStatus)bizInstance;

            //The SetupSecondaryData produced errors
            if (bizStatus.HasErrors) return;

            var inData = await toBizCopier.DoCopyToBizAsync<TBizIn>(repository, WrappedConfig.ToBizIMapper, inputData).ConfigureAwait(false);

            await ((IGenericActionInOnlyAsync<TBizIn>) bizInstance).BizActionAsync(inData).ConfigureAwait(false);

            //This handles optional call of save changes. Only be used if type of DbContext is being used and supplied via IRepository.
            await SaveChangedIfRequiredAndNoErrorsAsync(repository, bizStatus).ConfigureAwait(false);
            if (bizStatus.HasErrors)
                await toBizCopier.SetupSecondaryDataIfRequiredAsync(repository, bizStatus, inputData).ConfigureAwait(false);
        }
    }
}