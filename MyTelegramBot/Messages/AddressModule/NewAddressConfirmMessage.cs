﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types.InlineKeyboardButtons;
using Telegram.Bot.Types.ReplyMarkups;
using MyTelegramBot.Sevices.YandexMap;
using MyTelegramBot.Bot;


namespace MyTelegramBot.Messages
{
    /// <summary>
    /// Сообщение с вопросом подтверждением добавленного адреса
    /// </summary>
    public class NewAddressConfirmMessage:BotMessage
    {
        int FollowerId { get; set; }

        InlineKeyboardCallbackButton [] AnswerBtn { get; set; }

        private float latitude { get; set; }

        private float longitude { get; set; }

        private YandexMap map { get; set; }

        private int AddressId { get; set; }

        public NewAddressConfirmMessage(int FollowerId, float latitude, float longitude)
        {
            AnswerBtn = new InlineKeyboardCallbackButton[2];
            this.FollowerId = FollowerId;
            this.latitude = latitude;
            this.longitude = longitude;

            map = YandexMapSearch.Search(new Coordinates { latitude = latitude, longitude = longitude });
        }

        public NewAddressConfirmMessage BuildMessage ()
        {
            string country = map.response.GeoObjectCollection.featureMember.Where(f => f.GeoObject.metaDataProperty.GeocoderMetaData.kind == "country").ElementAt(0).GeoObject.name;

            var region = map.response.GeoObjectCollection.featureMember.Where(f => f.GeoObject.metaDataProperty.GeocoderMetaData.kind == "province").ToList();

            var city = map.response.GeoObjectCollection.featureMember.Where(f => f.GeoObject.metaDataProperty.GeocoderMetaData.kind == "locality").ToList();

            var street = map.response.GeoObjectCollection.featureMember.Where(f => f.GeoObject.metaDataProperty.GeocoderMetaData.kind == "street").ToList();

            var House = map.response.GeoObjectCollection.featureMember.Where(h => h.GeoObject.metaDataProperty.GeocoderMetaData.kind == "house").ToList();
           

            if (country== "Россия" && city!=null && city.Count>0 && street!=null && street.Count>0 && House!=null && House.Count>0 && FollowerId>0)
            {
                string HouseNumber = House[0].GeoObject.name.Substring(street[0].GeoObject.name.Length + 2);

                int RegionId = GetRegionId(region[0].GeoObject.name);

                int CityId = GetCityId(city.ElementAt(0).GeoObject.name, RegionId);

                int StreetId = GetSteetId(street[0].GeoObject.name, CityId);

                int HouseId = GetHouseId(HouseNumber, StreetId,latitude,longitude);

                int AddressId = GetAddressId(HouseId, FollowerId);


                base.TextMessage ="Ваш адреc "+Italic(city.FirstOrDefault().GeoObject.name+ ", " + street.FirstOrDefault().GeoObject.name + ", " + HouseNumber) +" ?";

                AnswerBtn[0] = YesBtn(FollowerId);
                AnswerBtn[1] = NoBtn();

                base.MessageReplyMarkup = new InlineKeyboardMarkup(AnswerBtn);
            }

            if(country != "Россия")
                base.TextMessage = "Доставка только по России";

            if (city == null || city != null && city.Count == 0)
                base.TextMessage = "Не удалось определить населенный пункт";

            if (street == null || street != null && street.Count == 0)
                base.TextMessage = "Не удалось определить улицу";

            if (House == null || House != null && House.Count == 0)
                base.TextMessage = "Не удалось определить номер дома";

            return this;
        }

        private InlineKeyboardCallbackButton YesBtn(int FollowerId)
        {
            InlineKeyboardCallbackButton btn = new InlineKeyboardCallbackButton("Да", BuildCallData(Bot.AddressBot.SaveChangesAddressCmd,this.AddressId));
            return btn; 
        }

        private InlineKeyboardCallbackButton NoBtn()
        {
            InlineKeyboardCallbackButton btn = new InlineKeyboardCallbackButton("Нет", BuildCallData(Bot.AddressBot.CmdDeleteAddress, this.AddressId));
            return btn;
        }

        private int GetRegionId (string name)
        {
            int id = 0;

            Region Region;

            using (MarketBotDbContext db = new MarketBotDbContext())
            {
                Region = db.Region.Where(r => r.Name == name).FirstOrDefault();

                if (Region != null)
                    id = Region.Id;

                else
                {
                    Region region = new Region { Name = name };
                    db.Region.Add(region);
                    db.SaveChanges();
                    id = region.Id;
                }

                return id;
            }
        }

        private int GetCityId (string name, int regionId)
        {
            int id = 0;          

            using (MarketBotDbContext db=new MarketBotDbContext())
            {
                var City=db.City.Where(c => c.Name == name && c.RegionId == regionId).FirstOrDefault();

                if (City != null)
                    id = City.Id;

                else
                {
                    City city = new City { Name = name, RegionId = regionId };
                    db.City.Add(city);
                    db.SaveChanges();
                    id = city.Id;
                }

                return id;
            }

        }

        private int GetSteetId (string name, int cityId)
        {
            int id = 0;
            using (MarketBotDbContext db=new MarketBotDbContext())
            {
                var Street = db.Street.Where(s => s.Name == name && s.CityId == cityId).FirstOrDefault();


                if (Street != null)
                    id = Street.Id;

                else
                {
                    Street NewStreet = new Street { Name = name, CityId = cityId };
                    db.Street.Add(NewStreet);
                    db.SaveChanges();
                    id = NewStreet.Id;
                }

                return id;
            }

        }

        private int GetHouseId (string number, int streetid, float latitude, float longitude)
        {
            int id = 0;

            using(MarketBotDbContext db=new MarketBotDbContext())
            {
                var House = db.House.Where(h => h.Number == number && h.StreetId == streetid).FirstOrDefault();

                if (House != null)
                    id = House.Id;

                else
                {
                    House house = new House
                    {
                        Number = number,
                        StreetId = streetid,
                        Latitude = latitude,
                        Longitude = longitude
                    };

                    db.House.Add(house);

                    if(db.SaveChanges()>0)
                        id = house.Id;

                }

                return id;
            }


        }

        private int GetAddressId (int houseid, int FollowerId)
        {
            int id = 0;

            using (MarketBotDbContext db=new MarketBotDbContext())
            {
                var Address = db.Address.Where(a => a.HouseId == houseid && a.FollowerId == FollowerId).FirstOrDefault();

                if (Address != null)
                    id = Address.Id;


                else
                {
                    Address address = new Address { HouseId = houseid };
                    db.Address.Add(address);
                    
                    if(db.SaveChanges()>0)
                        id = address.Id;
                }

                this.AddressId = id;
                return id;
            }

        }

    }
}
