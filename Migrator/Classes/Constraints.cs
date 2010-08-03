using System;
using System.Collections.Generic;
using System.Text;


namespace RefreshCache.Database
{
	/// <summary>
	/// This class defines basic information about a constraint in a database. It cannot be
	/// instantiated directly, you must use a concrete subclass to create an actual constraint.
	/// </summary>
	public abstract class Constraint
	{
		#region Properties

		/// <summary>
		/// The name of the constraint, must be unique to the entire database.
		/// </summary>
		public String Name { get; set; }
		
		/// <summary>
		/// The list of columns this constraint applies to. Most constraints will have
		/// only one column, but can have multiple columns (such as a multi-column primary
		/// key).
		/// </summary>
		public List<String> Columns { get { return _Columns; } }
		private List<String> _Columns;
		
		#endregion

		#region Constructors and Destructors
		
		/// <summary>
		/// Create a new constraint object. This method should be called via base during
		/// the constructor of a subclass. It initializes the Name and Columns properties.
		/// </summary>
		/// <param name="name">
		/// A <see cref="String"/> that identifies the unique constraint name in the database.
		/// </param>
		/// <param name="cols">
		/// A collection of <see cref="String"/> objects that specify which columns this constraint applies to.
		/// </param>
		internal Constraint(String name, params String[] cols)
		{
			Name = name;
			_Columns = new List<string>(cols);
		}
		
		/// <summary>
		/// Create a new constraint object. This method should be called via base during
		/// the constructor of a subclass. It initializes the Name and Columns properties.
		/// </summary>
		/// <param name="name">
		/// A <see cref="String"/> that identifies the unique constraint name in the database.
		/// </param>
		/// <param name="cols">
		/// A collection of <see cref="String"/> objects that specify which columns this constraint applies to.
		/// </param>
		internal Constraint(String name, List<String> cols)
		{
			Name = name;
			_Columns = new List<string>(cols);
		}
		
		#endregion
		
		/// <summary>
		/// Convert the list of column names into one that can be used in a
		/// SQL query.
		/// </summary>
		/// <param name="cols">
		/// A <see cref="List<String>"/> collection that contains the column names.
		/// </param>
		/// <returns>
		/// A <see cref="String"/> in the format of "[col1], [col2], ...".
		/// </returns>
		internal static String ColumnNames(List<String> cols)
		{
			StringBuilder sb = new StringBuilder();
			Boolean first = true;
			
			
			foreach (String col in cols)
			{
				if (first == true)
				{
					sb.AppendFormat("[{0}]", col);
					first = false;
				}
				else
					sb.AppendFormat(", [{0}]", col);
			}
			
			return sb.ToString();
		}
		
		/// <summary>
		/// Subclasses must override this method.
		/// </summary>
		/// <returns>
		/// Never returns.
		/// </returns>
		public override String ToString ()
		{
			throw new NotSupportedException("Constraint subclasses must override the ToString method.");
		}
	}


	/// <summary>
	/// Identifies a single primary key constraint within a table definition.
	/// </summary>
	public class PrimaryKeyConstraint : Constraint
	{
		/// <summary>
		/// Construct a new primary key constraint with the specified name for the identified columns.
		/// </summary>
		/// <param name="name">
		/// A <see cref="String"/> that uniquely identifies this constraint in the database.
		/// </param>
		/// <param name="cols">
		/// One or more <see cref="String"/>s that identify which columns this constraint applies to.
		/// </param>
		public PrimaryKeyConstraint(String name, params String[] cols)
			: base(name, cols)
		{
		}

		
		/// <summary>
		/// Provides the SQL statements needed to construct this constraint in the database.
		/// </summary>
		/// <returns>
		/// A <see cref="String"/> in SQL format.
		/// </returns>
		public override String ToString ()
		{
			return String.Format("CONSTRAINT [{0}] PRIMARY KEY ({1})", Name, Constraint.ColumnNames(Columns));
		}
	}

	
	/// <summary>
	/// Identifies a single unique column constraint within a table definition.
	/// </summary>
	public class UniqueConstraint : Constraint
	{
		/// <summary>
		/// Construct a new unique constraint with the specified name for the identified columns.
		/// </summary>
		/// <param name="name">
		/// A <see cref="String"/> that uniquely identifies this constraint in the database.
		/// </param>
		/// <param name="cols">
		/// One or more <see cref="String"/>s that identify which columns this constraint applies to.
		/// </param>
		public UniqueConstraint(String name, params String[] cols)
			: base(name, cols)
		{
		}
		
		
		/// <summary>
		/// Provides the SQL statements needed to construct this constraint in the database.
		/// </summary>
		/// <returns>
		/// A <see cref="String"/> in SQL format.
		/// </returns>
		public override String ToString ()
		{
			return String.Format("CONSTRAINT [{0}] UNIQUE ({1})", Name, Constraint.ColumnNames(Columns));
		}
	}
	
	
	/// <summary>
	/// Identifies a single foreign key constraint for a table definition.
	/// </summary>
	public class ForeignKeyConstraint : Constraint
	{
		#region Properties
		
		/// <summary>
		/// Specifies if delete operations should be cascaded in the database.
		/// </summary>
		public Boolean CascadeOnDelete { get; set; }
		
		/// <summary>
		/// Specifies if update operations should be cascaded in the database.
		/// </summary>
		public Boolean CascadeOnUpdate { get; set; }
		
		/// <summary>
		/// The name of the table that this foreign key references.
		/// </summary>
		public String ReferencedTable { get; set; }
		
		/// <summary>
		/// Specifies the referenced columns in this foreign key. The number of items here must
		/// match the number of items in the Columns property.
		/// </summary>
		public List<String> ReferencedColumns { get { return _ReferencedColumns; } }
		private List<String> _ReferencedColumns;
		
		#endregion
		
		
		/// <summary>
		/// Creates a new foreign key constraint by referencing multiple columns.
		/// </summary>
		/// <param name="name">
		/// A <see cref="String"/> that uniquely identifies this constraint in the database.
		/// </param>
		/// <param name="cols">
		/// A <see cref="List<String>"/> collection of the source columns to be used in the constraint.
		/// </param>
		/// <param name="ref_table">
		/// A <see cref="String"/> that identifies which table will be referenced by this foreign key.
		/// </param>
		/// <param name="ref_cols">
		/// A <see cref="List<String>"/> collection that identifies which columns in the referenced table.
		/// </param>
		public ForeignKeyConstraint(String name, List<String> cols, String ref_table, List<String> ref_cols)
			: base(name, cols)
		{
			ReferencedTable = ref_table;
			_ReferencedColumns = new List<String>(ref_cols);
			CascadeOnDelete = false;
			CascadeOnUpdate = false;
		}
		
		
		/// <summary>
		/// Creates a new foreign key constraint by referencing multiple columns.
		/// </summary>
		/// <param name="name">
		/// A <see cref="String"/> that uniquely identifies this constraint in the database.
		/// </param>
		/// <param name="col">
		/// A <see cref="String"/> that specifies the source column to be used in the constraint.
		/// </param>
		/// <param name="ref_table">
		/// A <see cref="String"/> that identifies which table will be referenced by this foreign key.
		/// </param>
		/// <param name="ref_col">
		/// A <see cref="String"/> that identifies which column in the referenced table.
		/// </param>
		public ForeignKeyConstraint(String name, String col, String ref_table, String ref_col)
			: base(name, new string[] { col })
		{
			ReferencedTable = ref_table;
			_ReferencedColumns = new List<String>(new String[] { ref_col });
			CascadeOnDelete = false;
			CascadeOnUpdate = false;
		}

		
		/// <summary>
		/// Provides the SQL statements needed to construct this constraint in the database.
		/// </summary>
		/// <returns>
		/// A <see cref="String"/> in SQL format.
		/// </returns>
		public override String ToString ()
		{
			return String.Format("CONSTRAINT [{0}] FOREIGN KEY ({1}) REFERENCES [{2}] ({3}) " +
			                     "ON DELETE {4} ON UPDATE {5}",
			                     Name,
			                     Constraint.ColumnNames(Columns),
			                     ReferencedTable,
			                     Constraint.ColumnNames(ReferencedColumns),
			                     (CascadeOnDelete ? "CASCADE" : "NO ACTION"),
			                     (CascadeOnUpdate ? "CASCADE" : "NO ACTION"));
		}
	}
}
