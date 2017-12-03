using System;
using System.Collections.Generic;

namespace MyTelegramBot
{
    public partial class Configuration
    {
        public int Id { get; set; }
        public string ExampleCsvFileId { get; set; }
        public string TemplateCsvFileId { get; set; }
        public bool? BotBlocked { get; set; }
        public string ManualFileId { get; set; }
        public string TemplateCsvFileImd5 { get; set; }
        public string ExampleCsvFileMd5 { get; set; }
        public string ManualFileMd5 { get; set; }

        /// <summary>
        /// Id чата куда добавлен бот для отправки уведомлений
        /// </summary>
        public string PrivateGroupChatId { get; set; }
        public int? BotInfoId { get; set; }

        /// <summary>
        /// флаг указывающий нужно ли требовать от пользователя указывать свой номер телефона перед оформлением заказа
        /// </summary>
        public bool VerifyTelephone { get; set; }

        /// <summary>
        /// флаг указывающий будет ли владелец получать уведомления о заказах, заявках в Личку.
        /// </summary>
        public bool OwnerPrivateNotify { get; set; }

        /// <summary>
        /// FileId картиники с подсказкой, как указать никнейм (юзернейм)
        /// </summary>
        public string UserNameFaqFileId { get; set; }
        public BotInfo BotInfo { get; set; }
    }
}
