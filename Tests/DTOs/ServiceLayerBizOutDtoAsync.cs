﻿// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Threading.Tasks;
using AutoMapper;
using GenericBizRunner;
using Microsoft.EntityFrameworkCore;
using TestBizLayer.BizDTOs;

namespace Tests.DTOs
{
    public class ServiceLayerBizOutDtoAsync : GenericActionFromBizDtoAsync<BizDataOut, ServiceLayerBizOutDtoAsync>
    {
        public string Output { get; set; }

        public bool SetupSecondaryOutputDataCalled { get; private set; }
        public bool CopyFromBizDataCalled { get; private set; }

        protected internal override async Task SetupSecondaryOutputDataAsync(object repository)
        {
            SetupSecondaryOutputDataCalled = true;
        }

        protected internal override async Task<ServiceLayerBizOutDtoAsync> CopyFromBizDataAsync(object repository, IMapper mapper, BizDataOut source)
        {
            var data = await base.CopyFromBizDataAsync(repository, mapper, source);
            data.CopyFromBizDataCalled = true;
            return data;
        }
    }
}