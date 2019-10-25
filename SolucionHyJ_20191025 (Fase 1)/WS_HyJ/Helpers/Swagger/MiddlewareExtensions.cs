using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WS_HyJ.Helpers.Swagger
{
    /// <summary>
    /// Extending Swagger services
    /// </summary>
    public static class MiddlewareExtensions
    {
        /// <summary>
        /// Enabling Swagger UI.
        /// Excluding from test environment
        /// </summary>
        /// <param name="app">IApplicationBuilder</param>
        public static void UseSwaggerDocuments(this IApplicationBuilder app)
        {
            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Test")
            {
                return;
            }

            app.UseSwagger();

            app.UseSwaggerUI();
        }
    }
}
