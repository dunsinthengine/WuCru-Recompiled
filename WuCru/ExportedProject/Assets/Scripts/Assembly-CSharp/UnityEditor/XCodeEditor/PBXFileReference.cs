using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace UnityEditor.XCodeEditor
{
	public class PBXFileReference : PBXObject
	{
		protected const string PATH_KEY = "path";

		protected const string NAME_KEY = "name";

		protected const string SOURCETREE_KEY = "sourceTree";

		protected const string EXPLICIT_FILE_TYPE_KEY = "explicitFileType";

		protected const string LASTKNOWN_FILE_TYPE_KEY = "lastKnownFileType";

		protected const string ENCODING_KEY = "fileEncoding";

		public string compilerFlags;

		public string buildPhase;

		public readonly Dictionary<TreeEnum, string> trees = new Dictionary<TreeEnum, string>
		{
			{
				TreeEnum.ABSOLUTE,
				"<absolute>"
			},
			{
				TreeEnum.GROUP,
				"<group>"
			},
			{
				TreeEnum.BUILT_PRODUCTS_DIR,
				"BUILT_PRODUCTS_DIR"
			},
			{
				TreeEnum.DEVELOPER_DIR,
				"DEVELOPER_DIR"
			},
			{
				TreeEnum.SDKROOT,
				"SDKROOT"
			},
			{
				TreeEnum.SOURCE_ROOT,
				"SOURCE_ROOT"
			}
		};

		public static readonly Dictionary<string, string> typeNames = new Dictionary<string, string>
		{
			{ ".a", "archive.ar" },
			{ ".app", "wrapper.application" },
			{ ".s", "sourcecode.asm" },
			{ ".c", "sourcecode.c.c" },
			{ ".cpp", "sourcecode.cpp.cpp" },
			{ ".framework", "wrapper.framework" },
			{ ".h", "sourcecode.c.h" },
			{ ".pch", "sourcecode.c.h" },
			{ ".icns", "image.icns" },
			{ ".m", "sourcecode.c.objc" },
			{ ".mm", "sourcecode.cpp.objcpp" },
			{ ".nib", "wrapper.nib" },
			{ ".plist", "text.plist.xml" },
			{ ".png", "image.png" },
			{ ".rtf", "text.rtf" },
			{ ".tiff", "image.tiff" },
			{ ".txt", "text" },
			{ ".xcodeproj", "wrapper.pb-project" },
			{ ".xib", "file.xib" },
			{ ".strings", "text.plist.strings" },
			{ ".bundle", "wrapper.plug-in" },
			{ ".dylib", "compiled.mach-o.dylib" },
			{ ".json", "text.json" }
		};

		public static readonly Dictionary<string, string> typePhases = new Dictionary<string, string>
		{
			{ ".a", "PBXFrameworksBuildPhase" },
			{ ".app", null },
			{ ".s", "PBXSourcesBuildPhase" },
			{ ".c", "PBXSourcesBuildPhase" },
			{ ".cpp", "PBXSourcesBuildPhase" },
			{ ".framework", "PBXFrameworksBuildPhase" },
			{ ".h", null },
			{ ".pch", null },
			{ ".icns", "PBXResourcesBuildPhase" },
			{ ".m", "PBXSourcesBuildPhase" },
			{ ".mm", "PBXSourcesBuildPhase" },
			{ ".nib", "PBXResourcesBuildPhase" },
			{ ".plist", "PBXResourcesBuildPhase" },
			{ ".png", "PBXResourcesBuildPhase" },
			{ ".rtf", "PBXResourcesBuildPhase" },
			{ ".tiff", "PBXResourcesBuildPhase" },
			{ ".txt", "PBXResourcesBuildPhase" },
			{ ".json", "PBXResourcesBuildPhase" },
			{ ".xcodeproj", null },
			{ ".xib", "PBXResourcesBuildPhase" },
			{ ".strings", "PBXResourcesBuildPhase" },
			{ ".bundle", "PBXResourcesBuildPhase" },
			{ ".dylib", "PBXFrameworksBuildPhase" }
		};

		public string name
		{
			get
			{
				if (!ContainsKey("name"))
				{
					return null;
				}
				return (string)_data["name"];
			}
		}

		public string path
		{
			get
			{
				if (!ContainsKey("path"))
				{
					return null;
				}
				return (string)_data["path"];
			}
		}

		public PBXFileReference(string guid, PBXDictionary dictionary)
			: base(guid, dictionary)
		{
		}

		public PBXFileReference(string filePath, TreeEnum tree = TreeEnum.SOURCE_ROOT)
		{
			Add("path", filePath);
			Add("name", Path.GetFileName(filePath));
			Add("sourceTree", (!Path.IsPathRooted(filePath)) ? trees[tree] : trees[TreeEnum.ABSOLUTE]);
			GuessFileType();
		}

		private void GuessFileType()
		{
			Remove("explicitFileType");
			Remove("lastKnownFileType");
			string extension = Path.GetExtension((string)_data["path"]);
			if (!typeNames.ContainsKey(extension))
			{
				Debug.LogWarning("Unknown file extension: " + extension + "\nPlease add extension and Xcode type to PBXFileReference.types");
				return;
			}
			Add("lastKnownFileType", typeNames[extension]);
			buildPhase = typePhases[extension];
		}

		private void SetFileType(string fileType)
		{
			Remove("explicitFileType");
			Remove("lastKnownFileType");
			Add("explicitFileType", fileType);
		}
	}
}
