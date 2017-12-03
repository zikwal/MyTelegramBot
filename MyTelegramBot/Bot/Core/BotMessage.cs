using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using Telegram.Bot;
using Telegram.Bot.Types;
using System.Threading.Tasks;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputMessageContents;
using Telegram.Bot.Types.ReplyMarkups;
using Newtonsoft.Json;
using System.Web;
using Telegram.Bot.Types.InlineKeyboardButtons;



namespace MyTelegramBot.Bot
{
    public class BotMessage
    {
        public BotMessage()
        {
          
        }

        /// <summary>
        /// Текст сообщения
        /// </summary>
        public string TextMessage { get; set; }

        /// <summary>
        /// Клавиатула из Inline кнопок
        /// </summary>
        public IReplyMarkup MessageReplyMarkup { get; set; }

        /// <summary>
        /// текст для AnswerCallbackQueryAsync
        /// </summary>
        public string CallBackTitleText { get; set; }

        public string Url { get; set; }

        /// <summary>
        /// Кнопка назад. Для некоторых случаев
        /// </summary>
        protected InlineKeyboardCallbackButton BackBtn { get; set; }


        public MediaFile MediaFile { get; set; }

        public string BuildCallData (string CommandName, params int [] Argument)
        {
            BotCommand command = new BotCommand
            {
                Cmd = CommandName,
                Arg = new List<int>()
            };

            for (int i = 0; i < Argument.Length; i++)
                command.Arg.Add(Argument[i]);

            return JsonConvert.SerializeObject(command);
        }

        public static string Bold(string value)
        {
            return "<b>" + value + "</b>";
        }

        public static string Italic(string value)
        {
            return "<i>" + value + "</i>";
        }

        public static string NewLine()
        {
            return "\r\n";
        }

        public static string HrefUrl(string url, string text)
        {
            const string quote = "\"";
            return "<a href=" + quote+ url + quote+ ">" + text + "</a>";
        }
    }

    public class MediaFile
    {
        public FileToSend FileTo { get; set; }

        public EnumMediaFile TypeFileTo { get; set; }

        /// <summary>
        /// Id файла в таблице AttachmentFs (таблица в которой храняться сами файлы)
        /// </summary>
        public int AttachmentFsId { get; set; }

        /// <summary>
        /// текстовое сообщние под файлом
        /// </summary>
        public string Caption { get; set; }

        public static EnumMediaFile HowMediaType(int? TypeId)
        {
            if (TypeId == 1)
                return EnumMediaFile.Photo;

            if (TypeId == 2)
                return EnumMediaFile.Video;

            if (TypeId == 3)
                return EnumMediaFile.Audio;

            if (TypeId == 4)
                return EnumMediaFile.Voice;

            if (TypeId == 5)
                return EnumMediaFile.VideoNote;

            if (TypeId == 6)
                return EnumMediaFile.Document;

            else
                return EnumMediaFile.Document;
        }
    }

    public enum EnumMediaFile
    {
         Photo=1,
         Video=2,
         Audio=3,
         Voice=4,
         VideoNote=5,
         Document=6

    }
}
