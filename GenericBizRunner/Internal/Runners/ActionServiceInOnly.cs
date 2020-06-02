// Original work Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Modified work Copyright (c) 2020 tchpeng, GitHub: tchpeng
// Licensed under MIT license. See License.txt in the project root for license information.

using GenericBizRunner.PublicButHidden;

namespace GenericBizRunner.Internal.Runners
{
    internal class ActionServiceInOnly<TBizInterface, TBizIn> : ActionServiceBase
    {
        public ActionServiceInOnly(bool requiresSaveChanges, IWrappedBizRunnerConfigAndMappings wrappedConfig)
            : base(requiresSaveChanges, wrappedConfig)
        {
        }

        public void RunBizActionDbAndInstance(object repository, TBizInterface bizInstance, object inputData)
        {
            var toBizCopier = DtoAccessGenerator.BuildCopier(inputData.GetType(), typeof(TBizIn), true, false, WrappedConfig.Config.TurnOffCaching);
            var bizStatus = (IBizActionStatus)bizInstance;

            //The SetupSecondaryData produced errors
            if (bizStatus.HasErrors) return;

            var inData = toBizCopier.DoCopyToBiz<TBizIn>(repository, WrappedConfig.ToBizIMapper, inputData);

            ((IGenericActionInOnly<TBizIn>)bizInstance).BizAction(inData);

            //This handles optional call of save changes. Only be used if type of DbContext is being used and supplied via IRepository.
            SaveChangedIfRequiredAndNoErrors(repository, bizStatus);
            if (bizStatus.HasErrors)
                toBizCopier.SetupSecondaryDataIfRequired(repository, bizStatus, inputData);
        }
    }
}