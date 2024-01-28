using System.Collections;

using UnityEngine;
using UnityEngine.SceneManagement;

namespace MakeMyFukuwarai {
	public class ChangeToEndScene : MonoBehaviour {
		[SerializeField] float time = 0f;

		public IEnumerator Start() {
			yield return new WaitForSeconds(time);

			DontDestroyOnLoad(GameManager.finalMask);
			if (GameManager.nowIsWin) {
				SceneManager.LoadScene("Win");
			} else {
				SceneManager.LoadScene("Win");
				//SceneManager.LoadScene("Lose");
			}
		}
	}
}