using System;
using System.Collections.ObjectModel;
using System.Text;

namespace RefreshCache.Packager
{
    /// <summary>
    /// An object that contains information about a single message that
    /// was the result of building a Package.
    /// </summary>
    public class BuildMessage
    {
        internal BuildMessageType Type { get { return _Type; } }
        private BuildMessageType _Type;
        private String Message;


        /// <summary>
        /// Create a new BuildMessage object with the given mesage type and
        /// textual description of the message.
        /// </summary>
        /// <param name="type">The type of BuildMessage object to create.</param>
        /// <param name="message">The user readable message.</param>
        internal BuildMessage(BuildMessageType type, String message)
        {
            _Type = type;
            Message = message;
        }

        /// <summary>
        /// Convert the BuildMessage object into a String that can be
        /// displayed to the user.
        /// </summary>
        /// <returns>Textual representation of the message in a manner the user can read.</returns>
        public override String ToString()
        {
            return Type.ToString() + ": " + Message;
        }
    }


    /// <summary>
    /// A collection of build messages that should be displayed to the user.
    /// If any of the build messages is marked as an error then the build
    /// has failed.
    /// </summary>
    public class BuildMessageCollection : Collection<BuildMessage>
    {
        internal BuildMessageCollection()
            : base()
        {
        }

        /// <summary>
        /// Convert the collection of build messages into a single string that
        /// can be displayed to the user. Each build message is separated by
        /// a newline character.
        /// </summary>
        /// <returns>A <see cref="String"/> object that should be displayed to the user.</returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();


            foreach (BuildMessage message in this)
            {
                if (sb.Length == 0)
                    sb.Append(message.ToString());
                else
                    sb.Append(Environment.NewLine + message.ToString());
            }

            return sb.ToString();
        }
    }


    /// <summary>
    /// The type of build message, warning, error etc.
    /// </summary>
    public enum BuildMessageType
    {
        /// <summary>
        /// Warning messages should denote messages that are probably not
        /// critical but should be fixed anyway.
        /// </summary>
        Warning = 0,

        /// <summary>
        /// Error messages are those that prevent a build from being successful
        /// such as trying to reference a file that doesn't exist.
        /// </summary>
        Error = 1
    }
}
