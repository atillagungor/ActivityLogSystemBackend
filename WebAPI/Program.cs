using Autofac;
using Autofac.Extensions.DependencyInjection;
using Business;
using Core;
using Core.CrossCuttingConcerns.Exceptions.Extensions;
using Core.Utilities.Security.Encyption;
using Core.Utilities.Security.Jwt;
using DataAccess;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Serilog yapýlandýrmasý
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.File(@"C:\Logs\log.txt")
            .CreateLogger();

        // Mevcut log saðlayýcýlarý temizleme ve Serilog'u ekleme
        builder.Host.UseSerilog((context, services, configuration) => configuration
            .ReadFrom.Configuration(context.Configuration)
            .ReadFrom.Services(services)
            .WriteTo.File(@"C:\Logs\log.txt"));

        // Add services to the container.
        builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
        builder.Host.ConfigureContainer<ContainerBuilder>(myBuilder => myBuilder.RegisterModule(new AutofacBusinessModule()));

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("myPolicy",
            builder =>
            {
                builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
            });
        });
        builder.Services.AddControllers();
        builder.Services.AddCoreServices();
        builder.Services.AddBusinessServices();
        builder.Services.AddDataAccessServices(builder.Configuration);

        var tokenOptions = builder.Configuration.GetSection("TokenOptions").Get<TokenOptions>();

        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidIssuer = tokenOptions.Issuer,
                    ValidAudience = tokenOptions.Audience,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = SecurityKeyHelper.CreateSecurityKey(tokenOptions.SecurityKey)
                };
            });

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(setup =>
        {
            setup.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Please enter a valid token",
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "Bearer"
            });
            setup.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new List<string>()
                }
            });
        });

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseCors("myPolicy");

        app.UseAuthentication();

        app.UseAuthorization();

        app.ConfigureCustomExceptionMiddleware();

        app.MapControllers();

        app.Run();
    }
}
