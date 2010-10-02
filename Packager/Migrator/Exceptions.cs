using System;
using System.Runtime.Serialization;

namespace RefreshCache.Packager.Migrator
{
    /// <summary>
    /// Identifies that an error occurred while running a database migration
    /// script against the database.
    /// </summary>
    [Serializable]
    public class DatabaseMigrationException : Exception, ISerializable
    {
        /// <summary>
        /// Create a new exception with a default error mesage.
        /// </summary>
        public DatabaseMigrationException()
            : base()
        {
        }

        /// <summary>
        /// Create a new exception with the specified error message.
        /// </summary>
        /// <param name="message">The message to include in the exception.</param>
        public DatabaseMigrationException(String message)
            : base(message)
        {
        }

        /// <summary>
        /// Create a new exception with the specified error message and an
        /// inner exception that provides more information about the error.
        /// </summary>
        /// <param name="message">The message to include in the exception.</param>
        /// <param name="innerException">The inner exception to this one.</param>
        public DatabaseMigrationException(String message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the exception with serialized data.
        /// </summary>
        /// <param name="info">The SerializationInfo that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The StreamingContext that contains contextual information about the source or destination.</param>
        public DatabaseMigrationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
