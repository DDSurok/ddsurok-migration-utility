using System.Data.SqlClient;

namespace migration
{
    internal static partial class DatabaseAdapter
    {
        /// <summary>
        /// Заполняем базу таблицами необходимыми для работы таблицами и триггерами.
        /// </summary>
        internal static void IntegrateServiceDataInDatabase()
        {
            SqlCommand command = connection.CreateCommand();
            command.CommandText = "ALTER DATABASE [" + Configuration.databaseName + "] SET TRUSTWORTHY ON";
            command.ExecuteNonQuery();
            DatabaseAdapter.RemoveServiceDataInDatabase();
            command.CommandText = functions.LoadFileToStringCollection("SQL/CreateSchema.sql");
            command.ExecuteNonQuery();
            command.CommandText = functions.LoadFileToStringCollection("SQL/CreateTables.sql");
            command.ExecuteNonQuery();
            System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(".");
            command.CommandText = @"CREATE ASSEMBLY CLRFunctions FROM '" + di.FullName + @"\SqlCLR\SqlCLR.dll' WITH PERMISSION_SET = UNSAFE";
            command.ExecuteNonQuery();
            //command.CommandText = @"CREATE ASYMMETRIC KEY [DDSurok] FROM EXECUTABLE FILE = '" + di.FullName + @"\SqlCLR\SqlCLR.dll'";
            //command.ExecuteNonQuery();
            command.CommandText = functions.LoadFileToStringCollection("SQL/CreateCLRFunction.sql");
            command.ExecuteNonQuery();
            command.CommandText = functions.LoadFileToStringCollection("SQL/CreateDDLTriggers.sql");
            command.ExecuteNonQuery();
            command.CommandText = @"sp_configure 'clr enabled', 1";
            command.ExecuteNonQuery();
            command.CommandText = @"reconfigure";
            command.ExecuteNonQuery();
        }
        /// <summary>
        /// Очистка базы данных от созданных при инициализации служебных структур.
        /// </summary>
        internal static void RemoveServiceDataInDatabase()
        {
            SqlCommand command = connection.CreateCommand();
            command.CommandText = functions.LoadFileToStringCollection("SQL/IfExists.sql");
            command.ExecuteNonQuery();
        }
    }
}
