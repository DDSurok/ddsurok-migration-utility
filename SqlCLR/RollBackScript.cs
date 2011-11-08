using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Xml;
using Microsoft.SqlServer.Management.Smo;

public class Options
{
    public string 
        EventType,
        ServerName,
        DatabaseName,
        ObjectName,
        SchemaName,
        CommandText;
}

public class UserDefinedFunctions
{
    [Microsoft.SqlServer.Server.SqlFunction]
    public static SqlString RollBackScript(SqlXml data)
    {
        Options options = new Options();
        // Поместите здесь свой код
        string returnString = string.Empty;
        using (XmlReader reader = data.CreateReader())
        {
            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        switch (reader.Name)
                        {
                            case "EventType":
                                reader.Read();
                                options.EventType = reader.Value;
                                break;
                            case "ServerName":
                                reader.Read();
                                options.ServerName = reader.Value;
                                break;
                            case "DatabaseName":
                                reader.Read();
                                options.DatabaseName = reader.Value;
                                break;
                            case "ObjectName":
                                reader.Read();
                                options.ObjectName = reader.Value;
                                break;
                            case "SchemaName":
                                reader.Read();
                                options.SchemaName = reader.Value;
                                break;
                            case "CommandText":
                                reader.Read();
                                options.CommandText = reader.Value;
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
        Server server = new Server(options.ServerName);
        Database db = server.Databases[options.DatabaseName];
        return new SqlString(returnString);
    }
};

