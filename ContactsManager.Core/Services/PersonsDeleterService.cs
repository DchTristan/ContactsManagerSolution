using CsvHelper;
using CsvHelper.Configuration;
using Entities;
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
    public class PersonsDeleterService : IPersonsDeleterService
    {


        //private field
        private readonly IPersonsRepository _personsRepository;
        private readonly ILogger<PersonsDeleterService> _logger;
        private readonly IDiagnosticContext _diagnosticContext;

        //constructor
        public PersonsDeleterService(IPersonsRepository personsRepository, ILogger<PersonsDeleterService> logger, IDiagnosticContext diagnosticContext)
        {
            _personsRepository = personsRepository;
            _logger = logger;
            _diagnosticContext = diagnosticContext;
        }

            public async Task<bool> DeletePerson(Guid? personID)
            {
                //Check if the given id is null
                if (personID == null) throw new ArgumentNullException(nameof(personID));

                //get the matching person id
                Person? matchingPerson = await _personsRepository.GetPersonByPersonID(personID.Value);
                if (matchingPerson == null)
                    return false;

                //Delete the matching Person object from List<Person>
               await _personsRepository.DeletePersonByPersonID(personID.Value);

                return true;
            }

            
    }

       

    }

