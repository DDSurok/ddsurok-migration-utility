using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Xml;

namespace migration
{
    /// <summary>
    /// Механизмы работы с файлом версии
    /// </summary>
    public struct RevisionInfo
    {
        public string FileName { get; set; }
        public DateTime GenerateDateTime { get; set; }
        public string Author { get; set; }
        public string Comment { get; set; }
        public int Id { get; set; }
        public string HashCode { get; set; }
        /// <summary>
        /// Получить информацию о ривизии из файла
        /// </summary>
        /// <param name="File">Имя файла</param>
        /// <returns>Данные о ревизии</returns>
        public static RevisionInfo GetInfoFromFile(string File)
        {
            RevisionInfo tempInfo = new RevisionInfo();
            tempInfo.FileName = File;
            using (XmlReader reader = XmlReader.Create(File))
            {
                DateTime date = new DateTime();
                DateTime time = new DateTime();

                // Читаем данные из Xml
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            switch (reader.Name)
                            {
                                case "Revision":
                                    date = DateTime.Parse(reader.GetAttribute("Create_date"));
                                    time = DateTime.Parse(reader.GetAttribute("Create_time"));
                                    tempInfo.HashCode = reader.GetAttribute("Id");
                                    break;
                                case "Comment":
                                    reader.Read();
                                    tempInfo.Comment = reader.Value;
                                    break;
                            }
                            break;
                        case XmlNodeType.Attribute:
                        case XmlNodeType.Text:
                        case XmlNodeType.XmlDeclaration:
                        case XmlNodeType.ProcessingInstruction:
                        case XmlNodeType.Comment:
                        case XmlNodeType.EndElement:
                            break;
                    }
                }

                tempInfo.GenerateDateTime = new DateTime(date.Year, date.Month, date.Day, time.Hour, time.Minute, time.Second);
            }

            string[] tempArray = File.Substring(File.LastIndexOf('\\') + 1).Split(new char[] { '-' }, 3, StringSplitOptions.None);

            tempInfo.Author = tempArray[2].Substring(0, tempArray[2].Length - 4);
            //tempInfo.Comment = tempArray[3].Substring(0, tempArray[3].Length - 4);
            return tempInfo;
        }
        /// <summary>
        /// Составление информации по ривизии по текущей дате и времени
        /// </summary>
        /// <param name="Comment">Коментарий к ревизии</param>
        public static RevisionInfo GenerateRevisionInfo(string Comment)
        {
            RevisionInfo returnValue = new RevisionInfo();
            // Сбор информации по ревизии
            returnValue.FileName = string.Empty;
            returnValue.Id = -1;
            returnValue.GenerateDateTime = new DateTime(DateTime.Today.Year,
                                                        DateTime.Today.Month,
                                                        DateTime.Today.Day,
                                                        DateTime.Now.Hour,
                                                        DateTime.Now.Minute,
                                                        DateTime.Now.Second);
            returnValue.Author = ConfigFile.nickName;
            returnValue.Comment = Comment;
            SHA1 sha = new SHA1CryptoServiceProvider();
            byte[] byteHash =
                sha.ComputeHash(
                    Encoding.Unicode.GetBytes(
                        DateTime.Today.ToShortDateString()
                        + " " + DateTime.Now.ToShortTimeString()));
            returnValue.HashCode = "";
            for (int i = 0; i < byteHash.Length; i++)
                returnValue.HashCode += string.Format("{0:x2}", byteHash[i]);
            return returnValue;
        }
        /// <summary>
        /// Взять скрипты повышения из текущей ревизии
        /// </summary>
        /// <returns>Список скриптов</returns>
        public List<string> GetUpScripts()
        {
            if (this.FileName == string.Empty)
            {
                throw new Exception("Запрашиваются скрипты не сохраненной ревизии");
            }
            List<string> ret = new List<string>();
            using (XmlReader reader = XmlReader.Create(this.FileName))
            {
                // Читаем данные из Xml
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            switch (reader.Name)
                            {
                                case "UpScripts":
                                    reader.Read();
                                    while (reader.Name != "DownScripts")
                                    {
                                        reader.Read();
                                        ret.Add(reader.Value);
                                        reader.Read();
                                    }
                                    break;
                            }
                            break;
                        case XmlNodeType.Attribute:
                        case XmlNodeType.Text:
                        case XmlNodeType.XmlDeclaration:
                        case XmlNodeType.ProcessingInstruction:
                        case XmlNodeType.Comment:
                        case XmlNodeType.EndElement:
                            break;
                    }
                }
            }
            return ret;
        }
        /// <summary>
        /// Взять скрипты понижения до текущей ревизии
        /// </summary>
        /// <returns>Список скриптов</returns>
        public List<string> GetDownScripts()
        {
            if (this.FileName == string.Empty)
            {
                throw new Exception("Запрашиваются скрипты не сохраненной ревизии");
            }
            List<string> ret = new List<string>();
            using (XmlReader reader = XmlReader.Create(this.FileName))
            {
                // Читаем данные из Xml
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            switch (reader.Name)
                            {
                                case "DownScripts":
                                    reader.Read();
                                    while (reader.Name != "DownScripts")
                                    {
                                        reader.Read();
                                        ret.Add(reader.Value);
                                        if (reader.Read())
                                            break;
                                    }
                                    break;
                            }
                            break;
                        case XmlNodeType.Attribute:
                        case XmlNodeType.Text:
                        case XmlNodeType.XmlDeclaration:
                        case XmlNodeType.ProcessingInstruction:
                        case XmlNodeType.Comment:
                        case XmlNodeType.EndElement:
                            break;
                    }
                }
            }
            ret.Reverse();
            return ret;
        }
        /// <summary>
        /// Преобразовать информацию о ревизии в одну строку
        /// </summary>
        /// <returns>Строка с информацией о ревизии</returns>
        public override string ToString()
        {
            return this.Id.ToString("0000  ") +
                    "Author: " + this.Author + "\t" +
                    this.GenerateDateTime.ToString("dd MMMM yyyy, hh:mm\t") +
                    "Comment: " + this.Comment.Replace("\n", " \t");
        }

        public string[] ToStrings()
        {
            string[] ret = new string[3];
            ret[0] = this.Id.ToString("0000  ") + "Author: " + this.Author;
            ret[1] = this.GenerateDateTime.ToString("dd MMMM yyyy, hh:mm");
            ret[2] = "Comment: " + this.Comment;
            return ret;
        }
    }
}