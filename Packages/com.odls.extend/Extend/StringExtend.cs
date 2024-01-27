using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace OdlsExtend {
	public static class StringExtend {
		static StringBuilder stringBuilder = new StringBuilder("");

		#region "Rich Text"
		public static string AddRichColor(this string p_str, Color p_color) {
			string _str = string.Format("<color=#{1:X}{2:X}{3:X}{4:X}>{0}</color>",
										p_str,
										((int)(p_color.r * 255)).ToString("X2"),
										((int)(p_color.g * 255)).ToString("X2"),
										((int)(p_color.b * 255)).ToString("X2"),
										((int)(p_color.a * 255)).ToString("X2"));
			return _str;
		}

		public static string AddRichSize(this string p_str, float p_size) {
			string _str = string.Format("<size={1}>{0}</size>",
										p_str,
										p_size);
			return _str;
		}

		public static string AddRichBold(this string p_str) {
			string _str = string.Format("<b>{0}</b>",
										p_str);
			return _str;
		}

		public static string AddRichItalic(this string p_str) {
			string _str = string.Format("<i>{0}</i>",
										p_str);
			return _str;
		}

		public static string SetColor(this string p_str, string p_target, Color p_color) {
			return p_str.Replace(p_target, AddRichColor(p_target, p_color));
		}

		public static string SetSize(this string p_str, string p_target, float p_size) {
			return p_str.Replace(p_target, AddRichSize(p_target, p_size));
		}

		public static string SetBold(this string p_str, string p_target) {
			return p_str.Replace(p_target, AddRichBold(p_target));
		}

		public static string SetItalic(this string p_str, string p_target) {
			return p_str.Replace(p_target, AddRichItalic(p_target));
		}
		#endregion

		#region "Unicode"
		// 改自: http://trufflepenne.blogspot.tw/2013/03/cunicode.html
		public static string ToUnicode(this string srcText) {
			string dst = "";
			char[] src = srcText.ToCharArray();
			for (int i = 0; i < src.Length; i++) {
				byte[] bytes = Encoding.Unicode.GetBytes(src[i].ToString());
				string str = @"\u" + bytes[1].ToString("X2") + bytes[0].ToString("X2");
				dst += str;
			}
			return dst;
		}
		private static string DecodeUTF8Code(this string p_code) {
			byte[] bytes = new byte[2];
			bytes[1] = byte.Parse(int.Parse(p_code.Substring(0, 2), System.Globalization.NumberStyles.HexNumber).ToString());
			bytes[0] = byte.Parse(int.Parse(p_code.Substring(2, 2), System.Globalization.NumberStyles.HexNumber).ToString());

			return Encoding.Unicode.GetString(bytes);
		}

		public static string DecodeUnicode(this string p_utf8str) {
			int i, _size, _len;
			string _dst, _str;

			string[] _strs = p_utf8str.Split('\\');
			_size = _strs.Length;
			_dst = _strs[0];

			for (i = 1; i < _size; i++) {
				_str = _strs[i];
				_len = _str.Length;

				if (_len == 0 || _str.Substring(0, 1) != "u") {
					_dst += '\\' + _str;
					continue;
				}

				if (_len < 5) {
					_dst += '\\' + _str;
					continue;
				}

				if (_len > 5) {
					_dst += DecodeUTF8Code(_str.Substring(1, 4));
					_dst += _str.Substring(5);
					continue;
				}

				_dst += DecodeUTF8Code(_str.Substring(1, 4));
			}

			//i don`t know why input string is "xxxx" ... so skip "
			_len = _dst.Length;
			if (_len > 1 && _dst[0] == '"' && _dst[_len - 1] == '"')
				_dst = _dst.Substring(1, _len - 2);

			return _dst;
		}

		public static byte[] ToAsciiByte(this string p_srcText) {
			return Encoding.ASCII.GetBytes(p_srcText);
		}
		public static int[] ToAscii(this string p_srcText) {
			byte[] _bytes = ToAsciiByte(p_srcText);
			int len = _bytes.Length;
			int[] _asciis = new int[len];
			for (int f = 0; f < len; f++) {
				_asciis[f] = (int)_bytes[f];
			}
			return _asciis;
		}
		public static string ToAsciiString(this string p_srcText) {
			byte[] _bytes = ToAsciiByte(p_srcText);
			return ArrayExtend.ArrayToString<byte>(_bytes);
		}
		#endregion

		#region "String"
		static char[] charBuffer = new char[256];
		static int charBufferSize = 256;
		static char[] GetCharBuffers(int p_length) {
			if (p_length > charBufferSize) {

				charBufferSize *= 2;
				while (p_length > charBufferSize) { charBufferSize *= 2; }

				charBuffer = new char[charBufferSize];
			}

			return charBuffer;
		}
		static char[] CopyToCharBuffers(this string p_str) {
			int _length = p_str.Length;
			char[] _charBuffer = GetCharBuffers(_length);
			for (int f = 0; f < _length; f++) {
				_charBuffer[f] = p_str[f];
			}
			return _charBuffer;
		}
		static public string SetChar(this string p_str, int p_index, char p_char) {
			stringBuilder.Length = 0;
			stringBuilder.Append(p_str);
			stringBuilder[p_index] = p_char;
			return stringBuilder.ToString();
		}
		static public bool MatchFilter(this string p_str, string p_filter, bool p_caseSenSitive = false) {
			if (p_filter == "") {
				return true;
			}
			return MatchFilter(p_str, p_filter.Split(' '), p_caseSenSitive);
		}
		static public bool MatchFilter(this string p_str, string[] p_filters, bool p_caseSenSitive = false) {
			if (p_filters == null) {
				return true;
			}

			foreach (string _filter in p_filters) {
				if (p_caseSenSitive) {
					if (!p_str.Contains(_filter)) {
						return false;
					}
				} else {
					if (!(p_str.IndexOf(_filter, System.StringComparison.OrdinalIgnoreCase) >= 0)) {
						return false;
					}
				}
			}

			return true;
		}
		static public string ReplaceNull(this string p_str) {
			return ((p_str == null) ? "" : p_str);
		}
		static public string GetStringWhenNotEmpty(this string p_str, string p_original) {
			if (string.IsNullOrEmpty(p_str)) {
				return p_str;
			} else {
				return p_original;
			}
		}
		static public int CompareByNumberInString(this string p_srtA, string p_srtB) {
			Regex _regex = new Regex(@"\d+");

			Match _matchA = _regex.Match(p_srtA);
			Match _matchB = _regex.Match(p_srtB);
			int _lenA = p_srtA.Length;
			int _lenB = p_srtB.Length;
			int _indexA = 0;
			int _indexB = 0;
			int _targetIndexA = 0;
			int _targetIndexB = 0;
			bool _hasNumberA, _hasNumberB;
			int _compare = 0;
			Capture _captureA, _captureB;
			//int _count = 0;
			while (true) {
				_hasNumberA = _matchA.Success;
				if (_hasNumberA) {
					_captureA = _matchA.Captures[0];
					_targetIndexA = _captureA.Index;
				} else {
					_captureA = null;
					_targetIndexA = _lenA;
				}
				_hasNumberB = _matchB.Success;
				if (_hasNumberB) {
					_captureB = _matchB.Captures[0];
					_targetIndexB = _captureB.Index;
				} else {
					_captureB = null;
					_targetIndexB = _lenB;
				}

				_compare = p_srtA.Substring(_indexA, _targetIndexA - _indexA).CompareTo(p_srtB.Substring(_indexB, _targetIndexB - _indexB));
				if (_compare != 0) {
					//Debug.Log("compare : " + _compare + " " + p_srtA.Substring(_indexA, _targetIndexA - _indexA) + " & " + p_srtB.Substring(_indexB, _targetIndexB - _indexB));
					return _compare;
				} else {
					//Debug.Log("Same : " + p_srtA.Substring(_indexA, _targetIndexA - _indexA) + " & " + p_srtB.Substring(_indexB, _targetIndexB - _indexB));
				}

				_indexA = _targetIndexA;
				_indexB = _targetIndexB;

				if (_hasNumberA) {
					_targetIndexA = _indexA + _captureA.Length;
					if (!_hasNumberB) {
						return 1;
					}
				}
				if (_hasNumberB) {
					_targetIndexB = _indexB + _captureB.Length;
					if (!_hasNumberA) {
						return -1;
					}
				}

				if (!_hasNumberA && !_hasNumberB) {
					return 0;
				}

				_compare = int.Parse(p_srtA.Substring(_indexA, _targetIndexA - _indexA)).CompareTo(int.Parse(p_srtB.Substring(_indexB, _targetIndexB - _indexB)));

				if (_compare != 0) {
					//Debug.Log("compare : " + _compare + " " + p_srtA.Substring(_indexA, _targetIndexA - _indexA) + " & " + p_srtB.Substring(_indexB, _targetIndexB - _indexB));
					return _compare;
				} else {
					//Debug.Log("Same : " + p_srtA.Substring(_indexA, _targetIndexA - _indexA) + " & " + p_srtB.Substring(_indexB, _targetIndexB - _indexB));
				}

				_indexA = _targetIndexA;
				_indexB = _targetIndexB;

				if (_hasNumberA) {
					_matchA = _matchA.NextMatch();
				}
				if (_hasNumberB) {
					_matchB = _matchB.NextMatch();
				}

				//_count++;
				//if(_count > 20) {
				//	Debug.LogError("Run Out");
				//	return 0;
				//}
			}
		}
		static public char ToUpper(this char p_char) {
			if(p_char>='a' && p_char <= 'z') {
				return (char)(p_char - 32);
			} else {
				return p_char;
			}
		}
		static public char ToLower(this char p_char) {
			if (p_char >= 'A' && p_char <= 'Z') {
				return (char)(p_char + 32);
			} else {
				return p_char;
			}
		}
		/// <summary>
		/// 例: Only first character is upper case.
		/// </summary>
		/// <param name="p_str"></param>
		/// <param name="p_setLow">是否將非首字的字元設為小寫</param>
		static public string ToSentence(this string p_str, bool p_setLow = true) {
			var _charBuffer = p_str.CopyToCharBuffers();
			int _length = p_str.Length;

			_charBuffer[0] = _charBuffer[0].ToUpper();
			if (p_setLow) {
				for (int f = 1; f < _length; f++) {
					_charBuffer[f] = _charBuffer[f].ToLower();
				}
			}
			return new string(_charBuffer, 0, _length);
		}
		/// <summary>
		/// 將每個詞的首字設為大寫
		/// 例: Only First Character Of Each Word Is Upper Case.
		/// </summary>
		/// <param name="p_str">輸入字串</param>
		/// <param name="p_splitChar">分割字元</param>
		/// <param name="p_setLow">是否將非首字的字元設為小寫</param>
		static public string ToCapitalize(this string p_str, char p_splitChar = ' ', bool p_setLow=true) {
			var _charBuffer = p_str.CopyToCharBuffers();
			int _length = p_str.Length;

			bool _isCap = true;
			for (int f = 0; f < _length; f++) {
				if(_charBuffer[f] == p_splitChar) {
					_isCap = true;
				} else if (_isCap) {
					_charBuffer[f] = _charBuffer[f].ToUpper();
					_isCap = false;
				} else if(p_setLow) {
					_charBuffer[f] = _charBuffer[f].ToLower();
				}
			}

			return new string(_charBuffer, 0, _length);
		}

		static public string ReplaceAsChar(this string p_str, char p_char) {
			return new string(p_char, p_str.Length);
		}
		#endregion

		#region "WildCards"
		/// <summary>
		/// 是否符合通配符
		/// </summary>
		/// <param name="p_src"></param>
		/// <param name="p_pattern"></param>
		/// <returns></returns>
		public static bool IsMatchWildCards(this string p_src, string p_pattern) {
			p_pattern = "^" + Regex.Escape(p_pattern).Replace("\\?", ".").Replace("\\*", ".*") + "$";
			return Regex.IsMatch(p_src, p_pattern);
		}
		#endregion
	}
}