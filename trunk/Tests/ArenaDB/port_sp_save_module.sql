CREATE               Proc [dbo].[port_sp_save_module]
@ModuleId int,
@UserId varchar(50),
@ModuleName varchar(100),
@ModuleUrl varchar(255),
@ModuleDesc text,
@AllowsChildModules bit,
@ImagePath varchar(255),
@ID int OUTPUT

AS

DECLARE @UpdateDateTime DateTime SET @UpdateDateTime = GETDATE()

IF @ModuleId = -1
BEGIN
	INSERT INTO port_module
	(	[date_created], 
		[date_modified], 
		[created_by], 
		[modified_by], 
		[module_name], 
		[module_url],
		[module_desc],
		[allows_child_modules],
		[image_path]) 
	values
	(	@UpdateDateTime,
		@UpdateDateTime,
		@UserID,
		@UserID,
		@ModuleName,
		@ModuleUrl,
		@ModuleDesc,
		@AllowsChildModules,
		@ImagePath)
	SET @ID = SCOPE_IDENTITY()

END
ELSE
BEGIN
	UPDATE port_module Set
		[date_modified] = @UpdateDateTime, 
		[modified_by] = @UserID, 
		[module_name] = @ModuleName, 
		[module_url] = @ModuleUrl, 
		[module_desc] = @ModuleDesc,
		[allows_child_modules] = @AllowsChildModules,
		[image_path] = @ImagePath
	WHERE
		module_id = @ModuleId

	SET @ID = @ModuleId

END

GO
