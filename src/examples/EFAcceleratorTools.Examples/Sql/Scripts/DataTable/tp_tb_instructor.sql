create type [dbo].[TP_TB_INSTRUCTOR] as table(
	[SQ_INSTRUCTOR] [bigint] NOT NULL,
	[TX_FULL_NAME] [nvarchar](100) NOT NULL,
	[DT_CREATION] [datetime] NOT NULL
);