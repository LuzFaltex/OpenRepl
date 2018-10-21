using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace OpenRepl.API
{
    public interface IDisassemblyService
    {
        IReadOnlyCollection<MetadataReference> References { get; }
        ImmutableArray<string> Imports { get; }
        string GetIl(string code);
    }
}
