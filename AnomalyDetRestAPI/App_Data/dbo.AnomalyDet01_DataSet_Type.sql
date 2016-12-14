USE [studentdb]
GO

/****** Object: Table [dbo].[AnomalyDet01_DataSet_Type] Script Date: 16-04-2016 11:50:11 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[AnomalyDet01_DataSet_Type] (
    [Data_Id]                 INT             NOT NULL,
    [DataSet_Name]            NVARCHAR (50)   NULL,
    [DataSet_Scalar_1]        INT             NULL,
    [DataSet_Scalar_2]        INT             NULL,
    [DataSet_Scalar_3]        INT             NULL,
    [Max_Threshhold_Distance] DECIMAL (18, 2) NULL,
    [Description]             NVARCHAR (50)   NULL,
    [DataSource]              NVARCHAR (50)   NULL,
    [Dimension]               INT             NULL
);


