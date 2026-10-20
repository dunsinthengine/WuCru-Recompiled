using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

namespace UnityEditor.XCodeEditor
{
	public class XCProject : IDisposable
	{
		private PBXDictionary _datastore;

		public PBXDictionary _objects;

		private PBXGroup _rootGroup;

		private string _rootObjectKey;

		private FileInfo projectFileInfo;

		private bool modified;

		private PBXSortedDictionary<PBXBuildFile> _buildFiles;

		private PBXSortedDictionary<PBXGroup> _groups;

		private PBXSortedDictionary<PBXFileReference> _fileReferences;

		private PBXDictionary<PBXNativeTarget> _nativeTargets;

		private PBXDictionary<PBXFrameworksBuildPhase> _frameworkBuildPhases;

		private PBXDictionary<PBXResourcesBuildPhase> _resourcesBuildPhases;

		private PBXDictionary<PBXShellScriptBuildPhase> _shellScriptBuildPhases;

		private PBXDictionary<PBXSourcesBuildPhase> _sourcesBuildPhases;

		private PBXDictionary<PBXCopyFilesBuildPhase> _copyBuildPhases;

		private PBXDictionary<PBXVariantGroup> _variantGroups;

		private PBXDictionary<XCBuildConfiguration> _buildConfigurations;

		private PBXSortedDictionary<XCConfigurationList> _configurationLists;

		private PBXProject _project;

		public string projectRootPath { get; private set; }

		public string filePath { get; private set; }

		public PBXProject project
		{
			get
			{
				return _project;
			}
		}

		public PBXGroup rootGroup
		{
			get
			{
				return _rootGroup;
			}
		}

		public PBXSortedDictionary<PBXBuildFile> buildFiles
		{
			get
			{
				if (_buildFiles == null)
				{
					_buildFiles = new PBXSortedDictionary<PBXBuildFile>(_objects);
				}
				return _buildFiles;
			}
		}

		public PBXSortedDictionary<PBXGroup> groups
		{
			get
			{
				if (_groups == null)
				{
					_groups = new PBXSortedDictionary<PBXGroup>(_objects);
				}
				return _groups;
			}
		}

		public PBXSortedDictionary<PBXFileReference> fileReferences
		{
			get
			{
				if (_fileReferences == null)
				{
					_fileReferences = new PBXSortedDictionary<PBXFileReference>(_objects);
				}
				return _fileReferences;
			}
		}

		public PBXDictionary<PBXVariantGroup> variantGroups
		{
			get
			{
				if (_variantGroups == null)
				{
					_variantGroups = new PBXDictionary<PBXVariantGroup>(_objects);
				}
				return _variantGroups;
			}
		}

		public PBXDictionary<PBXNativeTarget> nativeTargets
		{
			get
			{
				if (_nativeTargets == null)
				{
					_nativeTargets = new PBXDictionary<PBXNativeTarget>(_objects);
				}
				return _nativeTargets;
			}
		}

		public PBXDictionary<XCBuildConfiguration> buildConfigurations
		{
			get
			{
				if (_buildConfigurations == null)
				{
					_buildConfigurations = new PBXDictionary<XCBuildConfiguration>(_objects);
				}
				return _buildConfigurations;
			}
		}

		public PBXSortedDictionary<XCConfigurationList> configurationLists
		{
			get
			{
				if (_configurationLists == null)
				{
					_configurationLists = new PBXSortedDictionary<XCConfigurationList>(_objects);
				}
				return _configurationLists;
			}
		}

		public PBXDictionary<PBXFrameworksBuildPhase> frameworkBuildPhases
		{
			get
			{
				if (_frameworkBuildPhases == null)
				{
					_frameworkBuildPhases = new PBXDictionary<PBXFrameworksBuildPhase>(_objects);
				}
				return _frameworkBuildPhases;
			}
		}

		public PBXDictionary<PBXResourcesBuildPhase> resourcesBuildPhases
		{
			get
			{
				if (_resourcesBuildPhases == null)
				{
					_resourcesBuildPhases = new PBXDictionary<PBXResourcesBuildPhase>(_objects);
				}
				return _resourcesBuildPhases;
			}
		}

		public PBXDictionary<PBXShellScriptBuildPhase> shellScriptBuildPhases
		{
			get
			{
				if (_shellScriptBuildPhases == null)
				{
					_shellScriptBuildPhases = new PBXDictionary<PBXShellScriptBuildPhase>(_objects);
				}
				return _shellScriptBuildPhases;
			}
		}

		public PBXDictionary<PBXSourcesBuildPhase> sourcesBuildPhases
		{
			get
			{
				if (_sourcesBuildPhases == null)
				{
					_sourcesBuildPhases = new PBXDictionary<PBXSourcesBuildPhase>(_objects);
				}
				return _sourcesBuildPhases;
			}
		}

		public PBXDictionary<PBXCopyFilesBuildPhase> copyBuildPhases
		{
			get
			{
				if (_copyBuildPhases == null)
				{
					_copyBuildPhases = new PBXDictionary<PBXCopyFilesBuildPhase>(_objects);
				}
				return _copyBuildPhases;
			}
		}

		public Dictionary<string, object> objects
		{
			get
			{
				return null;
			}
		}

		public XCProject()
		{
		}

		public XCProject(string filePath)
			: this()
		{
			if (!Directory.Exists(filePath))
			{
				Debug.LogWarning("XCode project path does not exist: " + filePath);
				return;
			}
			if (filePath.EndsWith(".xcodeproj"))
			{
				Debug.Log("Opening project " + filePath);
				projectRootPath = Path.GetDirectoryName(filePath);
				this.filePath = filePath;
			}
			else
			{
				Debug.Log("Looking for xcodeproj files in " + filePath);
				string[] directories = Directory.GetDirectories(filePath, "*.xcodeproj");
				if (directories.Length == 0)
				{
					Debug.LogWarning("Error: missing xcodeproj file");
					return;
				}
				projectRootPath = filePath;
				if (!Path.IsPathRooted(projectRootPath))
				{
					projectRootPath = Application.dataPath.Replace("Assets", string.Empty) + projectRootPath;
				}
				this.filePath = directories[0];
			}
			projectFileInfo = new FileInfo(Path.Combine(this.filePath, "project.pbxproj"));
			string data = projectFileInfo.OpenText().ReadToEnd();
			PBXParser pBXParser = new PBXParser();
			_datastore = pBXParser.Decode(data);
			if (_datastore == null)
			{
				throw new Exception("Project file not found at file path " + filePath);
			}
			if (!_datastore.ContainsKey("objects"))
			{
				Debug.Log("Errore " + _datastore.Count);
				return;
			}
			_objects = (PBXDictionary)_datastore["objects"];
			modified = false;
			_rootObjectKey = (string)_datastore["rootObject"];
			if (!string.IsNullOrEmpty(_rootObjectKey))
			{
				_project = new PBXProject(_rootObjectKey, (PBXDictionary)_objects[_rootObjectKey]);
				_rootGroup = new PBXGroup(_rootObjectKey, (PBXDictionary)_objects[_project.mainGroupID]);
			}
			else
			{
				Debug.LogWarning("error: project has no root object");
				_project = null;
				_rootGroup = null;
			}
		}

		public bool AddOtherCFlags(string flag)
		{
			return AddOtherCFlags(new PBXList(flag));
		}

		public bool AddOtherCFlags(PBXList flags)
		{
			foreach (KeyValuePair<string, XCBuildConfiguration> buildConfiguration in buildConfigurations)
			{
				buildConfiguration.Value.AddOtherCFlags(flags);
			}
			modified = true;
			return modified;
		}

		public bool AddOtherLinkerFlags(string flag)
		{
			return AddOtherLinkerFlags(new PBXList(flag));
		}

		public bool AddOtherLinkerFlags(PBXList flags)
		{
			foreach (KeyValuePair<string, XCBuildConfiguration> buildConfiguration in buildConfigurations)
			{
				buildConfiguration.Value.AddOtherLinkerFlags(flags);
			}
			modified = true;
			return modified;
		}

		public bool overwriteBuildSetting(string settingName, string newValue, string buildConfigName = "all")
		{
			Debug.Log("overwriteBuildSetting " + settingName + " " + newValue + " " + buildConfigName);
			foreach (KeyValuePair<string, XCBuildConfiguration> buildConfiguration in buildConfigurations)
			{
				XCBuildConfiguration value = buildConfiguration.Value;
				if ((string)value.data["name"] == buildConfigName || (string)value.data["name"] == "all")
				{
					buildConfiguration.Value.overwriteBuildSetting(settingName, newValue);
					modified = true;
				}
			}
			return modified;
		}

		public bool AddHeaderSearchPaths(string path)
		{
			return AddHeaderSearchPaths(new PBXList(path));
		}

		public bool AddHeaderSearchPaths(PBXList paths)
		{
			Debug.Log("AddHeaderSearchPaths " + paths);
			foreach (KeyValuePair<string, XCBuildConfiguration> buildConfiguration in buildConfigurations)
			{
				buildConfiguration.Value.AddHeaderSearchPaths(paths);
			}
			modified = true;
			return modified;
		}

		public bool AddLibrarySearchPaths(string path)
		{
			return AddLibrarySearchPaths(new PBXList(path));
		}

		public bool AddLibrarySearchPaths(PBXList paths)
		{
			Debug.Log("AddLibrarySearchPaths " + paths);
			foreach (KeyValuePair<string, XCBuildConfiguration> buildConfiguration in buildConfigurations)
			{
				buildConfiguration.Value.AddLibrarySearchPaths(paths);
			}
			modified = true;
			return modified;
		}

		public bool AddFrameworkSearchPaths(string path)
		{
			return AddFrameworkSearchPaths(new PBXList(path));
		}

		public bool AddFrameworkSearchPaths(PBXList paths)
		{
			foreach (KeyValuePair<string, XCBuildConfiguration> buildConfiguration in buildConfigurations)
			{
				buildConfiguration.Value.AddFrameworkSearchPaths(paths);
			}
			modified = true;
			return modified;
		}

		public object GetObject(string guid)
		{
			return _objects[guid];
		}

		public PBXDictionary AddFile(string filePath, PBXGroup parent = null, string tree = "SOURCE_ROOT", bool createBuildFiles = true, bool weak = false)
		{
			PBXDictionary pBXDictionary = new PBXDictionary();
			if (filePath == null)
			{
				Debug.LogError("AddFile called with null filePath");
				return pBXDictionary;
			}
			string text = string.Empty;
			if (Path.IsPathRooted(filePath))
			{
				Debug.Log("Path is Rooted");
				text = filePath;
			}
			else if (tree.CompareTo("SDKROOT") != 0)
			{
				text = Path.Combine(Application.dataPath, filePath);
			}
			if (!File.Exists(text) && !Directory.Exists(text) && tree.CompareTo("SDKROOT") != 0)
			{
				Debug.Log("Missing file: " + filePath);
				return pBXDictionary;
			}
			if (tree.CompareTo("SOURCE_ROOT") == 0)
			{
				Debug.Log("Source Root File");
				Uri uri = new Uri(text);
				Uri uri2 = new Uri(projectRootPath + "/.");
				filePath = uri2.MakeRelativeUri(uri).ToString();
			}
			else if (tree.CompareTo("GROUP") == 0)
			{
				Debug.Log("Group File");
				filePath = Path.GetFileName(filePath);
			}
			if (parent == null)
			{
				parent = _rootGroup;
			}
			PBXFileReference file = GetFile(Path.GetFileName(filePath));
			if (file != null)
			{
				Debug.Log("File already exists: " + filePath);
				return null;
			}
			file = new PBXFileReference(filePath, (TreeEnum)Enum.Parse(typeof(TreeEnum), tree));
			parent.AddChild(file);
			fileReferences.Add(file);
			pBXDictionary.Add(file.guid, file);
			if (!string.IsNullOrEmpty(file.buildPhase) && createBuildFiles)
			{
				switch (file.buildPhase)
				{
				case "PBXFrameworksBuildPhase":
					foreach (KeyValuePair<string, PBXFrameworksBuildPhase> frameworkBuildPhase in frameworkBuildPhases)
					{
						BuildAddFile(file, frameworkBuildPhase, weak);
					}
					if (!string.IsNullOrEmpty(text) && tree.CompareTo("SOURCE_ROOT") == 0)
					{
						string firstValue = Path.Combine("$(SRCROOT)", Path.GetDirectoryName(filePath));
						if (File.Exists(text))
						{
							AddLibrarySearchPaths(new PBXList(firstValue));
						}
						else
						{
							AddFrameworkSearchPaths(new PBXList(firstValue));
						}
					}
					break;
				case "PBXResourcesBuildPhase":
					foreach (KeyValuePair<string, PBXResourcesBuildPhase> resourcesBuildPhase in resourcesBuildPhases)
					{
						Debug.Log("Adding Resources Build File");
						BuildAddFile(file, resourcesBuildPhase, weak);
					}
					break;
				case "PBXShellScriptBuildPhase":
					foreach (KeyValuePair<string, PBXShellScriptBuildPhase> shellScriptBuildPhase in shellScriptBuildPhases)
					{
						Debug.Log("Adding Script Build File");
						BuildAddFile(file, shellScriptBuildPhase, weak);
					}
					break;
				case "PBXSourcesBuildPhase":
					foreach (KeyValuePair<string, PBXSourcesBuildPhase> sourcesBuildPhase in sourcesBuildPhases)
					{
						Debug.Log("Adding Source Build File");
						BuildAddFile(file, sourcesBuildPhase, weak);
					}
					break;
				case "PBXCopyFilesBuildPhase":
					foreach (KeyValuePair<string, PBXCopyFilesBuildPhase> copyBuildPhase in copyBuildPhases)
					{
						Debug.Log("Adding Copy Files Build Phase");
						BuildAddFile(file, copyBuildPhase, weak);
					}
					break;
				case null:
					Debug.LogWarning("File Not Supported: " + filePath);
					break;
				default:
					Debug.LogWarning("File Not Supported.");
					return null;
				}
			}
			return pBXDictionary;
		}

		public PBXNativeTarget GetNativeTarget(string name)
		{
			PBXNativeTarget result = null;
			foreach (KeyValuePair<string, PBXNativeTarget> nativeTarget in nativeTargets)
			{
				string text = (string)nativeTarget.Value.data["name"];
				if (text == name)
				{
					result = nativeTarget.Value;
					break;
				}
			}
			return result;
		}

		public int GetBuildActionMask()
		{
			int result = 0;
			using (Dictionary<string, PBXCopyFilesBuildPhase>.Enumerator enumerator = copyBuildPhases.GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					result = (int)enumerator.Current.Value.data["buildActionMask"];
				}
			}
			return result;
		}

		public PBXCopyFilesBuildPhase AddEmbedFrameworkBuildPhase()
		{
			PBXCopyFilesBuildPhase result = null;
			PBXNativeTarget nativeTarget = GetNativeTarget("Unity-iPhone");
			if (nativeTarget == null)
			{
				Debug.Log("Not found Correct NativeTarget.");
				return result;
			}
			foreach (KeyValuePair<string, PBXCopyFilesBuildPhase> copyBuildPhase in copyBuildPhases)
			{
				object value = null;
				if (copyBuildPhase.Value.data.TryGetValue("name", out value))
				{
					string text = (string)value;
					if (text == "Embed Frameworks")
					{
						return copyBuildPhase.Value;
					}
				}
			}
			int buildActionMask = GetBuildActionMask();
			result = new PBXCopyFilesBuildPhase(buildActionMask);
			ArrayList arrayList = (ArrayList)nativeTarget.data["buildPhases"];
			arrayList.Add(result.guid);
			copyBuildPhases.Add(result);
			return result;
		}

		public void AddEmbedFramework(string fileName)
		{
			Debug.Log("Add Embed Framework: " + fileName);
			PBXFileReference file = GetFile(Path.GetFileName(fileName));
			if (file == null)
			{
				Debug.Log("Embed Framework must added already: " + fileName);
				return;
			}
			PBXCopyFilesBuildPhase pBXCopyFilesBuildPhase = AddEmbedFrameworkBuildPhase();
			if (pBXCopyFilesBuildPhase == null)
			{
				Debug.Log("AddEmbedFrameworkBuildPhase Failed.");
				return;
			}
			PBXBuildFile pBXBuildFile = new PBXBuildFile(file);
			pBXBuildFile.AddCodeSignOnCopy();
			buildFiles.Add(pBXBuildFile);
			pBXCopyFilesBuildPhase.AddBuildFile(pBXBuildFile);
		}

		private void BuildAddFile(PBXFileReference fileReference, KeyValuePair<string, PBXFrameworksBuildPhase> currentObject, bool weak)
		{
			PBXBuildFile pBXBuildFile = new PBXBuildFile(fileReference, weak);
			buildFiles.Add(pBXBuildFile);
			currentObject.Value.AddBuildFile(pBXBuildFile);
		}

		private void BuildAddFile(PBXFileReference fileReference, KeyValuePair<string, PBXResourcesBuildPhase> currentObject, bool weak)
		{
			PBXBuildFile pBXBuildFile = new PBXBuildFile(fileReference, weak);
			buildFiles.Add(pBXBuildFile);
			currentObject.Value.AddBuildFile(pBXBuildFile);
		}

		private void BuildAddFile(PBXFileReference fileReference, KeyValuePair<string, PBXShellScriptBuildPhase> currentObject, bool weak)
		{
			PBXBuildFile pBXBuildFile = new PBXBuildFile(fileReference, weak);
			buildFiles.Add(pBXBuildFile);
			currentObject.Value.AddBuildFile(pBXBuildFile);
		}

		private void BuildAddFile(PBXFileReference fileReference, KeyValuePair<string, PBXSourcesBuildPhase> currentObject, bool weak)
		{
			PBXBuildFile pBXBuildFile = new PBXBuildFile(fileReference, weak);
			buildFiles.Add(pBXBuildFile);
			currentObject.Value.AddBuildFile(pBXBuildFile);
		}

		private void BuildAddFile(PBXFileReference fileReference, KeyValuePair<string, PBXCopyFilesBuildPhase> currentObject, bool weak)
		{
			PBXBuildFile pBXBuildFile = new PBXBuildFile(fileReference, weak);
			buildFiles.Add(pBXBuildFile);
			currentObject.Value.AddBuildFile(pBXBuildFile);
		}

		public bool AddFolder(string folderPath, PBXGroup parent = null, string[] exclude = null, bool recursive = true, bool createBuildFile = true)
		{
			Debug.Log("Folder PATH: " + folderPath);
			if (!Directory.Exists(folderPath))
			{
				Debug.Log("Directory doesn't exist?");
				return false;
			}
			if (folderPath.EndsWith(".lproj"))
			{
				Debug.Log("Ended with .lproj");
				return AddLocFolder(folderPath, parent, exclude, createBuildFile);
			}
			DirectoryInfo directoryInfo = new DirectoryInfo(folderPath);
			if (exclude == null)
			{
				Debug.Log("Exclude was null");
				exclude = new string[0];
			}
			if (parent == null)
			{
				Debug.Log("Parent was null");
				parent = rootGroup;
			}
			PBXGroup parent2 = GetGroup(directoryInfo.Name, null, parent);
			Debug.Log("New Group created");
			string[] directories = Directory.GetDirectories(folderPath);
			foreach (string text in directories)
			{
				Debug.Log("DIR: " + text);
				if (text.EndsWith(".bundle"))
				{
					Debug.LogWarning("This is a special folder: " + text);
					AddFile(text, parent2, "SOURCE_ROOT", createBuildFile);
				}
				else if (recursive)
				{
					Debug.Log("recursive");
					AddFolder(text, parent2, exclude, recursive, createBuildFile);
				}
			}
			string pattern = string.Format("{0}", string.Join("|", exclude));
			string[] files = Directory.GetFiles(folderPath);
			foreach (string input in files)
			{
				if (!Regex.IsMatch(input, pattern))
				{
					Debug.Log("Adding Files for Folder");
					AddFile(input, parent2, "SOURCE_ROOT", createBuildFile);
				}
			}
			modified = true;
			return modified;
		}

		public bool AddLocFolder(string folderPath, PBXGroup parent = null, string[] exclude = null, bool createBuildFile = true)
		{
			DirectoryInfo directoryInfo = new DirectoryInfo(folderPath);
			if (exclude == null)
			{
				exclude = new string[0];
			}
			if (parent == null)
			{
				parent = rootGroup;
			}
			Uri uri = new Uri(projectFileInfo.DirectoryName);
			Uri uri2 = new Uri(folderPath);
			string path = uri.MakeRelativeUri(uri2).ToString();
			PBXGroup pBXGroup = GetGroup(directoryInfo.Name, path, parent);
			string name = directoryInfo.Name;
			string region = name.Substring(0, name.Length - ".lproj".Length);
			project.AddRegion(region);
			string pattern = string.Format("{0}", string.Join("|", exclude));
			string[] files = Directory.GetFiles(folderPath);
			foreach (string text in files)
			{
				if (!Regex.IsMatch(text, pattern))
				{
					PBXVariantGroup pBXVariantGroup = new PBXVariantGroup(Path.GetFileName(text), null, "GROUP");
					variantGroups.Add(pBXVariantGroup);
					pBXGroup.AddChild(pBXVariantGroup);
					AddFile(text, pBXVariantGroup, "GROUP", createBuildFile);
				}
			}
			modified = true;
			return modified;
		}

		public PBXFileReference GetFile(string name)
		{
			if (string.IsNullOrEmpty(name))
			{
				return null;
			}
			foreach (KeyValuePair<string, PBXFileReference> fileReference in fileReferences)
			{
				if (!string.IsNullOrEmpty(fileReference.Value.name) && fileReference.Value.name.CompareTo(name) == 0)
				{
					return fileReference.Value;
				}
			}
			return null;
		}

		public PBXGroup GetGroup(string name, string path = null, PBXGroup parent = null)
		{
			if (string.IsNullOrEmpty(name))
			{
				return null;
			}
			if (parent == null)
			{
				parent = rootGroup;
			}
			foreach (KeyValuePair<string, PBXGroup> group in groups)
			{
				if (string.IsNullOrEmpty(group.Value.name))
				{
					if (group.Value.path.CompareTo(name) == 0 && parent.HasChild(group.Key))
					{
						return group.Value;
					}
				}
				else if (group.Value.name.CompareTo(name) == 0 && parent.HasChild(group.Key))
				{
					return group.Value;
				}
			}
			PBXGroup pBXGroup = new PBXGroup(name, path);
			groups.Add(pBXGroup);
			parent.AddChild(pBXGroup);
			modified = true;
			return pBXGroup;
		}

		public void ApplyMod(string pbxmod)
		{
			XCMod xCMod = new XCMod(pbxmod);
			foreach (object lib in xCMod.libs)
			{
				Debug.Log("Library: " + lib);
			}
			ApplyMod(xCMod);
		}

		public void ApplyMod(XCMod mod)
		{
			PBXGroup parent = GetGroup(mod.group);
			Debug.Log("Adding libraries...");
			foreach (XCModFile lib in mod.libs)
			{
				string text = Path.Combine("usr/lib", lib.filePath);
				Debug.Log("Adding library " + text);
				AddFile(text, parent, "SDKROOT", true, lib.isWeak);
			}
			Debug.Log("Adding frameworks...");
			PBXGroup parent2 = GetGroup("Frameworks");
			foreach (string framework in mod.frameworks)
			{
				string[] array = framework.Split(':');
				bool weak = ((array.Length > 1) ? true : false);
				string text3 = Path.Combine("System/Library/Frameworks", array[0]);
				AddFile(text3, parent2, "SDKROOT", true, weak);
			}
			Debug.Log("Adding files...");
			foreach (string file in mod.files)
			{
				string text4 = Path.Combine(mod.path, file);
				AddFile(text4, parent);
			}
			Debug.Log("Adding embed binaries...");
			if (mod.embed_binaries != null)
			{
				overwriteBuildSetting("LD_RUNPATH_SEARCH_PATHS", "$(inherited) @executable_path/Frameworks", "Release");
				overwriteBuildSetting("LD_RUNPATH_SEARCH_PATHS", "$(inherited) @executable_path/Frameworks", "Debug");
				foreach (string embed_binary in mod.embed_binaries)
				{
					string fileName = Path.Combine(mod.path, embed_binary);
					AddEmbedFramework(fileName);
				}
			}
			Debug.Log("Adding folders...");
			foreach (string folder in mod.folders)
			{
				string text5 = Path.Combine(Application.dataPath, folder);
				Debug.Log("Adding folder " + text5);
				AddFolder(text5, parent, (string[])mod.excludes.ToArray(typeof(string)));
			}
			Debug.Log("Adding headerpaths...");
			foreach (string headerpath in mod.headerpaths)
			{
				if (headerpath.Contains("$(inherited)"))
				{
					Debug.Log("not prepending a path to " + headerpath);
					AddHeaderSearchPaths(headerpath);
				}
				else
				{
					string path4 = Path.Combine(mod.path, headerpath);
					AddHeaderSearchPaths(path4);
				}
			}
			Debug.Log("Adding compiler flags...");
			foreach (string compiler_flag in mod.compiler_flags)
			{
				AddOtherCFlags(compiler_flag);
			}
			Debug.Log("Adding linker flags...");
			foreach (string linker_flag in mod.linker_flags)
			{
				AddOtherLinkerFlags(linker_flag);
			}
			Debug.Log("Adding plist items...");
			string plistPath = projectRootPath + "/Info.plist";
			XCPlist xCPlist = new XCPlist(plistPath);
			xCPlist.Process(mod.plist);
			Consolidate();
		}

		public void Consolidate()
		{
			PBXDictionary pBXDictionary = new PBXDictionary();
			pBXDictionary.Append(buildFiles);
			pBXDictionary.Append(copyBuildPhases);
			pBXDictionary.Append(fileReferences);
			pBXDictionary.Append(frameworkBuildPhases);
			pBXDictionary.Append(groups);
			pBXDictionary.Append(nativeTargets);
			pBXDictionary.Add(project.guid, project.data);
			pBXDictionary.Append(resourcesBuildPhases);
			pBXDictionary.Append(shellScriptBuildPhases);
			pBXDictionary.Append(sourcesBuildPhases);
			pBXDictionary.Append(variantGroups);
			pBXDictionary.Append(buildConfigurations);
			pBXDictionary.Append(configurationLists);
			_objects = pBXDictionary;
			pBXDictionary = null;
		}

		public void Backup()
		{
			string text = Path.Combine(filePath, "project.backup.pbxproj");
			if (File.Exists(text))
			{
				File.Delete(text);
			}
			File.Copy(Path.Combine(filePath, "project.pbxproj"), text);
		}

		private void DeleteExisting(string path)
		{
			if (File.Exists(path))
			{
				File.Delete(path);
			}
		}

		private void CreateNewProject(PBXDictionary result, string path)
		{
			PBXParser pBXParser = new PBXParser();
			StreamWriter streamWriter = File.CreateText(path);
			streamWriter.Write(pBXParser.Encode(result, true));
			streamWriter.Close();
		}

		public void Save()
		{
			PBXDictionary pBXDictionary = new PBXDictionary();
			pBXDictionary.Add("archiveVersion", 1);
			pBXDictionary.Add("classes", new PBXDictionary());
			pBXDictionary.Add("objectVersion", 46);
			Consolidate();
			pBXDictionary.Add("objects", _objects);
			pBXDictionary.Add("rootObject", _rootObjectKey);
			string path = Path.Combine(filePath, "project.pbxproj");
			DeleteExisting(path);
			CreateNewProject(pBXDictionary, path);
		}

		public void Dispose()
		{
		}
	}
}
