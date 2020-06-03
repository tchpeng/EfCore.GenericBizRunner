// Original work Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Modified work Copyright (c) 2020 tchpeng, GitHub: tchpeng
// Licensed under MIT license. See License.txt in the project root for license information.

using GenericBizRunner.Configuration.Internal;
using GenericBizRunner.PublicButHidden;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace GenericBizRunner.Configuration
{
    /// <summary>
    /// This contains the code to register the GenericBizRunner library and also find and GenericBizRunner DTO and 
    /// </summary>
    public static class NetCoreBizRunnerDi
    {
        /// <summary>
        /// This is the method for registering GenericBizRunner and any GenericBizRunner DTOs with .NET Core DI provider
        /// </summary>
        /// <typeparam name="TDefaultRepository"></typeparam>
        /// <param name="services"></param>
        /// <param name="assembliesToScan">These are the assemblies to scan for DTOs</param>
        public static void RegisterBizRunnerWithDtoScans<TDefaultRepository>(this IServiceCollection services, params Assembly[] assembliesToScan)
            where TDefaultRepository : IRepository
        {
            services.RegisterBizRunnerWithDtoScans<TDefaultRepository>(new GenericBizRunnerConfig(), assembliesToScan);
        }

        /// <summary>
        /// This is the method for registering GenericBizRunner and any GenericBizRunner DTOs with .NET Core DI provider with config
        /// </summary>
        /// <typeparam name="TDefaultRepository"></typeparam>
        /// <param name="services"></param>
        /// <param name="config"></param>
        /// <param name="assembliesToScan">These are the assemblies to scan for DTOs</param>
        public static void RegisterBizRunnerWithDtoScans<TDefaultRepository>(this IServiceCollection services, IGenericBizRunnerConfig config,
            params Assembly[] assembliesToScan)
            where TDefaultRepository : IRepository
        {
            if (config == null) throw new ArgumentNullException(nameof(config));

            services.AddScoped<IRepository>(sp => sp.GetService<TDefaultRepository>());
            services.AddTransient(typeof(IActionService<>), typeof(ActionService<>));
            services.AddTransient(typeof(IActionServiceAsync<>), typeof(ActionServiceAsync<>));

            services.BuildRegisterWrappedConfig(config, assembliesToScan);
        }

        /// <summary>
        /// This is used to register GenericBizRunner and any GenericBizRunner DTOs with .NET Core DI provider to work with multiple repository
        /// </summary>
        /// <param name="services"></param>
        /// <param name="assembliesToScan">These are the assemblies to scan for DTOs</param>
        public static void RegisterBizRunnerMultiRepositoryWithDtoScans(this IServiceCollection services, params Assembly[] assembliesToScan)
        {
            services.RegisterBizRunnerMultiRepositoryWithDtoScans(new GenericBizRunnerConfig(), assembliesToScan);
        }

        /// <summary>
        /// This is used to register GenericBizRunner and any GenericBizRunner DTOs with .NET Core DI provider to work with multiple repository
        /// </summary>
        /// <param name="services"></param>
        /// <param name="config"></param>
        /// <param name="assembliesToScan">These are the assemblies to scan for DTOs</param>
        public static void RegisterBizRunnerMultiRepositoryWithDtoScans(this IServiceCollection services, IGenericBizRunnerConfig config,
            params Assembly[] assembliesToScan)
        {
            if (config == null) throw new ArgumentNullException(nameof(config));

            services.AddTransient(typeof(IActionService<,>), typeof(ActionService<,>));
            services.AddTransient(typeof(IActionServiceAsync<,>), typeof(ActionServiceAsync<,>));

            services.BuildRegisterWrappedConfig(config, assembliesToScan);
        }

        //---------------------------------------------------------
        //private parts

        private static void BuildRegisterWrappedConfig(this IServiceCollection services, 
            IGenericBizRunnerConfig config, params Assembly[] assembliesToScan)
        {
            //It is possible that the user would use both default repository and multiple repository, so we only add if not already there
            if (!services.Contains(
                new ServiceDescriptor(typeof(IWrappedBizRunnerConfigAndMappings), config), new CheckDescriptor()))
            {
                var wrapBuilder = new SetupDtoMappings(config);
                var wrapperConfig = wrapBuilder.BuildWrappedConfigByScanningForDtos(assembliesToScan.Distinct().ToArray(), config);
                //Register the IWrappedBizRunnerConfigAndMappings
                services.AddSingleton(wrapperConfig);
            }
        }

        private class CheckDescriptor : IEqualityComparer<ServiceDescriptor>
        {
            public bool Equals(ServiceDescriptor x, ServiceDescriptor y)
            {
                return x.ServiceType == y.ServiceType
                       && x.ImplementationType == y.ImplementationType
                       && x.Lifetime == y.Lifetime;
            }

            public int GetHashCode(ServiceDescriptor obj)
            {
                throw new NotImplementedException();
            }
        }
    }
}