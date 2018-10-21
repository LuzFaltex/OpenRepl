﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Scripting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;

namespace OpenRepl.API.Eval.ResultModels
{
    public class EvalResult
    {
        public EvalResult()
        {
        }

        public EvalResult(ScriptState<object> state, string consoleOut, TimeSpan executionTime, TimeSpan compileTime)
        {
            state = state ?? throw new ArgumentNullException(nameof(state));

            ReturnValue = state.ReturnValue;
            var type = state.ReturnValue?.GetType();

            if (type?.GetTypeInfo().ImplementedInterfaces.Contains(typeof(IEnumerator)) ?? false)
            {
                var genericParams = type.GetGenericArguments();

                if (genericParams.Length == 2)
                {
                    type = typeof(List<>).MakeGenericType(genericParams[1]);

                    ReturnValue = Activator.CreateInstance(type, ReturnValue);
                }
            }

            ReturnTypeName = type?.ParseGenericArgs();
            ExecutionTime = executionTime;
            CompileTime = compileTime;
            ConsoleOut = consoleOut;
            Code = state.Script.Code;
            Exception = state.Exception?.Message;
            ExceptionType = state.Exception?.GetType().Name;
        }

        public static EvalResult CreateErrorResult(string code, string consoleOut, TimeSpan compileTime, ImmutableArray<Diagnostic> compileErrors)
        {
            var ex = new CompilationErrorException(string.Join("\n", compileErrors.Select(a => a.GetMessage())), compileErrors);
            var errorResult = new EvalResult
            {
                Code = code,
                CompileTime = compileTime,
                ConsoleOut = consoleOut,
                Exception = ex.Message,
                ExceptionType = ex.GetType().Name,
                ExecutionTime = TimeSpan.FromMilliseconds(0),
                ReturnValue = null,
                ReturnTypeName = null
            };
            return errorResult;
        }

        public object ReturnValue { get; set; }

        public string ReturnTypeName { get; set; }

        public string Exception { get; set; }

        public string ExceptionType { get; set; }

        public string Code { get; set; }

        public string ConsoleOut { get; set; }

        public TimeSpan ExecutionTime { get; set; }

        public TimeSpan CompileTime { get; set; }

    }
}
