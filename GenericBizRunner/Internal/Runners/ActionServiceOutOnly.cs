// Original work Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Modified work Copyright (c) 2020 tchpeng, GitHub: tchpeng
// Licensed under MIT license. See License.txt in the project root for license information.


using GenericBizRunner.PublicButHidden;

namespace GenericBizRunner.Internal.Runners
{
    internal class ActionServiceOutOnly<TBizInterface, TBizOut> : ActionServiceBase
    {
        public ActionServiceOutOnly(bool requiresSaveChanges, IWrappedBizRunnerConfigAndMappings wrappedConfig)
            : base(requiresSaveChanges, wrappedConfig)
        {
        }

        public TOut RunBizActionDbAndInstance<TOut>(object repository, TBizInterface bizInstance)
        {
            var fromBizCopier = DtoAccessGenerator.BuildCopier(typeof(TBizOut), typeof(TOut), false, false, WrappedConfig.Config.TurnOffCaching);
            var bizStatus = (IBizActionStatus)bizInstance;

            var result = ((IGenericActionOutOnly<TBizOut>)bizInstance).BizAction();

            //This handles optional call of save changes. Only be used if type of DbContext is being used and supplied via IRepository.
            SaveChangedIfRequiredAndNoErrors(repository, bizStatus);
            if (bizStatus.HasErrors) return default(TOut);

            var data = fromBizCopier.DoCopyFromBiz<TOut>(repository, WrappedConfig.FromBizIMapper, result);
            return data;
        }
    }
}