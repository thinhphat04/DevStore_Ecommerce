﻿using Azure;
using BackEnd.Entities;
using System.Text.Json;

namespace BackEnd.RequestHelpers
{
    public static class HttpExtensions 
    {
        public static void AddPaginationHeader(this HttpResponse response, MetaData metaData) 
        {
            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

            response.Headers.Add("Pagination", JsonSerializer.Serialize(metaData, options));
            response.Headers.Add("Access-Control-Expose-Headers", "Pagination");
        }
    }
}
