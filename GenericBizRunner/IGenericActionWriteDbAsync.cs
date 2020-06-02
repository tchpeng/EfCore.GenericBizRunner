﻿// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.
namespace GenericBizRunner
{
    /// <summary>
    /// This is an async Action that takes an input and returns a result TOut
    /// It updates the database and therefore requires EF SaveChanges to be called to persist the changes
    /// Note: Only be used if type of DbContext is being used and supplied via IRepository
    /// </summary>
    /// <typeparam name="TIn">Input to the business logic</typeparam>
    /// <typeparam name="TOut">Output from the business logic</typeparam>
    public interface IGenericActionWriteDbAsync<in TIn, TOut> : IGenericActionAsync<TIn, TOut>
    {}
}