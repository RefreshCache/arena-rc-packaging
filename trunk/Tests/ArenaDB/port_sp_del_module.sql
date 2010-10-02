create            PROC [dbo].[port_sp_del_module]
@ModuleID int

AS

DELETE port_module
WHERE module_id = @ModuleID

GO
