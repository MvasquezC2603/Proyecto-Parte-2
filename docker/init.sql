IF DB_ID('BasketDB') IS NULL
BEGIN
  CREATE DATABASE BasketDB;
END
GO
USE BasketDB;
GO

IF OBJECT_ID('dbo.Matches','U') IS NULL
BEGIN
CREATE TABLE dbo.Matches(
  Id         INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
  HomeTeam   NVARCHAR(100)     NOT NULL,
  AwayTeam   NVARCHAR(100)     NOT NULL,
  StartAt    DATETIME2         NOT NULL,
  EndAt      DATETIME2         NULL,
  Quarter    INT               NOT NULL,
  ScoreHome  INT               NOT NULL,
  ScoreAway  INT               NOT NULL,
  FoulsHome  INT               NOT NULL,
  FoulsAway  INT               NOT NULL,
  Status     NVARCHAR(50)      NOT NULL
);
END
GO
