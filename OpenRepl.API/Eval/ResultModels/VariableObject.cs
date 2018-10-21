using Microsoft.CodeAnalysis.Scripting;
using System;

namespace OpenRepl.API.Eval.ResultModels
{
    public class VariableObject
    {
        public VariableObject() { }
        public VariableObject(ScriptVariable scriptVariable)
        {
            scriptVariable = scriptVariable ?? throw new ArgumentNullException(nameof(scriptVariable));
            IsReadOnly = scriptVariable.IsReadOnly;
            Name = scriptVariable.Name;
            Type = scriptVariable.Type;
            Value = scriptVariable.Value;
        }

        public bool IsReadOnly { get; set; }
        public string Name { get; set; }
        public Type Type { get; set; }
        public object Value { get; set; }
    }
}
