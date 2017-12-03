using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.InputMessageContents;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types.InlineKeyboardButtons;
using Telegram.Bot;

namespace MyTelegramBot
{
    public class Program
    {
        private const string ssl = "req -newkey rsa:2048 -sha256 -nodes -keyout YOURPRIVATE.key -x509 -days 365 -out YOURPUBLIC.pem -subj";
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();

            var builder = new ConfigurationBuilder()
            .SetBasePath(System.IO.Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json");
            string ddns = builder.Build().GetSection("DDNS").Value;
            string token= builder.Build().GetSection("BotToken").Value;
            
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build();

        private bool CreateSSL(string DDNS)
        {
            try
            {
                System.Diagnostics.Process.Start("\\Services\\OpenSSL\\bin\\openssl.exe", ssl + " \"" + "/C=US/ST=Russia/L=RUS/O=Bot Company/CN=" + DDNS + "\"");
                return true;
            }

            catch
            {
                return false;
            }
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
                TelegramBotClient telegram = new TelegramBotClient(token);
                await telegram.SetWebhookAsync("https:/"+DDNS+"/api/values", new FileToSend("@", null), 50);
                return await GetWebHookInfo(token);
            }

            catch
            {
                return null; 
            }
        }

        
    }
}
