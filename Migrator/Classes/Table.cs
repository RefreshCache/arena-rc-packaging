using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;


namespace RefreshCache.Database
{
	/// <summary>
	/// Defines a SQL Table that can then be created by the database engine. All attributes
	/// of the table are self-contained, including column definitions and contraints.
	/// </summary>
	public class Table
	{
		#region Properties
		
		/// <summary>
		/// Specifies the name this table should have in the database.
		/// </summary>
		public String Name { get; set; }

		/// <summary>
		/// The list of columns that are used to construct this table.
		/// </summary>
		public List<Column> Columns { get { return _Columns; } }
		private List<Column> _Columns;
		
		/// <summary>
		/// An optional list of constraints that will be used when creating the table.
		/// </summary>
		public List<Constraint> Constraints { get { return _Constraints; } }
		private List<Constraint> _Constraints;
		
		#endregion
		
		
		#region Constructors and Destructors
		
		/// <summary>
		/// Create a new Table definition with a list of columns specified.
		/// </summary>
		/// <param name="name">
		/// A <see cref="String"/> which identifies the name this table will have in the database.
		/// </param>
		/// <param name="cols">
		/// A <see cref="Column[]"/> collection which specifies the columns to define in this table.
		/// </param>
		public Table(String name, params Column[] cols)
		{
			Name = name;
			_Columns = new List<Column>(cols);
			_Constraints = new List<Constraint>();
		}
		
		#endregion
		
		
		/// <summary>
		/// Retrieves a <see cref="String"/> which can be used to create this table
		/// object in the database.
		/// </summary>
		/// <returns>
		/// A <see cref="String"/> containing the table as a SQL CREATE statement.
		/// </returns>
		public override String ToString ()
		{
			StringBuilder sb = new StringBuilder();
			Boolean first = true;
			

			//
			// Setup initial query.
			//
			sb.Append("CREATE TABLE [" + Name + "] (");
			
			//
			// Add each column definition in.
			//
			foreach (Column column in Columns)
			{
				if (first == true)
				{
					sb.Append(column.ToString());
					first = false;
				}
				else
					sb.Append(", " + column.ToString());
			}
			
			//
			// Add each constraint definition in.
			//
			foreach (Constraint con in Constraints)
			{
				sb.Append(", " + con.ToString());
			}
			
			//
			// Finish off the query.
			//
			sb.Append(")");
			
			return sb.ToString();
		}
	}
}
