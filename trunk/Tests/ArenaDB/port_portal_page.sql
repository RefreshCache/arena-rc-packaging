CREATE TABLE [dbo].[port_portal_page](
	[page_id] [int] IDENTITY(1,1) NOT NULL,
	[date_created] [datetime] NOT NULL,
	[date_modified] [datetime] NOT NULL,
	[created_by] [varchar](50) NOT NULL,
	[modified_by] [varchar](50) NOT NULL,
	[template_id] [int] NOT NULL,
	[parent_page_id] [int] NULL,
	[page_order] [int] NOT NULL,
	[display_in_nav] [bit] NOT NULL,
	[page_name] [varchar](100) NOT NULL,
	[page_desc] [varchar](1000) NOT NULL,
	[page_settings] [varchar](2000) NULL,
	[require_ssl] [bit] NOT NULL,
	[guid] [uniqueidentifier] NOT NULL,
	[system_flag] [bit] NOT NULL,
	[friendly_url] [varchar](255) NULL,
	[validate_request] [bit] NOT NULL,
 CONSTRAINT [PK_orgn_portal_page] PRIMARY KEY CLUSTERED 
(
	[page_id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[port_portal_page]  WITH NOCHECK ADD  CONSTRAINT [FK_orgn_portal_page_orgn_page_template] FOREIGN KEY([template_id])
REFERENCES [dbo].[port_template] ([template_id])
GO

ALTER TABLE [dbo].[port_portal_page] CHECK CONSTRAINT [FK_orgn_portal_page_orgn_page_template]
GO

ALTER TABLE [dbo].[port_portal_page]  WITH NOCHECK ADD  CONSTRAINT [FK_orgn_portal_page_orgn_portal_page] FOREIGN KEY([parent_page_id])
REFERENCES [dbo].[port_portal_page] ([page_id])
GO

ALTER TABLE [dbo].[port_portal_page] CHECK CONSTRAINT [FK_orgn_portal_page_orgn_portal_page]
GO

ALTER TABLE [dbo].[port_portal_page] ADD  CONSTRAINT [DF__port_port__date___390E6C01]  DEFAULT (getdate()) FOR [date_created]
GO

ALTER TABLE [dbo].[port_portal_page] ADD  CONSTRAINT [DF__port_port__date___3A02903A]  DEFAULT (getdate()) FOR [date_modified]
GO

ALTER TABLE [dbo].[port_portal_page] ADD  CONSTRAINT [DF_port_portal_page_require_ssl]  DEFAULT ((0)) FOR [require_ssl]
GO

ALTER TABLE [dbo].[port_portal_page] ADD  CONSTRAINT [DF_port_portal_page_guid]  DEFAULT (newid()) FOR [guid]
GO

ALTER TABLE [dbo].[port_portal_page] ADD  CONSTRAINT [DF_port_portal_page_system_flag]  DEFAULT ((0)) FOR [system_flag]
GO

ALTER TABLE [dbo].[port_portal_page] ADD  CONSTRAINT [DF_port_portal_page_validate_request]  DEFAULT ((1)) FOR [validate_request]
GO