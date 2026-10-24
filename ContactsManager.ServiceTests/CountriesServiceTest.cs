using System;
using System.Collections.Generic;
using System.Linq;
using Entities;
using ServiceContracts.DTO;
using ServiceContracts;
using Services;
using RepositoryContracts;
using Xunit.Abstractions;
using Moq;
using AutoFixture;
using Xunit;
using Serilog;
using Microsoft.Extensions.Logging;

namespace CRUDTests
{
    public class CountriesServiceTest
    {
        //private readonly fields
        private readonly ICountriesAdderService _countriesAdderService;
        private readonly ICountriesGetterService _countriesGetterService;
        private readonly ICountriesUploadFromExcelService _countriesUploadFromExcelService;
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly ICountriesRepository _countriesRepository;
        private readonly Mock<ICountriesRepository> _countriesRepositoryMock;
        private readonly IFixture _fixture;

        //constructor
        public CountriesServiceTest(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
            _fixture = new Fixture();
            _countriesRepositoryMock = new Mock<ICountriesRepository>();
            _countriesRepository = _countriesRepositoryMock.Object;

            _countriesGetterService = new CountriesGetterService(_countriesRepository);
            _countriesAdderService = new CountriesAdderService(_countriesRepository);
            _countriesUploadFromExcelService = new CountriesUploadFromExcelService(_countriesRepository);
        }

        #region AddCountry

        //When CountryAddRequest is null, it should throw ArgumentNullException
        [Fact]
        public async Task AddCountry_NullCountry()
        {
            //Arrange
            CountryAddRequest? request = null;

            //Act
            Func<Task> action = async () =>
            {
                await _countriesAdderService.AddCountry(request);
            };

            //Assert
            await Assert.ThrowsAsync<ArgumentNullException>(action);
        }

        //When the CountryName is null, it should throw ArgumentException
        [Fact]
        public async Task AddCountry_CountryNameIsNull()
        {
            //Arrange
            CountryAddRequest? request = _fixture.Build<CountryAddRequest>()
                .With(temp => temp.CountryName, null as string)
                .Create();

            //Act
            Func<Task> action = async () =>
            {
                await _countriesAdderService.AddCountry(request);
            };

            //Assert
            await Assert.ThrowsAsync<ArgumentException>(action);
        }

        //When the CountryName is duplicate, it should throw ArgumentException
        [Fact]
        public async Task AddCountry_DuplicateCountryName()
        {
            //Arrange
            CountryAddRequest? request1 = new CountryAddRequest() { CountryName = "Cameroon" };
            CountryAddRequest? request2 = new CountryAddRequest() { CountryName = "Cameroon" };

            Country country1 = request1.ToCountry();

            // Premier appel : le repository ne trouve rien -> pas de doublon
            // Deuxième appel : on simule que "Cameroon" existe déjà
            _countriesRepositoryMock
                .SetupSequence(temp => temp.GetCountryByCountryName(It.IsAny<string>()))
                .ReturnsAsync((Country?)null)   // pour request1
                .ReturnsAsync(country1);        // pour request2 -> doublon détecté

            _countriesRepositoryMock
                .Setup(temp => temp.AddCountry(It.IsAny<Country>()))
                .ReturnsAsync(country1);

            //Act
            Func<Task> action = async () =>
            {
                await _countriesAdderService.AddCountry(request1);
                await _countriesAdderService.AddCountry(request2);
            };

            //Assert
            await Assert.ThrowsAsync<ArgumentException>(action);
        }

        // When you supply proper country name, it should insert (add) the country
        [Fact]
        public async Task AddCountry_ProperCountryDetails()
        {
            //Arrange
            CountryAddRequest? request = new CountryAddRequest() { CountryName = "Japan" };
            Country country = request.ToCountry();
            country.CountryID = Guid.NewGuid();

            _countriesRepositoryMock
                .Setup(temp => temp.GetCountryByCountryName(It.IsAny<string>()))
                .ReturnsAsync((Country?)null);

            _countriesRepositoryMock
                .Setup(temp => temp.AddCountry(It.IsAny<Country>()))
                .ReturnsAsync(country);

            //Act
            CountryResponse response = await _countriesAdderService.AddCountry(request);

            //Assert
            Assert.True(response.CountryID != Guid.Empty);
        }

        #endregion

        #region GetAllCountries

        //The list of countries should be empty by default (before adding any countries)
        [Fact]
        public async Task GetAllCountries_EmptyList()
        {
            //Arrange
            _countriesRepositoryMock
                .Setup(temp => temp.GetAllCountries())
                .ReturnsAsync(new List<Country>());

            //Act
            List<CountryResponse> actual_country_response_list = await _countriesGetterService.GetAllCountries();

            //Assert
            Assert.Empty(actual_country_response_list);
        }

        [Fact]
        public async Task GetAllCountries_AddFewCountries()
        {
            //Arrange
            List<Country> countries = new List<Country>()
            {
                _fixture.Build<Country>().With(temp => temp.CountryName, "USA")
                .Without(temp =>temp.Persons)
                .Create(),
                _fixture.Build<Country>().With(temp => temp.CountryName, "UK")
                 .Without(temp =>temp.Persons)
                .Create()
            };

            List<CountryResponse> countries_response_list_expected =
                countries.Select(temp => temp.ToCountryResponse()).ToList();

            _countriesRepositoryMock
                .Setup(temp => temp.GetAllCountries())
                .ReturnsAsync(countries);

            //Act
            List<CountryResponse> actualCountryResponseList = await _countriesGetterService.GetAllCountries();

            //Assert
            foreach (CountryResponse expected_country in countries_response_list_expected)
            {
                Assert.Contains(expected_country, actualCountryResponseList);
            }
        }

        #endregion

        #region GetCountryByCountryID

        /// <summary>
        /// If we supply null as countryID, it should return null as CountryResponse
        /// </summary>
        [Fact]
        public async Task GetCountryByCountryID_NullCountryID()
        {
            //Arrange
            Guid? countryID = null;

            //Act
            CountryResponse? country_response_from_get_method =
                await _countriesGetterService.GetCountryByCountryID(countryID);

            //Assert
            Assert.Null(country_response_from_get_method);
        }

        // If we supply a valid country id, it should return the matching country details
        [Fact]
        public async Task GetCountryByCountryID_ValidCountryID()
        {
            //Arrange
            Country country = _fixture.Build<Country>()
                .With(temp => temp.CountryName, "China")
                 .Without(temp => temp.Persons)
                .Create();

            CountryResponse country_response_expected = country.ToCountryResponse();

            _countriesRepositoryMock
                .Setup(temp => temp.GetCountryByCountryID(It.IsAny<Guid>()))
                .ReturnsAsync(country);

            //Act
            CountryResponse? country_response_from_get =
                await _countriesGetterService.GetCountryByCountryID(country.CountryID);

            //Assert
            Assert.Equal(country_response_expected, country_response_from_get);
        }

        #endregion
    }
}