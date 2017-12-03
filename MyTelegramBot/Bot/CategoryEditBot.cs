using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Types;
using MyTelegramBot.Messages.Admin;
using MyTelegramBot.Messages;
using MyTelegramBot.Bot.AdminModule;

namespace MyTelegramBot.Bot
{
    public class CategoryEditBot : BotCore
    {
        public const string CategoryEditNameCmd = "EditCategoryName";

        public const string CategoryEditEnableCmd = "EditCategoryEnable";

        public const string SelectCategoryCmd = "SelectCategory";

        public const string NewNameForceReplyCmd = "Введите новое имя для:";

        private int CategoryId { get; set; }

        private string CategoryName { get; set; }

        AdminCategoryFuncMessage AdminCategoryFuncMsg { get; set; }

        CategoryListMessage CategoryListMsg { get; set; }

        public CategoryEditBot (Update update) :base (update)
        {
           
            
        }

        protected override void Constructor()
        {
            using (MarketBotDbContext db = new MarketBotDbContext())
            {
                try
                {
                    CategoryListMsg = new CategoryListMessage(SelectCategoryCmd);

                    if (Argumetns.Count > 0)
                    {
                        CategoryId = Argumetns[0];
                        AdminCategoryFuncMsg = new AdminCategoryFuncMessage(CategoryId);
                    }

                    if (CategoryId > 0)
                        CategoryName = db.Category.Where(c => c.Id == CategoryId).FirstOrDefault().Name;
                }

                catch
                {

                }
            }
        }

        public async override Task<IActionResult> Response()
        {

            if (IsOwner())
            {
                switch (base.CommandName)
                {
                //Пользователь нажал на Кнопку "Изменить категорию" сообщение редактирутся на список категорий
                    case AdminBot.CategoryEditCmd:
                        return await GetCategoryList();

                    //Пользователь выбрал какую категорию он хочет изменить. Сообщение редактируется на список доступных функций
                    case SelectCategoryCmd:
                        return await CategoryFunc(this.CategoryId);

                        //Пользотваель нажал на кнопку добавить новыую категорию. 
                        //Приход Relpy сообщение с просьбой указать имя новой категории
                    case "/newcategory":
                        return await ForceReplyBuilder(AdminBot.EnterNameNewCategoryCmd);

                    //Пользотватель нажал на кнопку "Изменить название категории. Приходи Reply сообщение с просьобой указать новое имя для этой категориии
                    case CategoryEditNameCmd:
                        return await ForceReplyBuilder(NewNameForceReplyCmd + CategoryName);


                    //Пользователь нажал на кнопку "Отобразить/Скрыть".Выполняется обновление данных в бд, а потом присылается сообщение с доступными функциями
                    case CategoryEditEnableCmd:
                        return await UpdateCategoryEnable();

                    default:
                        break;

                }

                //Пользователь отправил новое имя для категории. Выполняется обновление данных в бд, а потом присылается сообщение с доступными функциями
                if (base.OriginalMessage.Contains(NewNameForceReplyCmd))
                    return await UpdateCategoryName();

                ///Пользователь присалал название новой категории.Выполняется обновление данных в бд, а потом присылается сообщение с доступными функциями
                if (base.OriginalMessage.Contains(AdminBot.EnterNameNewCategoryCmd))
                    return await AddNewCategory();

                else
                    return null;
            }
              
            else
                return null;
        }

        private async Task<IActionResult> CategoryFunc(int CategoryId)
        {
            AdminCategoryFuncMsg = new AdminCategoryFuncMessage(CategoryId);
            if (await EditMessage(AdminCategoryFuncMsg.BuildMessage()) != null)
                return OkResult;

            else
                return NotFoundResult;
        }

        private async Task<IActionResult> GetCategoryList()
        {
            if (await EditMessage(CategoryListMsg.BuildMessage()) != null)
                return OkResult;

            else
                return NotFoundResult;
        }

        private async Task<IActionResult> AddNewCategory()
        {
            string Name = ReplyToMessageText;
            using (MarketBotDbContext db = new MarketBotDbContext())
            {
                var ReapetCategory = db.Category.Where(c => c.Name == Name).FirstOrDefault();

                if (ReapetCategory != null)
                    return await ErrorMessage(AdminBot.EnterNameNewCategoryCmd, "Такая категория уже существует");

                else
                {
                    Category category = new Category
                    {
                        Name = Name,
                        Enable = true

                    };
                    db.Category.Add(category);
                    db.SaveChanges();

                    AdminCategoryFuncMsg = new AdminCategoryFuncMessage(category.Id);

                    if (await SendMessage(AdminCategoryFuncMsg.BuildMessage()) != null)
                        return OkResult;

                    else
                        return NotFoundResult;

                }
            }
        }

        private async Task<IActionResult> UpdateCategoryName()
        {
            string NewName= ReplyToMessageText;

            string OldName = OriginalMessage.Substring(NewNameForceReplyCmd.Length);
            using (MarketBotDbContext db = new MarketBotDbContext())
            {
                var category = db.Category.Where(c => c.Name == OldName).FirstOrDefault();

                if (category != null)
                {
                    if (db.Category.Where(c => c.Name == NewName).Count() > 0)
                        return await ErrorMessage(NewNameForceReplyCmd, "Категория с таким именем уже существует");

                    else
                    {
                        category.Name = ReplyToMessageText;

                        db.SaveChanges();

                        return await CategoryFunc(category.Id);
                    }

                }

                else
                    return NotFoundResult;
            }
        }

        private async Task<IActionResult> UpdateCategoryEnable()
        {
            using (MarketBotDbContext db = new MarketBotDbContext())
            {
                var category = db.Category.Where(c => c.Id == CategoryId).FirstOrDefault();

                if (category != null)
                {

                    if (category.Enable)
                        category.Enable = false;

                    else
                        category.Enable = true;

                    return await CategoryFunc(category.Id);
                }

                else
                    return NotFoundResult;
            }
        }

        private async Task<IActionResult> ErrorMessage(string ForceReplyText, string ErrorMessage = "Ошибка")
        {
            if (await SendMessage(new BotMessage { TextMessage = ErrorMessage }) != null)
                return await ForceReplyBuilder(ForceReplyText);

            else
                return NotFoundResult;
        }
    }
}
