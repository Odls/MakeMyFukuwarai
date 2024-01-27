using System.Collections.Generic;

using UnityEngine;

namespace MakeMyFukuwarai {
	public class Face : MonoBehaviour {
		List<FaceItem> attachItems;
		[SerializeField] MeshRenderer faceMesh;

		[SerializeField] List<Transform> mAnchors;
		[SerializeField] List<string> wiggleAnimations;
		[SerializeField] Animator wiggleAnimator;

		public IEnumerable<Transform> anchors => mAnchors;

		static public Face instance { get; private set; }

		private void Start() {
			instance = this;
			attachItems = new List<FaceItem>();
			mAnchors = new List<Transform>();
			foreach (var  _child in transform) {
				Transform _anchor = _child as Transform;
				if((_child!=null) && (_anchor != transform)) {
					mAnchors.Add(_anchor);
				}
			}
			wiggleAnimator.Play("Idle");
		}

		public void Update() {
			if(Random.Range(0, 180) == 0) {
				wiggleAnimator.Play(wiggleAnimations[Random.Range(0, wiggleAnimations.Count)]);
			}
		}
		internal void Attach(FaceItem p_faceItem) {
			attachItems.Add(p_faceItem);
			p_faceItem.transform.SetParent(faceMesh.transform);
		}
	}
}