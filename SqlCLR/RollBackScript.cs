using System;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.IO;
using System.Xml;
using Microsoft.SqlServer.Server;

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
            string temp;
            do
            {
                temp = tr.ReadLine();
                returnString += temp + "_";
            } while (temp == "");
        }
        return new SqlString(returnString);
    }
};

