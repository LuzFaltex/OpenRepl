using OpenRepl.API.Eval.ResultModels;
using System;

namespace OpenRepl.API.Eval
{
    public class Globals
    {
        public static Random Random { get; set; }
        public static ConsoleLikeStringWriter Console { get; set; }
        public static BasicEnvironment Environment { get; set; }

        public void ResetButton()
        {
            System.Environment.Exit(0);
        }

        public void PowerButton()
        {
            System.Environment.Exit(1);
        }
    }
}
