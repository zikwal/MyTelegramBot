using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Types;
using System.IO;
using System.Text;
using System.Net;

namespace MyTelegramBot.Bot
{
    public class ImportCSVBot:BotCore
    {
        public ImportCSVBot(Update _update) : base(_update)
        {

        }

        protected override void Constructor()
        {
            
        }

        public async override Task<IActionResult> Response()
        {

            
            ///Пользователь прислал CSV файл для импорта данных
            if (this.Update.Message != null && this.Update.Message.Document != null 
                && this.Update.Message.Document.MimeType == "text/csv" && this.Update.Message.Document.FileName == "Import.csv" && IsOwner())
                return await ImportToDb(Update.Message.Document.FileId);

            else
                return null;
        }

        /// <summary>
        /// Парсим CSV файл
        /// </summary>
        /// <param name="CSV"></param>
        /// <returns></returns>
        private async Task<List<ParseStruct>> ParseCSV(Stream CSV)
        {
            List<string> rows = new List<string>();
            List<ParseStruct> ListParse = new List<ParseStruct>();

            try
            {
                StreamReader sr = new StreamReader(CSV, Encoding.GetEncoding(1251));
                while (sr.Peek() >= 0)
                {
                    rows.Add(await sr.ReadLineAsync());
                }
                sr.Close();
                sr.Dispose();

                for (int i = 1; i < rows.Count; i++)
                {
                    string[] split = rows[i].Split(';');

                    ParseStruct parseStruct = new ParseStruct();

                    if (split != null && split[0] != "") // Название
                        parseStruct.Name = split[0];

                    else
                        throw new Exception("Не удалось определить навзание товара");

                    if (split != null && split.Length > 2 && split[2] != "") // Категория
                        parseStruct.CategoryName = split[2];

                    else
                        throw new Exception("Не удалось определить категорию товара");


                    if (split != null && split.Length > 1) // Цена
                        parseStruct.Price = Convert.ToDouble(split[1]);

                    else
                        throw new Exception("Не удалось определить стоимость товара");

                    if (split != null && split.Length > 2) // проверяем есть ли описание
                        parseStruct.Desc = split[3];

                    if (split != null && split.Length > 3) // проверяем есть ли ссылка на заметку


                    //проверяем есть ли ссылка на файл фотографии. Если есть то скаичваем, отправляем боту, получаем FileId
                    if (split.Length > 4 && split[5] != null && split[5].Contains(".jpg")  ||
                        split.Length > 4 && split[5] != null && split[5].Contains(".png") )
                        parseStruct.PhotoUrl = split[5];

                    //Остаток
                    if (split.Length > 5 && split[6] != null)
                        parseStruct.StockBalance = Convert.ToInt32(split[6]);

                    else if (parseStruct.StockBalance < 0)
                        parseStruct.StockBalance = 0;

                    else
                        parseStruct.StockBalance = 0;

                    //Валюта
                    if (split.Length > 6 && split[7] != null)
                        parseStruct.CurrencyId = CurrencyCheck(split[7]);

                    else // Если валюта не указана
                        parseStruct.CurrencyId = 1;

                    if (split.Length > 7 && split[8] != null)
                        parseStruct.UnitId = UnitCheck(split[8]);

                    else
                        parseStruct.UnitId = 1;

                    ListParse.Add(parseStruct);
                }

                return ListParse;
            }

            catch
            {
                return null;
            }

        }



        /// <summary>
        /// Вставляем данные по товару в бд и уведомляем об это пользователя
        /// </summary>
        /// <param name="Newprod"></param>
        private async void InsertToDb(ParseStruct Newprod)
        {
            Newprod.CategoryId = CategoryCheck(Newprod.CategoryName);
            //скачиваем файл с указанного ресурса
            var photo = await DownloadPhoto(Newprod.PhotoUrl);

            

            if (ProductCheck(Newprod.Name) > 0)
                await SendMessage(new BotMessage { TextMessage = "Товар с таким название уже существует: " + Newprod.Name });

            if (Newprod.CategoryId > 0 && ProductCheck(Newprod.Name) == 0 && photo != null)
            {
                int ProductId = 0;

                Message message = new Message();

                using (MemoryStream ms=new MemoryStream(photo))
                {
                    // Отправляем скачаную фотографию через бота. Тем самым загружаем ее на сервере телеграм
                    message = await base.SendFileStream(Newprod.Name,ms);
                }

                if (message != null && message.Photo != null && message.Photo.Count() > 0)
                {
                    var fs= InsertToAttachmentFs(photo);

                    if (fs >0)
                    {
                        ProductId = InsertNewProduct(Newprod);

                        await InsertProductPhoto(ProductId, fs);
                        var attach = InsertAttachment(fs, message.Photo[message.Photo.Length - 1].FileId);
                    }
                }

                else
                    await SendMessage(new BotMessage { TextMessage = "Ошибка при отправки фотографии товара в телеграм: " + Newprod.Name });


                if (ProductId < 1)
                    await SendMessage(new BotMessage { TextMessage = "Не удалось добавить: " + Newprod.Name });

            }


            if (photo == null)
                await SendMessage(new BotMessage { TextMessage = "Не удалось скачать фотографию: " + Newprod.Name });

        }

        private int InsertToAttachmentFs(byte[] stream)
        {
            try
            {
                if (stream != null)
                {


                    AttachmentFs attachmentFs = new AttachmentFs
                    {
                        Fs = stream,
                        GuId = Guid.NewGuid()
                    };

                    using (MarketBotDbContext db = new MarketBotDbContext())
                    {
                         db.AttachmentFs.Add(attachmentFs);
                         db.SaveChanges();
                        return attachmentFs.Id;
                    }
                }

                else
                    return -1;
            }

            catch (Exception exp)
            {
                return -1;
            }
        }


        private AttachmentTelegram InsertAttachment(int AttachemntFsID, string FileID)
        {
            try
            {
                using (MarketBotDbContext db=new MarketBotDbContext())
                {
                    if (AttachemntFsID > 0 && FileID != null)
                    {
                        AttachmentTelegram attachment = new AttachmentTelegram
                        {
                            AttachmentFsId = AttachemntFsID,
                            BotInfoId = BotInfo.Id,
                            FileId = FileID
                        };

                        db.AttachmentTelegram.Add(attachment);
                        db.SaveChanges();
                        return attachment;
                    }

                    else
                        return null;
                }
            }

            catch
            {
                return null;
            }
        }

        private async Task<int> InsertProductPhoto(int ProductId, int AttachemntFsID)
        {
            try
            {
                using(MarketBotDbContext db=new MarketBotDbContext())
                {
                    if (ProductId > 0 && AttachemntFsID > 0)
                    {
                        ProductPhoto photo = new ProductPhoto
                        {
                            AttachmentFsId = AttachemntFsID,
                            ProductId = ProductId
                        };

                        db.ProductPhoto.Add(photo);
                        return await db.SaveChangesAsync();
                    }

                    else
                        return -1;
                }
            }

            catch
            {
                return -1;
            }
        }

        /// <summary>
        /// Скачивает фото по Url
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private async Task<byte[]> DownloadPhoto(string url)
        {
            try
            {
                using (WebClient client = new WebClient())
                {
                    byte[] Download = await client.DownloadDataTaskAsync(url);
                    return Download;

                }
            }

            catch
            {
                return null;
            }
        }

        /// <summary>
        ///Проверяет есть категория с таким навзанем, если нет то добавляет в БД и возращает Id
        /// </summary>
        /// <param name="Name"></param>
        /// <returns>id категории</returns>
        private int CategoryCheck(string Name)
        {
            try
            {
                using (MarketBotDbContext db = new MarketBotDbContext())
                {
                    var cat = db.Category.Where(c => c.Name == Name).FirstOrDefault();

                    if (cat != null) // Проверяем есть ли такая категоря в БД. 
                        return cat.Id;

                    else //Если нет то вставляем
                    {
                        Category category = new Category
                        {
                            Name = Name,
                            Enable = true
                        };

                        db.Category.Add(category);
                        db.SaveChanges();
                        return category.Id;
                    }
                }
            }

            catch
            {
                return -1;
            }

        }

        /// <summary>
        /// Если товар с таким название уже существует, то возращает id
        /// </summary>
        /// <param name="Name"></param>
        /// <returns>id товара если найде, 0 если нет, -1 если ошибка</returns>
        private int ProductCheck(string Name)
        {
            try
            {
                using (MarketBotDbContext db = new MarketBotDbContext())
                {
                    var prod = db.Product.Where(p => p.Name == Name).FirstOrDefault();

                    if (prod != null)
                        return prod.Id;

                    else
                        return 0;
                }
            }

            catch
            {
                return -1;
            }
        }

        private int CurrencyCheck (string Name)
        {
            try
            {
                using(MarketBotDbContext db=new MarketBotDbContext())
                {
                    var currency = db.Currency.Where(c => c.Name == Name).FirstOrDefault();

                    if (currency != null)
                        return currency.Id;

                    else // Если ни чего найдено, то выбираем первую по умолчанию. т.е Рубли
                        return db.Currency.FirstOrDefault().Id; 
                }
            }

            catch
            {
                return 1; // 1 - рубли
            }
        }

        private int UnitCheck(string Name)
        {
            try
            {
                using (MarketBotDbContext db = new MarketBotDbContext())
                {
                    var unit = db.Units.Where(c => c.Name == Name).FirstOrDefault();

                    if (unit != null)
                        return unit.Id;

                    else // Если ни чего найдено, то выбираем первую по умолчанию. т.е штуки
                        return db.Units.FirstOrDefault().Id;
                }
            }

            catch
            {
                return 1; // 1 - штуки
            }
        }

        /// <summary>
        /// Добавить новый товар в БД.
        /// </summary>
        /// <param name="Newprod">объект описывающий новый товар</param>
        /// <returns>Возращает Id</returns>
        private int InsertNewProduct(ParseStruct Newprod)
        {
            using (MarketBotDbContext db = new MarketBotDbContext())
            {
                try
                {
                    Product InsertNewProduct = new Product
                    {
                        Name = Newprod.Name,
                        CategoryId = Newprod.CategoryId,
                        TelegraphUrl = Newprod.NoteUrl,
                        Text = Newprod.Desc,
                        DateAdd = DateTime.Now,
                        PhotoId = Newprod.PhotoId,
                        Enable = true,
                        UnitId=Newprod.UnitId
                    };

                    db.Product.Add(InsertNewProduct);

                    if (db.SaveChanges() > 0)
                    {

                        ProductPrice productPrice = new ProductPrice
                        {
                            ProductId = InsertNewProduct.Id,
                            DateAdd = DateTime.Now,
                            Enabled = true,
                            Value = Newprod.Price,
                            CurrencyId=Newprod.CurrencyId,
                            Volume=1
                        };

                        Stock stock = new Stock
                        {
                            ProductId = InsertNewProduct.Id,
                            Balance = Newprod.StockBalance,
                            DateAdd = DateTime.Now,
                            Quantity = Newprod.StockBalance
                        };

                        db.Stock.Add(stock);
                        db.ProductPrice.Add(productPrice);
                        db.SaveChanges();

                        return InsertNewProduct.Id;
                    }

                    else
                        return -1;
                }

                catch
                {
                    return -1;
                }
            }

            /// <summary>
            /// импортируем данные из csv файла в бд
            /// </summary>
            /// <param name="id">id файлан на серевере Телегарм</param>
            /// <returns></returns>

        }

        /// <summary>
        /// Импорт всех данных и csv файла
        /// </summary>
        /// <param name="id">id csv файла на сервере телегарм</param>
        /// <returns></returns>
        private async Task<IActionResult> ImportToDb(string id)
        {
            var file = await GetFileAsync(id);
            Stream csv = file.FileStream;

            try
            {
                var List = await ParseCSV(csv);

                foreach (var product in List)
                    InsertToDb(product);

                return base.OkResult;

            }

            catch
            {
                return base.NotFoundResult;
            }
        }
    }
}

struct ParseStruct
{
    public string Name { get; set; }

    public string Desc { get; set; }

    public string NoteUrl { get; set; }

    public string PhotoUrl { get; set; }

    public double Price { get; set; }

    public string CategoryName { get; set; }

    public int CategoryId { get; set; }

    public string PhotoId { get; set; }

    public int StockBalance { get; set; }

    public int CurrencyId { get; set; }

    public int UnitId { get; set; }
}