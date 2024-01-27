#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using OdlsExtend;

public class JsonDataEdit : EditorWindow {
	static string[][] typeDatas = new string[][]{
		//			$Type$,			$DefaultValue$,						$GetFun$,						$UiType$,		$UiGetFun$,									$JsonFun$,
		new string[]{"?",					"p_json.TryGetObjectToData<?>",			"",								"",				"",                                         "JSONObject.CreateFromData<?>($Variable$)"},
		new string[]{"?[]",					"p_json.TryGetListToData<?>",			"",								"",				"",                                         "JSONObject.CreateFromDatas<?>($Variable$)"},
		new string[]{"int",					"(int)p_json.TryGetNumber",				"0",							"Text",			".text = p_data.$Variable$.ToString ()",    "JSONObject.Create($Variable$)"},
		new string[]{"int[]",				"p_json.TryGetList<int>",				"",								"",				"",                                         "JSONObject.Create<int>($Variable$)"},
		new string[]{"long",				"(long)p_json.TryGetNumber",			"0",							"Text",			".text = p_data.$Variable$.ToString ()",    "JSONObject.Create($Variable$)"},
		new string[]{"long[]",				"p_json.TryGetList<long>",				"",								"",				"",                                         "JSONObject.Create<long>($Variable$)"},
		new string[]{"float",				"(float)p_json.TryGetNumber",			"0",							"Text",			".text = p_data.$Variable$.ToString ()",    "JSONObject.Create($Variable$)"},
		new string[]{"float[]",				"p_json.TryGetList<float>",				"",								"",				"",                                         "JSONObject.Create<float>($Variable$)"},
		new string[]{"double",				"(double)p_json.TryGetNumber",			"0",							"Text",			".text = p_data.$Variable$.ToString ()",    "JSONObject.Create($Variable$)"},
		new string[]{"double[]",			"p_json.TryGetList<double>",			"",								"",				"",                                         "JSONObject.Create<double>($Variable$)"},
		new string[]{"bool",				"(bool)p_json.TryGetBool",				"false",						"Toggle",		".isOn = p_data.$Variable$",                "JSONObject.Create($Variable$)"},
		new string[]{"bool[]",				"p_json.TryGetList<bool>",				"",								"",				"",                                         "JSONObject.Create<bool>($Variable$)"},
		new string[]{"System.DateTime",		"p_json.TryGetDateTime",				"default(System.DateTime)",		"Text",			".text = p_data.$Variable$.ToString ()",    "JSONObject.Create($Variable$)"},
		new string[]{"System.DateTime[]",	"p_json.TryGetList<System.DateTime>",	"",								"",				"",                                         "JSONObject.Create<System.DateTime>($Variable$)"},
		new string[]{"string",				"p_json.TryGetString",					"\"\"",							"Text",			".text = p_data.$Variable$",                "JSONObject.CreateStringObject($Variable$)"},
		new string[]{"string[]",			"p_json.TryGetList<string>",			"",								"",				"",                                         "JSONObject.Create<string>($Variable$)"},
	};
	Dictionary<string,string[]> typeDataDictionary = new Dictionary<string,string[]>();

	static string dataScriptTemplate =
@"using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class $DataName$ : IJsonData{
$DeclareVariable$
	
	public $DataName$()	{}
	public $DataName$($DataName$ p_data){	SetData(p_data);	}
	public $DataName$(JSONObject p_json){	SetData(p_json);	}

	public object GetKey (){
		return $KeyVariable$;
	}

	public void SetData($DataName$ p_data){
$SetVariableByData$
	}
	public void SetData(JSONObject p_json){
$SetVariableByJson$
	}

	public JSONObject GetJson(){
		JSONObject _json = new JSONObject();
$GetJson$
		return _json;
	}
}";

	static string scrollEnementTemplate =
@"using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ScrollEnement$DataName$ : ScrollEnementBase<ScrollEnement$DataName$,$DataName$> {
$DeclareUiVariable$
	override public void OnSetData($DataName$ p_data){
$SetUiVariable$
	}
	override public void OnRemove(){}
	override public void OnAdd(){}
//	override public bool Filter(string p_type,int p_number = -1){
//		return false;
//	}
//	override public bool IsCountInNumber(){
//		return false;
//	}
}";
	static string scrollViewTemplate =
@"using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScrollView$DataName$ : ScrollViewBase<ScrollEnement$DataName$,$DataName$> {
}";

	static string declareVariableTemplate =
@"	/// <summary>$Annotation$</summary>
	public $Type$ $Variable$;
";
	static string setVariableByJsonTemplate =
@"		$Variable$ = $GetFun$(""$Variable$""$DefaultValue$);
";
	static string setVariableByJsonArrayTemplate =
@"		$Variable$ = $GetFun$(""$Variable$"").ToArray();
";
	static string setVariableByDataTemplate =
@"		$Variable$ = p_data.$Variable$;
";
	static string setVariableByDataArrayTemplate =
@"		$Variable$ = ($Type$)p_data.$Variable$.Clone();
";
	static string getJsonTemplate =
@"		_json.AddField(""$Variable$"", $JsonFun$);
";
	static string declareUiVariableTemplate =
@"	/// <summary>$Annotation$</summary>
	[NullAlarm]public $UiType$ $Variable$$Type$;
";

	static string setUiVariableTemplate =
@"		$Variable$$Type$$UiGetFun$;
";
	string setArg (string p_text,bool p_highlight,params string[] args) {
		Dictionary<string,string> _argDict = new Dictionary<string, string>();
		int f;
		int len = args.Length;
		for(f=0; f<len; f+=2){
			_argDict.Add(args[f],args[f + 1]);
		}

		string[] _texts =  p_text.Split("$"[0]);
		string _arg;
		StringBuilder _output = new StringBuilder(_texts[0]);

		len = _texts.Length;
		for(f=1; f<len; f+=2){
			if(_argDict.TryGetValue(_texts[f], out _arg)){
				if(p_highlight){
					_output.Append(StringExtend.AddRichColor(_arg,Color.yellow));
				}else{
					_output.Append(_arg);
				}
			}else{
				if(p_highlight){
					_output.Append(StringExtend.AddRichColor("$" + _texts[f] + "$",Color.yellow));
				}else{
					_output.Append("$" + _texts[f] + "$");
				}
			}
			_output.Append(_texts[f + 1]);
		}

		return _output.ToString();
	}

	static JsonDataEdit window;
	[MenuItem ("Window/JsonDataEdit")]
	static void Init () {
		window = (JsonDataEdit)EditorWindow.GetWindow (typeof (JsonDataEdit));

		window.InitDict ();
		window.Show();
	}

	void InitDict () {
		typeDataDictionary.Clear();
		foreach(string[] _data in typeDatas){
			typeDataDictionary.Add(_data[0],_data);
		}
		RefrashScript ();
	}
	GUIStyle textAreaStyle = null;
	GUIStyle boxStyle  = null;
	string errorLog = "";
	string dataScript = "";
	string scrollEnementScript = "";
	string scrollViewScript = "";
	string text = "";
	string dataName = "";
	Vector2 textScroll;
	Vector2 dataScriptScroll;
	Vector2 scrollScriptScroll;
	void OnGUI () {
		if(textAreaStyle == null){
			textAreaStyle = new GUIStyle(GUI.skin.textArea);
			textAreaStyle.richText = true;
		}
		if(boxStyle == null){
			boxStyle = new GUIStyle(GUI.skin.box);
			boxStyle.normal.textColor = Color.white;
		}

		GUI.enabled = true;
		GUILayout.BeginHorizontal();
			GUILayout.BeginVertical();
				EditorGUI.BeginChangeCheck();
				dataName = EditorGUILayout.TextField("dataName",dataName);

				textScroll = GUILayout.BeginScrollView(textScroll);
					text = EditorGUILayout.TextArea(text);
				GUILayout.EndScrollView();

				if(EditorGUI.EndChangeCheck()){
					RefrashScript ();
				}

				if(errorLog != ""){
					GUILayout.Box(errorLog, boxStyle);
				}else{
					if(GUILayout.Button("Save Data Script")){
						SaveScript(dataScript,			Application.dataPath + @"\Scripts\Sys\" + dataName + ".cs");
						AssetDatabase.Refresh();
					}
					if(GUILayout.Button("Save Scroll Script")){
						SaveScript(scrollEnementScript,	Application.dataPath + @"\Scripts\UI\ScrollEnement\ScrollEnement" + dataName + ".cs");
						SaveScript(scrollViewScript,	Application.dataPath + @"\Scripts\UI\ScrollEnement\ScrollView" + dataName + ".cs");
						AssetDatabase.Refresh();
					}
					if(GUILayout.Button("Save Data And Scroll Script")){
						SaveScript(dataScript,			Application.dataPath + @"\Scripts\Sys\" + dataName + ".cs");
						SaveScript(scrollEnementScript,	Application.dataPath + @"\Scripts\UI\ScrollEnement\ScrollEnement" + dataName + ".cs");
						SaveScript(scrollViewScript,	Application.dataPath + @"\Scripts\UI\ScrollEnement\ScrollView" + dataName + ".cs");
						AssetDatabase.Refresh();
					}
				}
			GUILayout.EndVertical();

			GUILayout.BeginVertical();
				dataScriptScroll = GUILayout.BeginScrollView(dataScriptScroll);
					GUILayout.BeginHorizontal();
						GUILayout.Box(dataName + ".cs", boxStyle);

						if(GUILayout.Button("Copy")){
							CopyScript(dataScript);
						}
					GUILayout.EndHorizontal();

					GUI.enabled = false;
					EditorGUILayout.TextArea(dataScript,textAreaStyle);
					GUI.enabled = true;
				GUILayout.EndScrollView();

			GUILayout.EndVertical();

			GUILayout.BeginVertical();
				scrollScriptScroll = GUILayout.BeginScrollView(scrollScriptScroll);

					GUILayout.BeginHorizontal();
						GUILayout.Box("ScrollView" + dataName + ".cs", boxStyle);

						if(GUILayout.Button("Copy")){
							CopyScript(scrollViewScript);
						}
					GUILayout.EndHorizontal();

					GUI.enabled = false;
					EditorGUILayout.TextArea(scrollViewScript,textAreaStyle);
					GUI.enabled = true;

					GUILayout.BeginHorizontal();
						GUILayout.Box("ScrollEnement" + dataName + ".cs", boxStyle);

						if(GUILayout.Button("Copy")){
							CopyScript(scrollEnementScript);
						}
					GUILayout.EndHorizontal();

					GUI.enabled = false;
					EditorGUILayout.TextArea(scrollEnementScript,textAreaStyle);
					GUI.enabled = true;
				GUILayout.EndScrollView();

			GUILayout.EndVertical();

		GUILayout.EndHorizontal();

	}
	void CopyScript (string p_script) {
		p_script = p_script.Replace("<color=#FFEB04FF>","").Replace("</color>","");
		EditorGUIUtility.systemCopyBuffer = p_script;
	}
	void SaveScript (string p_script, string p_path) {
		p_script = p_script.Replace("<color=#FFEB04FF>","").Replace("</color>","");
		FileExtend.PrepareDirectory(p_path);
		File.WriteAllText(p_path,p_script,Encoding.UTF8);
	}
	void RefrashScript () {
		if(typeDataDictionary.Count <= 0){
			InitDict ();
		}

		string _declareVariable = "";
		string _setVariableByJson = "";
		string _setVariableByData = "";
		string _getJson = "";
		string _keyVariable = "";
		string _declareUiVariable = "";
		string _seteUiVariable = "";

		if(text == "" || dataName == ""){
			dataScript = "";
			scrollViewScript = "";
			scrollEnementScript = "";
			errorLog = "Input Data";
			return;
		}

		string[] _variableStrs = text.Replace("\r","").Split("\n"[0]);
		_variableStrs.Clone();
		int len = _variableStrs.Length;
		int f;

		errorLog = "";
		for(f=0; f<len; f++){
			string _variableStr = _variableStrs[f];
			string[] _argStrs = _variableStr.Split("\t"[0]);

			if(_argStrs.Length != 3){
				errorLog = "Refrash Script Error At Lint " + f + " : \"" +  _variableStr + "\"";
				continue;
			}

			string _typeStr = _argStrs[0];
			string _nameStr = _argStrs[1];
			string _annotationStr = _argStrs[2];
			bool _isArray = _typeStr.EndsWith("[]");

			if(_typeStr == "DateTime") {
				_typeStr = "System.DateTime";
			}

			string[] _typeData;
			if(!typeDataDictionary.TryGetValue(_typeStr, out _typeData)){
				if(_isArray){
					string _objectTypeStr = _typeStr.Substring(0,_typeStr.Length - 2);
					_typeData = (string[])typeDatas[1].Clone();
					_typeData[0] = _typeData[0].Replace("?",_objectTypeStr);
					_typeData[1] = _typeData[1].Replace("?",_objectTypeStr);
					_typeData[5] = _typeData[5].Replace("?",_objectTypeStr);
				}else{
					_typeData = (string[])typeDatas[0].Clone();
					_typeData[0] = _typeData[0].Replace("?",_typeStr);
					_typeData[1] = _typeData[1].Replace("?",_typeStr);
					_typeData[5] = _typeData[5].Replace("?",_typeStr);
				}
			}

			string _getFunStr = _typeData[1];
			string _defaultValueAtDeclareStr = ((_typeData[2]=="")?"":(" = " + _typeData[2]));
			string _defaultValueAtSetStr = ((_typeData[2]=="")?"":(", " + _typeData[2]));
			string _uiTypeStr = _typeData[3];
			string _uiGetFunStr = setArg(_typeData[4],true,"Variable", _nameStr);
			string _JsonFunStr = setArg(_typeData[5], true, "Variable", _nameStr);

			string _nowDeclareVariable = setArg(declareVariableTemplate,true,
			                                    "Type",_typeStr,
			                                    "Variable",_nameStr,
			                                    "Annotation",_annotationStr,
			                                    "DefaultValue",_defaultValueAtDeclareStr);


			string _nowSetVariableByJson;
			if(_isArray){
				_nowSetVariableByJson = setArg(setVariableByJsonArrayTemplate,false,
				                               "GetFun",_getFunStr);
			}else{
				_nowSetVariableByJson = setArg(setVariableByJsonTemplate,false,
				                               "GetFun",_getFunStr);
			}

			_nowSetVariableByJson = setArg(_nowSetVariableByJson,true,
			                               "Variable",_nameStr,
			                               "DefaultValue",_defaultValueAtSetStr);

			string _nowSetVariableByData = setArg((_isArray?setVariableByDataArrayTemplate:setVariableByDataTemplate),true,
			                                      "Variable",_nameStr,
			                                      "Type",_typeStr);

			string _nowGetJson = setArg(getJsonTemplate, true,
										"Variable", _nameStr,
										"Type", _typeStr,
										"JsonFun", _JsonFunStr);

			_declareVariable += _nowDeclareVariable;
			_setVariableByJson += _nowSetVariableByJson;
			_setVariableByData += _nowSetVariableByData;
			_getJson += _nowGetJson;

			if(f == 0){
				_keyVariable = _nameStr;
			}

			if(_typeStr.Length >= 1){
				_typeStr = _typeStr[0].ToString().ToUpper() + _typeStr.Substring(1);
			}

			if(_uiTypeStr != ""){
				string _nowDeclareUiVariable = setArg(declareUiVariableTemplate,true,
				                                      "Type",_typeStr,
				                                      "Variable",_nameStr,
				                                      "UiType",_uiTypeStr,
				                                      "Annotation",_annotationStr);

				string _nowSetUiVariable = setArg(setUiVariableTemplate,false,
				                                  "UiGetFun",_uiGetFunStr);
				_nowSetUiVariable = setArg(_nowSetUiVariable,true,
				                           "Type",_typeStr,
				                           "Variable",_nameStr);

				_declareUiVariable += _nowDeclareUiVariable;
				_seteUiVariable += _nowSetUiVariable;
			}
		}

		dataScript = setArg(dataScriptTemplate,false,
		                    "DeclareVariable", _declareVariable,
		                    "SetVariableByJson", _setVariableByJson,
							"SetVariableByData", _setVariableByData,
							"GetJson", _getJson);

		dataScript = setArg(dataScript,true,
		                    "DataName",dataName,
		                    "KeyVariable",_keyVariable);

		scrollEnementScript = setArg(scrollEnementTemplate,false,
		                             "DeclareUiVariable",_declareUiVariable,
		                             "SetUiVariable",_seteUiVariable);
		scrollEnementScript = setArg(scrollEnementScript,true,
		                             "DataName",dataName);

		scrollViewScript = setArg(scrollViewTemplate,true,
		                          "DataName",dataName);

	}
}
#endif