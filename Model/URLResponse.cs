using System;
using System.Collections.Generic;
namespace rapide_shortener_service.Model
{
    public class URLResponse
    {
        public string Message { get; set; }
        public bool Success { get; set; }
        public URLModel Content { get; set; }
    }
}