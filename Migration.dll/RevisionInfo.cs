using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Xml;

namespace migration.Library
{
    /// <summary>
    /// Механизмы работы с файлом версии
    /// </summary>
    public struct RevisionInfo
    {
        /// <summary>
        /// Имя файла ревизии.
        /// Если равно <code>string.Empty</code>,
        /// то ревизия еще не сохранена в файл.
        /// </summary>
        public string FileName { get; set; }
        /// <summary>
        /// Дата и время создания ревизии.
        /// </summary>
        public DateTime GenerateDateTime { get; set; }
        /// <summary>
        /// Автор ревизии.
        /// </summary>
        public string Author { get; set; }
        /// <summary>
        /// Комментарий к ревизии.
        /// </summary>
        public string Comment { get; set; }
        /// <summary>
        /// Номер ревизии по порядку.
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Уникальный хеш-код ревизии.
        /// Обеспечивает уникальность на основе информации
        /// о дате и времени, а так же авторе ревизии.
        /// </summary>
        public string HashCode { get; set; }
        /// <summary>
        /// Получить информацию о ривизии из файла.
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
        /// Составление информации по ривизии по текущей дате и времени.
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
            returnValue.Author = Configuration.nickName;
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
        /// Взять скрипты повышения из текущей ревизии.
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
                            switch (reader.Name.Substring(0, 2))
                            {
                                case "up":
                                    reader.Read();
                                    ret.Add(reader.Value);
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
        /// Взять скрипты понижения до текущей ревизии.
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
                            switch (reader.Name.Substring(0, 4))
                            {
                                case "down":
                                    reader.Read();
                                    ret.Add(reader.Value);
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
        /// Преобразовать информацию о ревизии в одну строку.
        /// </summary>
        /// <returns>Строка с информацией о ревизии</returns>
        public override string ToString()
        {
            return this.Id.ToString("0000  ") +
                    "Author: " + this.Author + "\t" +
                    this.GenerateDateTime.ToString("dd MMMM yyyy, hh:mm\t") +
                    "Comment: " + this.Comment.Replace("\n", " \t");
        }
        /// <summary>
        /// Преобразовать информацию о ревизии в три строки.
        /// Данные хранятся в следующем виде:
        /// 1 строка -- Номер ревизии и автор;
        /// 2 строка -- Дата и время создания;
        /// 3 строка -- Комментарий к ревизии.
        /// </summary>
        /// <returns>Строки с информацией</returns>
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