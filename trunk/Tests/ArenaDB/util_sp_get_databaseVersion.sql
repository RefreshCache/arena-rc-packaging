CREATE PROCEDURE [dbo].[util_sp_get_databaseVersion]
AS
SELECT [value] as DatabaseVersion
FROM fn_listextendedproperty('Arena_DatabaseVersion', default, default, default, default, default, default)