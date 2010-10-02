CREATE TABLE [dbo].[port_module](
	[module_id] [int] IDENTITY(1,1) NOT NULL,
	[date_created] [datetime] NOT NULL,
	[date_modified] [datetime] NOT NULL,
	[created_by] [varchar](50) NOT NULL,
	[modified_by] [varchar](50) NOT NULL,
	[module_url] [varchar](255) NOT NULL,
	[module_name] [varchar](100) NOT NULL,
	[module_desc] [text] NOT NULL,
	[allows_child_modules] [bit] NOT NULL,
	[image_path] [varchar](255) NOT NULL,
 CONSTRAINT [PK_core_module] PRIMARY KEY CLUSTERED 
(
	[module_id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

ALTER TABLE [dbo].[port_module] ADD  CONSTRAINT [DF__port_modu__date___3D5E1FD2]  DEFAULT (getdate()) FOR [date_created]
GO

ALTER TABLE [dbo].[port_module] ADD  CONSTRAINT [DF__port_modu__date___3E52440B]  DEFAULT (getdate()) FOR [date_modified]
GO

ALTER TABLE [dbo].[port_module] ADD  CONSTRAINT [DF_port_module_allows_child_modules]  DEFAULT ((0)) FOR [allows_child_modules]
GO

ALTER TABLE [dbo].[port_module] ADD  CONSTRAINT [DF_port_module_image_path]  DEFAULT ('') FOR [image_path]
GO


