using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.InputMessageContents;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types.InlineKeyboardButtons;
using Telegram.Bot;
using System.IO;

namespace MyTelegramBot
{
    public class Startup
    {
        private const string ssl = "req -newkey rsa:2048 -sha256 -nodes -keyout YOURPRIVATE.key -x509 -days 365 -out YOURPUBLIC.pem -subj";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public async void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
            .SetBasePath(System.IO.Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json");
            string token = builder.Build().GetSection("BotToken").Value;
            string ddns = builder.Build().GetSection("DDNS").Value;

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
           
            app.UseMvc();

            //var info = await GetWebHookInfo(token);

            //if (info.Url == "")
            //    await SetWebHook(token, ddns);
        }


        private async Task<WebhookInfo> GetWebHookInfo(string token)
        {
            try
            {
                TelegramBotClient telegram = new TelegramBotClient(token);
                return await telegram.GetWebhookInfoAsync();
            }

            catch
            {
                return null;
            }
        }

        private async Task<WebhookInfo> SetWebHook(string token, string DDNS)
        {
            try
            {
                DDNS = "311cf420.ngrok.io";
                TelegramBotClient telegram = new TelegramBotClient(token);
                var stream = GetSSL();
                if(stream!=null)
                    await telegram.SetWebhookAsync("https://"+DDNS+"/api/values", new FileToSend("@Cert", GetSSL()), 50);
                else
                    await telegram.SetWebhookAsync("https://" + DDNS + "/api/values", null, 50);

                return await GetWebHookInfo(token);
            }

            catch (Exception exp)
            {
                return null; 
            }
        }

        private System.IO.Stream GetSSL()
        {
            try
            {
                return System.IO.File.OpenRead("YOURPUBLIC.pem");
            }

            catch
            {
                return null;
            }
        }
    }
}
