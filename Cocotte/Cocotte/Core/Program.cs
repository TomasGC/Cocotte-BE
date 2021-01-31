using System;
using CoreLib.Utils;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Cocotte.Core {
    /// <summary>
    /// Program class.
    /// </summary>
    public static class Program {
        /// <summary>
        /// Configuration.
        /// </summary>
        public static IConfiguration Configuration { get; } = new ConfigurationBuilder()
            .SetBasePath(Tools.GetExecutableRootPath())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();


        /// <summary>
        /// The host builder.
        /// </summary>
        /// <param name="args"></param>
        /// <returns>The created builder</returns>
        public static IHostBuilder CreateHostBuilder(string[] args) => Host.CreateDefaultBuilder(args).ConfigureWebHostDefaults(webBuilder => webBuilder.UseConfiguration(Configuration).ConfigureLogging(log => log.AddSerilog(Log.Logger)).UseStartup<Startup>().UseKestrel());

        /// <summary>
        /// Main method of the program.
        /// </summary>
        /// <param name="args"></param>
        public static int Main(string[] args) {
            // Create the Serilog logger, and configure the sinks.
            Log.Logger = new LoggerConfiguration().Enrich.WithCaller().ReadFrom.Configuration(Configuration).CreateLogger();

            // Wrap creating and running the host in a try-catch block.
            try {
                Log.Information("Starting host");
                CreateHostBuilder(args).Build().Run();
                return 0;
            }
            catch (Exception e) {
                Log.Fatal(e, "Host terminated unexpectedly");
                return 1;
            }
            finally {
                Log.CloseAndFlush();
            }
        }
    };
}