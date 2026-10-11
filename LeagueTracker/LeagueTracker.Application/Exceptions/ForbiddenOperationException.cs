using System;
using System.Collections.Generic;
using System.Text;

namespace LeagueTracker.Application.Exceptions;

public class ForbiddenOperationException : Exception
{
    public ForbiddenOperationException(string message) : base(message)
    {
        
    }
}
