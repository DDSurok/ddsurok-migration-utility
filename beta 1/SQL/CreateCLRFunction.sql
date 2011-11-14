CREATE FUNCTION [dds].RollBackScriptCLR(@data xml)
RETURNS [nvarchar](max)
WITH EXECUTE AS CALLER
AS 
EXTERNAL NAME CLRFunctions.UserDefinedFunctions.RollBackScript