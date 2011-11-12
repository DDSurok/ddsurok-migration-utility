CREATE PROCEDURE [dds].RollBackScriptCLR(@data xml)
WITH EXECUTE AS CALLER
AS 
EXTERNAL NAME CLRFunctions.UserDefinedFunctions.RollBackScript