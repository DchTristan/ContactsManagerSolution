using Entities;
using Microsoft.AspNetCore.Http;
using OfficeOpenXml;
using RepositoryContracts;
using ServiceContracts;
using ServiceContracts.DTO;

namespace Services
{
    public class CountriesUploadFromExcelService : ICountriesUploadFromExcelService
    {
        // private field
        private readonly ICountriesRepository _countriesRepository;

        //constructor
        public CountriesUploadFromExcelService(ICountriesRepository countriesRepository)
        {
            _countriesRepository = countriesRepository;
        }


        public async Task<int> UploadCountriesFromExcelFile(IFormFile formFile)
        {
            MemoryStream memoryStream = new MemoryStream();
           await formFile.CopyToAsync(memoryStream);

            int countriesInserted = 0;

            using (ExcelPackage excelPackage = new ExcelPackage(memoryStream))
            {
              ExcelWorksheet workSheet =  excelPackage.Workbook.Worksheets["Countries"];

                int rowCount = workSheet.Dimension.Rows;

                

                for(int row =2; row <= rowCount; row++)
                {
                 string? CellValue = Convert.ToString(workSheet.Cells[row, 1].Value);

                    if(!string.IsNullOrEmpty(CellValue))
                    {
                        string? countryName = CellValue;

                        if(await _countriesRepository.GetCountryByCountryName(countryName) == null)
                            {
                            Country country = new Country()
                            {
                                CountryName = countryName,
                            };
                          await  _countriesRepository.AddCountry(country);


                            countriesInserted++;
                            }
                    }
                }
            }
            return countriesInserted;
        }
    }
}

