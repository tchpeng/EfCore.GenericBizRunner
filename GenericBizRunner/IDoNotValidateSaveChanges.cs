// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.
namespace GenericBizRunner
{
    /// <summary>
    /// If you add this interface to the business logic then it will not validate the data when calling SaveChanges
    /// Note: Only be used if type of DbContext is being used and supplied via IRepository
    /// </summary>
    public interface IDoNotValidateSaveChanges
    {}
}