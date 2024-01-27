#if UNITY_EDITOR
//#define PERFTEST        //For testing performance of parse/stringify.  Turn on editor profiling to see how we're doing

using UnityEngine;
using UnityEditor;

public class JSONChecker : EditorWindow {
	string JSON = @"{
	""TestObject"": {
		""SomeText"": ""Blah"",
		""SomeObject"": {
			""SomeNumber"": 42,
			""SomeFloat"": 13.37,
			""SomeBool"": true,
			""SomeNull"": null
		},
		
		""SomeEmptyObject"": { },
		""SomeEmptyArray"": [ ],
		""EmbeddedObject"": ""{\""field\"":\""Value with \\\""escaped quotes\\\""\""}""
	}
}";	  //dat string literal...
	string URL = "";
	JSONObject j;
	[MenuItem("Window/JSONChecker")]
	static void Init() {
		GetWindow(typeof(JSONChecker));
	}
	void OnGUI() {
		JSON = EditorGUILayout.TextArea(JSON);
		GUI.enabled = !string.IsNullOrEmpty(JSON);
		if(GUILayout.Button("Check JSON")) {
#if PERFTEST
            Profiler.BeginSample("JSONParse");
			j = JSONObject.Create(JSON);
            Profiler.EndSample();
            Profiler.BeginSample("JSONStringify");
            j.ToString(true);
            Profiler.EndSample();
#else
			j = JSONObject.Create(JSON);
#endif
#if ShowDebug
			Debug.Log(j.ToString(true));
#endif
		}
		EditorGUILayout.Separator();
		URL = EditorGUILayout.TextField("URL", URL);
		if (GUILayout.Button("Get JSON")) {
#if ShowDebug
			Debug.Log(URL);
#endif
			WWW test = new WWW(URL);
			while (!test.isDone) ;
			if (!string.IsNullOrEmpty(test.error)) {
#if ShowDebug
				Debug.Log(test.error);
#endif
			} else {
#if ShowDebug
				Debug.Log(test.text);
#endif
				j = new JSONObject(test.text);
#if ShowDebug
				Debug.Log(j.ToString(true));
#endif
			}
		}
		if(j) {
#if ShowDebug
			//Debug.Log(System.GC.GetTotalMemory(false) + "");
#endif
			if(j.type == JSONObject.Type.NULL)
				EditorGUILayout.TextArea("JSON fail:\n" + j.ToString(true));
			else
				EditorGUILayout.TextArea("JSON success:\n" + j.ToString(true));

		}
	}
}
#endif