using PostalCode.API.Model;
using PostalCode.API.Service.Common;

namespace PostalCode.API.Service.Mapper
{
    public class SearchResultMapper
    { 
        
        private readonly PostCodeAPIConfig _postCodeAPIConfig;
        public SearchResultMapper(PostCodeAPIConfig postCodeAPIConfig)
        {
            _postCodeAPIConfig = postCodeAPIConfig;
        }

        public SearchResult Map(PostcodeResponse postcodeResponse)
        {
            var searchResult = new SearchResult();
            searchResult.Country = postcodeResponse.Result.Country;
            searchResult.Region = postcodeResponse.Result.Region;
            searchResult.ParliamentaryConstituency = postcodeResponse.Result.ParliamentaryConstituency;
            searchResult.AdminDistrict = postcodeResponse.Result.AdminDistrict;
            searchResult.Postcode = postcodeResponse.Result.Postcode; 

            double latitude;
            if (double.TryParse(_postCodeAPIConfig.LatitudeSouth, out double south) &&
                double.TryParse(_postCodeAPIConfig.LatitudeMidlands, out double midlands))
            {
                if (double.TryParse(postcodeResponse.Result.Latitude.ToString(), out latitude))
                {
                    switch (latitude)
                    {
                        case double l when l < south:
                            searchResult.Area = "South";
                            break;
                        case double l when l >= south && l< midlands:
                            searchResult.Area = "Midlands";
                            break;
                        default:
                            searchResult.Area = "North";
                            break;
                    }
                }
            }

            return searchResult;
        }
    }
}
