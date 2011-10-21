BEGIN TRAN
use proba;
create user ddsurok without login;
go
execute as user = 'ddsurok'
go
CREATE SCHEMA [ddsurok] AUTHORIZATION [ddsurok]
GO
CREATE TABLE [ddsurok].[migration_up]
(
id int NOT NULL IDENTITY(1, 1) PRIMARY KEY,
script text NOT NULL
)
GO
CREATE TABLE [ddsurok].[migration_down]
(
id int NOT NULL IDENTITY(1, 1) PRIMARY KEY,
script text NOT NULL
)
GO
if @@ERROR!=0
  ROLLBACK TRAN;
else
  COMMIT TRAN;
