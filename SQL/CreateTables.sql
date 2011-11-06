CREATE TABLE [dds].[up]
(
id INT NOT NULL IDENTITY(1, 1) PRIMARY KEY,
script TEXT NOT NULL
);
CREATE TABLE [dds].[down]
(
id INT NOT NULL IDENTITY(1, 1) PRIMARY KEY,
script TEXT NOT NULL
);
CREATE TABLE [dds].[version]
(
hashCode nvarchar(40) NOT NULL,
generateDateTime nvarchar(16) NOT NULL,
nickName nvarchar(100) NOT NULL,
comment nvarchar(MAX) NOT NULL
);