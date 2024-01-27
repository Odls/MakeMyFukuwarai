using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace OdlsExtend {
	public enum E_TIME_UNIT { NONE, YEAR, MONTH, WEEK, DAY, HOUR, MINUTE, SECOND }
	public static class NumberExtend {
		#region "String"
		public static string ToStringWithPlus(this int p_number, string p_format = "") {
			return ToStringWithPlus<int>(p_number, p_format);
		}
		public static string ToStringWithPlus<T>(this T p_number, string p_format = "") where T : System.IComparable, System.IFormattable {
			if (p_number.CompareTo(default(T)) > 0) {
				return "+" + p_number.ToString(p_format, null);
			} else {
				return p_number.ToString(p_format, null);
			}
		}
		//https://codereview.stackexchange.com/questions/13105/integer-to-alphabet-string-a-b-z-aa-ab
		const int columnBase = 26;
		const int digitMax = 7; // ceil(log26(Int32.Max))
		const string digits = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
		public static string ToAlphabet(this int p_number) {
			if (p_number < 0)
				return p_number.ToString();

			if (p_number < columnBase)
				return digits[p_number].ToString();

			StringBuilder _builder = new StringBuilder().Append(' ', digitMax);
			int _current = p_number + 1;
			int _offset = digitMax;
			while (_current > 0) {
				_builder[--_offset] = digits[--_current % columnBase];
				_current /= columnBase;
			}
			return _builder.ToString(_offset, digitMax - _offset);
		}
		#endregion

		#region "Vector"
		public static Vector2 RandomInBox(Vector2 minPos, Vector2 maxPos) {
			return new Vector2(
				Random.Range(minPos.x, maxPos.x),
				Random.Range(minPos.y, maxPos.y)
			);
		}
		public static Vector3 RandomInBox(Vector3 minPos, Vector3 maxPos) {
			return new Vector3(
				Random.Range(minPos.x, maxPos.x),
				Random.Range(minPos.y, maxPos.y),
				Random.Range(minPos.z, maxPos.z)
			);
		}
		#endregion

		#region "Mask"
		static List<int> maskList = new List<int>();
		static int maskLen = 0;
		static void FillMaskList(int p_index) {
			if (p_index >= maskLen) {
				for (int f = maskLen; f <= p_index; f++) {
					maskList.Add(1 << f);
					maskLen++;
				}
			}
		}
		public static int GetFlagMaskByIndex(int p_index) {
			FillMaskList(p_index);
			return maskList[p_index];
		}
		public static int GetFlagMaskByIndexs(params int[] p_indexs) {
			int _mask = 0;
			foreach (var _index in p_indexs) {
				_mask |= maskList[_index];
			}
			return _mask;
		}

		public static int[] GetIndexsFromFlagMask(this int p_flag) {
			tempIndexList.Clear();
			int _index = 0;
			while (p_flag != 0) {
				if ((p_flag & 1) == 1) {
					tempIndexList.Add(_index);
				}
				p_flag = p_flag >> 1;
				_index++;
			}
			return tempIndexList.ToArray();
		}
		public static int GetIndexFromFlagMask(this int p_flag) {
			int _index = 0;
			while (p_flag != 0) {
				if ((p_flag & 1) == 1) {
					return _index;
				}
				p_flag >>= 1;
				_index++;
			}
			return -1;
		}
		public static bool FlagIsOnByIndex(this int p_flag, int p_index) {
			int _mask = GetFlagMaskByIndex(p_index);
			return p_flag.FlagIsOnByMask(_mask);
		}
		public static bool FlagIsOnByMask(this int p_flag, int p_mask) {
			return (p_flag & p_mask) != 0;
		}
		public static int SetFlagAtIndex(this int p_flag, int p_index, bool p_on) {
			int _mask = GetFlagMaskByIndexs(p_index);
			if (p_on) {
				return p_flag | _mask;
			} else {
				return p_flag & ~_mask;
			}
		}
		#endregion

		#region "ByteMask"
		static byte[] byteMaskList = new byte[] {
			0b00000001, 0b00000010, 0b00000100, 0b00001000,
			0b00010000, 0b00100000, 0b01000000, 0b10000000
		};
		public static byte GetFlagByteMaskByIndex(int p_index) {
			return byteMaskList[p_index];
		}
		public static byte GetFlagByteMaskByIndexs(params int[] p_indexs) {
			byte _mask = 0;
			foreach (var _index in p_indexs) {
				_mask |= byteMaskList[_index];
			}
			return _mask;
		}

		static List<int> tempIndexList = new List<int>();
		public static int[] GetIndexsFromFlagMask(this byte p_flag) {
			tempIndexList.Clear();
			for (int _index = 0; _index < 8; _index++) {
				if (p_flag.FlagIsOnByIndex(_index)) {
					tempIndexList.Add(_index);
				}
			}
			return tempIndexList.ToArray();
		}
		public static int GetIndexFromFlagMask(this byte p_flag) {
			for (int _index = 0; _index < 8; _index++) {
				if (p_flag.FlagIsOnByIndex(_index)) {
					return _index;
				}
			}
			return -1;
		}
		public static bool FlagIsOnByIndex(this byte p_flag, int p_index) {
			byte _mask = GetFlagByteMaskByIndex(p_index);
			return p_flag.FlagIsOnByMask(_mask);
		}
		public static bool FlagIsOnByMask(this byte p_flag, byte p_mask) {
			return (p_flag & p_mask) != 0;
		}
		public static byte SetFlagAtIndex(this byte p_flag, int p_index, bool p_on) {
			byte _mask = GetFlagByteMaskByIndex(p_index);
			if (p_on) {
				return (byte)(p_flag | _mask);
			} else {
				return (byte)(p_flag & ~_mask);
			}
		}
		#endregion

		#region "Time"
		public static int GetBestUnitTime(this System.TimeSpan p_dateTime, out E_TIME_UNIT p_unit) {
			int _day = p_dateTime.Days;
			int _year = _day / 365;
			if (_year > 0) {
				p_unit = E_TIME_UNIT.YEAR;
				return _year;
			} else {
				int _month = _day / 30;
				if (_month > 0) {
					p_unit = E_TIME_UNIT.MONTH;
					return _month;
				} else {
					int _week = _day / 7;
					if (_week > 0) {
						p_unit = E_TIME_UNIT.WEEK;
						return _week;
					} else {
						if (_day > 0) {
							p_unit = E_TIME_UNIT.DAY;
							return _day;
						} else {
							int _hour = p_dateTime.Hours;
							if (_hour > 0) {
								p_unit = E_TIME_UNIT.HOUR;
								return _hour;
							} else {
								int _minute = p_dateTime.Minutes;
								if (_minute > 0) {
									p_unit = E_TIME_UNIT.MINUTE;
									return _minute;
								} else {
									int _second = p_dateTime.Seconds;
									if (_second > 0) {
										p_unit = E_TIME_UNIT.SECOND;
										return _second;
									} else {
										p_unit = E_TIME_UNIT.NONE;
										return 0;
									}
								}
							}
						}
					}
				}
			}
		}
		#endregion

		#region "Range"
		public class Range {
			float mMin = float.MinValue;
			float mMax = float.MaxValue;
			float range = float.MaxValue;
			public Range(float p_min, float p_max) {
				mMin = p_min;
				mMax = p_max;
				range = mMax - mMin;
			}
			public float min {
				get {
					return mMin;
				}
				set {
					mMin = value;
					range = mMax - mMin;
				}
			}
			public float max {
				get {
					return mMax;
				}
				set {
					mMax = value;
					range = mMax - mMin;
				}
			}
			public float GetRandom() {
				return Random.Range(mMin, mMax);
			}
			public float GetRandom(float p_clip) {
				return GetRandom(p_clip, p_clip);
			}
			public float GetRandom(float p_minClip, float p_maxClip) {
				return Random.Range(
					mMin + (range * p_minClip),
					mMax - (range * p_maxClip)
				);
			}
			public bool IsInside(float p_value) {
				return IsInRange(p_value, mMin, mMax);
			}
		}
		public class Range2D {
			Vector2 mMin = new Vector2(float.MinValue, float.MinValue);
			Vector2 mMax = new Vector2(float.MaxValue, float.MaxValue);
			Vector2 size = new Vector2(float.MaxValue, float.MaxValue);
			public Range2D(float p_left, float p_bottom, float p_right, float p_top) {
				mMin = new Vector2(p_left, p_bottom);
				mMax = new Vector2(p_right, p_top);
				size = mMax - mMin;
			}
			public Range2D(Vector2 p_min, Vector2 p_max) {
				mMin = p_min;
				mMax = p_max;
				size = mMax - mMin;
			}
			public Vector2 min {
				get {
					return mMin;
				}
				set {
					mMin = value;
					size = mMax - mMin;
				}
			}
			public Vector2 max {
				get {
					return mMax;
				}
				set {
					mMax = value;
					size = mMax - mMin;
				}
			}
			public Vector2 GetRandom() {
				return new Vector2(
					Random.Range(mMin.x, mMax.x),
					Random.Range(mMin.y, mMax.y)
				);
			}
			public Vector2 GetRandom(float p_clip) {
				return GetRandom(
					p_clip, p_clip,
					p_clip, p_clip
				);
			}
			public Vector2 GetRandom(float p_leftRightClip, float p_topBottomClip) {
				return GetRandom(
					p_leftRightClip, p_leftRightClip,
					p_topBottomClip, p_topBottomClip
				);
			}
			public Vector2 GetRandom(float p_leftClip, float p_bottomClip, float p_rightClip, float p_topClip) {
				return new Vector2(
					Random.Range(
						mMin.x + (size.x * p_leftClip),
						mMax.x - (size.x * p_rightClip)
					),
					Random.Range(
						mMin.y + (size.y * p_bottomClip),
						mMax.y - (size.y * p_topClip)
					)
				);
			}
			public bool IsInside(Vector2 p_value) {
				return IsInRange(p_value.x, mMin.x, mMax.x) && IsInRange(p_value.y, mMin.y, mMax.y);
			}
		}
		#endregion

		#region "Math"
		public static float SafeDiv(float A, float B) {
			return ((B == 0) ? 0 : (A / B));
		}
		public static int SafeDiv(int A, int B) {
			return ((B == 0) ? 0 : (A / B));
		}

		public static float SnapInt(this float p_num, float p_snapInt) {
			if (p_snapInt == 0) {
				return p_num;
			}
			float _floorNum = Mathf.Floor(p_num);
			if ((p_num - _floorNum) < p_snapInt) {
				return _floorNum;
			}
			float _ceilNum = Mathf.Ceil(p_num);
			if ((_ceilNum - p_num) < p_snapInt) {
				return _ceilNum;
			}
			return p_num;
		}

		public static bool WhenEqualOrOver(float p_lastNum, float p_nowNum, float p_threshold) {
			return IsInRangeCE(p_threshold, p_lastNum, p_nowNum);
		}
		public static bool WhenOver(float p_lastNum, float p_nowNum, float p_threshold) {
			return IsInRangeEC(p_threshold, p_lastNum, p_nowNum);
		}

		public static bool IsInRange(this float p_num, float p_min, float p_max) {
			int _compareMin = p_num.CompareTo(p_min);
			int _compareMax = p_num.CompareTo(p_max);
			return (_compareMin >= 0) && (_compareMax <= 0);
		}
		public static bool IsInRangeCE(this float p_num, float p_min, float p_max) {
			int _compareMin = p_num.CompareTo(p_min);
			int _compareMax = p_num.CompareTo(p_max);
			return (_compareMin > 0) && (_compareMax <= 0);
		}
		public static bool IsInRangeEC(this float p_num, float p_min, float p_max) {
			int _compareMin = p_num.CompareTo(p_min);
			int _compareMax = p_num.CompareTo(p_max);
			return (_compareMin >= 0) && (_compareMax < 0);
		}
		public static bool IsInRangeCC(this float p_num, float p_min, float p_max) {
			int _compareMin = p_num.CompareTo(p_min);
			int _compareMax = p_num.CompareTo(p_max);
			return (_compareMin > 0) && (_compareMax < 0);
		}

		public static bool WhenEqualOrOver(int p_lastNum, int p_nowNum, int p_threshold) {
			return IsInRangeCE(p_threshold, p_lastNum, p_nowNum);
		}
		public static bool WhenOver(int p_lastNum, int p_nowNum, int p_threshold) {
			return IsInRangeEC(p_threshold, p_lastNum, p_nowNum);
		}

		public static bool IsInRange(this int p_num, int p_min, int p_max) {
			int _compareMin = p_num.CompareTo(p_min);
			int _compareMax = p_num.CompareTo(p_max);
			return (_compareMin >= 0) && (_compareMax <= 0);
		}
		public static bool IsInRangeCE(this int p_num, int p_min, int p_max) {
			int _compareMin = p_num.CompareTo(p_min);
			int _compareMax = p_num.CompareTo(p_max);
			return (_compareMin > 0) && (_compareMax <= 0);
		}
		public static bool IsInRangeEC(this int p_num, int p_min, int p_max) {
			int _compareMin = p_num.CompareTo(p_min);
			int _compareMax = p_num.CompareTo(p_max);
			return (_compareMin >= 0) && (_compareMax < 0);
		}
		public static bool IsInRangeCC(this int p_num, int p_min, int p_max) {
			int _compareMin = p_num.CompareTo(p_min);
			int _compareMax = p_num.CompareTo(p_max);
			return (_compareMin > 0) && (_compareMax < 0);
		}

		public static bool IsInRange(this Vector2 p_point, Vector2 p_min, Vector2 p_max) {
			return
				IsInRange(p_point.x, p_min.x, p_max.x) &&
				IsInRange(p_point.y, p_min.y, p_max.y);
		}
		public static bool IsInRangeC(this Vector2 p_point, Vector2 p_min, Vector2 p_max) {
			return
				IsInRangeCC(p_point.x, p_min.x, p_max.x) &&
				IsInRangeCC(p_point.y, p_min.y, p_max.y);
		}

		public static bool IsInRange(this Vector3 p_point, Vector3 p_min, Vector3 p_max) {
			return
				IsInRange(p_point.x, p_min.x, p_max.x) &&
				IsInRange(p_point.y, p_min.y, p_max.y) &&
				IsInRange(p_point.z, p_min.z, p_max.z);
		}
		public static bool IsInRangeC(this Vector3 p_point, Vector3 p_min, Vector3 p_max) {
			return
				IsInRangeCC(p_point.x, p_min.x, p_max.x) &&
				IsInRangeCC(p_point.y, p_min.y, p_max.y) &&
				IsInRangeCC(p_point.z, p_min.z, p_max.z);
		}

		public static bool IsIndexOf(this int p_num, object[] p_array) {
			return IsInRange(p_num, 0, p_array.Length - 1);
		}
		public static bool IsIndexOf(this int p_num, IList p_list) {
			return IsInRange(p_num, 0, p_list.Count - 1);
		}

		public static float LoopIn(this float p_num, float p_start, float p_end, float p_snapInt) {
			float _len = p_end - p_start;
			return LoopLen(p_num, p_start, _len, p_snapInt);
		}
		public static float LoopIn(this float p_num, float p_start, float p_end) {
			float _len = p_end - p_start;
			return LoopLen(p_num, p_start, _len);
		}
		public static float LoopIn(this float p_num, float p_end) {
			return LoopLen(p_num, p_end);
		}

		public static float LoopLen(this float p_num, float p_start, float p_len, float p_snapInt) {
			p_num = LoopLen(p_num, p_start, p_len);
			return SnapInt(p_num, p_snapInt);
		}
		public static float LoopLen(this float p_num, float p_len) {
			p_num = (p_num % p_len);
			p_num = ((p_num < 0) ? (p_num + p_len) : p_num);
			return p_num;
		}
		public static float LoopLen(this float p_num, float p_start, float p_len) {
			p_num -= p_start;
			return LoopLen(p_num, p_len) + p_start;
		}

		public static int LoopIn(this int p_num, int p_start, int p_end) {
			int _len = p_end + 1 - p_start;
			return LoopLen(p_num, p_start, _len);
		}
		public static int LoopLen(this int p_num, int p_len) {
			p_num = (p_num % p_len);
			p_num = ((p_num < 0) ? (p_num + p_len) : p_num);
			return p_num;
		}
		public static int LoopLen(this int p_num, int p_start, int p_len) {
			p_num -= p_start;
			return LoopLen(p_num, p_len) + p_start;
		}

		public static float FixIn(this float p_num, float p_start, float p_end, float p_snapInt) {
			p_num = FixIn(p_num, p_start, p_end);
			return SnapInt(p_num, p_snapInt);
		}
		public static float FixIn(this float p_num, float p_start, float p_end) {
			if (p_num < p_start) {
				return p_start;
			} else if (p_num > p_end) {
				return p_end;
			} else {
				return p_num;
			}
		}

		public static int FixIn(int p_num, int p_start, int p_end) {
			if (p_num < p_start) {
				return p_start;
			} else if (p_num > p_end) {
				return p_end;
			} else {
				return p_num;
			}
		}

		public static float RemoveInteger(this float p_number) {
			return p_number - Mathf.Floor(p_number);
		}
		public static float ToColor(this float p_number) {
			float _c = RemoveInteger(p_number);
			if ((_c == 0) && (p_number > 0)) {
				return 1;
			} else {
				return _c;
			}
		}
		//	static CalcEngine.CalcEngine calcEngine = null;
		//	public static double Expression(string p_expression){
		//		if (calcEngine == null) {
		//			calcEngine = new CalcEngine.CalcEngine ();
		//		}
		//		return (double)calcEngine.Evaluate (p_expression);
		//	}
		#endregion

		#region "Value"
		public static int SetValueAndGetChange(ref int p_var, int p_newValue) {
			int _change = p_newValue - p_var;
			p_var = p_newValue;
			return _change;
		}
		public static float SetValueAndGetChange(ref float p_var, float p_newValue) {
			float _change = p_newValue - p_var;
			p_var = p_newValue;
			return _change;
		}
		public static bool SetValueAndCheckOver(ref int p_var, int p_newValue, int p_threshold) {
			int _lastValue = p_var;
			p_var = p_newValue;
			return WhenOver(_lastValue, p_newValue, p_threshold);
		}
		public static bool SetValueAndCheckOver(ref float p_var, float p_newValue, float p_threshold) {
			float _lastValue = p_var;
			p_var = p_newValue;
			return WhenOver(_lastValue, p_newValue, p_threshold);
		}
		public static bool SetValueAndCheckEqualOrOver(ref int p_var, int p_newValue, int p_threshold) {
			int _lastValue = p_var;
			p_var = p_newValue;
			return WhenEqualOrOver(_lastValue, p_newValue, p_threshold);
		}
		public static bool SetValueAndCheckEqualOrOver(ref float p_var, float p_newValue, float p_threshold) {
			float _lastValue = p_var;
			p_var = p_newValue;
			return WhenEqualOrOver(_lastValue, p_newValue, p_threshold);
		}

		public static float GetLerp(this float p_number, float p_min, float p_max) {
			float _range = p_max - p_min;
			return (p_number - p_min) / _range;
		}
		public static float GetFixLerp(this float p_number, float p_min, float p_max) {
			return FixIn(GetLerp(p_number, p_min, p_max), 0, 1);
		}

		public static int GetHammingWeight(this int p_value) { // count one in flag
			p_value = p_value - ((p_value >> 1) & 0x55555555); // reuse input as temporary
			p_value = (p_value & 0x33333333) + ((p_value >> 2) & 0x33333333); // temp
			return ((p_value + (p_value >> 4) & 0xF0F0F0F) * 0x1010101) >> 24; // count
		}
		#endregion

		#region Enum
		public static unsafe int ToInt<SourceT>(this SourceT p_enum)
		where SourceT : unmanaged, System.Enum {
			return *(int*)(&p_enum);
		}

		public static unsafe TargetT ToEnum<TargetT>(this int p_int)
		where TargetT : unmanaged, System.Enum {
			return *(TargetT*)(&p_int);
		}
		public static unsafe TargetT ToEnum<SourceT, TargetT>(this SourceT p_enum)
		where SourceT : unmanaged, System.Enum
		where TargetT : unmanaged, System.Enum {
			return *(TargetT*)(&p_enum);
		}
		public static bool FlagIsOn<EnumT>(this EnumT p_enum, EnumT p_flag)
		where EnumT : unmanaged, System.Enum {
			return (p_enum.ToInt() & p_flag.ToInt()) != 0;
		}
		public static EnumT AddFlag<EnumT>(this EnumT p_enum, EnumT p_flag)
		where EnumT : unmanaged, System.Enum {
			return (p_enum.ToInt() | p_flag.ToInt()).ToEnum<EnumT>();
		}
		public static EnumT RemoveFlag<EnumT>(this EnumT p_enum, EnumT p_flag)
		where EnumT : unmanaged, System.Enum {
			return (p_enum.ToInt() & ~p_flag.ToInt()).ToEnum<EnumT>();
		}

		public static unsafe bool IsEquals<SourceT, TargetT>(this SourceT p_enumA, TargetT p_enumB)
		where SourceT : unmanaged, System.Enum
		where TargetT : unmanaged, System.Enum {
			return *(int*)(&p_enumA) == *(int*)(&p_enumB);
		}

		abstract class EnumInfo {
			public int[] numbers;
			public string[] names;
			public int count => numbers.Length;

			Dictionary<int, string> mNameDict;
			public Dictionary<int, string> nameDict {
				get {
					if(mNameDict == null) {
						mNameDict = new Dictionary<int, string>();
						for(int f = 0; f < numbers.Length; f++) {
							mNameDict.Add(numbers[f], names[f]);
						}
					}
					return mNameDict;
				}
			}

		}
		class EnumInfo<EnumT> : EnumInfo
		where EnumT : unmanaged, System.Enum {
			static EnumInfo<EnumT> mInstance;
			public static EnumInfo<EnumT> instance => mInstance ??= new EnumInfo<EnumT>();

			List<EnumT> enums;

			Dictionary<string, EnumT> mNameToEnumDict;
			public Dictionary<string, EnumT> nameToEnumDict {
				get {
					if (mNameToEnumDict == null) {
						mNameToEnumDict = new Dictionary<string, EnumT>();
						for (int f = 0; f < numbers.Length; f++) {
							mNameToEnumDict.Add(names[f], enums[f]);
						}
					}
					return mNameToEnumDict;
				}
			}

			public EnumInfo() {
				enums = new List<EnumT>(System.Enum.GetValues(typeof(EnumT)).Cast<EnumT>());
				numbers = new int[enums.Count];
				names = new string[enums.Count];
				for (int f = 0; f < enums.Count; f++) {
					numbers[f] = enums[f].ToInt();
					names[f] = enums[f].ToString();
				}
			}

			public string GetName(EnumT p_enum) {
				int _number = p_enum.ToInt();
				if(!nameDict.TryGetValue(_number, out var _name)) {
					_name = "Unknow(" + _number+")";
					nameDict.Add(_number, _name);
				}
				return _name;

			}
		}

		public static IEnumerable<int> GetEnumNumbers<EnumT>()
		where EnumT : unmanaged, System.Enum {
			return EnumInfo<EnumT>.instance.numbers;
		}
		public static int GetEnumCount<EnumT>()
		where EnumT : unmanaged, System.Enum {
			return EnumInfo<EnumT>.instance.count;
		}

		public static string GetName<EnumT>(this EnumT p_enum)
		where EnumT : unmanaged, System.Enum {
			return EnumInfo<EnumT>.instance.GetName(p_enum);
		}

		public static EnumT ToEnumAsIndex<EnumT>(this int p_index)
		where EnumT : unmanaged, System.Enum {
			return EnumInfo<EnumT>.instance.numbers[p_index].ToEnum<EnumT>();
		}

		public static bool TryToEnum<EnumT>(this string p_name, out EnumT p_enum)
		where EnumT : unmanaged, System.Enum {
			return EnumInfo<EnumT>.instance.nameToEnumDict.TryGetValue(p_name, out p_enum);
		}
		#endregion

		#region "Random"
		static System.Random random = new System.Random();
		static byte[] randomLongBuf = new byte[8];
		public static long RandomLong() {
			random.NextBytes(randomLongBuf);
			return (
				((long)randomLongBuf[0]) |
				((long)randomLongBuf[1] << 8) |
				((long)randomLongBuf[2] << 16) |
				((long)randomLongBuf[3] << 24) |
				((long)randomLongBuf[4] << 32) |
				((long)randomLongBuf[5] << 40) |
				((long)randomLongBuf[6] << 48) |
				((long)randomLongBuf[7] << 56)
			);
		}
		public static long RandomLong(long min, long max) {
			long _number = RandomLong();
			return (System.Math.Abs(_number % (max - min)) + min);
		}
		public static double RandomDouble() => random.NextDouble();
		public static double RandomDouble(double min, double max) {
			var _number = RandomDouble();
			return (_number % (max - min)) + min;
		}
		#endregion
	}
}