using CsvHelper;
using CsvHelper.Configuration;
using Entities;
using Exceptions;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using RepositoryContracts;
using Serilog;
using SerilogTimings;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Services.Helpers;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq.Expressions;

namespace Services
{
    public class PersonsUpdaterService : IPersonsUpdaterService
    {


        //private field
        private readonly IPersonsRepository _personsRepository;
        private readonly ILogger<PersonsUpdaterService> _logger;
        private readonly IDiagnosticContext _diagnosticContext;

        //constructor
        public PersonsUpdaterService(IPersonsRepository personsRepository, ILogger<PersonsUpdaterService> logger, IDiagnosticContext diagnosticContext)
        {
            _personsRepository = personsRepository;
            _logger = logger;
            _diagnosticContext = diagnosticContext;
        }

            public async Task<PersonResponse> UpDatePerson(PersonUpdateRequest? personUpdateRequest)
            {
                //check if the personUpdateRequest is null
                if (personUpdateRequest == null)
                    throw new ArgumentNullException(nameof(Person));

                //Validation
                ValidationHelper.ModelValidation(personUpdateRequest);

            //get matching object to update
            Person? matchingPerson = await _personsRepository.GetPersonByPersonID(personUpdateRequest.PersonID);
                if (matchingPerson == null)
                {
                    throw new InvalidPersonIDException("Given person id doesn't exist");
                }

                //update all details
                matchingPerson.PersonName = personUpdateRequest.PersonName;
                matchingPerson.Email = personUpdateRequest.Email;
                matchingPerson.Address = personUpdateRequest.Address;
                matchingPerson.Email = personUpdateRequest.Email;
                matchingPerson.Gender = personUpdateRequest.Gender.ToString();
                matchingPerson.DateOfBirth = personUpdateRequest.DateOfBirth;
                matchingPerson.ReceiveNewsLetters = personUpdateRequest.ReceiveNewsLetters;

              await  _personsRepository.UpdatePerson(matchingPerson); //UPDATE

                //return the same object into PersonResponse type.
                return matchingPerson.ToPersonResponse();
            }  
    }

       

    }

