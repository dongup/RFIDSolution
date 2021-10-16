﻿using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RFIDSolution.Server.SignalRHubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RFIDSolution.Server
{
    public class Program
    {
        public static ReaderHepler Reader = new ReaderHepler();

        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
            //Console.ReadLine();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
