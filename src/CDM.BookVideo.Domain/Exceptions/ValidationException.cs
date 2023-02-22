using System.Net;
using System.Runtime.Serialization;

namespace CDM.BookVideo.Domain.Exceptions {
  [Serializable]
  public class ValidationException : Exception {
    /// <summary>
    /// Status code (400, 404, etc.)
    /// </summary>
    public HttpStatusCode StatusCode { get; }

    /// <summary>
    /// Creates a new instance of the exception
    /// </summary>
    /// <param name="statusCode">The status code of the response</param>
    /// <param name="message">The reponse message</param>
    public ValidationException(HttpStatusCode statusCode, string message) : base(message) {
      StatusCode = statusCode;
    }

    /// <summary>
    /// Serialization Constructor
    /// </summary>
    /// <param name="info">Serialization Info</param>
    /// <param name="context">Streaming Context</param>
    protected ValidationException(SerializationInfo info, StreamingContext context) : base(info, context) {
    }
  }
}
