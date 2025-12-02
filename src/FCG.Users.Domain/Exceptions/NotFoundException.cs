using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace FCG.Users.Domain.Exceptions
{
    [ExcludeFromCodeCoverage]
    public class NotFoundException : BaseException
    {
        public NotFoundException(string message) : base(HttpStatusCode.NotFound, message) { }
        public NotFoundException(string message, Exception innerException) : base(HttpStatusCode.NotFound, message, innerException) { }
    }
}
