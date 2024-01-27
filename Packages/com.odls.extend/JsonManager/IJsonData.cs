using UnityEngine;
using System.Collections;

public interface IJsonData {
	void SetData (JSONObject p_json);
	JSONObject GetJson();
}
public interface IDictJsonData<KeyT> : IJsonData {
	KeyT GetKey();
}