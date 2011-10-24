using System.Xml;

namespace migration
{
    public class Config
    {
        static internal void Load()
        {
            using (XmlReader reader = XmlReader.Create("migration.conf"))
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
                throw new System.ArgumentException("Ошибочный файл настроек программы.\nПерезапустите генератор файла конфигурации.");
        }
        static public string serverName { get; set; }
        static public string databaseName { get; set; }
    }
}
