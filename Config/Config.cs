using System.Xml;
using System.IO;

namespace migration
{
    static public class Config
    {
        static private string FileName = @"conf\migration.conf";
        static public void Load()
        {
            try
            {
                using (XmlReader reader = XmlReader.Create(Config.FileName))
                {
                    while (reader.Read())
                    {
                        switch (reader.NodeType)
                        {
                            case XmlNodeType.Element:
                                switch (reader.Name)
                                {
                                    case "serverName":
                                        reader.Read();
                                        Config.serverName = reader.Value;
                                        break;
                                    case "databaseName":
                                        reader.Read();
                                        Config.databaseName = reader.Value;
                                        break;
                                    case "remoteRepository":
                                        reader.Read();
                                        Config.remoteRepository = reader.Value;
                                        break;
                                }
                                break;
                            case XmlNodeType.Text:
                            case XmlNodeType.XmlDeclaration:
                            case XmlNodeType.ProcessingInstruction:
                            case XmlNodeType.Comment:
                            case XmlNodeType.EndElement:
                                break;
                        }
                    }
                }
                if ((Config.serverName.Trim() == "") || (Config.databaseName.Trim() == ""))
                {
                    File.Delete("migration.conf");
                    throw new System.ArgumentException("Ошибочный файл настроек программы.\nПерезапустите генератор файла конфигурации.");
                }
                Config.isLoad = true;
            }
            catch (FileNotFoundException)
            {
                Config.isLoad = false;
            }
        }
        static public void Write(string _Server, string _RemoteRepository)
        {
            Config.serverName = _Server;
            Config.remoteRepository = _RemoteRepository;
            Config.WriteData();
        }
        static public void Rewrite(string _Server, string _Database, string _RemoteRepository)
        {
            Config.serverName = _Server;
            Config.databaseName = _Database;
            Config.remoteRepository = _RemoteRepository;
            Config.WriteData();
        }
        static private void WriteData()
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
            settings.OmitXmlDeclaration = false;

            // FileName - имя файла, куда будет сохранен XML-документ
            // settings - настройки форматирования (и не только) вывода
            // (рассмотрен выше)
            using (XmlWriter output = XmlWriter.Create(Config.FileName, settings))
            {
                // Создали открывающийся тег
                output.WriteStartElement("MigrationConfigure");

                // Создаем элемент connectionString
                output.WriteElementString("serverName", Config.serverName);

                // Создаем элемент databaseName
                output.WriteElementString("databaseName", Config.databaseName);

                // Создаем элемент Repository
                output.WriteElementString("remoteRepository", Config.remoteRepository);

                // Сбрасываем буфферизированные данные
                output.Flush();

                // Закрываем фаил, с которым связан output
                output.Close();
            }
        }
        static public bool isLoad { get; private set; }
        static public string serverName { get; private set; }
        static public string databaseName { get; private set; }
        static public string remoteRepository { get; private set; }
    }
}
