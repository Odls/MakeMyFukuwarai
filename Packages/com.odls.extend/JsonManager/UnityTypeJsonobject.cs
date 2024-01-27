using System.Collections.Generic;
using UnityEngine;

public static class UnityTypeJsonobject {
#region "AddField"
	public static void AddField(this JSONObject p_json, string name, Vector2 p_value) {
		JSONObject _valueJson = Create(p_value);
		p_json.AddField(name, _valueJson);
	}

	public static void AddField(this JSONObject p_json, string name, Vector3 p_value) {
		JSONObject _valueJson = Create(p_value);
		p_json.AddField(name, _valueJson);
	}

	public static void AddField(this JSONObject p_json, string name, Vector4 p_value) {
		JSONObject _valueJson = Create(p_value);
		p_json.AddField(name, _valueJson);
	}

	public static void AddField(this JSONObject p_json, string name, Quaternion p_value) {
		JSONObject _valueJson = Create(p_value);
		p_json.AddField(name, _valueJson);
	}

	public static void AddField(this JSONObject p_json, string name, Matrix4x4 p_value) {
		JSONObject _valueJson = Create(p_value);
		p_json.AddField(name, _valueJson);
	}

	public static void AddField(this JSONObject p_json, string name, Rect p_value, string p_formate=null) {
		JSONObject _valueJson = Create(p_value);
		p_json.AddField(name, _valueJson);
	}

	public static void AddField(this JSONObject p_json, string name, Bounds p_value) {
		JSONObject _valueJson = Create(p_value);
		p_json.AddField(name, _valueJson);
	}

	public static void AddField(this JSONObject p_json, string name, Color p_value) {
		JSONObject _valueJson = Create(p_value);
		p_json.AddField(name, _valueJson);
	}
#endregion

#region "Create"
	static JSONObject Create<T>(ICollection<T> p_values, System.Func<T, JSONObject> p_create) {
		JSONObject p_json = JSONObject.Create();
		foreach (T p_value in p_values) {
			p_json.Add(p_create(p_value));
		}
		return p_json;
	}
	public static JSONObject CreateAsString<T>(T p_obj) {
		return JSONObject.Create(p_obj.ToString());
	}
	public static JSONObject CreateAsString<T>(ICollection<T> p_objs) => Create(p_objs, CreateAsString);
	public static JSONObject Create(Vector2 p_vector2) {
		JSONObject p_json = JSONObject.Create();
		p_json.SetField("x", p_vector2.x);
		p_json.SetField("y", p_vector2.y);
		return p_json;
	}
	public static JSONObject Create(ICollection<Vector2> p_vector2s) => Create(p_vector2s, Create);
	public static JSONObject Create(Vector3 p_vector3) {
		JSONObject p_json = JSONObject.Create();
		p_json.SetField("x", p_vector3.x);
		p_json.SetField("y", p_vector3.y);
		p_json.SetField("z", p_vector3.z);
		return p_json;
	}
	public static JSONObject Create(ICollection<Vector3> p_vector3s) => Create(p_vector3s, Create);
	public static JSONObject Create(Vector4 p_vector4) {
		JSONObject p_json = JSONObject.Create();
		p_json.SetField("x", p_vector4.x);
		p_json.SetField("y", p_vector4.y);
		p_json.SetField("z", p_vector4.z);
		p_json.SetField("w", p_vector4.w);
		return p_json;
	}
	public static JSONObject Create(ICollection<Vector4> p_vector4s) => Create(p_vector4s, Create);
	public static JSONObject Create(Quaternion p_quaternion) {
		JSONObject p_json = JSONObject.Create();
		p_json.SetField("x", p_quaternion.x);
		p_json.SetField("y", p_quaternion.y);
		p_json.SetField("z", p_quaternion.z);
		p_json.SetField("w", p_quaternion.w);
		return p_json;
	}
	public static JSONObject Create(ICollection<Quaternion> p_quaternions) => Create(p_quaternions, Create);
	public static JSONObject Create(Matrix4x4 p_matrix4x4) {
		JSONObject p_json = JSONObject.Create();
		p_json.SetField("m00", p_matrix4x4.m00);
		p_json.SetField("m01", p_matrix4x4.m01);
		p_json.SetField("m02", p_matrix4x4.m02);
		p_json.SetField("m03", p_matrix4x4.m03);
		p_json.SetField("m10", p_matrix4x4.m10);
		p_json.SetField("m11", p_matrix4x4.m11);
		p_json.SetField("m12", p_matrix4x4.m12);
		p_json.SetField("m13", p_matrix4x4.m13);
		p_json.SetField("m20", p_matrix4x4.m20);
		p_json.SetField("m21", p_matrix4x4.m21);
		p_json.SetField("m22", p_matrix4x4.m22);
		p_json.SetField("m23", p_matrix4x4.m23);
		p_json.SetField("m30", p_matrix4x4.m30);
		p_json.SetField("m31", p_matrix4x4.m31);
		p_json.SetField("m32", p_matrix4x4.m32);
		p_json.SetField("m33", p_matrix4x4.m33);
		return p_json;
	}
	public static JSONObject Create(ICollection<Matrix4x4> p_matrix4x4s) => Create(p_matrix4x4s, Create);
	public static JSONObject Create(Rect p_rect) {
		JSONObject p_json = JSONObject.Create();
		p_json.SetField("x", p_rect.x);
		p_json.SetField("y", p_rect.y);
		p_json.SetField("width", p_rect.width);
		p_json.SetField("height", p_rect.height);
		return p_json;
	}
	public static JSONObject Create(ICollection<Rect> p_rects) => Create(p_rects, Create);
	public static JSONObject Create(Bounds p_bounds) {
		JSONObject p_json = JSONObject.Create();
		p_json.SetField("center", Create(p_bounds.center));
		p_json.SetField("size", Create(p_bounds.size));
		return p_json;
	}
	public static JSONObject Create(ICollection<Bounds> p_boundses) => Create(p_boundses, Create);
	public static JSONObject Create(Color p_color) {
		JSONObject p_json = JSONObject.Create();
		p_json.SetField("r", p_color.r);
		p_json.SetField("g", p_color.g);
		p_json.SetField("b", p_color.b);
		p_json.SetField("a", p_color.a);
		return p_json;
	}
	public static JSONObject Create(ICollection<Color> p_colors) => Create(p_colors, Create);

#endregion
}
