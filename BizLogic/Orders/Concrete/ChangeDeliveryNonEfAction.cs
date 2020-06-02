// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using DataLayer.NonEf;
using GenericBizRunner;
using System;

namespace BizLogic.Orders.Concrete
{
    public class ChangeDeliverNonEfyAction : BizActionStatus, IChangeDeliverNonEfAction
    {
        private readonly NonEfRepo _repo;

        public ChangeDeliverNonEfyAction(NonEfRepo repo)
        {
            _repo = repo;
        }

        public void BizAction(BizChangeDeliverDto dto)
        {
            var order = _repo.GetOrder(dto.OrderId);
            if (order == null)
                throw new NullReferenceException("Could not find the order. Someone entering illegal ids?");

            var status = order.ChangeDeliveryDate(dto.UserId, dto.NewDeliveryDate);
            CombineStatuses(status);

            if (!status.HasErrors && _repo.ChangeOrderDeliveryDate(order.OrderId, order.ExpectedDeliveryDate))
            {
                Message = $"Your new delivery date is {dto.NewDeliveryDate.ToShortDateString()}.";
            }
            else
            {
                AddError("Fail to change delivery date.");
            }
            
        }
    }
}