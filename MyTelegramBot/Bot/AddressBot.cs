using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Types;
using MyTelegramBot.Messages;

namespace MyTelegramBot.Bot
{
    public class AddressBot:Bot.BotCore
    {
 
        private AddressListMessage ViewAddressListMsg { get; set; }

        private NewAddressConfirmMessage ConfirmNewAddress { get; set; }

        public const string SaveChangesAddressCmd = "SaveChangesAddress";

        public const string CmdAddAddress = "AddAddress";

        public const string CmdDeleteAddress = "DeleteAddress";

        public const string CmdGetAddressList = "GetAddressList";

        private int AddressId { get; set; }

        public AddressBot(Update _update) : base(_update)
        {

        }

        protected override void Constructor()
        {
            try
            {
                if (base.Argumetns.Count > 0)
                {
                    this.AddressId = Argumetns[0];

                    ViewAddressListMsg = new AddressListMessage(base.FollowerId);                 
                }

                if(base.Update.Message!=null && base.Update.Message.Location!=null)
                    ConfirmNewAddress = new NewAddressConfirmMessage(base.FollowerId, Update.Message.Location.Latitude, Update.Message.Location.Longitude);

            }

            catch
            {

            }
        }

        public async override Task<IActionResult> Response()
        {
           switch (base.CommandName)
            {
                case SaveChangesAddressCmd:
                    return await UpdateNewAddress();

                case CmdGetAddressList:
                    return await GetAddressList(base.MessageId);

                case CmdAddAddress:
                    return await AddAddressFaq();

                case CmdDeleteAddress:
                    return await DeleteAddress();

                default:
                    break;
            }
                

            //пользователь отрправил свою геолокацию, проверям есть ли у него хоть что нибудь в корзине && Connection.getConnection().Basket.Where(b=>b.FollowerId==FollowerId).Count()>0
            if (Update.Message != null && Update.Message.Location != null )
                return await ConfirmationRequestAddress();          

            else
                return null;
        }

        /// <summary>
        /// Обрабатываем локацию, находим адрес по яндекс картам и заносим его бд. Далее выходил сообщение с подверждением этого адреса
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> ConfirmationRequestAddress()
        {
            ConfirmNewAddress = new NewAddressConfirmMessage(base.FollowerId, Update.Message.Location.Latitude, Update.Message.Location.Longitude);
            var message= ConfirmNewAddress.BuildMessage();

            if (await SendMessage(message) != null)
                return base.OkResult;

            else
                return base.NotFoundResult;

        }

        /// <summary>
        /// Пользователь не подвердил этот адрес. Удаляем его из БД и выводим сообщение с инструкией как добавить адрес
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> DeleteAddress()
        {
            using(MarketBotDbContext db=new MarketBotDbContext())
            {
                Address address = new Address();

                address = db.Address.Where(a => a.Id == Argumetns[0]).FirstOrDefault();

                if(address!=null)
                {
                    db.Address.Remove(address);
                    db.SaveChanges();
                }

                return await AddAddressFaq();

            }
        }

        /// <summary>
        /// Пользователь подтвердил, что проживает по этому адресу. Значит в таблице Address заполняем поле FollowerId. Теперь этот адрес принадлежит пользователю
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> UpdateNewAddress()
        {
            using (MarketBotDbContext db = new MarketBotDbContext())
            {
                Address address = new Address();

                address = db.Address.Where(a => a.Id == Argumetns[0]).FirstOrDefault();

                if (address != null) // сохраняем и отправляем сообщение со списокм адерсов пользователя
                {
                    address.FollowerId = base.FollowerId;
                    db.SaveChanges();
                    return await GetAddressList();
                }

                else
                    return NotFoundResult;
            }
        }

        /// <summary>
        /// ПОказать одним сообщеним все адреса пользователя
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> GetAddressList(int MessageId=0)
        {
            if (await SendMessage(ViewAddressListMsg.BuildMessage(),MessageId) != null)
                return base.OkResult;

            else
                return base.NotFoundResult;
        }

        /// <summary>
        /// Сообщение с инстуркцией как добавить новый адрес
        /// </summary>
        /// <returns></returns>
        private async Task<IActionResult> AddAddressFaq()
        {
            if (await SendMessage(new BotMessage { TextMessage = "Отправьте боту объект Геолокация с вашими координатами" }) != null)
                return base.OkResult;

            else
                return base.NotFoundResult;
        }
    }
}
