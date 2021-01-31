using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.Json.Serialization;
using Cocotte.Hubs;
using CoreLib.Aggregations;
using CoreLib.Managers;
using CoreLib.Utils;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Serilog;

[assembly: ApiController]
namespace Cocotte.Core {
	/// <summary>
	/// Startup class.
	/// </summary>
	public class Startup {
		static readonly ILogger log = Log.ForContext(MethodBase.GetCurrentMethod().DeclaringType);

		/// <summary>
		/// Configuration of the server.
		/// </summary>
		public static IConfiguration Configuration { get; private set; }

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="configuration"></param>
		public Startup(IConfiguration configuration) => Configuration = configuration;

		/// <summary>
		/// This method gets called by the runtime. Use this method to add services to the container.
		/// </summary>
		/// <param name="services"></param>
		public void ConfigureServices(IServiceCollection services) {
			services.AddSingleton(Configuration.GetSection("AppConfiguration:AggregationTypes").Get<AggregationTypes>());
			services.AddSingleton(Configuration.GetSection("AppConfiguration:Settings").Get<Settings>());

			// Create CORS policy to apply.
			// Need to set the different origins into the appsettings file.
			services.AddCors(options => options.AddPolicy("CorsPolicy",
						builder => {
							builder.AllowAnyMethod()
							.AllowAnyHeader()
							.WithOrigins(Configuration.GetSection("AppConfiguration:corsOrigins").Get<string[]>())
							.AllowCredentials();
						}));

			//services.AddCors(options => options.AddPolicy("CorsPolicy",
			//			builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader().AllowCredentials()));

			// Register the Swagger generator, defining 1 or more Swagger documents.
			services.AddSwaggerGen(c => {
				c.SwaggerDoc("v1", new OpenApiInfo { Title = Settings.ApiName, Version = Settings.ApiVersion });

				c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme { 
					In = ParameterLocation.Header, 
					Description = @"Please put your session key here.",
					Name = "Authorization",
					Type = SecuritySchemeType.ApiKey,
					Scheme = "Bearer"
				});

				c.AddSecurityRequirement(new OpenApiSecurityRequirement { { new OpenApiSecurityScheme {
						Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" },
						Scheme = "oauth2",
						Name = "Bearer",
						In = ParameterLocation.Header,
					},
					new List<string>()
				} });

				// Set the comments path for the Swagger JSON and UI.
				var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
				var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
				c.IncludeXmlComments(xmlPath);
			});

			services.AddSwaggerGenNewtonsoftSupport();

			services.AddSingleton<EventHubHandler>();

			services.AddSignalR().AddJsonProtocol(options => options.PayloadSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

			services.AddControllers().AddNewtonsoftJson(options => {
				options.SerializerSettings.Converters.Add(new StringEnumConverter());
				options.SerializerSettings.PreserveReferencesHandling = PreserveReferencesHandling.Objects;
			});

			services.AddHttpContextAccessor();
		}

		/// <summary>
		/// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		/// </summary>
		/// <param name="app"></param>
		/// <param name="env"></param>
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {

			string basePath = Path.Combine(Tools.GetExecutableRootPath(), @"Data");
			string aggregationPipelineBasePath = string.Empty;
			string configFilePath = string.Empty;
			string credentialFilePath = string.Empty;

			if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) {
				log.Debug("Linux environment.");
				aggregationPipelineBasePath = $"{basePath}/AggregationPipelines/";
				configFilePath = $"{basePath}/MongoDB/MongoConfig.xml";
				credentialFilePath = $"{basePath}/MongoDB/MongoCredentials.xml";
			}
			else {
				log.Debug("Windows environment.");
				aggregationPipelineBasePath = $@"{basePath}\AggregationPipelines\";
				configFilePath = $@"{basePath}\MongoDB\MongoConfig.xml";
				credentialFilePath = $@"{basePath}\MongoDB\MongoCredentials.xml";
			}

			MongoManager.Configure(aggregationPipelineBasePath, configFilePath, credentialFilePath);

			log.Information($"{Settings.ApiName} started. Version = [{Assembly.GetExecutingAssembly().GetName().Version}].");

			if (env.IsDevelopment()) {
				app.UseDeveloperExceptionPage();
				log.Information("Development Env enabled.");
			}
			else
				log.Information("Production Env enabled.");


			// Enable middleware to serve generated Swagger as a JSON endpoint.
			app.UseSwagger(c => c.RouteTemplate = "/swagger/{documentName}/swagger.json");

			// Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
			// specifying the Swagger JSON endpoint.
			app.UseSwaggerUI(c => {
				c.SwaggerEndpoint("/swagger/v1/swagger.json", $"{Settings.ApiName} {Settings.ApiVersion}");
				c.RoutePrefix = string.Empty;
			});

			app.UseRouting();
			app.UseCors("CorsPolicy");
			app.UseAuthorization();


			EventHubHandler eventHubHandler = app.ApplicationServices.GetService<EventHubHandler>();
			eventHubHandler.Config(Configuration);

			app.UseEndpoints(endpoints => {
				endpoints.MapControllers();
				endpoints.MapHub<BaseHub<EventHubHandler>>("/websocket/hubs/events");
			});
		}
	};
}
