CREATE TRIGGER tr_db
ON DATABASE
FOR DDL_DATABASE_LEVEL_EVENTS
AS
	DECLARE @data XML;
	SET @data = EVENTDATA();
	INSERT INTO [dds].[up] (script) values (@data.value('(/EVENT_INSTANCE/TSQLCommand/CommandText)[1]','nvarchar(max)'));
	IF @data.value('(/EVENT_INSTANCE/ObjectType)[1]','nvarchar(max)') = 'TABLE'
	BEGIN
		IF @data.value('(/EVENT_INSTANCE/EventType)[1]','nvarchar(max)') = 'DROP_TABLE'
			INSERT INTO [dds].[down] (script) values (OBJECT_DEFINITION(OBJECT_ID('['+@data.value('(/EVENT_INSTANCE/SchemaName)[1]','nvarchar(max)')+'].['+@data.value('(/EVENT_INSTANCE/ObjectName)[1]','nvarchar(max)')+']')));
		ELSE IF @data.value('(/EVENT_INSTANCE/EventType)[1]','nvarchar(max)') = 'CREATE_TABLE'
			INSERT INTO [dds].[down] (script) values ('DROP TABLE '+@data.value('(/EVENT_INSTANCE/SchemaName)[1]','nvarchar(max)')+'.'+@data.value('(/EVENT_INSTANCE/ObjectName)[1]','nvarchar(max)'));
	END;
