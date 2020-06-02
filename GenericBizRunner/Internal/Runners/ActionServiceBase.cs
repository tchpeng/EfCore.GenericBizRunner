// Original work Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Modified work Copyright (c) 2020 tchpeng, GitHub: tchpeng
// Licensed under MIT license. See License.txt in the project root for license information.

using GenericBizRunner.Helpers;
using GenericBizRunner.PublicButHidden;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace GenericBizRunner.Internal.Runners
{
    internal abstract class ActionServiceBase
    {
        protected ActionServiceBase(bool requiresSaveChanges, IWrappedBizRunnerConfigAndMappings wrappedConfig)
        {
            RequiresSaveChanges = requiresSaveChanges;
            WrappedConfig = wrappedConfig;
        }

        /// <summary>
        /// This contains info on whether SaveChanges (with validation) should be called after a successful business logic has run
        /// Note: Only be used if type of DbContext is being used and supplied via IRepository
        /// </summary>
        private bool RequiresSaveChanges { get; }

        protected IWrappedBizRunnerConfigAndMappings WrappedConfig { get; }

        /// <summary>
        /// This handled optional save to database with various validation and/or handlers
        /// Note: if it did save successfully to the database it alters the message
        /// Note: Only be used if type of DbContext is being used and supplied via IRepository
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="bizStatus"></param>
        /// <returns></returns>
        protected void SaveChangedIfRequiredAndNoErrors(object repository, IBizActionStatus bizStatus)
        {
            if (repository.GetType().IsSubclassOf(typeof(DbContext)))
            {
                try
                {
                    DbContext db = (DbContext)repository;
                    if (!bizStatus.HasErrors && RequiresSaveChanges && db != null)
                    {
                        bizStatus.CombineStatuses(db.SaveChangesWithOptionalValidation(
                            bizStatus.ShouldValidateSaveChanges(WrappedConfig.Config), WrappedConfig.Config));
                        WrappedConfig.Config.UpdateSuccessMessageOnGoodWrite(bizStatus, WrappedConfig.Config);
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// This handled optional save to database with various validation and/or handlers
        /// Note: if it did save successfully to the database it alters the message
        /// Note: Only be used if type of DbContext is being used and supplied via IRepository
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="bizStatus"></param>
        /// <returns></returns>
        protected async Task SaveChangedIfRequiredAndNoErrorsAsync(object repository, IBizActionStatus bizStatus)
        {
            if (repository.GetType().IsSubclassOf(typeof(DbContext)))
            {
                try
                {
                    DbContext db = (DbContext)repository;
                    if (!bizStatus.HasErrors && RequiresSaveChanges && db != null)
                    {
                        bizStatus.CombineStatuses(await db.SaveChangesWithOptionalValidationAsync(
                            bizStatus.ShouldValidateSaveChanges(WrappedConfig.Config), WrappedConfig.Config)
                                .ConfigureAwait(false));
                        WrappedConfig.Config.UpdateSuccessMessageOnGoodWrite(bizStatus, WrappedConfig.Config);
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
    }
}