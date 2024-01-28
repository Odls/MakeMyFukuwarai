using UnityEngine;

namespace MakeMyFukuwarai {
	public class FinalMaskShow : MonoBehaviour {
		private void Awake() {
			if (GameManager.finalMask != null) {
				GameManager.finalMask.SetParent(transform);
				GameManager.finalMask.localPosition = Vector3.zero;
				GameManager.finalMask.localRotation = Quaternion.identity;
				GameManager.finalMask.localScale = Vector3.one;
			}
		}
	}
}