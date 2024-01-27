using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace OdlsExtend {
	internal static class ArrayExtend<T> {
		public static readonly List<T> tempList = new List<T>();
	}
	public static class ArrayExtend {
		#region "ToString"
		static public string Array3DToString<T>(this T[][][] p_arrayArray, string p_splitString = ",", string p_startString = "[", string p_endString = "]", string p_1DsplitString = "") {
			if (p_1DsplitString == "") {
				p_1DsplitString = p_splitString;
			}
			StringBuilder _str = new StringBuilder(p_startString);
			int f;
			int len = p_arrayArray.Length;
			for (f = 0; f < len; f++) {
				_str.Append(((f == 0) ? "" : p_splitString) + Array2DToString<T>(p_arrayArray[f], p_splitString, p_startString, p_endString, p_1DsplitString));
			}
			_str.Append(p_endString);
			return _str.ToString();
		}
		static public string Array2DToString<T>(this T[][] p_arrayArray, string p_splitString = ",", string p_startString = "[", string p_endString = "]", string p_1DsplitString = "") {
			if (p_1DsplitString == "") {
				p_1DsplitString = p_splitString;
			}
			StringBuilder _str = new StringBuilder(p_startString);
			int f;
			int len = p_arrayArray.Length;
			for (f = 0; f < len; f++) {
				_str.Append(((f == 0) ? "" : p_splitString) + ArrayToString(p_arrayArray[f], p_1DsplitString, p_startString, p_endString));
			}
			_str.Append(p_endString);
			return _str.ToString();
		}
		static public string ArrayToString<T>(this T[] p_array, int p_start = 0, int p_length = -1) {
			return ArrayToString(p_array, ",", "[", "]", p_start, p_length);
		}
		static public string ArrayToString<T>(this T[] p_array, string p_splitString, int p_start = 0, int p_length = -1) {
			return ArrayToString(p_array, p_splitString, "[", "]", p_start, p_length);
		}
		static public string ArrayToString<T>(this T[] p_array, string p_splitString, string p_startString, string p_endString, int p_start = 0, int p_length = -1) {
			StringBuilder _str = new StringBuilder();
			int _end = (
				(p_length < 0) ?
				p_array.Length :
				Mathf.Min(p_start+p_length, p_array.Length)
			);

			for (int f = p_start; f < _end; f++) {
				_str.Append(((f == p_start) ? p_startString : p_splitString) + p_array[f].ToString());
			}
			_str.Append(p_endString);
			return _str.ToString();
		}
		static public string ArrayToFormatString<T>(this T[] p_array, string p_formatString, int p_start = 0, int p_length = -1) {
			return ArrayToFormatString(p_array, p_formatString, ",", "[", "]", p_start, p_length);
		}
		static public string ArrayToFormatString<T>(this T[] p_array, string p_formatString, string p_splitString, int p_start = 0, int p_length = -1) {
			return ArrayToFormatString(p_array, p_formatString, p_splitString, "[", "]", p_start, p_length);
		}
		static public string ArrayToFormatString<T>(this T[] p_array, string p_formatString, string p_splitString, string p_startString, string p_endString, int p_start = 0, int p_length = -1) {
			StringBuilder _str = new StringBuilder();
			int _end = (
				(p_length < 0) ?
				p_array.Length :
				Mathf.Min(p_start + p_length, p_array.Length)
			);

			for (int f = p_start; f < _end; f++) {
				_str.Append(
					((f == p_start) ? p_startString : p_splitString) +
					string.Format(p_formatString, p_array[f])
				);
			}
			_str.Append(p_endString);
			return _str.ToString();
		}
		static public string CollectionToFormatString<T>(this IEnumerable<T> p_collection, string p_splitString = ",", string p_formatString = "") {
			StringBuilder _str = new StringBuilder("[");

			bool _isFirst = true;
			foreach (T _obj in p_collection) {
				_str.Append(
					(_isFirst ? "" : p_splitString) +
					((p_formatString == "") ? _obj.ToString() : string.Format(p_formatString, _obj))
				);
				_isFirst = false;
			}

			_str.Append("]");
			return _str.ToString();
		}
		static public string CollectionToString<T>(this IEnumerable<T> p_collection, string p_splitString = ",", string p_startString = "[", string p_endString = "]") {
			StringBuilder _str = new StringBuilder(p_startString);

			bool _isFirst = true;
			foreach (T _obj in p_collection) {
				_str.Append((_isFirst ? "" : p_splitString) + _obj.ToString());
				_isFirst = false;
			}

			_str.Append(p_endString);
			return _str.ToString();
		}
		#endregion

		#region "Convert"

		static public T[] CreateArray<T>(this IEnumerable<T> p_input) {
			var _temp = ArrayExtend<T>.tempList;
			_temp.Clear();
			_temp.AddRange(p_input);

			var _array = _temp.ToArray();
			_temp.Clear();
			return _array;
		}
		static public void CopyToArray<T>(this IEnumerable<T> p_input, T[] p_target) {
			int _index = 0;
			foreach (var _value in p_input) {
				p_target[_index] = _value;
				_index++;
			}
		}

		static public List<T> ToList<T>(this T[][] p_input) {
			List<T> _list = new List<T>();
			foreach (T[] _items in p_input) {
				foreach (T _item in _items) {
					_list.Add(_item);
				}
			}
			return _list;
		}

		static public T[] To1D<T>(this List<List<T>> p_input) {
			int _len = 0;
			for (int i = 0; i < p_input.Count; i++) {
				_len += p_input[i].Count;
			}
			T[] _array = new T[_len];
			CopyTo1D<T>(p_input, _array);
			return _array;
		}
		static public void CopyTo1D<T>(this List<List<T>> p_input, T[] p_target) {
			int _index = 0;
			for (int i = 0; i < p_input.Count; i++) {
				for (int f = 0; f < p_input[i].Count; f++) {
					p_target[_index + f] = p_input[i][f];
				}
				_index += p_input[i].Count;
			}
		}

		static public T[] To1D<T>(this T[][] p_input) {
			int _len = 0;
			for (int i = 0; i < p_input.Length; i++) {
				_len += p_input[i].Length;
			}
			T[] _array = new T[_len];
			CopyTo1D<T>(p_input, _array);
			return _array;
		}
		static public void CopyTo1D<T>(this T[][] p_input, T[] p_target) {
			int _index = 0;
			for (int i = 0; i < p_input.Length; i++) {
				for (int f = 0; f < p_input[i].Length; f++) {
					p_target[_index + f] = p_input[i][f];
				}
				_index += p_input[i].Length;
			}
		}
		static public T[][] To2D<T>(this T[] p_input, int p_height) {
			int _len = p_input.Length;
			int _maxX = Mathf.CeilToInt((float)_len / (float)p_height);
			T[][] _output = new T[_maxX][];
			for (int x = 0; x < _maxX; x++) {
				int _offset = x * p_height;
				int _maxY = Mathf.Min(_len - _offset, p_height);
				_output[x] = new T[_maxY];
				for (int y = 0; y < _maxY; y++) {
					_output[x][y] = p_input[_offset + y];
				}
			}
			return _output;
		}
		#endregion

		#region "All Do"
		static public void SetAllValue<T>(this T[] p_array, T p_value) {
			int len = p_array.Length;

			for (int f = 0; f < len; f++) {
				p_array[f] = p_value;
			}
		}
		static public void SetAllValue<T>(this IList<T> p_list, T p_value, int p_size = -1) {
			int len = p_list.Count;

			if (p_size >= 0) {
				if (p_size < len) {
					for (int f = len - 1; f >= p_size; f--) {
						p_list.RemoveAt(f);
					}
				}

				for (int f = 0; f < len; f++) {
					p_list[f] = p_value;
				}

				if (p_size > len) {
					for (int f = len; f < p_size; f++) {
						p_list.Add(p_value);
					}
				}
			} else {
				for (int f = 0; f < len; f++) {
					p_list[f] = p_value;
				}
			}
		}
		#endregion
		public static void Shuffle<T>(this IList<T> p_list) {
			int _n = p_list.Count;
			while (_n > 1) {
				_n--;
				int _targetIndex = Random.Range(0, _n + 1);
				T value = p_list[_targetIndex];
				p_list[_targetIndex] = p_list[_n];
				p_list[_n] = value;
			}
		}
		public static int Compare<T>(this T[] p_array1, T[] p_array2) where T : System.IComparable {
			int _len1 = p_array1.Length;
			int _len2 = p_array2.Length;
			int len = Mathf.Min(_len1, _len2);
			int _compare = 0;
			for (int f = 0; f < len; f++) {
				_compare = p_array1[f].CompareTo(p_array2[f]);
				if (_compare != 0) {
					return _compare;
				}
			}
			return _len1.CompareTo(_len2);
		}
	}


}
