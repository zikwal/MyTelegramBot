using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;


namespace MyTelegramBot.Controllers
{
    public class HomeController : Controller
    {
        MarketBotDbContext context;

        public IActionResult Index()
        {
            context = new MarketBotDbContext();

            return View(context.BotInfo.ToList());
        }


    }
}
