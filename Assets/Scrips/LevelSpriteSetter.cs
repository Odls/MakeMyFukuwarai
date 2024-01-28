using System.Collections.Generic;

using UnityEngine;

namespace MakeMyFukuwarai {
	public class LevelSpriteSetter : MonoBehaviour {
		[SerializeField] SpriteRenderer spriteRenderer;
		[SerializeField] List<Sprite> sprites;

		private void Awake() {
			spriteRenderer.sprite = sprites[GameManager.nowLevelIndex];
		}

	}
}