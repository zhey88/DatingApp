//namespace should be the physical location of the file, else it will have errors
﻿using System.Text.Json;

namespace API.Extensions
{
    public static class HttpExtensions
    {
        public static void AddPaginationHeader(this HttpResponse response, PaginationHeader header)
        {
            //we need to serialize this into JSON so that it can go back with the header, 
            //which it needs to be in JSON formats
            var jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            response.Headers.Append("Pagination", JsonSerializer.Serialize(header, jsonOptions));
            //it is a custom header,need to do something to explicitly 
            //allow cause policy inside here as well
            //Otherwise, our clients will not be able to access the information inside this header
            response.Headers.Append("Access-Control-Expose-Headers", "Pagination");
        }
    }
}