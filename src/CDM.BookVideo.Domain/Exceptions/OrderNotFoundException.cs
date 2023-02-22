using System.Net;
using System.Runtime.Serialization;

namespace CDM.BookVideo.Domain.Exceptions {
  /// <summary>
  /// Order not found exception
  /// </summary>
  [Serializable]
  public class OrderNotFoundException : ValidationException {
    public OrderNotFoundException(string message) : base(HttpStatusCode.NotFound, message) { }
    protected OrderNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context) { }
  }
}
