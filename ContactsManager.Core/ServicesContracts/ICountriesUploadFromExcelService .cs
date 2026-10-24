using Microsoft.AspNetCore.Http;
using ServiceContracts.DTO;

namespace ServiceContracts
{
    /// <summary>
    /// Represents business logic for manipulating Country entity
    /// </summary>
    public interface ICountriesUploadFromExcelService
    {  /// <summary>
        /// Uploads countries from excel file into database
        /// </summary>
        /// <param name="formFile"> Excel file with list of countries added</param>
        /// <returns></returns>
        Task<int> UploadCountriesFromExcelFile(IFormFile formFile);
    }
}
