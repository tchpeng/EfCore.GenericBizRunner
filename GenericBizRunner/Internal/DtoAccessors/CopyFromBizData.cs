// Original work Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Modified work Copyright (c) 2020 tchpeng, GitHub: tchpeng
// Licensed under MIT license. See License.txt in the project root for license information.

using AutoMapper;

namespace GenericBizRunner.Internal.DtoAccessors
{
    internal class CopyFromBizData<TBizOut, TDtoOut>
        where TBizOut : class
        where TDtoOut : GenericActionFromBizDto<TBizOut, TDtoOut>, new()
    {
        private readonly TDtoOut _dtoInstance = new TDtoOut();

        public TDtoOut CopyFromBiz(object repository, IMapper mapper, object source)
        {
            return _dtoInstance.CopyFromBizData(repository, mapper, (TBizOut) source);
        }

        public void SetupSecondaryOutputData(object repository, object dto)
        {
            ((TDtoOut)dto).SetupSecondaryOutputData(repository);
        }
    }
}