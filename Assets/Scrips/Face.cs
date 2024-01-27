using Sirenix.OdinInspector;

using System.Collections.Generic;

using UnityEngine;

namespace MakeMyFukuwarai {
	public class Face : MonoBehaviour {
		List<FaceItem> attachItems;
		[SerializeField] MeshRenderer faceMesh;
		[SerializeField] MeshRenderer answerFaceMesh;

		[SerializeField, FoldoutGroup("Animation")] Animator wiggleAnimator;
		[SerializeField, FoldoutGroup("Animation")] List<string> wiggleAnimations;

		[SerializeField, FoldoutGroup("Anchor")] List<Transform> mAnchors;
		[SerializeField, FoldoutGroup("Anchor")] Transform anchorEyeL, anchorEyeR, anchorEyebrowL, anchorEyebrowR, anchorMouth, anchorNose, anchorOtherL, anchorOtherR;
		[SerializeField, FoldoutGroup("Answer")] Transform answerAnchorEyeL, answerAnchorEyeR, answerAnchorEyebrowL, answerAnchorEyebrowR, answerAnchorMouth, answerAnchorNose, answerAnchorOtherL, answerAnchorOtherR;

		[ShowInInspector, ReadOnly, FoldoutGroup("Item")] MeshRenderer answerEyeL, answerEyeR, answerEyebrowL, answerEyebrowR, answerMouth, answerNose, answerOtherL, answerOtherR;
		[SerializeField, FoldoutGroup("Item")] FaceItemTable mouthTable, noseTable;
		[SerializeField, FoldoutGroup("Item")] FaceItemPairTable eyeTable, eyebrowTable, otherTable;

		public IEnumerable<Transform> anchors => mAnchors;

		static public Face instance { get; private set; }

		[Button, FoldoutGroup("Anchor")]
		void GetAnchor() {
			mAnchors = new List<Transform>();
			foreach (var _child in faceMesh.transform) {
				Transform _anchor = _child as Transform;
				if ((_anchor != null) && (_anchor != faceMesh.transform)) {
					mAnchors.Add(_anchor);
					if ((_anchor.name == "eye_L") || (_anchor.name == "L_eye")) {
						anchorEyeL = _anchor;
					} else if ((_anchor.name == "eye_R") || (_anchor.name == "R_eye")) {
						anchorEyeR = _anchor;
					} else if ((_anchor.name == "eyebrow_R") || (_anchor.name == "R_eyebrow")) {
						anchorEyebrowR = _anchor;
					} else if ((_anchor.name == "eyebrow_L") || (_anchor.name == "L_eyebrow")) {
						anchorEyebrowL = _anchor;
					} else if (_anchor.name == "mouth") {
						anchorMouth = _anchor;
					} else if (_anchor.name == "nose") {
						anchorNose = _anchor;
					} else if ((_anchor.name == "other_R") || (_anchor.name == "R_other")) {
						anchorOtherR = _anchor;
					} else if ((_anchor.name == "other_L") || (_anchor.name == "L_other")) {
						anchorOtherL = _anchor;
					}
				}
			}

			foreach (var _child in answerFaceMesh.transform) {
				Transform _anchor = _child as Transform;
				if ((_anchor != null) && (_anchor != answerFaceMesh.transform)) {
					if ((_anchor.name == "eye_L") || (_anchor.name == "L_eye")) {
						answerAnchorEyeL = _anchor;
					} else if ((_anchor.name == "eye_R") || (_anchor.name == "R_eye")) {
						answerAnchorEyeR = _anchor;
					} else if ((_anchor.name == "eyebrow_R") || (_anchor.name == "R_eyebrow")) {
						answerAnchorEyebrowR = _anchor;
					} else if ((_anchor.name == "eyebrow_L") || (_anchor.name == "L_eyebrow")) {
						answerAnchorEyebrowL = _anchor;
					} else if (_anchor.name == "mouth") {
						answerAnchorMouth = _anchor;
					} else if (_anchor.name == "nose") {
						answerAnchorNose = _anchor;
					} else if ((_anchor.name == "other_R") || (_anchor.name == "R_other")) {
						answerAnchorOtherR = _anchor;
					} else if ((_anchor.name == "other_L") || (_anchor.name == "L_other")) {
						answerAnchorOtherL = _anchor;
					}
				}
			}
		}
		private void Start() {
			instance = this;
			attachItems = new List<FaceItem>();
			
			wiggleAnimator.Play("Idle");

			GenerateAnswer();
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
	
		void AddToAnswer(MeshRenderer p_meshRenderer, Transform p_answerAnchor) {
			var _answerView = Instantiate(p_meshRenderer, p_answerAnchor);
			_answerView.gameObject.layer = LayerMask.NameToLayer("Answer");
			_answerView.transform.localScale = Vector3.one;
			_answerView.transform.localPosition = Vector3.zero;
			_answerView.transform.localRotation = Quaternion.identity;

		}

		internal void GenerateAnswer() {
			if (anchorEyeL != null) {
				var _eyeAnswer = eyeTable.GetRandomItem();
				answerEyeL = _eyeAnswer.left;
				AddToAnswer(answerEyeL, answerAnchorEyeL);
				answerEyeR = _eyeAnswer.right;
				AddToAnswer(answerEyeR, answerAnchorEyeR);
			}

			if (anchorEyebrowL != null) {
				var _eyebrowAnswer = eyebrowTable.GetRandomItem();
				answerEyebrowL = _eyebrowAnswer.left;
				AddToAnswer(answerEyebrowL, answerAnchorEyebrowL);
				answerEyebrowR = _eyebrowAnswer.right;
				AddToAnswer(answerEyebrowR, answerAnchorEyebrowR);
			}

			if (anchorMouth != null) {
				answerMouth = mouthTable.GetRandomItem();
				AddToAnswer(answerMouth, answerAnchorMouth);
			}

			if (anchorNose != null) {
				answerNose = noseTable.GetRandomItem();
				AddToAnswer(answerNose, answerAnchorNose);
			}

			if (anchorOtherL != null) {
				var _otherAnswer = otherTable.GetRandomItem();
				answerOtherL = _otherAnswer.left;
				AddToAnswer(answerOtherL, answerAnchorOtherL);
				answerOtherR = _otherAnswer.right;
				AddToAnswer(answerOtherR, answerAnchorOtherR);
			}
		}
	
	}
}