namespace Genesis.Common.Exceptions
{
    public class GenesisDalException : Exception
    {
        public GenesisDalException()
        {
        }

        public GenesisDalException(string message, params object[] args)
            : base(message)
        {
            Arguments = args;
        }


        public GenesisDalException(string message)
            : base(message)
        {
        }

        public GenesisDalException(string message, string paramName)
            : base(message)
        {
            ParameterName = paramName;
        }

        public GenesisDalException(string message, Exception inner)
            : base(message, inner)
        {
        }

        public GenesisDalException(string message, Exception inner, params object[] args)
            : base(message, inner)
        {
            Arguments = args;
        }

        public object[] Arguments { get; }
        public string ParameterName { get; }
    }
}
