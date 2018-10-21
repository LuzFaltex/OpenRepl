using Microsoft.CodeAnalysis.Scripting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenRepl.API.Eval.ResultModels
{
    public class ScriptObject
    {
        public ScriptObject() { }
        public ScriptObject(Script script)
        {
            Code = script.Code;
            GlobalsType = script.GlobalsType;
            ReturnType = script.ReturnType;
            Previous = script.Previous == null ? null : new ScriptObject(script.Previous);
            Options = new OptionsObject(script.Options);
        }

        public string Code { get; set; }
        public Type GlobalsType { get; set; }
        public Type ReturnType { get; set; }
        public ScriptObject Previous { get; set; }
        internal OptionsObject Options { get; set; }
    }
}
