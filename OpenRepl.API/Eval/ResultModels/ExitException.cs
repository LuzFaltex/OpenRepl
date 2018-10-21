using System;

namespace OpenRepl.API.Eval.ResultModels
{
    public class ExitException : Exception
    {
        public ExitException(int exitCode) : base($"Script has exited with code {exitCode}") { }
    }
}
