using TMPro;

using UnityEngine;

namespace MakeMyFukuwarai {
	public class ShowScore : MonoBehaviour {
		[SerializeField] public TextMeshPro scoreText;

		private void Awake() {
			scoreText.text = (GameManager.totalScore*100).ToString("00.0") + "%";
		}
	}
}