using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Seasharpbooking
{
    public static class ApiConnection
    {
        public static HttpClient ApiClient = new HttpClient();

        static ApiConnection()
        {
            ApiClient.BaseAddress = new Uri("http://localhost:57101/");
            ApiClient.DefaultRequestHeaders.Clear();
            ApiClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
    }
}