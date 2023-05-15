﻿using Microsoft.AspNetCore.Mvc;
using PostalCode.API.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PostcodeLookup.API.Interfaces
{
    public interface IPostCodesLookupController
    {
        Task<ActionResult<PostcodeResult>> LookupPostcodeAsync(string postcode);
        Task<ActionResult<IEnumerable<SearchResult>>> AutocompletePostcode(string partialpostcode);


    }
}
