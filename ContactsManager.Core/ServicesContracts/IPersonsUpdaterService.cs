using ServiceContracts.DTO;
using ServiceContracts.Enums;
using System;


namespace ServiceContracts
{
    /// <summary>
    /// Represents business logic for manipulating Person entity
    /// </summary>
    public interface IPersonsUpdaterService
    {
        /// <summary>
        /// Update the specified person details based on the given person ID
        /// </summary>
        /// <param name="personUpdateRequest">Person details to update including person ID</param>
        /// <returns>Returns the person response object after updation</returns>
      Task<PersonResponse>  UpDatePerson(PersonUpdateRequest? personUpdateRequest);
 
    }
}
