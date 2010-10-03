CREATE TABLE [dbo].[secu_template](
	[template_id] [int] IDENTITY(1,1) NOT NULL,
	[name] [varchar](200) NOT NULL,
	[description] [varchar](8000) NOT NULL,
	[object_type] [int] NOT NULL,
	[date_created] [datetime] NOT NULL,
	[created_by] [varchar](50) NOT NULL,
	[date_modified] [datetime] NOT NULL,
	[modified_by] [varchar](50) NOT NULL,
 CONSTRAINT [PK_secu_template] PRIMARY KEY CLUSTERED 
(
	[template_id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[secu_template] ADD  CONSTRAINT [DF__secu_templ__name__7154AA7B]  DEFAULT ('') FOR [name]
GO

ALTER TABLE [dbo].[secu_template] ADD  CONSTRAINT [DF__secu_temp__descr__7248CEB4]  DEFAULT ('') FOR [description]
GO

ALTER TABLE [dbo].[secu_template] ADD  CONSTRAINT [DF__secu_temp__objec__733CF2ED]  DEFAULT ((-1)) FOR [object_type]
GO

ALTER TABLE [dbo].[secu_template] ADD  CONSTRAINT [DF__secu_temp__date___74311726]  DEFAULT (getdate()) FOR [date_created]
GO

ALTER TABLE [dbo].[secu_template] ADD  CONSTRAINT [DF__secu_temp__creat__75253B5F]  DEFAULT ('') FOR [created_by]
GO

ALTER TABLE [dbo].[secu_template] ADD  CONSTRAINT [DF__secu_temp__date___76195F98]  DEFAULT (getdate()) FOR [date_modified]
GO

ALTER TABLE [dbo].[secu_template] ADD  CONSTRAINT [DF__secu_temp__modif__770D83D1]  DEFAULT ('') FOR [modified_by]
GO


