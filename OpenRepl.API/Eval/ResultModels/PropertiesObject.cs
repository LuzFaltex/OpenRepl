﻿using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Linq;

namespace OpenRepl.API.Eval.ResultModels
{
    public class PropertiesObject
    {
        public PropertiesObject() { }
        public PropertiesObject(MetadataReferenceProperties properties)
        {
            Aliases = properties.Aliases.ToList();
            EmbedInteropTypes = properties.EmbedInteropTypes;
            Kind = properties.Kind;
        }

        public List<string> Aliases { get; set; }
        public bool EmbedInteropTypes { get; set; }
        public MetadataImageKind Kind { get; set; }
    }
}
