using ServiceContracts.DTO;
using ServiceContracts.Enums;
using System;


namespace ServiceContracts
{
    /// <summary>
    /// Represents business logic for manipulating Person entity
    /// </summary>
    public interface IPersonsDeleterService 
    {
        /// <summary>
        /// Delete a person based on the given person ID
        /// </summary>
        /// <param name="PersonID"> PersonID to delete</param>
        /// <returns>Returns true, if the deletion is successful otherwise false</returns>
        Task<bool> DeletePerson(Guid? personID);

    }
}
