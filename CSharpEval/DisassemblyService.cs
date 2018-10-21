﻿using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.Disassembler;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Newtonsoft.Json;
using OpenRepl.API;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace CSharpEval
{
    public class DisassemblyService : IDisassemblyService
    {
        internal IReadOnlyCollection<MetadataReference> References
        {
            get
            {
                return ImmutableArray.Create(
                    MetadataReference.CreateFromFile(typeof(Binder).GetTypeInfo().Assembly.Location),
                    MetadataReference.CreateFromFile(typeof(ValueTuple<>).GetTypeInfo().Assembly.Location),
                    MetadataReference.CreateFromFile(typeof(Enumerable).GetTypeInfo().Assembly.Location),
                    MetadataReference.CreateFromFile(typeof(List<>).GetTypeInfo().Assembly.Location),
                    MetadataReference.CreateFromFile(typeof(JsonConvert).GetTypeInfo().Assembly.Location),
                    MetadataReference.CreateFromFile(typeof(string).GetTypeInfo().Assembly.Location),
                    MetadataReference.CreateFromFile(typeof(HttpClient).GetTypeInfo().Assembly.Location),
                    MetadataReference.CreateFromFile(typeof(Regex).GetTypeInfo().Assembly.Location),
                    MetadataReference.CreateFromFile(typeof(BinaryExpression).GetTypeInfo().Assembly.Location),
                    MetadataReference.CreateFromFile(typeof(Console).GetTypeInfo().Assembly.Location),
                    MetadataReference.CreateFromFile(Assembly.Load("System.Runtime, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a").Location)
                );
            }
        }

        internal ImmutableArray<string> Imports =>
            ImmutableArray.Create(
                "System",
                "System.IO",
                "System.Linq",
                "System.Linq.Expressions",
                "System.Collections.Generic",
                "System.Text",
                "System.Text.RegularExpressions",
                "System.Net",
                "System.Threading",
                "System.Threading.Tasks",
                "System.Net.Http",
                "Newtonsoft.Json",
                "Newtonsoft.Json.Linq",
                "System.Reflection",
                "System.Reflection.Emit"
            );

        IReadOnlyCollection<MetadataReference> IDisassemblyService.References => References;

        ImmutableArray<string> IDisassemblyService.Imports => Imports;

        public string GetIl(string code)
        {
            StringBuilder imports = new StringBuilder();
            foreach (var import in Imports)
            {
                imports.AppendLine($"using {import};");
            }

            string toExecute = $@"
            {imports}

            namespace Eval
            {{
                public class Code
                {{
                    public object Main()
                    {{
                        {code}
                    }}
                }}
            }}
            ";

            var opts = CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.Latest).WithKind(SourceCodeKind.Regular);

            var scriptSyntaxTree = CSharpSyntaxTree.ParseText(toExecute, opts);
            var compOpts = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary).WithOptimizationLevel(OptimizationLevel.Release).WithAllowUnsafe(true).WithPlatform(Platform.AnyCpu);

            var compilation = CSharpCompilation.Create(Guid.NewGuid().ToString(), options: compOpts, references: References).AddSyntaxTrees(scriptSyntaxTree);

            var sb = new StringBuilder();
            using (var pdb = new MemoryStream())
            using (var dll = new MemoryStream())
            {
                var result = compilation.Emit(dll, pdb);
                if (!result.Success)
                {
                    sb.AppendLine("Emit Failed");
                    sb.AppendLine(string.Join(Environment.NewLine, result.Diagnostics.Select(a => a.GetMessage())));
                }
                else
                {
                    dll.Seek(0, SeekOrigin.Begin);
                    using (var module = ModuleDefinition.ReadModule(dll))
                    using (var writer = new StringWriter(sb))
                    {
                        module.Name = compilation.AssemblyName;
                        var plainOutput = new PlainTextOutput(writer);
                        var rd = new ReflectionDisassembler(plainOutput, CancellationToken.None)
                        {
                            DetectControlStructure = false
                        };
                        var ignoredMethods = new[] { ".ctor" };
                        var methods = module.Types.SelectMany(a => a.Methods).Where(a => !ignoredMethods.Contains(a.Name));
                        foreach (var method in methods)
                        {
                            rd.DisassembleMethod(method);
                            plainOutput.WriteLine();
                        }
                    }
                }

                var final = sb.ToString();

                return final;
            }
        }
    }
}
