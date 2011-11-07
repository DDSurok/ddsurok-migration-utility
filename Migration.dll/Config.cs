using System;
using System.IO;
using System.Xml;

namespace migration
{
    public static class Config
    {
        public static string configFileName = @"migration.conf";
        public static XmlWriterSettings XmlSettings()
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
        /// 
        /// </summary>
        /// <param name="Comment"></param>
        /// <returns></returns>
        internal static string GetFileName(RevisionInfo info)
        {
            if (ConfigFile.isLoad)
            {
                if (!Directory.Exists(ConfigFile.versionDirectory))
                    Directory.CreateDirectory(ConfigFile.versionDirectory);
                return ConfigFile.versionDirectory + @"\" + info.GenerateDateTime.ToString("dd.MM.yyyy-") + info.GenerateDateTime.ToString("hh.mm-") + info.Author + "-" + info.Comment + ".xml";
            }
            else
                throw new Exception("Не найден файл конфигурации");
        }
        /// <summary>
        /// 
        /// </summary>
        internal static void DeleteVersionDirectory()
        {
            if (ConfigFile.isLoad)
            {
                if (Directory.Exists(ConfigFile.versionDirectory))
                    Directory.Delete(ConfigFile.versionDirectory, true);
            }
            else
                throw new Exception("Не найден файл конфигурации");
        }
    }
}
        