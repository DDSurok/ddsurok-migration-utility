CREATE TRIGGER tr_db
ON DATABASE
FOR DDL_DATABASE_LEVEL_EVENTS
AS
	DECLARE @data XML;
	SET @data = EVENTDATA();
	INSERT INTO [dds].[up] (script) values (@data.value('(/EVENT_INSTANCE/TSQLCommand/CommandText)[1]','nvarchar(max)'));
	INSERT INTO [dds].[down] (script) values (dds.RollBackScriptCLR(@data));
	