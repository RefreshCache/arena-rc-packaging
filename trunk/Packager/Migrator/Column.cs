using System;
using System.Data.SqlClient;
using System.Text;


namespace RefreshCache.Packager.Migrator
{
	/// <summary>
	/// A TableColumn identifies a single column that will be created in a table. It
	/// is a self-contained unit that will provide the SQL query needed to create this
	/// column inside of a CREATE TABLE statement.
	/// </summary>
	public class Column
	{
		#region Properties
		
		/// <summary>
		/// The name of the column in the database.
		/// </summary>
		public String Name { get; set; }
		
		/// <summary>
		/// The database type this column is defined as.
		/// </summary>
		public ColumnType Type { get; set; }
		
		/// <summary>
		/// The flags that will be used when defining this column, such as primary key or not nullable.
		/// </summary>
		public ColumnAttribute Flags { get; set; }
		
		/// <summary>
		/// The default value for this column. If not null this value will be placed directly
		/// after the DEFAULT keyword without modification, therefor if you want to use a string
		/// value as a default value you must include the single quotes.
		/// <example>column.Default = "'hello world'";</example>
		/// </summary>
		public String Default { get; set; }
		
		/// <summary>
		/// The length of this columns data type. Type checking is not done so only set the
		/// length value if the data type supports it, i.e. varchar and such.
		/// </summary>
		public Int32 Length { get; set; }
		
		/// <summary>
		/// The precision of the numeric type. Again no type checking is done so only use it
		/// when it is appropriate.
		/// </summary>
		public Int32 Precision { get; set; }
		
		/// <summary>
		/// The scale of the numeric type. Again no type checking is done so only use it when
		/// it is appropriate.
		/// </summary>
		public Int32 Scale { get; set; }
		
		#endregion
		
		
		#region Constructors and Destructors
		
		public Column(String name, ColumnType type, ColumnAttribute flags, Int32 length, Int32 precision, Int32 scale)
		{
            if (String.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");
            if (!Enum.IsDefined(typeof(ColumnType), type))
                throw new ArgumentOutOfRangeException("type");

			Name = name;
			Type = type;
			Flags = flags;
			Length = length;
			Precision = precision;
			Scale = scale;
		}
		
		
		/// <summary>
		/// Construct a new database column with the specific name and type only. Appropriate
		/// for use when you are creating a generic column without any flags or length.
		/// </summary>
		/// <param name="name">
		/// A <see cref="String"/> that identifies the name of this column in the database.
		/// </param>
		/// <param name="type">
		/// A <see cref="ColumnType"/> that specificies the data type for this column.
		/// </param>
		public Column(String name, ColumnType type)
			: this(name, type, ColumnAttribute.None, -1, -1, -1)
		{
		}
		
		
		/// <summary>
		/// Construct a new database column by specifying the name, type and special flags for
		/// the column. This constructor is approriate for use when construction, for example,
		/// primary keys or simple data types that will not be nullable.
		/// </summary>
		/// <param name="name">
		/// A <see cref="String"/> that identifies the name of this column in the database.
		/// </param>
		/// <param name="type">
		/// A <see cref="ColumnType"/> that identifies the data type for this column.
		/// </param>
		/// <param name="flags">
		/// A collection of one or more <see cref="ColumnFlags"/> values that set special attributes for this column.
		/// </param>
		public Column(String name, ColumnType type, ColumnAttribute flags)
			: this(name, type, flags, -1, -1, -1)
		{
		}
		
		
		/// <summary>
		/// Construct a new database column by specifying the name, type and length of the data
		/// type for the new column. Appropriate for use when constructing generic varchar columns
		/// and other simple data types.
		/// </summary>
		/// <param name="name">
		/// A <see cref="String"/> which identifies the name of this column in the database.
		/// </param>
		/// <param name="type">
		/// A <see cref="ColumnType"/> that identifies the data type this column will have.
		/// </param>
		/// <param name="length">
		/// A <see cref="Int32"/> value which specifies the data length of this data type.
		/// </param>
		public Column(String name, ColumnType type, Int32 length)
			: this(name, type, ColumnAttribute.None, length, -1, -1)
		{
		}

        /// <summary>
        /// Construct a new database column by specifying the name, type precision and
        /// scale of the data type for the new column. Appropriate for use when constructing
        /// generic decimal style columns.
        /// </summary>
        /// <param name="name">
        /// A <see cref="String"/> which identifies the name of this column in the database.
        /// </param>
        /// <param name="type">
        /// A <see cref="ColumnType"/> that identifies the data type this column will have.
        /// </param>
        /// <param name="precision">
        /// A <see cref="Int32"/> value which specifies the precision of the data type.
        /// </param>
        /// <param name="scale">
        /// A <see cref="Int32"/> value which specifies the scale of the data type.
        /// </param>
        public Column(String name, ColumnType type, Int32 precision, Int32 scale)
            : this(name, type, ColumnAttribute.None, -1, precision, scale)
        {
        }
		
		#endregion
		
		
		/// <summary>
		/// Convert the <see cref="ColumnType"/> into a <see cref="String"/> representation
		/// that can be used in SQL statements.
		/// </summary>
		/// <param name="t">
		/// A <see cref="ColumnType"/> to be converted to a SQL string.
		/// </param>
		/// <returns>
		/// A <see cref="String"/> in a format understood by the database.
		/// </returns>
		public static String DbName(ColumnType t)
		{
			switch (t)
			{
			case ColumnType.Bit:
				return "bit";
				
			case ColumnType.TinyInt:
				return "tinyint";
				
			case ColumnType.SmallInt:
				return "smallint";
				
			case ColumnType.Int:
				return "int";
				
			case ColumnType.BigInt:
				return "bigint";
				
			case ColumnType.Numeric:
				return "numeric";
				
			case ColumnType.Decimal:
				return "decimal";
				
			case ColumnType.SmallMoney:
				return "smallmoney";
				
			case ColumnType.Money:
				return "money";
				
			case ColumnType.Float:
				return "float";
				
			case ColumnType.Real:
				return "real";
				
			case ColumnType.Time:
				return "time";
				
			case ColumnType.Date:
				return "date";
				
			case ColumnType.SmallDateTime:
				return "smalldatetime";
				
			case ColumnType.DateTime:
				return "datetime";
				
			case ColumnType.DateTimeOffset:
				return "datetimeoffset";
				
			case ColumnType.Char:
				return "char";
				
			case ColumnType.VarChar:
				return "varchar";
				
			case ColumnType.Text:
				return "text";
				
			case ColumnType.NChar:
				return "nchar";
				
			case ColumnType.NVarChar:
				return "nvarchar";
				
			case ColumnType.NText:
				return "ntext";
				
			case ColumnType.Binary:
				return "binary";
				
			case ColumnType.VarBinary:
				return "varbinary";
				
			case ColumnType.Image:
				return "image";
				
			case ColumnType.UniqueIdentifier:
				return "uniqueidentifier";
				
			default:
                throw new InvalidOperationException("Invalid column type specified");
			}
		}
		
		
		/// <summary>
		/// Creates and returns a <see cref="String"/> that can be understood by the database
		/// engine and used to create the column.
		/// </summary>
		/// <returns>
		/// A <see cref="String"/> which identifies this column to the database.
		/// </returns>
		public override String ToString ()
		{
			StringBuilder sb = new StringBuilder();
			
			
			sb.Append("[" + Name + "] " + DbName(Type) + " ");
			if (Length != -1)
			{
				sb.Append("(" + Length.ToString() + ") ");
			}
			else if (Precision != -1 && Scale == -1)
			{
				sb.Append("(" + Precision.ToString() + ") ");
			}
			else if (Precision != -1 && Scale != -1)
			{
				sb.Append("(" + Precision.ToString() + ", " + Scale.ToString() + ") ");
			}
			
			if ((Flags & ColumnAttribute.Identity) == ColumnAttribute.Identity)
			{
				sb.Append("IDENTITY ");
			}
			if ((Flags & ColumnAttribute.PrimaryKey) == ColumnAttribute.PrimaryKey)
			{
				sb.Append("PRIMARY KEY ");
			}
			if ((Flags & ColumnAttribute.Unique) == ColumnAttribute.Unique)
			{
				sb.Append("UNIQUE ");
			}
			if ((Flags & ColumnAttribute.NotNull) == ColumnAttribute.NotNull)
			{
				sb.Append("NOT NULL ");
			}
			if (Default != null)
			{
				sb.Append("DEFAULT " + Default);
			}
			
			return sb.ToString().Trim();
		}
	}
	
	/// <summary>
	/// Special attributes that will identify how the column is to be used in the
	/// database.
	/// </summary>
	public enum ColumnAttribute
	{
		/// <summary>
		/// No attributes have been defined, this is the default.
		/// </summary>
		None = 0x00,
		
		/// <summary>
		/// This column will be created as a primary key.
		/// </summary>
		PrimaryKey = 0x01,

		/// <summary>
		/// This column will not allow null values.
		/// </summary>
		NotNull = 0x08,
		
		/// <summary>
		/// This will be an identity column, which means it will auto-increment.
		/// </summary>
		Identity = 0x10,
		
		/// <summary>
		/// This column will only allow unique values.
		/// </summary>
		Unique = 0x20,
		
		/// <summary>
		/// Creates both a Primary Key and an Identity column together.
		/// </summary>
		PrimaryKeyIdentity = (PrimaryKey | Identity)
	}
	
	/// <summary>
	/// The type of column to define. This list basically mirrors the column data
	/// type names.
	/// </summary>
	public enum ColumnType
	{
		Bit = 1,
		TinyInt = 2,
		SmallInt = 3,
		Int = 4,
		BigInt = 5,
		Numeric = 6,
		Decimal = 7,
		SmallMoney = 8,
		Money = 9,
		Float = 10,
		Real = 11,
		Time = 12,
		Date = 13,
		SmallDateTime = 14,
		DateTime = 15,
		DateTimeOffset = 16,
		Char = 17,
		VarChar = 18,
		Text = 19,
		NChar = 20,
		NVarChar = 21,
		NText = 22,
		Binary = 23,
		VarBinary = 24,
		Image = 25,
		UniqueIdentifier = 26
	}
}

