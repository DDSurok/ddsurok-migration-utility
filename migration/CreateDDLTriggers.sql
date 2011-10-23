USE proba
GO
CREATE TRIGGER tr_db
ON DATABASE
FOR DDL_DATABASE_LEVEL_EVENTS
AS
	--INSERT INTO [sc_ddsurok].[up] (
	--INSERT INTO [sc_ddsurok].[up] EVENTDATA().value('(/EVENT_INSTANCE/TSQLCommand/CommandText)[1]','nvarchar(max)')
	PRINT 'COMMIT'
GO