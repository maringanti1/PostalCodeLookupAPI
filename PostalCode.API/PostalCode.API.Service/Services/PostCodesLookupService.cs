using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PostalCode.API.Model;
using PostalCode.API.Service.Common;
using PostalCode.API.Service.Interfaces;
using PostalCode.API.Service.Mapper;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace PostalCode.API.Service
{
    public class PostCodesLookupService : IPostCodesLookupService
    {
        private readonly IHttpClientRepository _repository;
        private readonly SearchResultMapper _searchResultMapper;
        private readonly PostCodeAPIConfig _postCodeAPIConfig;
        private readonly ILogger _logger;
        private readonly JsonSerializerOptions _options;
        public PostCodesLookupService(IHttpClientRepository repository,
            IConfiguration configuration, SearchResultMapper searchResultMapper,
            PostCodeAPIConfig postCodeAPIConfig, ILogger logger)
        { 
            _options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            _repository = repository;
            _searchResultMapper = searchResultMapper;
            _logger = logger;
            _postCodeAPIConfig = postCodeAPIConfig;
        }

        public async Task<PostcodeResult> LookupPostcode(string postcode)
        {
            _logger.LogInformation("Lookup post code execution is stearted");
            var lookupEndpoint = string.Format(_postCodeAPIConfig.LookupEndpoint, postcode);
            var limit = _postCodeAPIConfig.PostCodeAPILimit;
            var response = await _repository.GetAsync($"{_postCodeAPIConfig.BaseURL}{lookupEndpoint}?limit={limit}");

            if (response.IsSuccessStatusCode)
            {
                var jsonContent = await response.Content.ReadAsStringAsync();
                PostcodeResponse postcodeResponse = JsonSerializer.Deserialize<PostcodeResponse>(jsonContent, _options);
                _logger.LogInformation("Lookup post code execution is completed");
                return postcodeResponse.Result;
            }

            throw new CustomException(_logger, (int)response.StatusCode, response.ReasonPhrase);
        }


        public async Task<IEnumerable<SearchResult>> AutocompletePostcode(string partialPostcode)
        {
            _logger.LogInformation("Auto complete postcode execution is stearted");
            List<SearchResult> searchResults = new List<SearchResult>();
            var AutoCompleteEndpoint = string.Format(_postCodeAPIConfig.AutoCompleteEndpoint, partialPostcode);
            var limit = _postCodeAPIConfig.PostCodeAPILimit;
            var response = await _repository.GetAsync($"{_postCodeAPIConfig.BaseURL}{AutoCompleteEndpoint}?limit={limit}");
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();


                AutocompleteResponse autocompleteResponse = JsonSerializer.Deserialize<AutocompleteResponse>(result, _options);
                if (autocompleteResponse.Result != null)
                {
                    foreach (var postcode in autocompleteResponse.Result)
                    {
                        var lookupEndpoint = string.Format(_postCodeAPIConfig.LookupEndpoint, postcode);
                        var data = await _repository.GetAsync($"{_postCodeAPIConfig.BaseURL}{lookupEndpoint}?limit={limit}");
                        if (data.IsSuccessStatusCode)
                        {
                            var jsonContent = await data.Content.ReadAsStringAsync();
                            PostcodeResponse postcodeResponse = JsonSerializer.Deserialize<PostcodeResponse>(jsonContent, _options);
                            searchResults.Add(_searchResultMapper.Map(postcodeResponse));
                        }
                    }
                }
                _logger.LogInformation("Auto complete postcode execution is completed");
                return searchResults;
            }
            throw new CustomException(_logger, (int)response.StatusCode, response.ReasonPhrase);
        }
    }

}
