// Original work Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Modified work Copyright (c) 2020 tchpeng, GitHub: tchpeng
// Licensed under MIT license. See License.txt in the project root for license information.

using AutoMapper;
using System.Threading.Tasks;

namespace GenericBizRunner.Internal.DtoAccessors
{
    internal class CopyToBizDataAsync<TBizIn, TDtoIn>
        where TBizIn : class, new()
        where TDtoIn : GenericActionToBizDtoAsync<TBizIn, TDtoIn>, new()
    {
        public async Task<TBizIn> CopyToBizAsync(object repository, IMapper mapper, object source)
        {
            return await ((TDtoIn)source).CopyToBizDataAsync(repository, mapper, (TDtoIn)source).ConfigureAwait(false);
        }

        public async Task SetupSecondaryDataAsync(object repository, IBizActionStatus status, object dto)
        {
            await ((TDtoIn) dto).SetupSecondaryDataAsync(repository, status).ConfigureAwait(false);
        }
    }
}