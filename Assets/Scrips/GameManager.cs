using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;

namespace MakeMyFukuwarai {


	public class GameManager : MonoBehaviour {
			[SerializeField] List<string> levelNames;

		public static GameManager instance { get; private set; }

		public static int nowLevelIndex { get; private set; }
		public static string nowLevelName => instance.levelNames[nowLevelIndex];

		private void Awake() {
			instance = this;
		}

		public void SelectLevel(int p_index) {
			nowLevelIndex = p_index;
			SceneManager.LoadScene("Story");
		}
	}
}