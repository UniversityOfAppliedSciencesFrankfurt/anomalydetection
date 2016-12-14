USE [studentdb]
GO

/****** Object: Table [dbo].[AnomalyDet01_Cluster_Detail] Script Date: 16-04-2016 11:46:19 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[AnomalyDet01_Cluster_Detail] (
    [Id]                       INT             IDENTITY (1, 1) NOT NULL,
    [Data_Id]                  INT             NULL,
    [Cluster_Id]               INT             NULL,
    [NumCluster]               INT             NULL,
    [Centeroid_Scalar_1_Value] DECIMAL (18, 2) NULL,
    [Centeroid_Scalar_2_Value] DECIMAL (18, 2) NULL,
    [Centeroid_Scalar_3_Value] DECIMAL (18, 2) NULL,
    [Max_Distance]             DECIMAL (18, 2) NULL,
    [Cluster_Name]             NVARCHAR (50)   NULL
);


