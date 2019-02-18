﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.FileExtensions;
using Microsoft.Extensions.Configuration.Json;

namespace testing_console
{

    class Program
    {
        private static string userid = "";
        static async Task Main(string[] args)
        {

             var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");
            var config = builder.Build();
            userid = config.GetValue<string>("userid");

            var results = await makeRequest();

            using (var writer = File.CreateText("badges.txt"))
            {
                writer.WriteLine("<div class=\"flex-container\">");

                foreach (PluralsiteTestResult r in results)
                {
                    writer.WriteLine($"<div class=\"card\"><div class=\"box-header\"></div><div class=\"skill-header\"><img src=\"{r.thumbnailUrl}\" />");
                    writer.WriteLine($"<div class=\"skill-title\">{r.title}</div>");
                    writer.WriteLine($"<div class=\"skill-stats\"><div class=\"skill-measurement\"><span class=\"SVGInline\"><svg class=\"svg-img\" viewBox=\"0 0 67 67\" width=\"11\" height=\"13\" xmlns=\"http://www.w3.org/2000/svg\"><path d=\"m33.5 67c-18.5015391 0-33.5-14.9984609-33.5-33.5s14.9984609-33.5 33.5-33.5 33.5 14.9984609 33.5 33.5v33.5zm0-14c10.7695526 0 19.5-8.7304474 19.5-19.5s-8.7304474-19.5-19.5-19.5-19.5 8.7304474-19.5 19.5 8.7304474 19.5 19.5 19.5z\"></path></svg></span>");
                    writer.WriteLine($"{r.score} {r.level}</div><div class=\"percentile\">{r.percentile} Percentile </div><div class=\"verified\">Verified {r.dateCompleted}</div></div></div></div>");


                    //writer.WriteLine($"{r.title} : {r.score} {r.level} {r.percentile} {r.thumbnailUrl} {r.url}"); 
                }
                writer.WriteLine("</div>");
            }

        }

        static async Task<IEnumerable<PluralsiteTestResult>> makeRequest()
        {
            using (var client = new HttpClient())
            {

                var result = await client.GetAsync($"https://app.pluralsight.com/profile/data/skillmeasurements/{userid}");

                if (result.IsSuccessStatusCode)
                {
                    IEnumerable<PluralsiteTestResult> PSResults = JsonConvert.DeserializeObject<IEnumerable<PluralsiteTestResult>>(await result.Content.ReadAsStringAsync());

                    return PSResults;
                }
                else
                {
                    return null;
                }
            }
        }


    }
}
