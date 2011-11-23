using System;
using System.IO;
using System.Xml;

namespace migration
{
    internal static class functions
    {
        /// <summary>
        /// Настройки XML для удобного представления в текстовом редакторе.
        /// </summary>
        /// <returns>Настройки XML</returns>
        internal static XmlWriterSettings XmlSettings()
        {
            XmlWriterSettings settings = new XmlWriterSettings();

            // включаем отступ для элементов XML документа
            // (позволяет наглядно изобразить иерархию XML документа)
            settings.Indent = true;
            settings.IndentChars = "  "; // задаем отступ, здесь у меня 2 пробела

            // задаем переход на новую строку
            settings.NewLineChars = "\n";

            // Нужно ли опустить строку декларации формата XML документа
            // речь идет о строке вида "<?xml version="1.0" encoding="utf-8"?>"
            settings.OmitXmlDeclaration = true;
            return settings;
        }
        /// <summary>
        /// Получить имя файла для сохранения ревизии на основе информации о ней.
        /// </summary>
        /// <param name="info">Информация о ревизии</param>
        /// <returns>Имя файла ревизии</returns>
        internal static string GetFileName(RevisionInfo info)
        {
            if (Configuration.isLoad)
            {
                if (!Directory.Exists(Configuration.versionDirectory))
                    Directory.CreateDirectory(Configuration.versionDirectory);
                return Configuration.versionDirectory + @"\" + info.GenerateDateTime.ToString("dd.MM.yyyy-") + info.GenerateDateTime.ToString("hh.mm-") + info.Author + ".xml";
            }
            else
                throw new Exception("Не найден файл конфигурации");
        }
        /// <summary>
        /// Уничтожение файлов версий и каталога их хранения.
        /// </summary>
        internal static void DeleteVersionDirectory()
        {
            if (Configuration.isLoad)
            {
                if (Directory.Exists(Configuration.versionDirectory))
                    Directory.Delete(Configuration.versionDirectory, true);
            }
            else
                throw new Exception("Не найден файл конфигурации");
        }
        /// <summary>
        /// Загружает содержимое файла в одну строку.
        /// </summary>
        /// <param name="fileName">Имя файла, содержимое которого надо загрузить</param>
        /// <returns>Содержимое файла, преобразованное в одну строку через пробел</returns>
        internal static string LoadFileToStringCollection(string fileName)
        {
            string retStr = "";

            using (TextReader reader = File.OpenText(fileName))
            {
                string s = "";
                do
                {
                    s = reader.ReadLine();
                    if (s != null) if (s.Trim() != "") retStr += s.Trim() + " ";
                } while (s != null);
            }
            return retStr;
        }
    }
}
        