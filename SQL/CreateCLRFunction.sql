CREATE FUNCTION [dds].RollBackScriptCLR(@TypeEvent [nvarchar](max), @UpScript [nvarchar](max))
RETURNS [nvarchar](max)
WITH EXECUTE AS CALLER
AS 
EXTERNAL NAME CLRFunctions.UserDefinedFunctions.RollBackScript