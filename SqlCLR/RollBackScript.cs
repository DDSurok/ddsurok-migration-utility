using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Xml;
using System.Data.SqlClient;
using System.IO.Pipes;
using System.IO;
using System.Security;
using System.Security.Principal;
using Microsoft.SqlServer.Server;
using System;
using System.Collections;

public class Options
{
    public string 
        EventType,
        ServerName,
        DatabaseName,
        ObjectName,
        ObjectType,
        SchemaName,
        CommandText;
}

public class UserDefinedFunctions
{
    [SqlProcedure]
    public static void RollBackScript(SqlXml data)
    {
        using (StreamWriter fw = File.CreateText(@"c:/hello.txt"))
        {
            fw.AutoFlush = true;
            fw.WriteLine("Hello, World!");
            fw.Close();
        }

        string returnString = "";
        try
        {
            //WindowsIdentity winId = SqlContext.WindowsIdentity;
            //if (winId == null)
            //    throw new SecurityException("Won't work with SQL Server authentication");
            //WindowsImpersonationContext impersCtxt = null;
            //impersCtxt = winId.Impersonate();
            
            Options options = new Options();
            // Поместите здесь свой код
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
                    }
                    break;
                default:
                    returnString = "необработанная операция";
                    break;
            }
            using (SqlConnection connection = new SqlConnection("context connection=true"))
            {
                connection.Open();
                string sql = "INSERT INTO [dds].[down] (script) values ( @script )";
                SqlCommand cmd = new SqlCommand(sql, connection);
                cmd.Parameters.Add("@script", returnString);
                cmd.ExecuteNonQuery();
                connection.Close();
            }
        }
        catch (Exception ex)
        {
            SqlContext.Pipe.Send(ex.Message);
            returnString = "ERROR: " + ex.Message;
            using (SqlConnection connection = new SqlConnection("context connection=true"))
            {
                connection.Open();
                string sql = "INSERT INTO [dds].[down] (script) values ( @script )";
                SqlCommand cmd = new SqlCommand(sql, connection);
                cmd.Parameters.Add("@script", returnString);
                cmd.ExecuteNonQuery();
                connection.Close();
            }
        }
    }
    
    private static string GetScriptCreateTable(Options options)
    {
        try
        {
            using (NamedPipeClientStream pipeClient =
                //new NamedPipeClientStream(@"migration-dds"))
                new NamedPipeClientStream("localhost", "migration-dds",
                PipeDirection.InOut, PipeOptions.None,
                TokenImpersonationLevel.None))
            {
                using (StreamWriter sw = new StreamWriter(pipeClient))
                using (StreamReader sr = new StreamReader(pipeClient))
                {
                    sw.AutoFlush = true;
                    pipeClient.Connect();

                    // Verify that this is the "true server"
                    // The client security token is sent with the first write.
                    sw.WriteLine(options.ServerName);
                    sw.WriteLine(options.DatabaseName);
                    sw.WriteLine(options.EventType);
                    sw.WriteLine(options.ObjectName);
                    sw.WriteLine(options.SchemaName);

                    string ret = sr.ReadLine();
                    pipeClient.Close();
                    return ret;
                }
            }
        }
        catch (Exception ex)
        {
            SqlContext.Pipe.Send(ex.Message);
            return @"ERROR";
        }
    }
};

