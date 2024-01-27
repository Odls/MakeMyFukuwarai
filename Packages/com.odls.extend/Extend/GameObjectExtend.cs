//#define log
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace OdlsExtend {
	public static class GameObjectExtend {
#if log
		static string log = "";
#endif

		#region "Hierarchy"
		public static string GetFullPath(this GameObject p_obj) {
			return GetFullPath(p_obj.transform);
		}
		public static string GetFullPath(this Component p_component) {
			return GetFullPath(p_component.transform);
		}
		public static string GetFullPath(this Transform p_transform) {
			Transform _nowTrans = p_transform;

			List<string> _pathList = new List<string>();

			_pathList.Add(_nowTrans.name);

			while (_nowTrans.parent != null) {
				_pathList.Insert(0, _nowTrans.parent.name);
				_nowTrans = _nowTrans.parent;
			}

			return string.Join("/", _pathList.ToArray());
		}
		#endregion

		#region "Transform"
#if UNITY_EDITOR
		public static Vector2 screenSize => Handles.GetMainGameViewSize();
#else
	public static Vector2 screenSize => new Vector2(Screen.width, Screen.height);
#endif
		public static void SetLossyScale(this Transform p_tran, Vector3 p_lossyScale) {
			Transform _parentTrans = p_tran.parent;
			if (_parentTrans == null) {
				p_tran.localScale = p_lossyScale;
			} else {
				Vector3 _parentScale = _parentTrans.lossyScale;
				p_tran.localScale = new Vector3((_parentScale.x == 0 ? 0 : p_lossyScale.x / _parentScale.x),
												(_parentScale.y == 0 ? 0 : p_lossyScale.y / _parentScale.y),
												(_parentScale.z == 0 ? 0 : p_lossyScale.z / _parentScale.z));
			}
		}

		public static Vector2 WorldPointToCanvasPoint(this Canvas p_canvas, Vector3 p_worldPoint, bool p_normalize = false) {
			Vector2 _canvasPos = p_canvas.transform.InverseTransformPoint(p_worldPoint);

			if (!p_normalize) {
				return _canvasPos;
			} else {
				Rect _canvasRect = ((RectTransform)p_canvas.transform).rect;
				return new Vector2(_canvasPos.x / _canvasRect.width, _canvasPos.y / _canvasRect.height) + new Vector2(0.5f, 0.5f);
			}
		}

		static Vector3[] tempWorldCorners = new Vector3[4];
		public static Rect RectTransformToCanvasRect(this Canvas p_canvas, RectTransform p_rectTransform, bool p_normalize = false) {
			p_rectTransform.GetWorldCorners(tempWorldCorners);
			var _min = WorldPointToCanvasPoint(p_canvas, tempWorldCorners[0], p_normalize);
			var _max = WorldPointToCanvasPoint(p_canvas, tempWorldCorners[2], p_normalize);
			var _size = _max - _min;

			return new Rect(_min, _size);
		}

		public static Rect RectTransformToScreenRect(this Canvas p_canvas, RectTransform p_rectTransform, bool p_normalize = false) {
			p_rectTransform.GetWorldCorners(tempWorldCorners);
			var _min = WorldPointToScreenPoint(p_canvas, tempWorldCorners[0], p_normalize);
			var _max = WorldPointToScreenPoint(p_canvas, tempWorldCorners[2], p_normalize);
			var _size = _max - _min;

			return new Rect(_min, _size);
		}

		public static Camera GetRenderCamera(this Canvas p_canvas) {
			switch (p_canvas.renderMode) {
			case RenderMode.ScreenSpaceCamera:
				return p_canvas.worldCamera;
				break;
			case RenderMode.WorldSpace:
				return p_canvas.worldCamera ?? Camera.main;
				break;
			default:
				return null;
				break;
			}
		}
		public static Vector2 WorldPointToScreenPoint(this Canvas p_canvas, Vector3 p_worldPoint, bool p_normalize = false) {
			Camera _camera = p_canvas.GetRenderCamera();
			Vector2 _screenPoint;

			if (_camera != null) {
				_screenPoint = _camera.WorldToScreenPoint(p_worldPoint);
				if (p_normalize) {
					return new Vector2(_screenPoint.x / screenSize.x, _screenPoint.y / screenSize.y);
				}
			} else {
				Vector2 _normalizeCanvasPos = p_canvas.WorldPointToCanvasPoint(p_worldPoint, true);
				if (p_normalize) {
					_screenPoint = _normalizeCanvasPos;
				} else {
					_screenPoint = new Vector2(_normalizeCanvasPos.x * screenSize.x, _normalizeCanvasPos.y * screenSize.y);
				}
			}
			return _screenPoint;
		}
		public static Vector2 CameraPointToScreenPoint(this Camera p_camera, Vector3 p_cameraPoint) {
			Rect _cameraRect = p_camera.rect;
			Vector2 _scale = new Vector2(_cameraRect.width, _cameraRect.height);
			Vector2 _offset = new Vector2(_cameraRect.x, _cameraRect.y);
			return Vector2.Scale(p_cameraPoint, _scale) + _offset;
		}

		#endregion

		#region "Child"
		public static void DelAllChild(this GameObject p_obj) {
			Transform[] _childs = p_obj.transform.GetComponentsInChildren<Transform>();
			int f;
			int len = _childs.Length;
			for (f = 1; f < len; f++) {
				GameObject.Destroy(_childs[f].gameObject);
			}
		}
		public static void AddChild(this GameObject p_obj, GameObject p_child) {
			p_child.transform.SetParent(p_obj.transform);
		}
		public static void AddChildAndResetTransform(this GameObject p_obj, GameObject p_child) {
			AddChild(p_obj, p_child, Vector3.zero, Quaternion.identity, Vector3.one);
		}
		public static void AddChild(this GameObject p_obj, GameObject p_child, Vector3 p_pos, Vector3 p_rota, Vector3 p_scale) {
			AddChild(p_obj, p_child, p_pos, Quaternion.Euler(p_rota), p_scale);
		}
		public static void AddChild(this GameObject p_obj, GameObject p_child, Vector3 p_pos, Quaternion p_rota, Vector3 p_scale) {
			p_child.transform.SetParent(p_obj.transform);
			if (p_pos != null) {
				p_child.transform.localPosition = p_pos;
			}
			if (p_rota != null) {
				p_child.transform.localRotation = p_rota;
			}
			if (p_scale != null) {
				p_child.transform.localScale = p_scale;
			}
		}
		#endregion

		#region "Component"
		static List<GameObject> tempRootGameObjects = new List<GameObject>();
		static IEnumerable<GameObject> GetRootGameObjects() {
			foreach (var _scene in SceneManager.GetAllScenes()) {
				_scene.GetRootGameObjects(tempRootGameObjects);
				foreach (var _obj in tempRootGameObjects) {
					yield return _obj;
				}
			}
		}

		public static T GetComponentInParentWithoutSelfComponent<T>(this Component p_self) where T : Component {
			foreach(var _target in p_self.GetComponents<T>()) {
				if(_target!= p_self) {
					return _target;
				}
			}
			return GetComponentInParentWithoutSelf<T>(p_self.gameObject);
		}
		public static T GetComponentInParentWithoutSelf<T>(this GameObject p_obj) where T : Component => p_obj.transform.GetComponentInParentWithoutSelf<T>();
		public static T GetComponentInParentWithoutSelf<T>(this Transform p_tran) where T : Component {
			Transform _parent = p_tran.parent;
			return _parent?.GetComponentInParent<T>();
		}
		public static T GetComponentInScene<T>() where T : Component {
			GameObject[] _rootObjs = SceneManager.GetActiveScene().GetRootGameObjects();

			foreach (var _obj in _rootObjs) {
				T _component = _obj.GetComponentInChildren<T>(true);
				if (_component != null) {
					return _component;
				}
			}
			return null;
		}

		public static List<T> GetComponentsInScene<T>() where T : Component {
			GameObject[] _rootObjs = SceneManager.GetActiveScene().GetRootGameObjects();
			List<T> _list = new List<T>();

			foreach (var _obj in _rootObjs) {
				T[] _components = _obj.GetComponentsInChildren<T>(true);
				if (_components != null) {
					_list.AddRange(_components);
				}
			}
			return _list;
		}

		public static GameObject FindGameObjectInScene(string p_name, bool includeInactive = false) {
			var _findObj = GameObject.Find(p_name);
			if (_findObj != null) { return _findObj; }
			if (!includeInactive) { return null; }

			Queue<GameObject> _objs = new Queue<GameObject>();
			foreach (var _obj in SceneManager.GetActiveScene().GetRootGameObjects()) {
				_objs.Enqueue(_obj);
			}

			int _splitIndex = p_name.IndexOf('/');
			if (_splitIndex < 0) {
				// No Path, Only Name

				return FindGameObjectInScene((p_obj) => (p_obj.name == p_name) ? p_obj : null);
			} else {
				// Has Path, Find If Is Target Name
				string _targetTopName = p_name.Substring(0, _splitIndex);
				string _findName = p_name.Substring(_splitIndex + 1);

				return FindGameObjectInScene((p_obj) => (p_obj.name == _targetTopName) ? p_obj.Find(_findName) : null);
			}
		}

		static Queue<Transform> tempObjs = new Queue<Transform>();
		public static GameObject FindGameObjectInScene(System.Func<Transform, Transform> p_getTarget, bool includeInactive = false) {
			foreach (var _obj in GetRootGameObjects()) {
				tempObjs.Enqueue(_obj.transform);
			}

			while (tempObjs.Count > 0) {
				var _obj = tempObjs.Dequeue();
				var _findObj = p_getTarget.Invoke(_obj);
				if (_findObj != null) { return _findObj.gameObject; }

				foreach (Transform _child in _obj.transform) {
					if (_child == _obj.transform) { continue; }
					tempObjs.Enqueue(_child);
				}
			}
			tempObjs.Clear();
			return null;
		}
		public static List<Transform> GetChildList(this GameObject p_obj) {
			return GetChildList(p_obj.transform);
		}
		public static List<Transform> GetChildList(this Transform p_trans) {
			int _childLen = p_trans.childCount;
			List<Transform> _list = new List<Transform>();
			for (int f = 0; f < _childLen; f++) {
				_list.Add(p_trans.GetChild(f));
			}
			return _list;
		}

		static BindingFlags fieldFlags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Default;

		public static void CopyGameObjectValue(this GameObject p_targetGameObject, GameObject p_sourceGameObject, bool p_includeChild = false, bool p_delOther = false) {

			p_targetGameObject.name = p_sourceGameObject.name + "_Copy";
#if log
		log = "";
#endif
			DoCopyGameObjectValue(p_targetGameObject, p_sourceGameObject, p_includeChild, p_delOther);
#if log
		Debug.Log (log);
#endif


			string _targetPath = GetFullPath(p_targetGameObject);
			string _sourcePath = GetFullPath(p_sourceGameObject);
#if log
		log = "";
		try{
#endif
			DoFixGameObjectSelfReference(p_targetGameObject, _targetPath, _sourcePath, p_includeChild);
#if log
		}catch(System.Exception e){
			Debug.LogError (e.Message + "\n" + e.StackTrace + "\n---------\n");
		}
		Debug.Log (log);
		GUIUtility.systemCopyBuffer = log;
#endif
			p_targetGameObject.name = p_sourceGameObject.name;
		}

		static void DoCopyGameObjectValue(this GameObject p_targetGameObject, GameObject p_sourceGameObject, bool p_includeChild = false, bool p_delOther = false) {

#if log
		log += "CopyGameObject " + p_targetGameObject.name + "\n";
#endif
			Component[] _sourceComponents = p_sourceGameObject.GetComponents<Component>();
			List<Component> _targetComponents = new List<Component>(p_targetGameObject.GetComponents<Component>());
			int _targetLen = _targetComponents.Count;
			foreach (Component _sourceComponent in _sourceComponents) {
				for (int f = 0; f < _targetLen; f++) {
					Component _targetComponent = _targetComponents[f];
					if (_sourceComponent.GetType().Name == _targetComponent.GetType().Name) {
						CopyComponentValue(_targetComponent, _sourceComponent);
						_targetComponents.RemoveAt(f);
						_targetLen--;
						break;
					}
				}
			}

			if (p_includeChild) {
				List<Transform> _sourceChilds = GetChildList(p_sourceGameObject);
				List<Transform> _targetChilds = GetChildList(p_targetGameObject);
				_targetLen = _targetChilds.Count;
				foreach (Transform _sourceChild in _sourceChilds) {
					Transform _targetChild = null;
					for (int f = 0; f < _targetLen; f++) {
						if (_sourceChild.name == _targetChilds[f].name) {
							_targetChild = _targetChilds[f];
							_targetChilds.RemoveAt(f);
							_targetLen--;
							break;
						}
					}
					if (_targetChild == null) {
						_targetChild = GameObject.Instantiate(_sourceChild.gameObject).transform;
						_targetChild.name = _sourceChild.name;
						_targetChild.SetParent(p_targetGameObject.transform);
					}
					DoCopyGameObjectValue(_targetChild.gameObject, _sourceChild.gameObject, true, p_delOther);
				}

				if (p_delOther) {
					foreach (Transform _targetChild in _targetChilds) {
						if (_targetChild != null) {
							GameObject.DestroyImmediate(_targetChild.gameObject);
						}
					}
				}
			}
		}

		static void DoFixGameObjectSelfReference(this GameObject p_targetGameObject, string p_targetPath, string p_sourcePath, bool p_includeChild = false) {
#if log
		log += "FixGameObject " + p_targetGameObject.name + "\n";
#endif
			List<Component> _targetComponents = new List<Component>(p_targetGameObject.GetComponents<Component>());
			int _targetLen = _targetComponents.Count;
			for (int f = 0; f < _targetLen; f++) {
				Component _targetComponent = _targetComponents[f];
				FixComponentSelfReference(_targetComponent, p_targetPath, p_sourcePath);
			}

			if (p_includeChild) {
				List<Transform> _targetChilds = GetChildList(p_targetGameObject);
				_targetLen = _targetChilds.Count;

				for (int f = 0; f < _targetLen; f++) {
					DoFixGameObjectSelfReference(_targetChilds[f].gameObject, p_targetPath, p_sourcePath, true);
				}
			}
		}

		public static void CopyComponentValue(this Component p_targetComponent, Component p_sourceComponent) {
#if UNITY_EDITOR
			UnityEditorInternal.ComponentUtility.CopyComponent(p_sourceComponent);
			UnityEditorInternal.ComponentUtility.PasteComponentValues(p_targetComponent);
#endif
		}
		public static void FixComponentSelfReference(this Component p_targetComponent, string p_targetPath, string p_sourcePath) {
#if log
		log += "  FixComponent " + p_targetComponent.GetType().Name + "\n";
#endif

			System.Type _type = p_targetComponent.GetType();

			FieldInfo[] _fields = _type.GetFields(fieldFlags);
			foreach (FieldInfo _field in _fields) {
				FixFieldSelfReference(_field, p_targetComponent, p_targetPath, p_sourcePath);
			}

		}
		static void FixObjMemberSelfReference(object p_value, string p_targetPath, string p_sourcePath) {
			System.Type _type = p_value.GetType();
			FieldInfo[] _fields = _type.GetFields(fieldFlags);
			foreach (FieldInfo _field in _fields) {
				FixFieldSelfReference(_field, p_value, p_targetPath, p_sourcePath);
			}
		}
		static void FixFieldSelfReference(FieldInfo p_field, object p_targetObject, string p_targetPath, string p_sourcePath) {
			object _value = p_field.GetValue(p_targetObject);
			_value = FixObjSelfReference(p_field.Name, _value, p_targetPath, p_sourcePath);
			if (_value != null) {
				p_field.SetValue(p_targetObject, _value);
			}
		}
		static object FixObjSelfReference(string p_objName, object _value, string p_targetPath, string p_sourcePath) {
			string _path;

			if ((_value == null) || (_value.ToString() == "null")) {
#if log
			log += "    FixField " + p_objName + " (Not Need [Null]) \n";
#endif
				return null;
			}

			string _typeName = _value.GetType().Name;

			Component _component = _value as Component;
			if (_component != null) {
				_path = GetFullPath(_component.gameObject);
				if (_path.Contains(p_sourcePath)) {
					_path = _path.Replace(p_sourcePath, p_targetPath);
					GameObject _newObj = GameObject.Find(_path);
					if (_newObj) {
						Component _newComponent = _newObj.GetComponent(_component.GetType());
						if (_newComponent) {
#if log
						log += "    FixField " + p_objName + " (Component SetValue [" + _path + "]) \n";
#endif
							//						FixObjMemberSelfReference (_value, p_targetPath, p_sourcePath);
							return _newComponent;
						}
					}
				}
#if log
			log += "    FixField " + p_objName + " (Component Not Self [" + _path + "]) \n";
#endif
				return null;
			}

			GameObject _gameObject = _value as GameObject;
			if (_gameObject != null) {
				_path = GetFullPath(_gameObject);
				if (_path.Contains(p_sourcePath)) {
					_path = _path.Replace(p_sourcePath, p_targetPath);
					GameObject _newObj = GameObject.Find(_path);
					if (_newObj) {
#if log
					log += "    FixField " + p_objName + " (GameObject SetValue [" + _path + "]) \n";
#endif
						//					FixObjMemberSelfReference (_value, p_targetPath, p_sourcePath);
						return _newObj;
					}
				}
#if log
			log += "    FixField " + p_objName + " (GameObject Not Self [" + _path + "]) \n";
#endif
				return null;
			}

			Object _object = _value as Object;
			if (_object != null) {
#if log
			log += "    FixField " + p_objName + " (Not Need [" + _typeName + "]) \n";
#endif
				return null;
			}

			IList _list = _value as IList;
			if (_list != null) {
#if log
			log += "    FixField " + p_objName + " (Check List [" + _typeName + "]) \n";
#endif
				int len = _list.Count;
				for (int f = 0; f < len; f++) {
					object _subValue = _list[f];
					_subValue = FixObjSelfReference(p_objName + "[" + f + "]", _subValue, p_targetPath, p_sourcePath);
					if (_subValue != null) {
						_list[f] = _subValue;
					}
				}
				return _list;
			}

#if log
		log += "    FixField " + p_objName + " (Not Need [" + _typeName + "]) \n";
#endif
			FixObjMemberSelfReference(_value, p_targetPath, p_sourcePath);
			return null;
		}
		#endregion

		#region "Shader"
		static Dictionary<string, Shader> shaderDict = new Dictionary<string, Shader>();

		public static void ReloadShader(this GameObject p_obj) {
#if log
		string _log = "";
#endif

			if (p_obj == null) {
				Debug.LogError("ReloadShader Failed : No GameObject");
				return;
			}

			try {
				Renderer[] _renders = p_obj.GetComponentsInChildren<Renderer>(true);
				Material[] _materials;
				int f, f2;
				int len = _renders.Length;

				if (len <= 0) {
					Debug.LogError("No Material In GameObject : " + p_obj.name);
					return;
				}

				int len2;
				Shader _shader;
#if log
			_log += "Reload Shader : " + p_obj.name;
#endif
				for (f = 0; f < len; f++) {
					_materials = _renders[f].sharedMaterials;

					len2 = _materials.Length;
					for (f2 = 0; f2 < len2; f2++) {
						if (_materials[f2].shader == null) {
#if log
						_log += "\n  Set Shader " + _renders[f].gameObject.name + " material " + f2.ToString() +
							StringExtend.AddRichColor(" Failed",Color.green) + "\n     Shader is Null";
#endif
						} else {
							string _name = _materials[f2].shader.name;

#if log
						_log += "\n  Set Shader [" + _name + "] to " + _renders[f].gameObject.name + " material " + f2.ToString();
#endif

							if (!shaderDict.TryGetValue(_name, out _shader)) {
								_shader = Shader.Find(_name);
								if (_shader == null) {
#if log
								_log += StringExtend.AddRichColor(" Failed",Color.green) + "\n     Can't Find Shader : [" + _name + "]";
#endif
									continue;
								}
							}

							_materials[f2].shader = _shader;
#if log
						_log += StringExtend.AddRichColor(" OK",Color.green);
#endif
						}
					}
				}
#if log
			Debug.Log(_log);
#endif
			} catch (System.Exception e) {
#if log
			_log += StringExtend.AddRichColor("\nException : " + e.Message,Color.red);
			Debug.LogError(_log);
#endif
				Debug.LogException(e);
			}
		}
		#endregion
	}
}