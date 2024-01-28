using System.Collections;

using UnityEngine;
using UnityEngine.SceneManagement;

namespace MakeMyFukuwarai {
	public class ChangeScene : MonoBehaviour {
			[SerializeField] float time = 0f;
		[SerializeField] string targetName;

		public IEnumerator Start() {
			yield return new WaitForSeconds(time);
			SceneManager.LoadScene(targetName);
		
		}
	}
}