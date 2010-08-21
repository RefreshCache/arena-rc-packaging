using System;
using System.Runtime.Serialization;

namespace RefreshCache.Packager.Installer
{
    /// <summary>
    /// Identifies some kind of version mismatch error during an operation.
    /// </summary>
    public class PackageVersionException : Exception
    {
        /// <summary>
        /// Create a new exception with a default error mesage.
        /// </summary>
        public PackageVersionException()
            : base()
        {
        }

        /// <summary>
        /// Create a new exception with the specified error message.
        /// </summary>
        /// <param name="message">The message to include in the exception.</param>
        public PackageVersionException(String message)
            : base(message)
        {
        }

        /// <summary>
        /// Create a new exception with the specified error message and an
        /// inner exception that provides more information about the error.
        /// </summary>
        /// <param name="message">The message to include in the exception.</param>
        /// <param name="innerException">The inner exception to this one.</param>
        public PackageVersionException(String message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the exception with serialized data.
        /// </summary>
        /// <param name="info">The SerializationInfo that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The StreamingContext that contains contextual information about the source or destination.</param>
        public PackageVersionException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }

    /// <summary>
    /// Identifies a dependency error occurred during a package operation.
    /// </summary>
    public class PackageDependencyException : Exception
    {
        /// <summary>
        /// Create a new exception with a default error mesage.
        /// </summary>
        public PackageDependencyException()
            : base()
        {
        }

        /// <summary>
        /// Create a new exception with the specified error message.
        /// </summary>
        /// <param name="message">The message to include in the exception.</param>
        public PackageDependencyException(String message)
            : base(message)
        {
        }

        /// <summary>
        /// Create a new exception with the specified error message and an
        /// inner exception that provides more information about the error.
        /// </summary>
        /// <param name="message">The message to include in the exception.</param>
        /// <param name="innerException">The inner exception to this one.</param>
        public PackageDependencyException(String message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the exception with serialized data.
        /// </summary>
        /// <param name="info">The SerializationInfo that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The StreamingContext that contains contextual information about the source or destination.</param>
        public PackageDependencyException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }

    /// <summary>
    /// Identifies a request to do something with a package that is not
    /// installed on the system.
    /// </summary>
    public class PackageNotInstalledException : Exception
    {
        /// <summary>
        /// Create a new exception with a default error mesage.
        /// </summary>
        public PackageNotInstalledException()
            : base()
        {
        }

        /// <summary>
        /// Create a new exception with the specified error message.
        /// </summary>
        /// <param name="message">The message to include in the exception.</param>
        public PackageNotInstalledException(String message)
            : base(message)
        {
        }

        /// <summary>
        /// Create a new exception with the specified error message and an
        /// inner exception that provides more information about the error.
        /// </summary>
        /// <param name="message">The message to include in the exception.</param>
        /// <param name="innerException">The inner exception to this one.</param>
        public PackageNotInstalledException(String message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the exception with serialized data.
        /// </summary>
        /// <param name="info">The SerializationInfo that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The StreamingContext that contains contextual information about the source or destination.</param>
        public PackageNotInstalledException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }

    /// <summary>
    /// Represents a conflict with something that is going to be
    /// created by installing a package.
    /// </summary>
    public class PackageLocalConflictException : Exception
    {
        /// <summary>
        /// Create a new exception with a default error mesage.
        /// </summary>
        public PackageLocalConflictException()
            : base()
        {
        }

        /// <summary>
        /// Create a new exception with the specified error message.
        /// </summary>
        /// <param name="message">The message to include in the exception.</param>
        public PackageLocalConflictException(String message)
            : base(message)
        {
        }

        /// <summary>
        /// Create a new exception with the specified error message and an
        /// inner exception that provides more information about the error.
        /// </summary>
        /// <param name="message">The message to include in the exception.</param>
        /// <param name="innerException">The inner exception to this one.</param>
        public PackageLocalConflictException(String message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the exception with serialized data.
        /// </summary>
        /// <param name="info">The SerializationInfo that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The StreamingContext that contains contextual information about the source or destination.</param>
        public PackageLocalConflictException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
