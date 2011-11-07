using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Security.Cryptography;
using System.Text;
using System.Globalization;
using System.Data.SqlClient;

namespace migration

{
    public static class RevisionList
    {
        public static List<RevisionInfo> GetRevisionList()
        {
            List<RevisionInfo> returnList = new List<RevisionInfo>();
            int i = 0;
            foreach(string File in Directory.GetFiles(ConfigFile.versionDirectory))
            {
                RevisionInfo tempInfo = new RevisionInfo();
                tempInfo.Id = i;
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
                                        date = DateTime.ParseExact(reader.GetAttribute("Create_date"), "dd.MM.yyyy", CultureInfo.InvariantCulture);
                                        time = DateTime.ParseExact(reader.GetAttribute("Create_time"), "hh:mm", CultureInfo.InvariantCulture);
                                        tempInfo.HashCode = reader.GetAttribute("Id");
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

                string[] tempArray = File.Substring(File.LastIndexOf('\\') + 1).Split(new char[] { '-' }, 4, StringSplitOptions.None);

                tempInfo.Author = tempArray[2];
                tempInfo.Comment = tempArray[3].Substring(0, tempArray[3].Length - 4);

                // Добавляем информацию в список
                returnList.Insert(0, tempInfo);

                // Переключаем счетчик файлов
                i++;
            }
            return returnList;
        }
        public static int GetCurrentRevision()
        {
            using (SqlConnection connection = new SqlConnection("Data Source=" + ConfigFile.serverName + ";Integrated Security=True"))
            {
                connection.Open();
                connection.ChangeDatabase(ConfigFile.databaseName);
                SqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT [dds].[version].[hashCode] AS hashCode FROM [dds].[version]";
                SqlDataReader reader = command.ExecuteReader();
                reader.Read();
                string hash = reader["hashCode"].ToString();
                foreach (RevisionInfo info in RevisionList.GetRevisionList())
                {
                    if (info.HashCode == hash)
                        return info.Id;
                }
            }
            throw new Exception("Текущая версия базы данных не найдена в списке ревизий");
        }
    }
    public struct RevisionInfo
    {
        public DateTime GenerateDateTime { get; set; }
        public string Author { get; set; }
        public string Comment { get; set; }
        public int Id { get; set; }
        public string HashCode { get; set; }
        /// <summary>
        /// Составление информации по ривизии по текущей дате и времени
        /// </summary>
        /// <param name="Comment">Коментарий к ревизии</param>
        public static RevisionInfo GenerateRevisionInfo(string Comment)
        {
            RevisionInfo returnValue = new RevisionInfo();
            // Сбор информации по ревизии
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
    }
    internal struct RevisionSimpleInfo
    {
        public string FileName { get; set; }
        public int Id { get; set; }
    }
}
