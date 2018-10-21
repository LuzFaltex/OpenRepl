using System;

namespace OpenRepl.API.Eval.ResultModels
{
    public class FailFastException : Exception
    {
        public FailFastException(string message, Exception exception) : base($"Script fast-failed with message \"{message}\"", exception) { }
    }
}
