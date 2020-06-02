// Original work Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Modified work Copyright (c) 2020 tchpeng, GitHub: tchpeng
// Licensed under MIT license. See License.txt in the project root for license information.

using AutoMapper;

namespace GenericBizRunner.Internal.DtoAccessors
{
    internal class CopyToBizData<TBizIn, TDtoIn>
        where TBizIn : class, new()
        where TDtoIn : GenericActionToBizDto<TBizIn, TDtoIn>, new()
    {
        public TBizIn CopyToBiz(object repository, IMapper mapper, object source)
        {
            return ((TDtoIn) source).CopyToBizData(repository, mapper, (TDtoIn) source);
        }

        public void SetupSecondaryData(object repository, IBizActionStatus status, object dto)
        {
            ((TDtoIn) dto).SetupSecondaryData(repository, status);
        }
    }
}