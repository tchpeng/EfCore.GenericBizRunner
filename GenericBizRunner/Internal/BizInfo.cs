﻿// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using GenericBizRunner.PublicButHidden;
using System;
using System.Linq;
using System.Reflection;

namespace GenericBizRunner.Internal
{
    [Flags]
    internal enum RequestedInOut : byte
    {
        None = 0,
        In = 1,
        Out = 2,
        InOut = In | Out,
        InOrInOut = 4,
        OptionalAsync = 8,
        Async = 16,
        NonAsyncFlagsMask = InOut | InOrInOut,
        AllFlagsMask = NonAsyncFlagsMask | Async
    }

    internal class BizInfo
    {
        private readonly Type _extractedActionInterface;

        private readonly Type _iBizType;

        private readonly ServiceBuilderLookup _matchingServiceType;
        private dynamic _bizInstance;

        public BizInfo(Type iBizType, Type extractedActionInterface, ServiceBuilderLookup matchingServiceType)
        {
            _iBizType = iBizType;
            _extractedActionInterface = extractedActionInterface;
            _matchingServiceType = matchingServiceType;
        }

        /// <summary>
        /// True if an Async method
        /// </summary>
        public bool IsAsync => _matchingServiceType.TypeOfService.HasFlag(RequestedInOut.Async);

        /// <summary>
        /// True if the interface name contains "WriteDb"
        /// Note: Only be used if type of DbContext is being used and supplied via IRepository
        /// </summary>
        public bool RequiresSaveChanges => _matchingServiceType.RequiresSaveChanges;

        public override string ToString()
        {
            return string.Format(
                "IBizType: {0}, ExtractedActionInterface: {1}, MatchingServiceType: {2}, IsAsync: {3}, RequiresSaveChanges: {4}",
                _iBizType.Name, _extractedActionInterface.Name, _matchingServiceType, IsAsync, RequiresSaveChanges);
        }

        /// <summary>
        /// This is the instance that can be called to run the service
        /// </summary>
        public dynamic GetServiceInstance(IWrappedBizRunnerConfigAndMappings wrappedConfig)
        {
            return _bizInstance ??
                   (_bizInstance =
                       CreateRequiredServiceInstance(_matchingServiceType, _iBizType, _extractedActionInterface, wrappedConfig));
        }

        public Type GetBizInType()
        {
            if (!_matchingServiceType.TypeOfService.HasFlag(RequestedInOut.In))
                throw new InvalidOperationException("This business logic does not have an input type");

            return _extractedActionInterface.GetGenericArguments().First();
        }

        public Type GetBizOutType()
        {
            if (!_matchingServiceType.TypeOfService.HasFlag(RequestedInOut.Out))
                throw new InvalidOperationException("This business logic does not have an output type");

            return _extractedActionInterface.GetGenericArguments().Last();
        }

        /// <summary>
        /// When running a transaction we need to set the type of the output dynamically. 
        /// This method uses reflection to create the method we need to invoke to call the service
        /// </summary>
        /// <returns></returns>
        public MethodInfo GetRunMethod()
        {
            //Now build the type of the class so we can create the right Method
            var genericAgruments = _extractedActionInterface.GetGenericArguments().ToList();
            genericAgruments.Insert(0, _iBizType);
            var genericType = _matchingServiceType.ServiceHandleType.MakeGenericType(genericAgruments.ToArray());
            var genericRunMethod =
                genericType.GetMethod(IsAsync ? "RunBizActionDbAndInstanceAsync" : "RunBizActionDbAndInstance");
            if (genericRunMethod == null)
                throw new NullReferenceException(
                    "Could not find a run method in the created internal service instance.");

            return _matchingServiceType.TypeOfService.HasFlag(RequestedInOut.Out)
                ? genericRunMethod.MakeGenericMethod(GetBizOutType())
                : genericRunMethod;
        }

        //---------------------------------------------------
        //private methods

        private dynamic CreateRequiredServiceInstance(ServiceBuilderLookup serviceBaseInfo,
            Type iBizType, Type genericInterfacePart, IWrappedBizRunnerConfigAndMappings wrappedConfig)
        {
            var genericAgruments = genericInterfacePart.GetGenericArguments().ToList();
            genericAgruments.Insert(0, iBizType);
            return Activator.CreateInstance(
                serviceBaseInfo.ServiceHandleType.MakeGenericType(genericAgruments.ToArray()),
                new object[] { RequiresSaveChanges, wrappedConfig});
        }
    }
}