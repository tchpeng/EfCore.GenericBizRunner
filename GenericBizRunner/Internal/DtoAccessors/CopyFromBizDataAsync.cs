// Original work Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Modified work Copyright (c) 2020 tchpeng, GitHub: tchpeng
// Licensed under MIT license. See License.txt in the project root for license information.

using AutoMapper;
using System.Threading.Tasks;

namespace GenericBizRunner.Internal.DtoAccessors
{
    internal class CopyFromBizDataAsync<TBizOut, TDtoOut>
        where TBizOut : class
        where TDtoOut : GenericActionFromBizDtoAsync<TBizOut, TDtoOut>, new()
    {
        private readonly TDtoOut _dtoInstance = new TDtoOut();

        public async Task<TDtoOut> CopyFromBizAsync(object repository, IMapper mapper, object source)
        {
            return await _dtoInstance.CopyFromBizDataAsync(repository, mapper, (TBizOut) source).ConfigureAwait(false);
        }

        public async Task SetupSecondaryOutputDataAsync(object repository, object dto)
        {
            await ((TDtoOut)dto).SetupSecondaryOutputDataAsync(repository).ConfigureAwait(false);
        }
    }
}