create      PROC [dbo].[port_sp_get_moduleByUrl]
@ModuleUrl varchar(255)

AS

SELECT TOP 1 * 
FROM port_module
WHERE module_url = @ModuleUrl

GO
