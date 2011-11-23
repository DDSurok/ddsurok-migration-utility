using System.IO;
using System.Xml;

namespace migration
{
    public static class cFix
    {
        /// <summary>
        /// Хранит информацию о текущей ревизии базы данных.
        /// </summary>
        static private RevisionInfo currentRevision;
        /// <summary>
        /// Класс доступа к файлу новой ревизии.
        /// </summary>
        static private XmlWriter output;
        /// <summary>
        /// Основной метод класса. Выполняет фиксацию изменений в новую ревизию.
        /// </summary>
        /// <param name="Comment">Комментарий новой ревизии</param>
        static internal void _Main(string Comment)
        {
            if (DatabaseAdapter.GetCountChanges() > 0)    // Если изменения есть, то начинаем
            {
                cFix.currentRevision = RevisionInfo.GenerateRevisionInfo(Comment);
                FileStream fs = new FileStream(functions.GetFileName(cFix.currentRevision), FileMode.Append);
                output = XmlWriter.Create(fs, functions.XmlSettings());
                // Записываем в файл заголовок коммита
                WriteXMLHeader();
                // Пишем скрипты повышения
                cFix.WriteScriptsUp();
                // Пишем скрипты понижения
                cFix.WriteScriptsDown();
                // Чистим список изменений и обновляем версию СУБД
                DatabaseAdapter.ClearUpDownScripts();
                DatabaseAdapter.UpdateVersionDatabase(cFix.currentRevision);
                // Пишем подвал коммита
                WriteXMLSuffix();
                output.Close();
                fs.Close();        // Закрываем поток
            }
        }
        /// <summary>
        /// Запись заголовка записи в XML.
        /// </summary>
        static private void WriteXMLHeader()
        {
            cFix.output.WriteStartElement("Revision");
            cFix.output.WriteAttributeString("Database", Configuration.databaseName);
            cFix.output.WriteAttributeString("Create_date", cFix.currentRevision.GenerateDateTime.ToShortDateString());
            cFix.output.WriteAttributeString("Create_time", cFix.currentRevision.GenerateDateTime.ToShortTimeString());
            cFix.output.WriteAttributeString("Id", cFix.currentRevision.HashCode);
            cFix.output.WriteStartElement("Comment");
            cFix.output.WriteString(cFix.currentRevision.Comment);
            cFix.output.WriteEndElement();
        }
        /// <summary>
        /// Записываем в XML скрипты повышения.
        /// </summary>
        static public void WriteScriptsUp()
        {
            cFix.output.WriteStartElement("UpScripts");
            int i = 1;
            foreach (string script in DatabaseAdapter.GetUpScripts())
            {
                cFix.output.WriteElementString(i.ToString("up00000000"), script);
                i++;
            }
            cFix.output.WriteEndElement();
        }
        /// <summary>
        /// Записываем в XML скрипты понижения.
        /// </summary>
        static public void WriteScriptsDown()
        {
            cFix.output.WriteStartElement("DownScripts");
            int i = 1;
            foreach (string script in DatabaseAdapter.GetDownScripts())
            {
                cFix.output.WriteElementString(i.ToString("down00000000"), script);
            }
            cFix.output.WriteEndElement();
        }
        /// <summary>
        /// Запись подвала записи в XML.
        /// </summary>
        static private void WriteXMLSuffix()
        {
            cFix.output.WriteEndElement(); // "Revision"
            cFix.output.WriteEndDocument();
        }
    }
}
