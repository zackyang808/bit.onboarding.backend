using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace bit.common.Email
{
    public static class Extension
    {
        public static void AddSendGrid(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<SendGridOption>(configuration.GetSection("sendGrid"));
        }
    }
}
