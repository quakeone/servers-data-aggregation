using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServersDataAggregation.Query;

/// <summary>
/// Exception thrown when there's an error parsing the response from a server
/// </summary>
public class ServerQueryParseException : Exception
{
    public ServerQueryParseException(Exception pInnerException) : base(pInnerException.Message, pInnerException) { }
}
/// <summary>
/// Exception thrown when a serverinfo request has timed out
/// </summary>
public class ServerNotRespondingException : Exception
{
    public ServerNotRespondingException(Exception pInnerException) : base(pInnerException.Message, pInnerException) { }
}
/// <summary>
/// Exception thrown when a route to a server could nto be found
/// </summary>
public class ServerNotFoundException : Exception
{
    public ServerNotFoundException(Exception pInnerException) : base(pInnerException.Message, pInnerException) { }
}