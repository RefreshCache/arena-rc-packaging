CREATE TABLE [dbo].[port_module_instance](
	[module_instance_id] [int] IDENTITY(1,1) NOT NULL,
	[date_created] [datetime] NOT NULL,
	[date_modified] [datetime] NOT NULL,
	[created_by] [varchar](50) NOT NULL,
	[modified_by] [varchar](50) NOT NULL,
	[module_id] [int] NOT NULL,
	[module_title] [varchar](100) NOT NULL,
	[show_title] [bit] NOT NULL,
	[template_frame_name] [varchar](50) NOT NULL,
	[template_frame_order] [int] NOT NULL,
	[module_details] [ntext] NULL,
	[system_flag] [bit] NOT NULL,
	[page_id] [int] NULL,
	[template_id] [int] NULL,
	[parent_module_instance_id] [int] NULL,
	[mandatory] [bit] NOT NULL,
	[movable] [bit] NOT NULL,
	[description] [varchar](max) NOT NULL,
	[image_path] [varchar](255) NOT NULL,
	[module_instance_guid] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_orgn_page_module] PRIMARY KEY CLUSTERED 
(
	[module_instance_id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

ALTER TABLE [dbo].[port_module_instance]  WITH NOCHECK ADD  CONSTRAINT [FK_orgn_module_instance_core_module] FOREIGN KEY([module_id])
REFERENCES [dbo].[port_module] ([module_id])
GO

ALTER TABLE [dbo].[port_module_instance] CHECK CONSTRAINT [FK_orgn_module_instance_core_module]
GO

ALTER TABLE [dbo].[port_module_instance]  WITH CHECK ADD  CONSTRAINT [FK_port_module_instance_port_module_instance] FOREIGN KEY([parent_module_instance_id])
REFERENCES [dbo].[port_module_instance] ([module_instance_id])
GO

ALTER TABLE [dbo].[port_module_instance] CHECK CONSTRAINT [FK_port_module_instance_port_module_instance]
GO

ALTER TABLE [dbo].[port_module_instance]  WITH CHECK ADD  CONSTRAINT [FK_port_module_instance_port_portal_page] FOREIGN KEY([page_id])
REFERENCES [dbo].[port_portal_page] ([page_id])
GO

ALTER TABLE [dbo].[port_module_instance] CHECK CONSTRAINT [FK_port_module_instance_port_portal_page]
GO

ALTER TABLE [dbo].[port_module_instance]  WITH CHECK ADD  CONSTRAINT [FK_port_module_instance_port_template] FOREIGN KEY([template_id])
REFERENCES [dbo].[port_template] ([template_id])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[port_module_instance] CHECK CONSTRAINT [FK_port_module_instance_port_template]
GO

ALTER TABLE [dbo].[port_module_instance] ADD  CONSTRAINT [DF__port_modu__date___5B2E79DB]  DEFAULT (getdate()) FOR [date_created]
GO

ALTER TABLE [dbo].[port_module_instance] ADD  CONSTRAINT [DF__port_modu__date___5C229E14]  DEFAULT (getdate()) FOR [date_modified]
GO

ALTER TABLE [dbo].[port_module_instance] ADD  CONSTRAINT [DF_port_module_instance_module_config_setting]  DEFAULT ('') FOR [module_details]
GO

ALTER TABLE [dbo].[port_module_instance] ADD  CONSTRAINT [DF_port_module_instance_system_flag]  DEFAULT ((0)) FOR [system_flag]
GO

ALTER TABLE [dbo].[port_module_instance] ADD  CONSTRAINT [DF_port_module_instance_allow_user_close]  DEFAULT ((0)) FOR [mandatory]
GO

ALTER TABLE [dbo].[port_module_instance] ADD  CONSTRAINT [DF_port_module_instance_movable]  DEFAULT ((0)) FOR [movable]
GO

ALTER TABLE [dbo].[port_module_instance] ADD  CONSTRAINT [DF_port_module_instance_description]  DEFAULT ('') FOR [description]
GO

ALTER TABLE [dbo].[port_module_instance] ADD  CONSTRAINT [DF_port_module_instance_image_path]  DEFAULT ('') FOR [image_path]
GO

ALTER TABLE [dbo].[port_module_instance] ADD  CONSTRAINT [DF_port_module_instance_module_instance_guid]  DEFAULT (newid()) FOR [module_instance_guid]
GO


