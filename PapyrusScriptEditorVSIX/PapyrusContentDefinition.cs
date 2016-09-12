using Microsoft.VisualStudio.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Papyrus {
    internal static class PapyrusContentDefinition {
        public const string ContentType = "papyrus";
        public const string FileExtension = ".psc";

        [Export]
        [Name(ContentType)]
        [BaseDefinition("code")]
        internal static ContentTypeDefinition PapyrusContentTypeDefinition = null;

        [Export]
        [FileExtension(FileExtension)]
        [ContentType(ContentType)]
        internal static FileExtensionToContentTypeDefinition PapyrusFileExtensionDefinition = null;
    }
}
