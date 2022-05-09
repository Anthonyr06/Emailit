using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Emailit.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Emailit
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //Run migrations before running the app
            CreateHostBuilder(args).Build().MigrateDatabase().Run();
        }
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
