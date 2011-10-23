CREATE SCHEMA [sc_ddsurok]
GO
CREATE TABLE [sc_ddsurok].[up]
(
id INT NOT NULL IDENTITY(1, 1) PRIMARY KEY,
script TEXT NOT NULL
)
CREATE TABLE [sc_ddsurok].[down]
(
id INT NOT NULL IDENTITY(1, 1) PRIMARY KEY,
script TEXT NOT NULL
)
GO
