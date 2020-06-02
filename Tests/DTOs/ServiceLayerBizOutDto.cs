// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using AutoMapper;
using GenericBizRunner;
using Microsoft.EntityFrameworkCore;
using TestBizLayer.BizDTOs;

namespace Tests.DTOs
{
    public class ServiceLayerBizOutDto : GenericActionFromBizDto<BizDataOut, ServiceLayerBizOutDto>
    {
        public string Output { get; set; }

        public bool SetupSecondaryOutputDataCalled { get; private set; }
        public bool CopyFromBizDataCalled { get; private set; }

        protected internal override void SetupSecondaryOutputData(object repository)
        {
            SetupSecondaryOutputDataCalled = true;
            base.SetupSecondaryOutputData(repository);
        }

        protected internal override ServiceLayerBizOutDto CopyFromBizData(object repository, IMapper mapper,
            BizDataOut source)
        {
            var result = base.CopyFromBizData(repository, mapper, source);
            result.CopyFromBizDataCalled = true;
            return result;
        }
    }
}