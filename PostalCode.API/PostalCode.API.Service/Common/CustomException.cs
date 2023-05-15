using Microsoft.Extensions.Logging;
using System;

namespace PostalCode.API.Service.Common
{
    public class CustomException: Exception
    {
        private readonly ILogger<CustomException> _logger;
        public CustomException(ILogger<CustomException> logger ) { 
        _logger= logger;
        }

        public int StatusCode { get; }
        public CustomException(ILogger _logger, int statusCode, string message) : base(message)
        {
            _logger.LogError(statusCode, message); 
            StatusCode = statusCode;
        }
    }
}
