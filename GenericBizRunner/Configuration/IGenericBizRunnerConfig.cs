﻿// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using Microsoft.EntityFrameworkCore;
using StatusGeneric;
using System;

namespace GenericBizRunner.Configuration
{
    /// <summary>
    /// This is the signature for the method you can add which will be called before SaveChanges
    /// Note: Only be used if type of DbContext is being used and supplied via IRepository
    /// </summary>
    /// <param name="context">Access to DbContext</param>
    /// <returns></returns>
    public delegate IStatusGeneric BeforeSaveChangesBizRunner(DbContext context);

    /// <summary>
    /// This is the interface to the configuration information for the GenericBizRunner
    /// </summary>
    public interface IGenericBizRunnerConfig
    {
        /// <summary>
        /// By default the call to SaveChanges will have validation added to it, as its often useful to validate 
        /// the data that your business logic writes to the database 
        /// (Validation inspects data attributes like [MaxLength], plus the IValidatableObject interface if present)
        /// However, validation does take longer, so you can turn it off globally by setting this to true.
        /// Alternatively you can add the IDoNotValidateSaveChanges interface to individual business logic interface definitions
        /// Note: Only be used if type of DbContext is being used and supplied via IRepository
        /// </summary>
        bool DoNotValidateSaveChanges { get; set; }

        /// <summary>
        /// GenericBizRunner uses a static variable to cache the decoding of bizLogic interfaces to their componnent parts
        /// For normal use this should be false, but for unit testing it should be true 
        /// </summary>
        bool TurnOffCaching { get; set; }

        /// <summary>
        /// If the business logic does not set a success message then this default message will be returned on a success.
        /// </summary>
        string DefaultSuccessMessage { get; set; }

        /// <summary>
        /// If the business logic writes to the database and does not set a success message then this default message will be returned on a success.
        /// Note: Only be used if type of DbContext is being used and supplied via IRepository
        /// </summary>
        string DefaultSuccessAndWriteMessage { get; set; }

        /// <summary>
        /// If the business logic writes to the database and its success the message does not end with a full stop, 
        /// then this message will be appended this message to the end of the message.
        /// Note: Only be used if type of DbContext is being used and supplied via IRepository
        /// </summary>
        string AppendToMessageOnGoodWriteToDb { get; set; }

        /// <summary>
        /// This updates the message on a successful write to the database
        /// Note: Only be used if type of DbContext is being used and supplied via IRepository
        /// </summary>
        Action<IBizActionStatus, IGenericBizRunnerConfig> UpdateSuccessMessageOnGoodWrite { get; }

        /// <summary>
        /// When SaveChangesWithValidation is called if there is an exception then this method
        /// is called. If it returns null then the error is rethrown, but if it returns a ValidationResult
        /// then that is turned into a error message that is shown to the user via the IBizActionStatus
        /// See section 10.7.3 of my book "Entity Framework Core in Action" on how to use this to turn
        /// SQL errors into user-friendly errors
        /// Note: Only be used if type of DbContext is being used and supplied via IRepository
        /// </summary>
        Func<Exception, DbContext, IStatusGeneric> SaveChangesExceptionHandler { get; }

        /// <summary>
        /// This will be called just before SaveChanges/SaveChangesAsync is called.
        /// I allows you to do things like your own validation, logging, etc. without needing to put code inside your application's DbContext
        /// If the status returned by BeforeSaveChanges has errors, then SaveChanges won't be called. 
        /// Note: Only be used if type of DbContext is being used and supplied via IRepository
        /// </summary>
        BeforeSaveChangesBizRunner BeforeSaveChanges { get; }
    }
}