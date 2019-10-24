using System;
using System.Runtime.Serialization;

namespace Application.Exceptions
{
  [Serializable]
  public class InvalidStripeUser : Exception
  {
    //
    // For guidelines regarding the creation of new exception types, see
    //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
    // and
    //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
    //

    public InvalidStripeUser() { }

    public InvalidStripeUser(string message) : base(message) { }

    public InvalidStripeUser(string message, Exception inner) : base(message, inner) { }

    protected InvalidStripeUser(
      SerializationInfo info,
      StreamingContext context) : base(info, context) { }
  }
}