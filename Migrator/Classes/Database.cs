using System;
using System.Data.SqlClient;

namespace RefreshCache.Database
{
	public class Database
	{
		#region Parameters
		
		/// <summary>
		/// The database connection we are working with.
		/// </summary>
		private SqlConnection dbConnection;
		
		/// <summary>
		/// The current SqlCommand (and transaction) that we have running.
		/// </summary>
		private SqlCommand dbCommand;
		
		/// <summary>
		/// If this parameter is set to True then all SQL commands will be output
		/// to the console as well as run against the server.
		/// </summary>
		public Boolean Verbose { get; set; }
		
		/// <summary>
		/// If this parameter is set to true then no SQL commands will actually be
		/// executed. This parameter is only useful in conjunction with the Verbose
		/// parameter.
		/// </summary>
		public Boolean Dryrun { get; set; }
		
		#endregion
		
		
		/// <summary>
		/// Used internally for test purposes only.
		/// </summary>
		internal Database()
		{
		}
		
		
		/// <summary>
		/// Create a new database object with the specifies SQL connection.
		/// </summary>
		/// <param name="connection">
		/// A <see cref="SqlConnection"/> that identifies the SQL data source we are working with.
		/// </param>
		public Database(SqlConnection connection)
		{
			if (connection.State != System.Data.ConnectionState.Open)
				connection.Open();
			dbConnection = connection;
		}
		

		/// <summary>
		/// This method simply performs a few tests to see if this object is
		/// in a sane state to perform a database operation.
		/// </summary>
		private void TestOperationState()
		{
			if (Dryrun)
				return;

			if (dbConnection == null)
				throw new InvalidOperationException("Cannot perform database operation unless connected to a database.");
			
			if (dbCommand == null)
				throw new InvalidOperationException("Cannot perform database operation unless there is a valid command object.");
		}
		
		
		/// <summary>
		/// Begin a transaction. All operations must be performed inside a transaction.
		/// </summary>
		public void BeginTransaction()
		{
			if (Dryrun)
				return;
			
			if (dbConnection == null)
				throw new InvalidOperationException("Cannot begin transaction unless connected to a database.");
			
			if (dbCommand != null)
				throw new InvalidOperationException("Cannot begin transaction while an outstanding transaction exists.");
			
			dbCommand = dbConnection.CreateCommand();
			dbCommand.Transaction = dbConnection.BeginTransaction();
		}
		
		
		/// <summary>
		/// Commit the transaction and write all changes we have made to the database.
		/// </summary>
		public void CommitTransaction()
		{
			if (Dryrun)
				return;
			
			if (dbCommand == null)
				throw new InvalidOperationException("Cannot commit transaction without an outstanding transaction.");
			
			if (dbConnection == null)
				throw new InvalidOperationException("Cannot commit transaction unless connected to a database.");
			
			dbCommand.Transaction.Commit();
			dbCommand = null;
		}
		
		
		/// <summary>
		/// Rollback the current transaction an erase everything we did.
		/// </summary>
		public void RollbackTransaction()
		{
			if (Dryrun)
				return;
			
			if (dbCommand == null)
				throw new InvalidOperationException("Cannot commit transaction without an outstanding transaction.");
			
			if (dbConnection == null)
				throw new InvalidOperationException("Cannot commit transaction unless connected to a database.");
			
			dbCommand.Transaction.Rollback();
			dbCommand = null;
		}
		
		
		/// <summary>
		/// Determine if an object exists in the database.
		/// </summary>
		/// <param name="objectName">
		/// A <see cref="String"/> representing the name of the object to check for existance.
		/// </param>
		/// <returns>
		/// A <see cref="Boolean"/> value which represents if the object exists or not.
		/// </returns>
		public Boolean ObjectExists(String objectName)
		{
			Boolean result = false;
			SqlDataReader reader;
			
			
			TestOperationState();

			dbCommand.CommandText = "SELECT * FROM sys.objects WHERE name = N'" + objectName + "')";
			reader = dbCommand.ExecuteReader();
			result = reader.HasRows;
			reader.Close();

			return result;
		}

		
		/// <summary>
		/// Determine if a particular object exists in the database.
		/// </summary>
		/// <param name="objectName">
		/// A <see cref="String"/> representing the name of the object to check for existance.
		/// </param>
		/// <param name="objectType">
		/// A <see cref="String"/> representing the type of object to look for.
		/// </param>
		/// <returns>
		/// A <see cref="Boolean"/> value which represents if the object exists or not.
		/// </returns>
		public Boolean ObjectExists(String objectName, String objectType)
		{
			Boolean result = false;
			SqlDataReader reader;
			
			
			TestOperationState();

			dbCommand.CommandText = "SELECT * FROM sys.objects WHERE name = N'" + objectName + "' AND type = N'" + objectType + "')";
			reader = dbCommand.ExecuteReader();
			result = reader.HasRows;
			reader.Close();

			return result;
		}

		
		/// <summary>
		/// Execute a SQL statement that does not return any information.
		/// </summary>
		/// <param name="query">
		/// A <see cref="String"/> which contains the SQL query to be executed.
		/// </param>
		public void ExecuteNonQuery(String query)
		{
			TestOperationState();

			
			//
			// If we are running in verbose mode, then output the SQL query to the console
			// as well.
			//
			if (Verbose)
			{
				Console.WriteLine(query);
			}

			//
			// A dry run means we don't actually do anything, we just go through the actions.
			//
			if (!Dryrun)
			{
				dbCommand.CommandText = query;
				dbCommand.ExecuteNonQuery();
			}
		}
		
		
		/// <summary>
		/// Creates a new table in the database.
		/// </summary>
		/// <param name="table">
		/// A <see cref="Table"/> object which identifies the table to be created.
		/// </param>
		public void CreateTable(Table table)
		{
			ExecuteNonQuery(table.ToString());
		}
		

		/// <summary>
		/// Delete the specified table from the database.
		/// </summary>
		/// <param name="name">
		/// A <see cref="String"/> which specifies the table to be deleted.
		/// </param>
		public void DropTable(String name)
		{
			ExecuteNonQuery(String.Format("DROP TABLE [{0}]", name));
		}
		
		
		/// <summary>
		/// Alter an existing table by adding a new column to it.
		/// </summary>
		/// <param name="tableName">
		/// A <see cref="String"/> that identifies which table to work with.
		/// </param>
		/// <param name="col">
		/// A <see cref="Column"/> that specifies the new column information to add.
		/// </param>
		public void AddTableColumn(String tableName, Column col)
		{
			ExecuteNonQuery(String.Format("ALTER TABLE [{0}] ADD {1}", tableName, col.ToString()));
		}
		

		/// <summary>
		/// Alter an existing table by deleting a column from it.
		/// </summary>
		/// <param name="tableName">
		/// A <see cref="String"/> that identifies which table to work with.
		/// </param>
		/// <param name="columnName">
		/// A <see cref="String"/> that identifies the column to be removed.
		/// </param>
		public void DropTableColumn(String tableName, String columnName)
		{
			ExecuteNonQuery(String.Format("ALTER TABLE [{0}] DROP COLUMN {1}", tableName, columnName));
		}
		
		
		/// <summary>
		/// Alter an existing table by adding a new constraint to it.
		/// </summary>
		/// <param name="tableName">
		/// A <see cref="String"/> that identifies which table to work with.
		/// </param>
		/// <param name="con">
		/// A <see cref="Constraint"/> that specifies the new constraint information to add.
		/// </param>
		public void AddTableConstraint(String tableName, Constraint con)
		{
			ExecuteNonQuery(String.Format("ALTER TABLE [{0}] ADD {1}", tableName, con.ToString()));
		}
		
		
		/// <summary>
		/// Alter an existing table by deleting a constraint from it.
		/// </summary>
		/// <param name="tableName">
		/// A <see cref="String"/> that identifies which table to work with.
		/// </param>
		/// <param name="constraintName">
		/// A <see cref="String"/> that identifies the constraint to be removed.
		/// </param>
		public void DropTableConstraint(String tableName, String constraintName)
		{
			ExecuteNonQuery(String.Format("ALTER TABLE [{0}] DROP CONSTRAINT {1}", tableName, constraintName));
		}
		
		
		/// <summary>
		/// Deletes a stored procedure from the database.
		/// </summary>
		/// <param name="procedureName">
		/// A <see cref="String"/> which identifies the stored procedure to be removed from the database.
		/// </param>
		public void DropProcedure(String procedureName)
		{
			ExecuteNonQuery(String.Format("DROP PROCEDURE [{0}]", procedureName));
		}

		
		/// <summary>
		/// Deletes a function from the database.
		/// </summary>
		/// <param name="functionName">
		/// A <see cref="String"/> which identifies the function to be removed from the database.
		/// </param>
		public void DropFunction(String functionName)
		{
			ExecuteNonQuery(String.Format("DROP FUNCTION [{0}]", functionName));
		}
	}
}
