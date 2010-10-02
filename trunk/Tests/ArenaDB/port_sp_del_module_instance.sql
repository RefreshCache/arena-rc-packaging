create            PROC [dbo].[port_sp_del_module_instance]
@ModuleInstanceID int

AS

DELETE FROM port_module_instance
WHERE parent_module_instance_id = @ModuleInstanceID

DELETE port_module_instance
WHERE module_instance_id = @ModuleInstanceID

EXEC [secu_sp_del_permissionByKey] 2, @ModuleInstanceID

GO
