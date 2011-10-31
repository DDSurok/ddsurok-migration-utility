using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Server;

public partial class UserDefinedFunctions
{
    [Microsoft.SqlServer.Server.SqlFunction]
    public static SqlString RollBackScript(SqlString TypeEvent, SqlString UpScript)
    {
        // Поместите здесь свой код
        return new SqlString(TypeEvent.ToString() + "___" + UpScript.ToString());
    }
};

