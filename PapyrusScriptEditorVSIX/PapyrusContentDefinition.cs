using Microsoft.VisualStudio.Utilities;
using System.ComponentModel.Composition;

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
