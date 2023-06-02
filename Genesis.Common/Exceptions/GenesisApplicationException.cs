namespace Genesis.Common.Exceptions;

public class GenesisApplicationException : Exception
{
    public GenesisApplicationException()
    {
    }

    public GenesisApplicationException(string message, params object[] args)
        : base(message)
    {
        Arguments = args;
    }


    public GenesisApplicationException(string message)
        : base(message)
    {
    }

    public GenesisApplicationException(string message, string paramName)
        : base(message)
    {
        ParameterName = paramName;
    }

    public GenesisApplicationException(string message, Exception inner)
        : base(message, inner)
    {
    }

    public GenesisApplicationException(string message, Exception inner, params object[] args)
        : base(message, inner)
    {
        Arguments = args;
    }

    public object[] Arguments { get; }
    public string ParameterName { get; }
}