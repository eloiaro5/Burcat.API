using BurcatProtocol;
using System;
using System.Collections.Generic;
using System.Text;

namespace Burcat.API
{
    [BurcatIdentity("c5dfaa56-6ac1-4e53-b4f8-99ef8054c99d")]
    public class InvalidPermissionException : BurcatException
    {
        public InvalidPermissionException() : base("The user associated with the provided stream hasn't permission to handle the provided object.") { }
        public InvalidPermissionException(string message, string? stackTrace = null, IBurcatObject? payload = null, BurcatException? innerException = null) : base(message, stackTrace, payload, innerException) { }
    }
}
