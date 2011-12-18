using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Xml;
using System.Data.SqlClient;

namespace migration.SqlCLR
{
    internal class Options
    {
        public string EventType;
        public string ServerName;
        public string DatabaseName;
        public string ObjectName;
        public string ObjectType;
        public string SchemaName;
        public string CommandText;
    }

    /// <summary>
    /// Стандартный класс для реализации функций, определяемых пользователем.
    /// </summary>
    public class UserDefinedFunctions
    {
        /// <summary>
        /// Реализует генерацию скриптов, отменяющих изменения.
        /// </summary>
        /// <param name="data">Данные о изменении</param>
        /// <returns>Строка откатывающего скрипта</returns>
        [Microsoft.SqlServer.Server.SqlFunction]
        public static SqlString RollBackScript(SqlXml data)
        {
            Options options = new Options();
            // Поместите здесь свой код
            string returnString = "";
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
                                case "ObjectType":
                                    reader.Read();
                                    options.ObjectType = reader.Value;
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
            switch (options.ObjectType)
            {
                case "TABLE":
                    switch (options.EventType)
                    {
                        case "DROP_TABLE":
                            returnString = GetScriptCreateTable(options);
                            break;
                        case "CREATE_TABLE":
                            returnString = "DROP TABLE [" + options.SchemaName + "].[" + options.ObjectName + "]";
                            break;
                        default:
                            returnString = "не удалось сгенерировать";
                            break;
                    }
                    break;
                default:
                    returnString = "не удалось сгенерировать";
                    break;
            }
            return new SqlString(returnString);
        }

        private static string GetScriptCreateTable(Options options)
        {
            //SqlCommand command = connection.CreateCommand();
            //command.CommandText = "SELECT OBJECT_DEFINITION(OBJECT_ID('[" + options.SchemaName + "].[" + options.ObjectName + "]')) AS SCRIPT";
            //SqlDataReader reader = command.ExecuteReader();
            //reader.Read();
            //return reader["SCRIPT"].ToString();
            return "Не удалось сгенерировать";
        }
    }
}