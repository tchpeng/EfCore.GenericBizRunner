// Original work Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Modified work Copyright (c) 2020 tchpeng, GitHub: tchpeng
// Licensed under MIT license. See License.txt in the project root for license information.

using AutoMapper;
using GenericBizRunner.PublicButHidden;

namespace GenericBizRunner
{
    /// <summary>
    /// This is the class that your sync DTOs for input should inherit
    /// </summary>
    /// <typeparam name="TBizOut"></typeparam>
    /// <typeparam name="TDtoOut"></typeparam>
    public abstract class GenericActionFromBizDto<TBizOut, TDtoOut> : GenericActionFromBizDtoSetup<TBizOut, TDtoOut>
        where TBizOut : class
        where TDtoOut : GenericActionFromBizDto<TBizOut, TDtoOut>, new()
    {
        /// <summary>
        /// This is called after the mapping from the BizData to the GenericActionDto has been done.
        /// Useful if the biz method returns say primary keys only and you would like to look up
        /// data to show to the user.
        /// </summary>
        /// <param name="repository"></param>
        protected internal virtual void SetupSecondaryOutputData(object repository)
        {
        }

        /// <summary>
        /// This copies the Business logic's output data into the GenericAction's DTO class.
        /// Override this if you need to do some more complex calculation during the copy 
        /// Note: Look at AutoMapperSetup method first as that can handle a number of mapping issues
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="mapper"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        protected internal virtual TDtoOut CopyFromBizData(object repository, IMapper mapper, TBizOut source)
        {
            return mapper.Map<TDtoOut>(source);
        }
    }
}