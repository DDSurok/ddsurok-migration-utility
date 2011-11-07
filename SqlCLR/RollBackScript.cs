using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Xml;
using System.IO;

public partial class UserDefinedFunctions
{
    [Microsoft.SqlServer.Server.SqlFunction]
    public static SqlString RollBackScript(SqlXml data)
    {
        // Поместите здесь свой код
        string returnString = string.Empty;
        Stream ms = new MemoryStream();
        using (XmlWriter output = XmlWriter.Create(ms))
        {
            output.WriteNode(data.CreateReader(), true);
        }
        using (TextReader tr = new StreamReader(ms))
        {
            SqlConnection connection = new SqlConnection("Data Source=" + migration.ConfigFile.serverName + ";Integrated Security=True");
            connection.Open();
            connection.ChangeDatabase(migration.ConfigFile.databaseName);
            SqlCommand command = connection.CreateCommand();
            command.CommandText = "INSERT INTO [dds].[down] (script) VALUES ('" + tr.ReadLine() + "')";
            command.ExecuteNonQuery();
            connection.Close();
        }
        //string[] array = data..ToString().Split(new char[] { '_' }, StringSplitOptions.RemoveEmptyEntries);
        //switch (array[1].ToUpper())
        //{
        //    case "TABLE":
        //        switch (array[0].ToUpper())
        //        {
        //            case "CREATE":
        //                returnString = "DROP TABLE ";
        //                break;
        //        }
        //        break;
        //}
        return new SqlString(returnString);
    }
};

