CREATE TABLE [dbo].[secu_permission](
	[permission_id] [int] IDENTITY(1,1) NOT NULL,
	[date_created] [datetime] NOT NULL,
	[date_modified] [datetime] NOT NULL,
	[created_by] [varchar](50) NOT NULL,
	[modified_by] [varchar](50) NOT NULL,
	[object_type] [int] NOT NULL,
	[operation_type] [int] NOT NULL,
	[subject_type] [int] NOT NULL,
	[object_key] [int] NOT NULL,
	[subject_key] [int] NOT NULL,
	[template_id] [int] NULL,
 CONSTRAINT [PK_secu_permission] PRIMARY KEY CLUSTERED 
(
	[permission_id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[secu_permission]  WITH CHECK ADD  CONSTRAINT [FK_secu_permission_secu_template] FOREIGN KEY([template_id])
REFERENCES [dbo].[secu_template] ([template_id])
ON DELETE SET NULL
GO

ALTER TABLE [dbo].[secu_permission] CHECK CONSTRAINT [FK_secu_permission_secu_template]
GO

ALTER TABLE [dbo].[secu_permission] ADD  CONSTRAINT [DF_secu_permission_date_created]  DEFAULT (getdate()) FOR [date_created]
GO

ALTER TABLE [dbo].[secu_permission] ADD  CONSTRAINT [DF_secu_permission_date_modified]  DEFAULT (getdate()) FOR [date_modified]
GO

ALTER TABLE [dbo].[secu_permission] ADD  CONSTRAINT [DF_secu_permission_created_by]  DEFAULT ('') FOR [created_by]
GO

ALTER TABLE [dbo].[secu_permission] ADD  CONSTRAINT [DF_secu_permission_modified_by]  DEFAULT ('') FOR [modified_by]
GO
