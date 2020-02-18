using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using bit.api.Domain.Contracts;
using bit.api.Domain.Services;
using bit.common.Auth;
using bit.common.Aws;
using bit.common.Contracts;
using bit.common.Email;
using bit.common.Email.Contracts;
using bit.common.Email.Services;
using bit.common.Mongo;
using bit.common.Security;
using bit.common.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace bit.api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("AllowWebAppAccess", builder =>
                {
                    builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials();
                });
            });
            services.AddMvc()
                .AddJsonOptions(options =>
                {
                    options.SerializerSettings.ContractResolver
                        = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver();
                });
            services.AddMongoDB(Configuration);
            services.AddJwt(Configuration);
            services.AddAws(Configuration);
            services.AddSendGrid(Configuration);
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<ICountryService, CountryService>();
            services.AddScoped<IResidenceService, ResidenceService>();
            services.AddScoped<IBlockService, BlockService>();
            services.AddScoped<IPostService, PostService>();
            services.AddScoped<ICommentService, CommentService>();
            services.AddScoped<IProfileService, ProfileService>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            Func<IServiceProvider, IPrincipal> getPrincipal =
                     (sp) => sp.GetService<IHttpContextAccessor>().HttpContext.User;
            services.AddScoped(typeof(Func<IPrincipal>), sp =>
            {
                Func<IPrincipal> func = () =>
                {
                    return getPrincipal(sp);
                };
                return func;
            });
            services.AddScoped<IUserAppContext, UserAppContext>();
            services.AddSingleton<IPasswordStorage, PasswordStorage>();
            services.AddScoped<IJwtHandler, JwtHandler>();
            services.AddScoped<IWeb3Service, Web3Service>();
            services.AddScoped<IFileService, FileService>();
            services.AddScoped<IAwsService, AwsService>();
            services.AddScoped<IEmailService, EmailService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseCors("AllowWebAppAccess");
            app.UseMvc();
        }
    }
}
