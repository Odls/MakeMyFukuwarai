using System.Collections.Generic;

using UnityEngine;

namespace MakeMyFukuwarai {
	public class LevelObjOpener : MonoBehaviour {
			[SerializeField] List<GameObject> objs;

		private void Awake() {
			foreach (var _obj in objs) {
				if (_obj) {
					_obj.SetActive(false);
				}
			}
			objs[GameManager.nowLevelIndex].SetActive(true);
		}
	}
}