using System.IO;
using System.Xml;

namespace migration
{
    static public class ConfigFile
    {
        static public void Load()
        {
            try
            {
                using (XmlReader reader = XmlReader.Create(Config.configFileName))
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
                                        ConfigFile.serverName = reader.Value;
                                        break;
                                    case "databaseName":
                                        reader.Read();
                                        ConfigFile.databaseName = reader.Value;
                                        break;
                                    case "versionDirectory":
                                        reader.Read();
                                        ConfigFile.versionDirectory = reader.Value;
                                        break;
                                    case "nickName":
                                        reader.Read();
                                        ConfigFile.nickName = reader.Value;
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
                if ((ConfigFile.serverName.Trim() == "") || (ConfigFile.databaseName.Trim() == ""))
                {
                    File.Delete("migration.conf");
                    throw new System.ArgumentException("Ошибочный файл настроек программы.\nПерезапустите генератор файла конфигурации.");
                }
                ConfigFile.isLoad = true;
            }
            catch (FileNotFoundException)
            {
                ConfigFile.isLoad = false;
            }
        }
        static public void Write(string _Server, string _VersionDirectory, string _NickName)
        {
            ConfigFile.serverName = _Server;
            ConfigFile.versionDirectory = _VersionDirectory;
            ConfigFile.nickName = _NickName;
            ConfigFile.WriteData();
        }
        static public void Rewrite(string _Server, string _Database, string _VersionDirectory, string _NickName)
        {
            ConfigFile.serverName = _Server;
            ConfigFile.databaseName = _Database;
            ConfigFile.versionDirectory = _VersionDirectory;
            ConfigFile.nickName = _NickName;
            ConfigFile.WriteData();
        }
        static private void WriteData()
        {
            using (XmlWriter output = XmlWriter.Create(Config.configFileName, Config.XmlSettings()))
            {
                // Создали открывающийся тег
                output.WriteStartElement("MigrationConfigure");

                // Создаем элемент connectionString
                output.WriteElementString("serverName", ConfigFile.serverName);

                // Создаем элемент databaseName
                output.WriteElementString("databaseName", ConfigFile.databaseName);

                // Создаем элемент versionDirectory
                output.WriteElementString("versionDirectory", ConfigFile.versionDirectory);

                // Создаем элемент nickName
                output.WriteElementString("nickName", ConfigFile.nickName);

                // Сбрасываем буфферизированные данные
                output.Flush();

                // Закрываем фаил, с которым связан output
                output.Close();
            }
        }
        static public bool isLoad { get; private set; }
        static public string serverName { get; private set; }
        static public string databaseName { get; private set; }
        static public string versionDirectory { get; private set; }
        static public string nickName { get; private set; }
    }
}
