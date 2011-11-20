using System.Data.SqlClient;
using System.IO;
using System.Xml;

namespace migration
{
    public static class Fix
    {
        private static RevisionInfo currentRevision;
        private static XmlWriter output;
        public static void Run(string Comment)
        {
            if (Database.GetCountChanges() > 0)    // Если изменения есть, то начинаем
            {
                Fix.currentRevision = RevisionInfo.GenerateRevisionInfo(Comment);
                FileStream fs = new FileStream(Config.GetFileName(Fix.currentRevision), FileMode.Append);
                output = XmlWriter.Create(fs, Config.XmlSettings());
                // Записываем в файл заголовок коммита
                WriteXMLHeader();
                // Пишем скрипты повышения
                Database.WriteScriptsUp(Fix.output);
                // Пишем скрипты понижения
                Database.WriteScriptsDown(Fix.output);
                // Чистим список изменений и обновляем версию СУБД
                Database.ClearUpDownScripts();
                Database.UpdateVersionDatabase(Fix.currentRevision);
                // Пишем подвал коммита
                WriteXMLSuffix();
                output.Close();
                fs.Close();        // Закрываем поток
            }
        }
        /// <summary>
        /// Запись заголовка записи в XML
        /// </summary>
        /// <param name="output">XML, куда пишется запись</param>
        private static void WriteXMLHeader()
        {
            Fix.output.WriteStartElement("Revision");
            Fix.output.WriteAttributeString("Database", ConfigFile.databaseName);
            Fix.output.WriteAttributeString("Create_date", Fix.currentRevision.GenerateDateTime.ToShortDateString());
            Fix.output.WriteAttributeString("Create_time", Fix.currentRevision.GenerateDateTime.ToShortTimeString());
            Fix.output.WriteAttributeString("Id", Fix.currentRevision.HashCode);
            Fix.output.WriteStartElement("Comment");
            Fix.output.WriteString(Fix.currentRevision.Comment);
            Fix.output.WriteEndElement();
        }
        /// <summary>
        /// Запись подвала записи в XML
        /// </summary>
        /// <param name="output"></param>
        private static void WriteXMLSuffix()
        {
            Fix.output.WriteEndElement(); // "Revision"
            Fix.output.WriteEndDocument();
        }
    }
}
