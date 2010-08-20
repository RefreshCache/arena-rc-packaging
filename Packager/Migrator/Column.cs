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
        /// <param name="flags">
        /// One or more <see cref="ColumnAttribute"/> bit flags that provide column behavior.
        /// </param>
        /// <param name="length">
        /// The length of the data type for this column.
        /// </param>
        /// <param name="precision">
        /// A <see cref="Int32"/> value which specifies the precision of the data type.
        /// </param>
        /// <param name="scale">
        /// A <see cref="Int32"/> value which specifies the scale of the data type.
        /// </param>
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
		/// A collection of one or more <see cref="ColumnAttribute"/> values that set special attributes for this column.
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

                case ColumnType.XML:
                    return "xml";

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
        /// <summary>
        /// A column that has only two values, true or false.
        /// </summary>
		Bit = 1,

        /// <summary>
        /// Numeric value between 0 and 255.
        /// </summary>
		TinyInt = 2,

        /// <summary>
        /// Numeric value between -32,768 and 32,767.
        /// </summary>
		SmallInt = 3,

        /// <summary>
        /// Numeric value between -2,147,483,648 and 2,147,483,647.
        /// </summary>
		Int = 4,

        /// <summary>
        /// Numeric value between -9,223,372,036,854,775,808 and 9,223,372,036,854,775,807.
        /// </summary>
		BigInt = 5,

        /// <summary>
        /// Fixed precision and scale numbers. When maximum precision is used,
        /// valid values are from - 10^38 +1 through 10^38 - 1.
        /// </summary>
		Numeric = 6,

        /// <summary>
        /// Fixed precision and scale numbers. When maximum precision is used,
        /// valid values are from - 10^38 +1 through 10^38 - 1.
        /// </summary>
		Decimal = 7,

        /// <summary>
        /// Represents monetary or currency values between -214,748.3648 and 214,748.3647.
        /// </summary>
		SmallMoney = 8,

        /// <summary>
        /// Represents monetary or currency values between -922,337,203,685,477.5808 and
        /// 922,337,203,685,477.5807.
        /// </summary>
		Money = 9,

        /// <summary>
        /// Approximate-number data types for use with floating point numeric data. Floating
        /// point data is approximate; therefore, not all values in the data type range can
        /// be represented exactly.
        /// </summary>
		Float = 10,

        /// <summary>
        /// Approximate-number data types for use with floating point numeric data. Floating
        /// point data is approximate; therefore, not all values in the data type range can
        /// be represented exactly.
        /// </summary>
		Real = 11,

        /// <summary>
        /// Defines a time of a day. The time is without time zone awareness and is based on
        /// a 24-hour clock.
        /// </summary>
		Time = 12,

        /// <summary>
        /// Defines a calendar date. The range is 0001-01-01 through 9999-12-31.
        /// </summary>
		Date = 13,

        /// <summary>
        /// Defines a calendar date and time. The date range is 1900-01-01 through 2079-06-06
        /// while the time is only accurate to 1 minute (no seconds).
        /// </summary>
		SmallDateTime = 14,

        /// <summary>
        /// Defines a calendar date and time. The date range is 1753-01-01 through 9999-12-31
        /// while the time is accurate to 0.003 seconds.
        /// </summary>
		DateTime = 15,

        /// <summary>
        /// Defines a calendar date and time. The date range is 0001-01-01 through 9999-12-31
        /// while the time is accurate to 100 nanoseconds.
        /// </summary>
		DateTimeOffset = 16,

        /// <summary>
        /// Fixed-length, non-Unicode character data with a length of n bytes. n must be a
        /// value from 1 through 8,000. The storage size is n bytes.
        /// </summary>
		Char = 17,

        /// <summary>
        /// Variable-length, non-Unicode character data. n can be a value from 1 through 8,000.
        /// </summary>
		VarChar = 18,

        /// <summary>
        /// Variable-length non-Unicode data in the code page of the server and with a maximum
        /// length of 2^31-1 (2,147,483,647) characters.
        /// </summary>
		Text = 19,

        /// <summary>
        /// Fixed-length Unicode character data of n characters. n must be a value from 1
        /// through 4,000.
        /// </summary>
		NChar = 20,

        /// <summary>
        /// Variable-length Unicode character data. ncan be a value from 1 through 4,000.
        /// </summary>
		NVarChar = 21,

        /// <summary>
        /// Variable-length Unicode data with a maximum length of 2^30 - 1 (1,073,741,823)
        /// characters.
        /// </summary>
		NText = 22,

        /// <summary>
        /// Fixed-length binary data with a length of n bytes, where n is a value from 1
        /// through 8,000.
        /// </summary>
		Binary = 23,

        /// <summary>
        /// Variable-length binary data. n can be a value from 1 through 8,000.
        /// </summary>
		VarBinary = 24,

        /// <summary>
        /// Variable-length binary data from 0 through 2^31-1 (2,147,483,647) bytes.
        /// </summary>
		Image = 25,

        /// <summary>
        /// A 16-byte GUID.
        /// </summary>
		UniqueIdentifier = 26,

        /// <summary>
        /// A data type to store XML data.
        /// </summary>
        XML = 27
	}
}

