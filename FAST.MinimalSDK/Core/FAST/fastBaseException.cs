using FAST.Core.Models;
using System.Runtime.Serialization;

namespace FAST.Core
{
    /// <summary>
    /// fastBaseException is the base exception class for the FAST libraries.
    /// All library exceptions are derived from this.
    /// </summary>
    /// <remarks>NOTE: Not all exceptions thrown will be derived from this class.
    /// A variety of other exceptions are possible for example <see cref="ArgumentNullException"></see></remarks>

    [Serializable]
	public class fastBaseException : ApplicationException
	{
		/// <summary>
		/// Deserialization constructor 
		/// </summary>
		/// <param name="info"><see cref="System.Runtime.Serialization.SerializationInfo"/> for this constructor</param>
		/// <param name="context"><see cref="StreamingContext"/> for this constructor</param>
		protected fastBaseException(SerializationInfo info, StreamingContext context ): base( info, context )
		{
		}
		/// <summary>
        /// Initializes a new instance of the fastBaseException class.
		/// </summary>
		public fastBaseException()
		{
		}
		
		/// <summary>
        /// Initializes a new instance of the fastBaseException class with a specified error message.
		/// </summary>
		/// <param name="message">A message describing the exception.</param>
		public fastBaseException(string message)
			: base(message)
		{
		}

		/// <summary>
        /// Initializes a new instance of the fastBaseException class with a specified
		/// error message and a reference to the inner exception that is the cause of this exception.
		/// </summary>
		/// <param name="message">A message describing the exception.</param>
		/// <param name="innerException">The inner exception</param>
		public fastBaseException(string message, Exception innerException): base(message, innerException)
		{
		}

        /// <summary>
        /// Initializes a new instance of the fastBaseException class with a specified
        /// </summary>
        /// <param name="message"></param>
        /// <param name="errors"></param>
        public fastBaseException(string message, IerrorCarrier errors) : base($"{message} ${errors.errorText}")
		{
		}

        /// <summary>
        /// Initializes a new instance of the fastBaseException class with a specified
        /// </summary>
        /// <param name="errors"></param>
        public fastBaseException(IerrorCarrier errors):base($"{errors.errorText}")
		{
		}

    }
}
