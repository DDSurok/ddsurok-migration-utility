using System.Collections.Specialized;
using System.Xml;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using Microsoft.SqlServer.Management.Smo;

namespace migration
{
    static public class cInit
    {
        static private XmlWriter output;
        static private Microsoft.SqlServer.Management.Smo.Database database;
        static private Server server;
        static private RevisionInfo currentRevision;
        /// <summary>
        /// Основной метод класса
        /// </summary>
        static internal void _Main(string Comment)
        {
            functions.DeleteVersionDirectory();
            cInit.currentRevision = RevisionInfo.GenerateRevisionInfo(Comment);
            using (cInit.output = XmlWriter.Create(functions.GetFileName(cInit.currentRevision), functions.XmlSettings()))
            {
                // Создание объектов для работы с БД
                cInit.server = new Server(ConfigFile.serverName);
                cInit.database = server.Databases[ConfigFile.databaseName];
                    
                // Пишем информацию в XML о типе записи
                cInit.WriteXMLHeader();

                // Инициализируем базу данных на работу с нашей программой
                DatabaseAdapter.IntegrateServiceDataInDatabase();
                DatabaseAdapter.UpdateVersionDatabase(cInit.currentRevision);

                // Сохранение информации о таблицах
                cInit.GenerateTableScriptsWithDependence();
                //// Собственно схемы таблиц
                //Init.GenerateTableScripts();
                // Триггеры к таблицам
                cInit.GenerateTriggerScripts();
                //// Индексы к таблицам
                //Init.GenerateIndexScripts();
                //// Проверки к таблицам
                //Init.GenerateCheckScripts();
                //// Внешние ключи
                //Init.GenerateForeignKeyScripts();
                // Сохранение информации о правилах
                cInit.GenerateRuleScripts();
                // Сохранение информации о ролях
                cInit.GenerateRoleScripts();
                // Сохранение информации о хранимых процедурах
                cInit.GenerateStoredProcScripts();

                // Пишем подвал XML-документа
                cInit.WriteXMLSuffix();
            }
        }
        /// <summary>
        /// Записываем заголовок файла версий
        /// </summary>
        private static void WriteXMLHeader()
        {
            cInit.output.WriteStartDocument();
            cInit.output.WriteStartElement("Revision");
            cInit.output.WriteAttributeString("Database", ConfigFile.databaseName);
            cInit.output.WriteAttributeString("Create_date", cInit.currentRevision.GenerateDateTime.ToShortDateString());
            cInit.output.WriteAttributeString("Create_time", cInit.currentRevision.GenerateDateTime.ToShortTimeString());
            cInit.output.WriteAttributeString("Id", cInit.currentRevision.HashCode);
            cInit.output.WriteStartElement("Comment");
            cInit.output.WriteString(cInit.currentRevision.Comment);
            cInit.output.WriteEndElement();
            cInit.output.WriteStartElement("UpScripts");
            cInit.output.WriteStartElement("IfExistsDatabase");
            cInit.output.WriteString("\n");
            cInit.output.WriteString("IF  EXISTS (SELECT name FROM sys.databases WHERE name = N'" + ConfigFile.databaseName + "')\n");
            cInit.output.WriteString("DROP DATABASE " + ConfigFile.databaseName + "\n");
            cInit.output.WriteString("GO\n");
            cInit.output.WriteEndElement(); // "IfExistsDatabase"
            cInit.output.WriteStartElement("CreateDatabase");
            cInit.output.WriteString("\n");
            cInit.output.WriteString("CREATE DATABASE " + ConfigFile.databaseName + "\n");
            cInit.output.WriteString("GO\n");
            cInit.output.WriteEndElement(); // "CreateDatabase"
        }
        /// <summary>
        /// Записываем подвал файла версий
        /// </summary>
        private static void WriteXMLSuffix()
        {
            cInit.output.WriteEndElement(); // "UpScripts"
            cInit.output.WriteStartElement("DownScripts");
            cInit.output.WriteEndElement(); // "DownScripts"
            cInit.output.WriteEndElement(); // "Revision"
            cInit.output.WriteEndDocument();
        }
        /// <summary>
        /// Генерирует скрипты создания таблиц со всеми зависимостями и пишет их в <code>output</code>
        /// </summary>
        private static void GenerateTableScriptsWithDependence()
        {
            cInit.output.WriteStartElement("Tables");
            Scripter script = new Scripter(server);
            script.Options.ScriptDrops = false;
            script.Options.WithDependencies = true;
            script.Options.Indexes = true;   // To include indexes
            script.Options.DriAllConstraints = true;   // to include referential constraints in the script

            // Iterate through the tables in database and script each one. Display the script.   
            foreach (Table tb in database.Tables)
            {
                // check if the table is not a system table
                if (tb.IsSystemObject == false)
                {
                    cInit.output.WriteElementString("Header", "[" + tb.Schema + "].[" + tb.Name + "]");
                    StringCollection sc = script.Script(new Urn[] { tb.Urn });
                    cInit.output.WriteStartElement("script");
                    cInit.output.WriteString("\n");
                    foreach (string st in sc)
                    {
                        cInit.output.WriteString(st.Trim() + "\n");
                    }
                    cInit.output.WriteEndElement();
                }
            }
            cInit.output.WriteEndElement();
        }
        /// <summary>
        /// Генерирует скрипты создания хранимых процедур и пишет их в <code>output</code>
        /// </summary>
        private static void GenerateStoredProcScripts()
        {
            // Создали открывающийся тег
            cInit.output.WriteStartElement("Stored-Procedures");
            foreach (StoredProcedure proc in cInit.database.StoredProcedures)
            {
                if ((proc.Schema != "sys") && ((proc.Schema != "dbo") || (proc.Name.Substring(0, 2) != "sp")))
                {
                    cInit.output.WriteElementString("Header", proc.Name);
                    StringCollection strCollection = new StringCollection();
                    SqlSmoObject[] smoObj = new SqlSmoObject[1];
                    smoObj[0] = proc;
                    Scripter scriptor = new Scripter(cInit.server);
                    scriptor.Options.AllowSystemObjects = false;
                    scriptor.Options.Indexes = true;
                    scriptor.Options.DriAll = true;
                    scriptor.Options.Default = true;
                    scriptor.Options.IncludeIfNotExists = true;
                    scriptor.Options.WithDependencies = true;
                    scriptor.Options.ScriptSchema = true;
                    strCollection = scriptor.Script(smoObj);
                    cInit.output.WriteStartElement("script");
                    cInit.output.WriteString("\n");
                    foreach (string s in strCollection)
                    {
                        cInit.output.WriteString(s.Trim() + "\n");
                    }
                    cInit.output.WriteEndElement();
                }
            }
            cInit.output.WriteEndElement();
        }
        /// <summary>
        /// Генерирует скрипты создания ролей и пишет их в <code>output</code>
        /// </summary>
        private static void GenerateRoleScripts()
        {
            // Создали открывающийся тег
            cInit.output.WriteStartElement("Roles");
            foreach (DatabaseRole role in cInit.database.Roles)
            {
                if (role.Name == "public")
                    continue;
                cInit.output.WriteElementString("Header", role.Name);
                StringCollection strCollection = new StringCollection();
                SqlSmoObject[] smoObj = new SqlSmoObject[1];
                smoObj[0] = role;
                Scripter scriptor = new Scripter(cInit.server);
                scriptor.Options.AllowSystemObjects = false;
                scriptor.Options.IncludeIfNotExists = true;
                strCollection = scriptor.Script(smoObj);
                cInit.output.WriteStartElement("script");
                cInit.output.WriteString("\n");
                foreach (string s in strCollection)
                {
                    cInit.output.WriteString(s.Trim() + "\n");
                }
                cInit.output.WriteEndElement();
            }
            cInit.output.WriteEndElement();
        }
        /// <summary>
        /// Генерирует скрипты создания правил и пишет их в <code>output</code>
        /// </summary>
        private static void GenerateRuleScripts()
        {
            // Создали открывающийся тег
            cInit.output.WriteStartElement("Rules");
            foreach (Rule rule in cInit.database.Rules)
            {
                cInit.output.WriteElementString("Header", rule.Name);
                StringCollection strCollection = new StringCollection();
                SqlSmoObject[] smoObj = new SqlSmoObject[1];
                smoObj[0] = rule;
                Scripter scriptor = new Scripter(cInit.server);
                scriptor.Options.AllowSystemObjects = false;
                scriptor.Options.Indexes = true;
                scriptor.Options.DriAll = true;
                scriptor.Options.Default = true;
                scriptor.Options.IncludeIfNotExists = true;
                scriptor.Options.WithDependencies = true;
                scriptor.Options.ScriptSchema = true;
                strCollection = scriptor.Script(smoObj);
                cInit.output.WriteStartElement("script");
                cInit.output.WriteString("\n");
                foreach (string s in strCollection)
                {
                    cInit.output.WriteString(s.Trim() + "\n");
                }
                cInit.output.WriteEndElement();
            }
            output.WriteEndElement();
        }
        /// <summary>
        /// Генерирует скрипты создания таблиц и пишет их в <code>output</code>
        /// </summary>
        private static void GenerateTableScripts()
        {
            // Создали открывающийся тег
            cInit.output.WriteStartElement("Tables");
            foreach (Table table in cInit.database.Tables)
            {
                cInit.output.WriteElementString("Header", "[" + table.Schema + "].[" + table.Name + "]");
                StringCollection strCollection = new StringCollection();
                SqlSmoObject[] smoObj = new SqlSmoObject[1];
                smoObj[0] = table;
                Scripter scriptor = new Scripter(cInit.server);
                scriptor.Options.AllowSystemObjects = false;
                //scriptor.Options.Indexes = true;
                //scriptor.Options.DriAll = true;
                scriptor.Options.IncludeIfNotExists = true;
                scriptor.Options.Default = true;
                scriptor.Options.WithDependencies = true;
                scriptor.Options.ScriptSchema = true;
                strCollection = scriptor.Script(smoObj);
                cInit.output.WriteStartElement("script");
                cInit.output.WriteString("\n");
                foreach (string s in strCollection)
                {
                    cInit.output.WriteString(s.Trim() + "\n");
                }
                cInit.output.WriteEndElement();
            }
            cInit.output.WriteEndElement();
        }
        /// <summary>
        /// Генерирует скрипты создания триггеров и пишет их в <code>output</code>
        /// </summary>
        private static void GenerateTriggerScripts()
        {
            // Создали открывающийся тег
            cInit.output.WriteStartElement("DML-Triggers");
            foreach (Table table in cInit.database.Tables)
            {
                foreach (Trigger trigger in table.Triggers)
                {
                    cInit.output.WriteElementString("Header", trigger.Name);
                    StringCollection strCollection = new StringCollection();
                    SqlSmoObject[] smoObj = new SqlSmoObject[1];
                    smoObj[0] = trigger;
                    Scripter scriptor = new Scripter(cInit.server);
                    scriptor.Options.AllowSystemObjects = false;
                    scriptor.Options.DriAll = true;
                    scriptor.Options.Default = true;
                    scriptor.Options.IncludeIfNotExists = true;
                    scriptor.Options.WithDependencies = false;
                    scriptor.Options.ScriptSchema = true;
                    strCollection = scriptor.Script(smoObj);
                    cInit.output.WriteStartElement("script");
                    cInit.output.WriteString("\n");
                    foreach (string s in strCollection)
                    {
                        cInit.output.WriteString(s.Trim() + "\n");
                    }
                    cInit.output.WriteEndElement();
                }
            }
            cInit.output.WriteEndElement();
        }
        /// <summary>
        /// Генерирует скрипты создания проверок и пишет их в <code>output</code>
        /// </summary>
        private static void GenerateCheckScripts()
        {
            // Создали открывающийся тег
            cInit.output.WriteStartElement("Checks");
            foreach (Table table in cInit.database.Tables)
            {
                foreach (Check check in table.Checks)
                {
                    cInit.output.WriteElementString("Header", check.Name);
                    StringCollection strCollection = new StringCollection();
                    SqlSmoObject[] smoObj = new SqlSmoObject[1];
                    smoObj[0] = check;
                    Scripter scriptor = new Scripter(cInit.server);
                    scriptor.Options.IncludeIfNotExists = true;
                    scriptor.Options.AllowSystemObjects = false;
                    scriptor.Options.DriAll = true;
                    scriptor.Options.Default = true;
                    scriptor.Options.WithDependencies = false;
                    scriptor.Options.ScriptSchema = true;
                    strCollection = scriptor.Script(smoObj);
                    cInit.output.WriteStartElement("script");
                    cInit.output.WriteString("\n");
                    foreach (string s in strCollection)
                    {
                        cInit.output.WriteString(s.Trim() + "\n");
                    }
                    cInit.output.WriteEndElement();
                }
            }
            cInit.output.WriteEndElement();
        }
        /// <summary>
        /// Генерирует скрипты создания внешних ключей и пишет их в <code>output</code>
        /// </summary>
        private static void GenerateForeignKeyScripts()
        {
            // Создали открывающийся тег
            cInit.output.WriteStartElement("Foreign-Key");
            foreach (Table table in cInit.database.Tables)
            {
                foreach (ForeignKey fk in table.ForeignKeys)
                {
                    cInit.output.WriteElementString("Header", fk.Name);
                    StringCollection strCollection = new StringCollection();
                    SqlSmoObject[] smoObj = new SqlSmoObject[1];
                    smoObj[0] = fk;
                    Scripter scriptor = new Scripter(cInit.server);
                    scriptor.Options.IncludeIfNotExists = true;
                    scriptor.Options.AllowSystemObjects = false;
                    scriptor.Options.DriAll = true;
                    scriptor.Options.Default = true;
                    scriptor.Options.WithDependencies = false;
                    scriptor.Options.ScriptSchema = true;
                    strCollection = scriptor.Script(smoObj);
                    cInit.output.WriteStartElement("script");
                    cInit.output.WriteString("\n");
                    foreach (string s in strCollection)
                    {
                        cInit.output.WriteString(s.Trim() + "\n");
                    }
                    cInit.output.WriteEndElement();
                }
            }
            cInit.output.WriteEndElement();
        }
        /// <summary>
        /// Генерирует скрипты создания индексов и пишет их в <code>output</code>
        /// </summary>
        private static void GenerateIndexScripts()
        {
            // Создали открывающийся тег
            cInit.output.WriteStartElement("Indexes");
            foreach (Table table in cInit.database.Tables)
            {
                foreach (Index index in table.Indexes)
                {
                    cInit.output.WriteElementString("Header", index.Name);
                    StringCollection strCollection = new StringCollection();
                    SqlSmoObject[] smoObj = new SqlSmoObject[1];
                    smoObj[0] = index;
                    Scripter scriptor = new Scripter(cInit.server);
                    scriptor.Options.IncludeIfNotExists = true;
                    scriptor.Options.AllowSystemObjects = false;
                    scriptor.Options.DriAll = true;
                    scriptor.Options.Default = true;
                    scriptor.Options.WithDependencies = false;
                    scriptor.Options.ScriptSchema = true;
                    strCollection = scriptor.Script(smoObj);
                    cInit.output.WriteStartElement("script");
                    cInit.output.WriteString("\n");
                    foreach (string s in strCollection)
                    {
                        cInit.output.WriteString(s.Trim() + "\n");
                    }
                    cInit.output.WriteEndElement();
                }
            }
            cInit.output.WriteEndElement();
        }
    }
}
