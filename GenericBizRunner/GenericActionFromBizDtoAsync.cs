﻿// Original work Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Modified work Copyright (c) 2020 tchpeng, GitHub: tchpeng
// Licensed under MIT license. See License.txt in the project root for license information.

using AutoMapper;
using GenericBizRunner.PublicButHidden;
using System.Threading.Tasks;

namespace GenericBizRunner
{
    /// <summary>
    /// This is the class that your async DTOs for input should inherit
    /// This is an Async vesion of the GenericActionFromBizDto, i.e. the two supporting methods work asynchnously
    /// </summary>
    /// <typeparam name="TBizOut"></typeparam>
    /// <typeparam name="TDtoOut"></typeparam>
    public abstract class
        GenericActionFromBizDtoAsync<TBizOut, TDtoOut> : GenericActionFromBizDtoSetup<TBizOut, TDtoOut>
        where TBizOut : class
        where TDtoOut : GenericActionFromBizDtoAsync<TBizOut, TDtoOut>, new()
    {
        /// <summary>
        /// This is called after the mapping from the BizData to the GenericActionDto has been done.
        /// Useful if the biz method returns say primary keys only and you would like to look up
        /// data to show to the user.
        /// </summary>
        /// <param name="repository"></param>
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        internal protected virtual async Task SetupSecondaryOutputDataAsync(object repository)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
        }


        /// <summary>
        /// This copies the Business logic's output data into the GenericAction's DTO class.
        /// Override this if you need to do some more complex calculation during the copy 
        /// Note: Look at AutoMapperSetup method first as that can handle a number of mapping issues
        /// Also note that this method should not fail because any write to the database (optional)
        /// has already happened by the time this is called.
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="mapper"></param>
        /// <param name="source"></param>
        /// <returns></returns>
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        internal protected virtual async Task<TDtoOut> CopyFromBizDataAsync(object repository, IMapper mapper, TBizOut source)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            return mapper.Map<TDtoOut>(source);
        }
    }
}