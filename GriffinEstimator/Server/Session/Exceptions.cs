using System.Runtime.CompilerServices;

namespace GriffinEstimator.Server.Session;

public class StartRoundException : Exception
{
    public StartRoundException(string message) : base(message)
    {   
    }
}

public class InvalidEstimativeException : Exception
{
    public string MemberName { get; }
    public int Estimate { get; }

    public InvalidEstimativeException(string memberName, int estimate) : base("Invalid estimative.")
    {
        MemberName = memberName;
        Estimate = estimate;
    }
}

public class InvalidOperationAtStateException : Exception
{
    public string MemberName { get; }
    public SessionState CurrentState { get; }
    public string Operation { get; }

    public InvalidOperationAtStateException(string memberName, SessionState currentState, [CallerMemberName] string operation = null)
        : base($"Cannot perform operation '{operation}' at current state '{currentState}'. Member: '{memberName}'.")
    {
        MemberName = memberName;
        CurrentState = currentState;
        Operation = operation;
    }

    public InvalidOperationAtStateException(SessionState currentState, [CallerMemberName] string operation = null)
        : base($"Cannot perform operation '{operation}' at current state '{currentState}'.")
    {
        CurrentState = currentState;
        Operation = operation;
    }
}

public class MemberNotFoundException : Exception
{
    public string MemberName { get; }

    public MemberNotFoundException(string memberName) : base($"Member '{memberName}' not found.")
    {
        MemberName = memberName;
    }
}

public class SessionNotFoundException : Exception
{
    public string SessionId { get; }

    public SessionNotFoundException(string sessionId) : base($"Session '{sessionId}' was not found.")
    {
        SessionId = sessionId;
    }
}
