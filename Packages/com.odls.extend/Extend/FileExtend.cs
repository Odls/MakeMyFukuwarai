using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace OdlsExtend {
	public static class FileExtend {
		public static string GetDirectoryName(string p_path) {
			if (Path.GetExtension(p_path) != "") {
				return Path.GetDirectoryName(p_path);
			} else {
				return p_path;
			}
		}
		public static void PrepareDirectory(string p_path) {
			p_path = GetDirectoryName(p_path);
			if (!Directory.Exists(p_path)) {
				Directory.CreateDirectory(p_path);
			}
		}
		public static void ClearDirectory(string p_path) {
			p_path = GetDirectoryName(p_path);
			if (Directory.Exists(p_path)) {
				DirectoryInfo _directoryInfo = new DirectoryInfo(p_path);
				foreach (FileInfo _file in _directoryInfo.EnumerateFiles()) {
					_file.Delete();
				}
				foreach (DirectoryInfo _directory in _directoryInfo.EnumerateDirectories()) {
					_directory.Delete(true);
				}
			}
		}
		public static void PrepareClearDirectory(string p_path) {
			p_path = GetDirectoryName(p_path);
			if (!Directory.Exists(p_path)) {
				Directory.CreateDirectory(p_path);
			} else {
				ClearDirectory(p_path);
			}
		}
		public static List<string> GetFileInDirectory(string p_path) {
			List<string> _list = new List<string>();
			p_path = GetDirectoryName(p_path);
			if (Directory.Exists(p_path)) {
				string[] _files = Directory.GetFiles(p_path, "*", SearchOption.AllDirectories);
				int f;
				int len = _files.Length;
				for (f = 0; f < len; f++) {
					_list.Add(_files[f].Replace("\\", "/"));
				}
			}
			return _list;
		}
		/// <param name="p_includes">包含的附檔名， ".*" 表示包含任意，複數條件用'|'分格，例如 : ".png|.jpg|.bmp"</param>
		/// <param name="p_excludes">排除的附檔名， "" 表示不排除，複數條件用'|'分格，例如 : ".meta|.bak|.temp"</param>
		public static List<string> GetFileInDirectory(string p_path, string p_includes = ".*", string p_excludes = "") {
			List<string> _list = new List<string>();
			p_path = GetDirectoryName(p_path);
			if (Directory.Exists(p_path)) {
				string[] _files = Directory.GetFiles(p_path, "*", SearchOption.AllDirectories);

				SplitIncludeExclude(p_includes, p_excludes, out var _includeAll, out var _noExclude, out var _includes, out var _excludes);

				for (int f = 0; f < _files.Length; f++) {
					bool _pass = CheckExt(_files[f], _includes, _excludes, _includeAll, _noExclude);

					if (_pass) {
						_list.Add(_files[f].Replace("\\", "/"));
					}
				}
			}
			return _list;
		}
		private static void SplitIncludeExclude(string p_include, string p_exclude, out bool p_includeAll, out bool p_noExclude, out string[] p_includes, out string[] p_excludes) {
			p_includeAll = (p_include == ".*");
			p_noExclude = (p_include == "");
			p_includes = p_includeAll ? null : p_include.ToLower().Split('|');
			p_excludes = p_noExclude ? null : p_exclude.ToLower().Split('|');
		}
		/// <param name="p_includes">包含的附檔名， ".*" 表示包含任意，複數條件用'|'分格，例如 : ".png|.jpg|.bmp"</param>
		/// <param name="p_excludes">排除的附檔名， "" 表示不排除，複數條件用'|'分格，例如 : ".meta|.bak|.temp"</param>
		static bool CheckExt(string p_path, string p_includes = ".*", string p_excludes = "") {
			SplitIncludeExclude(p_includes, p_excludes, out var _includeAll, out var _noExclude, out var _includes, out var _excludes);
			return CheckExt(p_path, _includes, _excludes, _includeAll, _noExclude);
		}

		static bool CheckExt(string p_path, string[] p_includes, string[] p_excludes, bool p_includeAll, bool p_noExclude) {
			bool _pass = true;
			string _ext = Path.GetExtension(p_path);

			if (!p_includeAll) {
				_pass = false;
				foreach (string _include in p_includes) {
					if (string.Equals(_ext, _include, System.StringComparison.OrdinalIgnoreCase)) {
						_pass = true;
						break;
					}
				}
			}

			if (_pass && (!p_noExclude)) {
				foreach (string _exclude in p_excludes) {
					if (string.Equals(_ext, _exclude, System.StringComparison.OrdinalIgnoreCase)) {
						_pass = false;
						break;
					}
				}
			}

			return _pass;
		}

#if UNITY_EDITOR

		static string mFullAssetPath = null;
		public static string fullAssetPath => mFullAssetPath ??= Application.dataPath + "/";

		public static string ToFullAssetPath(this string p_path) {
			if (string.IsNullOrEmpty(p_path)) { return fullAssetPath; }
			var _ext = Path.GetExtension(p_path);
			p_path = ((_ext=="") && (p_path[p_path.Length - 1] != '/')) ? p_path + "/" : p_path;

			if (p_path == "Assets/") {
				return fullAssetPath;
			} else if (p_path.StartsWith("Assets/")) {
				return fullAssetPath + p_path.Substring(7);
			} else {
				return fullAssetPath + p_path;
			}
		}

		static string mFullPackagePath = null;
		public static string fullPackagePath => mFullPackagePath ??= Application.dataPath.Substring(0, Application.dataPath.Length - 6) + "Packages/";

		public static string ToFullPackagePath(this string p_path) {
			if (string.IsNullOrEmpty(p_path)) { return fullPackagePath; }
			var _ext = Path.GetExtension(p_path);
			p_path = ((_ext == "") && (p_path[p_path.Length - 1] != '/')) ? p_path + "/" : p_path;

			if (p_path == "Packages/") {
				return fullPackagePath;
			} else if (p_path.StartsWith("Packages/")) {
				return fullPackagePath + p_path.Substring(9);
			} else {
				return fullPackagePath + p_path;
			}
		}

		static bool CanRunPackage(string p_path, bool p_includePackage, out string p_fullPackagePath) {
			p_fullPackagePath = null;

			if (!p_includePackage) { return false; }
			if ((p_path == "Assets") || (p_path.StartsWith("Assets/"))) { return false; }

			p_fullPackagePath = p_path.ToFullPackagePath();
			return true;
		}
		public static bool FileIsExistsInProject(bool p_includePackage = false) => FileIsExistsInProject("", p_includePackage);
		public static bool FileIsExistsInProject(string p_path, bool p_includePackage = false) {
			if (File.Exists(p_path.ToFullAssetPath())) {
				return true;
			}

			return CanRunPackage(p_path, p_includePackage, out var _fullPackagePath) && File.Exists(_fullPackagePath);

		}
		public static bool DirectoryIsExistsInProject(bool p_includePackage = false) => DirectoryIsExistsInProject("", p_includePackage);
		public static bool DirectoryIsExistsInProject(string p_path, bool p_includePackage = false) {
			if (Directory.Exists(p_path.ToFullAssetPath())) {
				return true;
			}

			return CanRunPackage(p_path, p_includePackage, out var _fullPackagePath) && Directory.Exists(_fullPackagePath);
		}
		public static List<string> GetFilesInProject(bool p_includePackage = false) => GetFilesInProject("", p_includePackage);
		public static List<string> GetFilesInProject(string p_path, bool p_includePackage = false) {
			var _files = GetFileInDirectory(p_path.ToFullAssetPath());
			if (CanRunPackage(p_path, p_includePackage, out var _fullPackagePath)) {
				_files.AddRange(GetFileInDirectory(_fullPackagePath));
			}
			return _files;
		}
		/// <param name="p_includes">包含的附檔名， ".*" 表示包含任意，複數條件用'|'分格，例如 : ".png|.jpg|.bmp"</param>
		/// <param name="p_excludes">排除的附檔名， "" 表示不排除，複數條件用'|'分格，例如 : ".meta|.bak|.temp"</param>
		public static List<string> GetFilesInProject(string p_includes = ".*", string p_excludes = "", bool p_includePackage = false) => GetFilesInProject("", p_includes, p_excludes, p_includePackage);
		/// <param name="p_includes">包含的附檔名， ".*" 表示包含任意，複數條件用'|'分格，例如 : ".png|.jpg|.bmp"</param>
		/// <param name="p_excludes">排除的附檔名， "" 表示不排除，複數條件用'|'分格，例如 : ".meta|.bak|.temp"</param>
		public static List<string> GetFilesInProject(string p_path, string p_includes = ".*", string p_excludes = "", bool p_includePackage = false) {
			var _files = GetFileInDirectory(p_path.ToFullAssetPath(), p_includes, p_excludes);
			if (CanRunPackage(p_path, p_includePackage, out var _fullPackagePath)) {
				_files.AddRange(GetFileInDirectory(_fullPackagePath, p_includes, p_excludes));
			}
			return _files;
		}
		/// <param name="p_includes">包含的附檔名， ".*" 表示包含任意，複數條件用'|'分格，例如 : ".png|.jpg|.bmp"</param>
		/// <param name="p_excludes">排除的附檔名， "" 表示不排除，複數條件用'|'分格，例如 : ".meta|.bak|.temp"</param>
		public static List<T> LoadAllAsset<T>(string p_includes = ".*", string p_excludes = "")
		where T : Object {
			List<T> _objs = new List<T>();
			LoadAllAsset(_objs, null, p_includes, p_excludes);
			return _objs;
		}
		/// <param name="p_includes">包含的附檔名， ".*" 表示包含任意，複數條件用'|'分格，例如 : ".png|.jpg|.bmp"</param>
		/// <param name="p_excludes">排除的附檔名， "" 表示不排除，複數條件用'|'分格，例如 : ".meta|.bak|.temp"</param>
		public static void LoadAllAsset<T>(List<T> p_objs, string p_includes = ".*", string p_excludes = "") where T : Object => LoadAllAsset(p_objs, null, p_includes, p_excludes);
		/// <param name="p_includes">包含的附檔名， ".*" 表示包含任意，複數條件用'|'分格，例如 : ".png|.jpg|.bmp"</param>
		/// <param name="p_excludes">排除的附檔名， "" 表示不排除，複數條件用'|'分格，例如 : ".meta|.bak|.temp"</param>
		public static List<T> LoadAllAsset<T>(List<string> p_paths, string p_includes = ".*", string p_excludes = "")
		where T : Object {
			List<T> _objs = new List<T>();
			LoadAllAsset(_objs, p_paths, p_includes, p_excludes);
			return _objs;
		}
		/// <param name="p_includes">包含的附檔名， ".*" 表示包含任意，複數條件用'|'分格，例如 : ".png|.jpg|.bmp"</param>
		/// <param name="p_excludes">排除的附檔名， "" 表示不排除，複數條件用'|'分格，例如 : ".meta|.bak|.temp"</param>
		public static void LoadAllAsset<T>(List<T> p_objs, List<string> p_paths, string p_includes = ".*", string p_excludes = "")
		where T : Object {
			p_objs?.Clear();
			p_paths?.Clear();

			SplitIncludeExclude(p_includes, p_excludes, out var _includeAll, out var _noExclude, out var _includes, out var _excludes);

			var _type = typeof(T);
			string _search = string.IsNullOrEmpty(_type.Namespace) ? "t:" + _type.Name : $"t:{_type.Namespace}.{_type.Name}";
			foreach (var _guid in AssetDatabase.FindAssets(_search)) {
				string _assetPath = AssetDatabase.GUIDToAssetPath(_guid);

				if (CheckExt(_assetPath, _includes, _excludes, _includeAll, _noExclude)) {
					T _obj = AssetDatabase.LoadAssetAtPath<T>(_assetPath);
					if (_obj != null) {
						p_objs?.Add(_obj);
						p_paths?.Add(_assetPath);
					}
				}
			}
		}
#endif
	}
}