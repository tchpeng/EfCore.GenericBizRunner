﻿// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using AutoMapper;
using GenericBizRunner;
using StatusGeneric;
using TestBizLayer.BizDTOs;

namespace TestNetStandard2_0Only.DTOs
{
    public class ServiceLayerBizInDto : GenericActionToBizDto<BizDataIn, ServiceLayerBizInDto>
    {
        public int Num { get; set; }

        public bool RaiseErrorInSetupSecondaryData { get; set; }

        public bool SetupSecondaryDataCalled { get; private set; }

        public bool CopyToBizDataCalled { get; private set; }

        protected override void SetupSecondaryData(object repository, IStatusGenericHandler status)
        {
            SetupSecondaryDataCalled = true;
            if (RaiseErrorInSetupSecondaryData)
                status.AddError("Error in SetupSecondaryData");
        }

        protected override BizDataIn CopyToBizData(object repository, IMapper mapper, ServiceLayerBizInDto source)
        {
            CopyToBizDataCalled = true;
            return base.CopyToBizData(repository, mapper, source);
        }
    }
}