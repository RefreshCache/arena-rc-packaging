CREATE TABLE [dbo].[port_module_instance_setting](
	[module_instance_setting_id] [int] IDENTITY(1,1) NOT NULL,
	[module_instance_id] [int] NOT NULL,
	[name] [varchar](100) NOT NULL,
	[value] [varchar](2000) NOT NULL,
	[type_id] [smallint] NOT NULL,
	[person_id] [int] NULL,
 CONSTRAINT [PK_port_module_instance_setting] PRIMARY KEY CLUSTERED 
(
	[module_instance_setting_id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[port_module_instance_setting]  WITH CHECK ADD  CONSTRAINT [FK_port_module_instance_setting_port_module_instance] FOREIGN KEY([module_instance_id])
REFERENCES [dbo].[port_module_instance] ([module_instance_id])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[port_module_instance_setting] CHECK CONSTRAINT [FK_port_module_instance_setting_port_module_instance]
GO

ALTER TABLE [dbo].[port_module_instance_setting] ADD  CONSTRAINT [DF_port_module_instance_setting_value]  DEFAULT ('') FOR [value]
GO


