using FAST.Core.Models;

namespace FAST.Services.Models
{
    /// <summary>
    /// Interface to define the elementary model property of the response.
    /// </summary>
    public interface IelementaryModel : IdataFoundQuestion, ImultiErrorCarrier, IerrorCarrier
    {
        /// <summary>
        /// Indicates if data was not found.
        /// </summary>
        new bool dataNotFound { get; set; }

        /// <summary>
        /// List of error messages.
        /// </summary>
        new List<string> errors { get; set; }

        /// <summary>
        /// Primary error message.
        /// </summary>
        new string errorText { get; set; }

        /// <summary>
        /// Detailed error message.
        /// </summary>
        new string extendedErrorText { get; set; }

        /// <summary>
        /// Indicates if there was an error.
        /// </summary>
        new bool hasError { get; set; }

        /// <summary>
        /// A tag for additional context or identification.
        /// </summary>
        string tag { get; set; }
    }
}