using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types.InlineKeyboardButtons;
using Telegram.Bot.Types.ReplyMarkups;
using Microsoft.EntityFrameworkCore;

namespace MyTelegramBot.Messages
{
    /// <summary>
    /// Сообщение со списком адресов
    /// </summary>
    public class AddressListMessage:Bot.BotMessage
    {
        private InlineKeyboardCallbackButton [][] AddressListBtn { get; set; }

        private List<Address> AddressIdList { get; set; }

        private int FollowerId { get; set; }

        public AddressListMessage (int FollowerId)
        {
            this.FollowerId = FollowerId;
            BackBtn = new InlineKeyboardCallbackButton("Назад", BuildCallData(Bot.BasketBot.BackToBasketCmd));
        }
        public  AddressListMessage BuildMessage()
        {
            
            using (MarketBotDbContext db=new MarketBotDbContext())
                AddressIdList=db.Address.Where(a => a.FollowerId == FollowerId).Include(a=>a.House).Include(a=>a.House.Street).Include(a=>a.House.Street.House).Include(a=>a.House.Street.City).ToList();
            

            if (AddressIdList != null && AddressIdList.Count() > 0)
            {
                AddressListBtn = new InlineKeyboardCallbackButton[AddressIdList.Count() + 2][];
                int counter = 0;

                foreach (Address address in AddressIdList)
                {
                    int? HouseId = address.HouseId;
                    var House = address.House;
                    var Street = address.House.Street;
                    var City = address.House.Street.City;

                    string Adr = City.Name + ", " + Street.Name + ", " + House.Number;
                    AddressListBtn[counter] = new InlineKeyboardCallbackButton[1];
                    AddressListBtn[counter][0] = AddressBtn(Adr, Convert.ToInt32(address.Id));
                    counter++;
                }

                AddressListBtn[counter] = new InlineKeyboardCallbackButton[1];
                AddressListBtn[counter][0] = AddAddress();

                AddressListBtn[counter + 1] = new InlineKeyboardCallbackButton[1];
                AddressListBtn[counter + 1][0] = BackBtn;


                base.MessageReplyMarkup = new InlineKeyboardMarkup(AddressListBtn);
                base.TextMessage = "Выберите адрес";

            }

            else
            {
                AddressListBtn = new InlineKeyboardCallbackButton[1][];
                AddressListBtn[0] = new InlineKeyboardCallbackButton[1];
                AddressListBtn[0][0] = AddAddress();
                base.MessageReplyMarkup = new InlineKeyboardMarkup(AddressListBtn);
                base.TextMessage = "Выберите адрес";
    
            }

            return this;
        }

        private InlineKeyboardCallbackButton AddressBtn(string text, int Id)
        {
            string data = BuildCallData(Bot.OrderBot.CmdGetAddress, Id);
            InlineKeyboardCallbackButton  btn = new InlineKeyboardCallbackButton(text, data);
            return btn;
        }

        private InlineKeyboardCallbackButton AddAddress()
        {
            InlineKeyboardCallbackButton btn = new InlineKeyboardCallbackButton("Добавить", BuildCallData(Bot.AddressBot.CmdAddAddress));
            return btn;
        }
    }
}
