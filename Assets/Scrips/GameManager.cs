using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;

namespace MakeMyFukuwarai {
	public class GameManager : MonoBehaviour {
		[SerializeField] List<string> levelNames;

		public static GameManager instance { get; private set; }

		public static int nowLevelIndex { get; private set; }
		public static string nowLevelName => instance.levelNames[nowLevelIndex];
		public static bool nowIsWin { get; private set; }
		public static float totalScore { get; private set; }

		public static Transform finalMask { get; private set; }
		private void Awake() {
			instance = this;
		}

		public void SelectLevel(int p_index) {
			nowLevelIndex = p_index;
			SceneManager.LoadScene("Story");
		}

		public static void EndGame(Transform p_finalMask, float p_totalScore, bool p_win) {
			nowIsWin = p_win;
			finalMask = p_finalMask;
			totalScore = p_totalScore;
			SceneManager.LoadScene("EndGame");
		}
	}
}