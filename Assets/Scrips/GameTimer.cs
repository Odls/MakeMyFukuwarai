using TMPro;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace MakeMyFukuwarai {
	public class GameTimer : MonoBehaviour {
		[SerializeField] Image bar;
		[SerializeField] TextMeshProUGUI text;

		[SerializeField] float winThreshold = 0.4f;
		[SerializeField] float timeLimit = 40f;

		float nowTime;

		void Start() {
			nowTime = timeLimit;
		}

		void Update() {
			nowTime -= Time.deltaTime;
			if (nowTime < 0) {
				nowTime = 0;
				Face.instance.CalculateScore(out var _totalScore, out var _finalMask);
				if(_totalScore > winThreshold) {
					GameManager.EndGame(_finalMask, true);
				} else {
					GameManager.EndGame(_finalMask, false);
				}
			}

			bar.fillAmount = nowTime / timeLimit;
			text.text = $"{nowTime:00.0}";
		}
	}
}