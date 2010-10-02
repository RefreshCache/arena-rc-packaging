CREATE TABLE [dbo].[port_template](
	[template_id] [int] IDENTITY(1,1) NOT NULL,
	[date_created] [datetime] NOT NULL,
	[date_modified] [datetime] NOT NULL,
	[created_by] [varchar](50) NOT NULL,
	[modified_by] [varchar](50) NOT NULL,
	[template_name] [varchar](100) NOT NULL,
	[template_desc] [varchar](255) NOT NULL,
	[template_url] [varchar](100) NOT NULL,
 CONSTRAINT [PK_orgn_page_template] PRIMARY KEY CLUSTERED 
(
	[template_id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[port_template] ADD  CONSTRAINT [DF__port_temp__date___49C3F6B7]  DEFAULT (getdate()) FOR [date_created]
GO

ALTER TABLE [dbo].[port_template] ADD  CONSTRAINT [DF__port_temp__date___4AB81AF0]  DEFAULT (getdate()) FOR [date_modified]
GO

INSERT INTO [dbo].[port_template]
	(created_by
	,modified_by
	,template_name
	,template_desc
	,template_url)
	VALUES
	('ArenaInstall'
	,'ArenaInstall'
	,'ArenaChMS'
	,'The template used for internal ArenaChMS'
	,'~/templates/ArenaChMS.ascx')
GO
