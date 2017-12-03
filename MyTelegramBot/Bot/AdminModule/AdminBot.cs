using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using System.IO;
using MyTelegramBot.Messages.Admin;
using MyTelegramBot.Messages;
using Microsoft.EntityFrameworkCore;

namespace MyTelegramBot.Bot.AdminModule
{
    public class AdminBot : BotCore
    {
        private AdminPanelCmdMessage AdminCmdListMsg { get; set; }

        private CategoryListMessage CategoryListMsg { get; set; }

        private AdminProductListMessage AdminProductListMsg { get; set; }

        private AdminProductFuncMessage AdminProductFuncMsg { get; set; }

        private ContactEditMessage ContactEditMsg { get; set; }

        private AdminAllProductsViewMessage AdminAllProductsViewMsg { get; set; }

        private AdminCurrentStockMessage AdminCurrentStockMsg { get; set; }

        private AdminPayMethodsSettings AdminPayMethodsSettingsMsg { get; set; }

        private AdminQiwiSettingsMessage AdminQiwiSettingsMsg { get; set; }

        private StatisticMessage StatisticMsg { get; set; }

        private AdminControlMessage AdminControlMsg { get; set; }

        public const string ProductCreateCmd = "ProductCreate";

        public const string ProductEditCmd = "ProductEdit";

        public const string CategoryEditCmd = "CategoryEdit";

        public const string CategoryCreateCmd = "CategoryCreate";

        public const string NotyfiCreateCmd = "NotyfiCreate";

        public const string SelectProductCmd = "SelectProduct";

        public const string AdminProductInCategoryCmd = "AdminProductInCategory";

        public const string EnterNameNewProductCmd = "Введите данные для нового товара";

        public const string EnterNameNewCategoryCmd = "Введите название новой категории";

        public const string BackToAdminPanelCmd = "BackToAdminPanel";

        private const string AdminKeyCmd = "/adminkey";

        public const string ImportCsvCmd = "ImportCsv";

        public const string OrderExportCmd = "OrderExport";

        public const string StockExportCmd = "StockExport";

        public const string NoConfirmOrderCmd = "NoConfirmOrder";

        public const string ContactEditCmd = "ContactEdit";

        public const string VkEditCmd = "VkEdit";

        public const string InstagramEditCmd = "InstagramEdit";

        public const string ChatEditCmd = "ChatEdit";

        public const string ChannelEditCmd = "ChannelEdit";

        private const string ForceReplyVk = "Vk.com";

        private const string ForceReplyInstagram = "Instagram.com";

        private const string ForceReplyChat = "Чат";

        private const string ForceReplyChannel = "Канал";

        public const string PaymentTypeEnableCmd = "PaymentTypeEnable";

        public const string PayMethodsListCmd = "/paymethods";

        public const string QiwiEditCmd = "QiwiEdit";

        public const string QiwiAddEdit = "QiwiAdd";

        private const string EnterPhoneNumber = "Введите номер телефона";

        private const string EnterQiwiApi = "Введите Qiwi ключ";

        public const string StatCmd = "/stat";

        public const string WhatIsQiwiApiCmd = "/whatisqiwiapi";

        private const string AddGroup = "/addchat";

        private const string RevomeGroup = "/delchat";

        private int Parametr { get; set; }
        public AdminBot(Update _update) : base(_update)
        {
          
        }

        protected override void Constructor()
        {
            try
            {
                AdminQiwiSettingsMsg = new AdminQiwiSettingsMessage();
                AdminCmdListMsg = new AdminPanelCmdMessage(base.FollowerId);
                CategoryListMsg = new CategoryListMessage(AdminProductInCategoryCmd);
                ContactEditMsg = new ContactEditMessage();
                AdminAllProductsViewMsg = new AdminAllProductsViewMessage();
                AdminCurrentStockMsg = new AdminCurrentStockMessage();
                AdminPayMethodsSettingsMsg = new AdminPayMethodsSettings();
                AdminControlMsg = new AdminControlMessage();
                StatisticMsg = new StatisticMessage();

                if (base.Argumetns.Count > 0)
                {
                    Parametr = base.Argumetns[0];
                    AdminProductListMsg = new AdminProductListMessage(this.Parametr);
                    AdminProductFuncMsg = new AdminProductFuncMessage(Parametr);
                }


            }

            catch
            {

            }
        }

        public async override Task<IActionResult> Response()
        {
            if(IsOperator() || IsOwner())
            {
                    switch (base.CommandName)
                    {
                        //Панель администратора /admin
                        case "/admin":
                            return await SendAdminControlPanelMsg();

                        //Вернуть в Панель администратора
                        case BackToAdminPanelCmd:
                            return await BackToAdminPanel();

                        case NoConfirmOrderCmd:
                            return await NoConfirmOrder();

                        case "/stockexport":
                            return await StockExport();

                        case "/allprod":
                            return await SendAllProductsView();

                        case "/currentstock":
                            return await SendCurrentStock();

                        case "/on":
                            return await OnOffPrivateMessage(true);

                        case "/off":
                            return await OnOffPrivateMessage(false);

                        default:
                            break;
                    }
                
            }

            if (IsOwner())
            {
                switch (base.CommandName)
                {

                    //Пользователь нажал на кнопку "Добавить товар", ему пришло Сообщение с иструкцией по добавлению
                    case "/newprod":
                        return await SendInsertProductFAQ();

                    //Пользователь нажал на "Изменить товар", ему пришло сообещние с выбором категории
                    case ProductEditCmd:
                        return await EditProduct();

                    //Пользователь нажал на кнопку "Импорт из CSV" ему пришло сообщение с интрукцией
                    case "/import":
                        return await SendImportFAQ();

                    case ContactEditCmd:
                        return await ContactEdit();

                    case VkEditCmd:
                        return await ForceReplyBuilder(ForceReplyVk);

                    case InstagramEditCmd:
                        return await ForceReplyBuilder(ForceReplyInstagram);

                    case ChatEditCmd:
                        return await ForceReplyBuilder(ForceReplyChat);

                    case ChannelEditCmd:
                        return await ForceReplyBuilder(ForceReplyChannel);

                    case "/export":
                        return await OrderExport();

                    case "/qiwi":
                        return await SendQiwiInfo();

                    case PayMethodsListCmd:
                        return await SendPaymentMethods();

                    case PaymentTypeEnableCmd:
                        return await PaymentMethodEnable();

                    case WhatIsQiwiApiCmd:
                        return await SendWhatIsQiwiApi();

                    case StatCmd:
                        return await SendStat();

                    // пользователь нажал на кнопку "изменить" в настройках киви.
                    case QiwiEditCmd:
                        return await ForceReplyBuilder(EnterPhoneNumber);

                    // пользователь нажал на кнопку "добавить" в настройках киви.
                    case EnterPhoneNumber:
                        return await ForceReplyBuilder(EnterPhoneNumber);

                    case "/operators":
                        return await SendOperatorList();

                    case "GenerateKey":
                        return await GenerateKey();

                    case AddGroup:
                       return await AddBotToChat();
                    default:
                        break;
                }

                if (base.OriginalMessage.Contains(ForceReplyVk))
                    return await UpdateVk();

                if (base.OriginalMessage.Contains(ForceReplyInstagram))
                    return await UpdateInstagram();

                if (base.OriginalMessage.Contains(ForceReplyChat))
                    return await UpdateChat();

                if (base.OriginalMessage.Contains(ForceReplyChannel))
                    return await UpdateChannel();

                if (base.OriginalMessage == EnterPhoneNumber) // пользователь отправил номер телефона своего киви кошелька
                    return await AddQiwiTelephone();

                if (base.OriginalMessage == EnterQiwiApi)
                    return await AddQiwiApiKey();

                else
                    return null;
            }

            else
            {
                if (base.CommandName.Contains("/key"))
                    return await CheckOperatorKey(CommandName.Substring(5));

                else
                    return null;
            }
        }

        /// <summary>
        /// Делам так что бы бот мог отсылать в этот чат Админские уведомления
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> AddBotToChat()
        {
            try
            {
                using (MarketBotDbContext db=new MarketBotDbContext())
                {
                    db.Configuration.FirstOrDefault().PrivateGroupChatId =base.GroupChatId.ToString();

                    if (db.SaveChanges() > 0)
                        await SendMessage(base.GroupChatId, new BotMessage { TextMessage="Успех!" });

                    return OkResult;
                }
            }

            catch
            {
                return NotFoundResult;
            }


        }

        /// <summary>
        /// Генерируем новый ключ для оператора. 
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> GenerateKey()
        {
            string hash= GeneralFunction.GenerateHash();

            if (hash != null)
            {
                using (MarketBotDbContext db = new MarketBotDbContext())
                {

                    AdminKey key = new AdminKey { DateAdd = DateTime.Now, Enable = false, KeyValue = hash };
                    db.AdminKey.Add(key);

                    if (db.SaveChanges() > 0)
                    {
                        await SendMessage(new BotMessage { TextMessage = "Пользователь который должен получить права оператора должен ввести следующую команду:" + Bot.BotMessage.NewLine()+ Bot.BotMessage.Italic("/key " + key.KeyValue) });
                        return OkResult;
                    }

                    else
                        return NotFoundResult;

                }
            }

            else return NotFoundResult;
           
        }

        private async Task<IActionResult> SendOperatorList()
        {
            try
            {
                if(AdminControlMsg!=null && await SendMessage(AdminControlMsg.BuildMessage()) != null)
                    return OkResult;

                else
                    return NotFoundResult;
            }

            catch
            {
                return NotFoundResult;
            }
        }

        private async Task<IActionResult> SendPaymentMethods()
        {
            try
            {
                await SendMessage(AdminPayMethodsSettingsMsg.BuildMessage());
                return OkResult;
            }

            catch
            {
                return NotFoundResult;
            }
        }

        private async Task<IActionResult> SendCurrentStock()
        {
            try
            {
                await SendMessage(AdminCurrentStockMsg.BuildMessage());
                return OkResult;
            }
            catch
            {
                return NotFoundResult;
            }
        }

        private async Task<IActionResult> SendAllProductsView()
        {
            try
            {
                await SendMessage(AdminAllProductsViewMsg.BuildMessage());
                return OkResult;
            }

            catch
            {
                return NotFoundResult; 
            }
        }

        /// <summary>
        /// Отправить сообщение с описание что такое Киви  Апи и где взять ключ
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> SendWhatIsQiwiApi()
        {
            try
            {
                string text = "1) Перейдите на сайт https://qiwi.com/api" + BotMessage.NewLine() +
                 "2) Нажмите на кнопку Выпустить новый токен" + BotMessage.NewLine() +
                 "3) Выбрите следующие пункты: Запрос информации о профиле кошелька, Просмотр истории платежей " + Bot.BotMessage.NewLine() +
                 "4) Подтвердите все действия и сохраните токен, что бы потом отправить его Боту" + BotMessage.NewLine() +
                 "5) Настройте Qiwi /qiwi";


                await SendMessage(new BotMessage { TextMessage = text });

                return OkResult;
            }

            catch
            {
                return NotFoundResult;
            }
        }

        /// <summary>
        /// Отправить сообщение со статистикой
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> SendStat()
        {           

            try
            {
                
                TimeSpan diff = DateTime.Now - LastReportsRequest();
                if (diff.Minutes >= 5)
                {
                    var msgs = StatisticMsg.BuildMessage();
                    await SendMessage(msgs[0]);
                    await SendDocument(msgs[1]);
                    await SendDocument(msgs[2]);
                    InsertReportsRequest();
                    return OkResult;
                }

                else
                {
                    await SendMessage(new BotMessage { TextMessage = "Не более одного запроса в 5 минут" });
                    return OkResult;
                }
            }

            catch
            {
                return NotFoundResult;
            }
        }

        /// <summary>
        /// Добавить номер телефона киви кошелька в бд
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> AddQiwiTelephone()
        {
            QiwiApi qiwi = new QiwiApi
            {
                DateAdd = DateTime.Now,
                Enable = false,
                Telephone = base.ReplyToMessageText
            };
           
            using (MarketBotDbContext db = new MarketBotDbContext())
            {
                db.QiwiApi.Add(qiwi);

                if (VerifyPhoneNumber(base.ReplyToMessageText) && db.SaveChanges()>0 && await ForceReplyBuilder(EnterQiwiApi) !=null)
                    return OkResult;

                else
                {
                    await SendMessage(new BotMessage { TextMessage = "Ошибка! Не удалось определить номер телефона." });
                    await ForceReplyBuilder(EnterPhoneNumber);
                    return OkResult;
                }
            }
        }

        /// <summary>
        /// Добавить токен киви в бд
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> AddQiwiApiKey()
        {
            using (MarketBotDbContext db = new MarketBotDbContext())
            {
                var qiwi = db.QiwiApi.Where(q => q.Enable == false).OrderByDescending(q=>q.Id).FirstOrDefault();

                if (qiwi != null && await Services.Qiwi.QiwiFunction.TestConnection(qiwi.Telephone, base.ReplyToMessageText))
                {
                    qiwi.Token = base.ReplyToMessageText;
                    qiwi.Enable = true;
                    db.SaveChanges();
                    await SendMessage(new BotMessage { TextMessage = "Успех!" });
                    return await SendQiwiInfo();
                }

                else
                {
                    await SendMessage(new BotMessage { TextMessage = "Не удалось подключиться к API QIWI" });
                    await ForceReplyBuilder(EnterQiwiApi);
                    return NotFoundResult;
                }
            }
        }

        /// <summary>
        /// Отпраявляем сообщение с текущими настройками киви кошелька
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> SendQiwiInfo()
        {
                    
            if(AdminQiwiSettingsMsg!=null && await SendMessage(AdminQiwiSettingsMsg.BuildMessage()) !=null)
                return OkResult;

            else
                return NotFoundResult;
        }

        /// <summary>
        /// Активировать / Деактивировать метод оплаты
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> PaymentMethodEnable()
        {
            int Id = Argumetns[0];
            using (MarketBotDbContext db = new MarketBotDbContext())
            {

                var Method = db.PaymentType.Where(p => p.Id == Id).FirstOrDefault();

                

                if (Method != null && Method.Enable == true && db.PaymentType.Where(p => p.Enable == true).ToList().Count > 1) // если был активет, делаем не активным
                {
                    Method.Enable = false;
                    db.SaveChanges();
                    await EditMessage(AdminPayMethodsSettingsMsg.BuildMessage());
                    return OkResult;
                }

                var qiwi = db.QiwiApi.Where(q => q.Enable == true).OrderByDescending(q => q.Id).FirstOrDefault();


                if (Method != null && Method.Enable == false && Method.Id == 2
                    && qiwi != null && await Services.Qiwi.QiwiFunction.TestConnection(qiwi.Telephone, qiwi.Token)) // если был не активет, делаем  активным. И если это Киви, то проверяем настроен ли он
                {
                    Method.Enable = true;
                    db.SaveChanges();
                    await EditMessage(AdminPayMethodsSettingsMsg.BuildMessage());
                    return OkResult;
                }

                if (Method != null && Method.Enable == false && Method.Id == 2
                    && qiwi == null) // если был не активет, делаем  активным. И если это Киви, то проверяем настроен ли он. Если киви даже не настроен то присывлаем сообщение 
                {
                    await SendMessage(new BotMessage { TextMessage = "Ошибка! Qiwi платежи не настроены. Нажмите сюда если хотите настроить /qiwi" });
                    return OkResult;
                }

                if (Method != null && Method.Enable == false && Method.Id == 2
                    && qiwi != null && await Services.Qiwi.QiwiFunction.TestConnection(qiwi.Telephone, qiwi.Token) == false) // При тестировании киви апи произошла ошибка и функция вернула False
                {
                    await SendMessage(new BotMessage { TextMessage = "Ошибка! Qiwi платежи не настроены. Нажмите сюда если хотите настроить /qiwi" });
                    return OkResult;
                }

                if (Method != null && Method.Enable == false && Method.Id!=2) // если был не активет, делаем  активным
                {
                    Method.Enable = true;
                    db.SaveChanges();
                    await EditMessage(AdminPayMethodsSettingsMsg.BuildMessage());
                    return OkResult;
                }



                else
                {
                    await base.AnswerCallback();
                    return OkResult;
                }

            }
        }

        /// <summary>
        /// Сообщение с не согласоваными заказми
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> NoConfirmOrder()
        {
            NoConfirmOrdersMessage no = new NoConfirmOrdersMessage();
            await EditMessage(no.BuildMessage());
            return OkResult;
        }

        /// <summary>
        /// Экспорт всех заказов
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> OrderExport()
        {

            TimeSpan diff = DateTime.Now - LastReportsRequest();

            if (diff.Minutes > 1)
            {
                InsertReportsRequest();
                OrderExport export = new OrderExport();

                await SendDocument(new FileToSend { Content = export.Export(), Filename = "Orders.csv" }, "Все заказы в БД");
                return OkResult;
            }

            else
            {
                await SendMessage(new BotMessage { TextMessage = "Не более одного запроса в минуту" });
                return base.OkResult;
            }
        }

        /// <summary>
        /// Экспорт всех данных из таблицы с остатками
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> StockExport()
        {
            TimeSpan diff = DateTime.Now - LastReportsRequest();

            if (diff.Minutes > 1)
            {
                InsertReportsRequest();
                StockExport export =new StockExport();
                await SendDocument(new FileToSend { Content = export.Export(), Filename = "Stock.csv" });
                return OkResult;
            }

            else
            {
                await SendMessage(new BotMessage { TextMessage = "Не более одного запроса в минуту" });
                return base.OkResult;
            }
        }

        /// <summary>
        /// Записываем в бд об успешном запросе
        /// </summary>
        /// <returns></returns>
        private int InsertReportsRequest()
        {
            using (MarketBotDbContext db=new MarketBotDbContext())
            {
                ReportsRequestLog log = new ReportsRequestLog
                {
                    FollowerId = FollowerId,
                    DateAdd = DateTime.Now
                };

                db.ReportsRequestLog.Add(log);

                return db.SaveChanges();
            }
        }

        /// <summary>
        /// Время последнего запроса к БД, для формирования запроса
        /// </summary>
        /// <returns></returns>
        private DateTime LastReportsRequest()
        {
            using (MarketBotDbContext db=new MarketBotDbContext())
            {
               var log= db.ReportsRequestLog.Where(r => r.FollowerId == FollowerId).OrderByDescending(r => r.Id).FirstOrDefault();

                if (log != null)
                    return Convert.ToDateTime(log.DateAdd);

                else
                   return DateTime.Now.AddDays(-1);
            }
        }

        /// <summary>
        /// Изменит текущее сообщение с кнопками контактов.
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> ContactEdit()
        {
            if (ContactEditMsg != null &&await EditMessage(ContactEditMsg.BuildMessage()) != null)
                return base.OkResult;

            else
                return base.NotFoundResult;
        }

        /// <summary>
        /// Изменить ссылку на вк
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> UpdateVk()
        {
            using(MarketBotDbContext db=new MarketBotDbContext())
            {
               var company= db.Company.Where(c => c.Enable == true).FirstOrDefault();

                if (company != null)
                {
                    company.Vk = base.ReplyToMessageText;
                    await db.SaveChangesAsync();
                }

                return base.OkResult;
            }
        }

        /// <summary>
        /// Изменить ссылку на Инст
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> UpdateInstagram()
        {
            using (MarketBotDbContext db = new MarketBotDbContext())
            {
                var company = db.Company.Where(c => c.Enable == true).FirstOrDefault();

                if (company != null)
                {
                    company.Instagram = base.ReplyToMessageText;
                    await db.SaveChangesAsync();
                }

                return base.OkResult;
            }
        }

        /// <summary>
        /// Изменить ссылку на чат
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> UpdateChat()
        {
            using (MarketBotDbContext db = new MarketBotDbContext())
            {
                var company = db.Company.Where(c => c.Enable == true).FirstOrDefault();

                if (company != null)
                {
                    company.Chat = base.ReplyToMessageText;
                    await db.SaveChangesAsync();
                }

                return base.OkResult;
            }
        }

        /// <summary>
        /// Изменить ссылку на канал
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> UpdateChannel()
        {
            using (MarketBotDbContext db = new MarketBotDbContext())
            {
                var company = db.Company.Where(c => c.Enable == true).FirstOrDefault();

                if (company != null)
                {
                    company.Chanel = base.ReplyToMessageText;
                    await db.SaveChangesAsync();
                }

                return base.OkResult;
            }
        }

        /// <summary>
        /// Сообщение с инстуркцией по импорту данных и csv
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> SendImportFAQ()
        {
            try
            {

               await base.SendMessage(new BotMessage { TextMessage = "1) Заполните csv файл " + BotMessage.NewLine() + "2) Сохраните файл как Import.csv" + BotMessage.NewLine() + "3) Отправьте файл боту" });

                using(MarketBotDbContext db=new MarketBotDbContext())
                {
                    Configuration configuration = db.Configuration.FirstOrDefault();

                    // FileId файла Пример.csv есть в базе
                    if (configuration!=null && configuration.ExampleCsvFileId!= null) 
                    {
                        FileToSend fileToSend = new FileToSend
                        {
                            Filename = "Пример.csv",
                            FileId = configuration.ExampleCsvFileId
                        };

                        var message = await SendDocument(fileToSend, "Пример заполнения");
                    }

                    // FileID в базе нет, отправляяем файл и сохраняем в бд FileID
                    if (configuration!=null && configuration.ExampleCsvFileId==null) 
                    {
                    var stream= System.IO.File.Open("Пример.csv", FileMode.Open);

                    FileToSend fileToSend = new FileToSend
                    {
                        Filename = "Пример.csv",
                        Content = stream
                    };

                    var message = await SendDocument(fileToSend, "Пример заполнения");

                    configuration.ExampleCsvFileId = message.Document.FileId;
                    db.SaveChanges();

                    }

                    // FileId файла Шаблон.csv есть в базе
                    if (configuration != null && configuration.TemplateCsvFileId != null)
                    {
                        FileToSend fileToSend = new FileToSend
                        {
                            Filename = "Шаблон.csv",
                            FileId = configuration.ExampleCsvFileId
                        };

                        var message = await SendDocument(fileToSend, "Пример заполнения");
                    }

                    // FileID в базе нет, отправляяем файл и сохраняем в бд FileID
                    if (configuration != null && configuration.TemplateCsvFileId == null) 
                    {
                        var stream = System.IO.File.Open("Шаблон.csv", FileMode.Open);

                        FileToSend fileToSend = new FileToSend
                        {
                            Filename = "Шаблон.csv",
                            Content = stream
                        };

                        var message = await SendDocument(fileToSend, "Пример заполнения");

                        configuration.TemplateCsvFileId = message.Document.FileId;
                        db.SaveChanges();

                    }

                }
                 
                return base.OkResult;
            }

            catch (Exception exp)
            {
                return base.NotFoundResult;
            }
        }

        /// <summary>
        /// Сообщение с панелью администратора
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> SendAdminControlPanelMsg()
        {
            using (MarketBotDbContext db = new MarketBotDbContext())
            {

                if (AdminCmdListMsg!=null && await SendMessage(AdminCmdListMsg.BuildMessage()) != null)
                    return base.OkResult;

                else
                    return base.OkResult;
            }

        }

        /// <summary>
        /// Пользователй хочет получить права оператора. Проверка ключа
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private async Task<IActionResult> CheckOperatorKey(string key)
        {
            using (MarketBotDbContext db = new MarketBotDbContext())
            {
                var Key = db.AdminKey.Where(a=>a.Enable==false && a.KeyValue==key).Include(a=>a.Admin).FirstOrDefault();

                if (Key != null && Key.Admin.Count==0)
                    return await AddNewOpearator(Key);

                else
                    return base.OkResult;
            }
        }

        /// <summary>
        /// Добавить нового оператора
        /// </summary>
        /// <param name="KeyId"></param>
        /// <returns></returns>
        private async Task<IActionResult> AddNewOpearator(AdminKey adminKey)
        {
            using (MarketBotDbContext db = new MarketBotDbContext())
            {
                var admin = db.Admin.Where(a => a.FollowerId == FollowerId && a.Enable).Include(a=>a.AdminKey).Include(a=>a.Follower).FirstOrDefault();

                if (admin != null)
                    return await SendAdminControlPanelMsg();

                else
                {
                    Admin NewAdmin = new Admin
                    {
                        FollowerId = FollowerId,
                        DateAdd = DateTime.Now,
                        AdminKeyId = adminKey.Id,
                        Enable = true

                    };
                   
                    db.Admin.Add(NewAdmin);
                    adminKey.Enable = true;
                    if (db.SaveChanges() > 0)
                    {
                        string meessage = "Зарегистрирован новый оператор системы: " + db.Follower.Where(f=>f.Id==FollowerId).FirstOrDefault().FirstName
                            +Bot.BotMessage.NewLine()+"Ключ: "+ adminKey.KeyValue;
                        await SendMessage(BotOwner, new BotMessage { TextMessage = meessage });
                        return await SendAdminControlPanelMsg();
                    }
                    else
                        return OkResult;
                }
            }
        }

        /// <summary>
        /// Присылает сообщение с инструкцией как добавить новый товар в БД.
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> SendInsertProductFAQ()
        {
            string Currencies = "";
            string Units = "";
            using (MarketBotDbContext db=new MarketBotDbContext())
            {
                var CurrencyList = db.Currency.ToList();

                var UnitList = db.Units.ToList();

                foreach(Currency c in CurrencyList)
                    Currencies += c.Name + " - " + c.ShortName;

                foreach (Units u in UnitList)
                    Units += u.Name + "-" + u.ShortName;
            }

            const string quote = "\"";
            string Example = "Пришлите фотографию товара, а в поле под фотографией(можно без фотографии, просто ответьте на сообщение бота) добавьте комментарий следующего вида:" +
                             " Название товара, Категория, Цена, Валюта, Еденица измерения, В наличии, " + quote + "Краткое описание [не обязательно]" + quote + BotMessage.NewLine()
                             + BotMessage.Bold("Например: ") + "Хреновуха, Настойки,500, руб., шт., 5, " + quote + "40 градусов" + quote + BotMessage.NewLine()
                             + BotMessage.Bold("Например: ") + "Рис, Крупы,100, руб., кг., 100" + BotMessage.NewLine()
                             + BotMessage.Bold("Например: ") + "Сникерс, Конфеты, 50, руб., г., 1000" + quote + "Вкусные конфеты. Ага" + quote
                             + BotMessage.NewLine() + BotMessage.NewLine() + BotMessage.Bold("Доступные валюты: ") + Currencies
                             + BotMessage.NewLine()+ BotMessage.Bold("Еденицы измерения: ") + Units;


            ForceReply forceReply = new ForceReply
            {
                Force = true,

                Selective = true
            };

            if (await SendMessage(new BotMessage { TextMessage = Example }) != null
                && await SendMessage(new BotMessage { TextMessage = EnterNameNewProductCmd, MessageReplyMarkup = forceReply }) != null)
                return base.OkResult;


            else
                return base.NotFoundResult;
        }


        /// <summary>
        /// Пользователь нажал на Изменить продук. Появляется сообещение с выбором категории
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> EditProduct()
        {

            if (await EditMessage(CategoryListMsg.BuildMessage()) != null)
                return base.OkResult;


            else
                return base.NotFoundResult;
        }


        /// <summary>
        /// Вернуть к панели администратора
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> BackToAdminPanel()
        {
            if (await EditMessage(AdminCmdListMsg.BuildMessage()) != null)
                return OkResult;

            else
                return NotFoundResult;
        }

        /// <summary>
        /// Проверка номера телефона. Длина должна быть 11 символов и начинаться должен с 7
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        private bool VerifyPhoneNumber(string number)
        {
            try
            {
                var seven = number.Substring(0, 1);

                if (number.Length == 11 && seven=="7")
                {
                    long value = Convert.ToInt64(number);
                    return true;
                }

                else
                    return false;
            }

            catch
            {
                return false;
            }
        }

        /// <summary>
        /// вкл/выкл уведомления от бота в лс
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> OnOffPrivateMessage(bool value)
        {
            try
            {
                using (MarketBotDbContext db=new MarketBotDbContext())
                {


                    var admin= db.Admin.Where(a => a.FollowerId == FollowerId).FirstOrDefault();

                    if (IsOperator() && admin != null)
                    {
                        admin.NotyfiActive = value;

                        if(db.SaveChanges()>0)
                            await SendMessage(new BotMessage { TextMessage = "Сохранено" });
                    }

                    if(IsOwner())
                    {
                        var conf = db.Configuration.Where(c => c.BotInfoId == BotInfo.Id).FirstOrDefault();

                        conf.OwnerPrivateNotify = value;

                        if (db.SaveChanges() > 0)
                            await SendMessage(new BotMessage { TextMessage = "Сохранено" });
                    }

                    return OkResult;
                }
            }

            catch (Exception e)
            {
                return NotFoundResult;
            }
        }


    }
}
class NewProduct
{
    public string Name { get; set; }

    public int CategoryId { get; set; }

    public string Text { get; set; }

    public double Price { get; set; }

    public int AttacmentFsId { get; set; }

    public int Currency { get; set; }

    public int Unit { get; set; }

    public int Volume { get; set; }

    public int Stock { get; set; }

}
