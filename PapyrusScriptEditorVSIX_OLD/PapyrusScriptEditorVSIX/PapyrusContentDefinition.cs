using Microsoft.VisualStudio.Utilities;
using System.ComponentModel.Composition;

namespace Papyrus {
    internal static class PapyrusContentDefinition {
        public const string ContentType = "papyrus";
        public const string FileTypeName = "psc";
        public const string FileExtension = ".psc";
        public const string ProjectFileTypeName = "pscproj";
        public const string ProjectFileExtension = ".pscproj";

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
