using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PostalCode.API.Model;
using PostalCode.API.Service.Common;
using PostalCode.API.Service.Interfaces;
using PostcodeLookup.API.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PostcodeLookup.API.Controllers
{
    [EnableCors("AllowOrigin")]
    [ApiController]
    [Route("api/[controller]")]    
    public class PostCodesLookupController : IPostCodesLookupController
    { 
        private readonly IPostCodesLookupService _service;
        private readonly ILogger _logger; 
        public PostCodesLookupController(ILogger logger, 
            IPostCodesLookupService service )  
        {
            _logger = logger;
            _service = service; 
        }

        [HttpGet("autocomplete/{partialpostcode}")]
        public async Task<ActionResult<IEnumerable<SearchResult>>> AutocompletePostcode(string partialpostcode)
        {
            _logger.LogInformation("Auto complete postcode execution is started");
            try
            {
                
                if (string.IsNullOrEmpty(partialpostcode))
                {
                    throw new Exception("Autocomplete postcode is missing or empty");
                }

                var result = await _service.AutocompletePostcode(partialpostcode);
                return result.ToList();
            }
            catch (CustomException ex)
            {
                throw new CustomException(_logger, ex.StatusCode, ex.Message);
            }
            finally
            {
                _logger.LogInformation("Auto complete postcode execution is completed");
            }
        }

        [HttpGet("lookup/{postcode}")]
        public async Task<ActionResult<PostcodeResult>> LookupPostcodeAsync(string postcode)
        {
            _logger.LogInformation("Lookup postcode execution is started");
            try
            {
                 
                if (string.IsNullOrEmpty(postcode))
                {
                    // Handle missing or empty config value
                    throw new Exception("Postcode is missing or empty!");
                }
                var result = await _service.LookupPostcode(postcode);
                return result;
            }
            catch (CustomException ex)
            {
                throw new CustomException(_logger, ex.StatusCode, ex.Message); ;
            }
            finally
            {
                _logger.LogInformation("Lookup postcode execution is completed");
            
            }
        }
    }
}
