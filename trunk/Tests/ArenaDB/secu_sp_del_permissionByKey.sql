create proc [dbo].[secu_sp_del_permissionByKey]
@ObjectType int,
@ObjectKey int
AS

	DELETE secu_permission
	WHERE [object_type] = @ObjectType
	AND [object_key] = @ObjectKey
 

GO

