create           PROC [dbo].[port_sp_del_portal_page]
@PageID int

AS

DELETE FROM port_module_instance
WHERE parent_module_instance_id IN
	(
		SELECT module_instance_id
		FROM port_module_instance
		WHERE page_id = @PageID
	)

DELETE port_module_instance
WHERE page_id = @PageID

DELETE port_portal_page
WHERE page_id = @PageID

EXEC [secu_sp_del_permissionByKey] 1, @PageID

GO

