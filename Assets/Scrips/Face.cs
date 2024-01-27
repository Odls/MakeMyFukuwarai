using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using OdlsExtend;
using UnityEditor.Experimental.GraphView;
using UnityEngine.XR;

namespace MakeMyFukuwarai {

	struct FaceItemResult {
		public FaceItem item;
		public float score;
	}
	public class Face : MonoBehaviour {
		List<FaceItem> attachItems;
		[SerializeField] MeshRenderer faceMesh;
		[SerializeField] MeshRenderer answerFaceMesh;

		[SerializeField, FoldoutGroup("Animation")] Animator wiggleAnimator;
		[SerializeField, FoldoutGroup("Animation")] List<string> wiggleAnimations;

		[SerializeField, FoldoutGroup("Anchor")] List<Transform> mAnchors;
		[SerializeField, FoldoutGroup("Anchor")] Transform anchorEyeL, anchorEyeR, anchorEyebrowL, anchorEyebrowR, anchorMouth, anchorNose, anchorOtherL, anchorOtherR;
		[SerializeField, FoldoutGroup("Answer")] Transform answerAnchorEyeL, answerAnchorEyeR, answerAnchorEyebrowL, answerAnchorEyebrowR, answerAnchorMouth, answerAnchorNose, answerAnchorOtherL, answerAnchorOtherR;

		[ShowInInspector, ReadOnly, FoldoutGroup("Anchor")] List<MeshRenderer> mAnswers;
		[ShowInInspector, ReadOnly, FoldoutGroup("Item")] MeshRenderer answerEyeL, answerEyeR, answerEyebrowL, answerEyebrowR, answerMouth, answerNose, answerOtherL, answerOtherR;
		[SerializeField, FoldoutGroup("Item")] FaceItem faceItemPrefab;
		[SerializeField, FoldoutGroup("Item")] FaceItemTable mouthTable, noseTable;
		[SerializeField, FoldoutGroup("Item")] FaceItemPairTable eyeTable, eyebrowTable, otherTable;

		public IEnumerable<Transform> anchors => mAnchors;

		static public Face instance { get; private set; }

		[Button, FoldoutGroup("Anchor")]
		void GetAnchor() {
			mAnchors ??= new List<Transform>();
			mAnchors.Clear();

			foreach (var _child in faceMesh.transform) {
				Transform _anchor = _child as Transform;
				if ((_anchor != null) && (_anchor != faceMesh.transform)) {
					mAnchors.Add(_anchor);
					if ((_anchor.name.EndsWith("eye_L")) || (_anchor.name.EndsWith("L_eye"))) {
						anchorEyeL = _anchor;
					} else if ((_anchor.name.EndsWith("eye_R") || (_anchor.name.EndsWith("R_eye")))) {
						anchorEyeR = _anchor;
					} else if ((_anchor.name.EndsWith("eyebrow_R") || (_anchor.name.EndsWith("R_eyebrow")))) {
						anchorEyebrowR = _anchor;
					} else if ((_anchor.name.EndsWith("eyebrow_L") || (_anchor.name.EndsWith("L_eyebrow")))) {
						anchorEyebrowL = _anchor;
					} else if (_anchor.name.EndsWith("mouth")) {
						anchorMouth = _anchor;
					} else if (_anchor.name.EndsWith("nose")) {
						anchorNose = _anchor;
					} else if ((_anchor.name.EndsWith("other_R") || (_anchor.name.EndsWith("R_other")))) {
						anchorOtherR = _anchor;
					} else if ((_anchor.name.EndsWith("other_L") || (_anchor.name.EndsWith("L_other")))) {
						anchorOtherL = _anchor;
					}
				}
			}

			foreach (var _child in answerFaceMesh.transform) {
				Transform _anchor = _child as Transform;
				if ((_anchor != null) && (_anchor != answerFaceMesh.transform)) {
					if ((_anchor.name.EndsWith("eye_L") || (_anchor.name.EndsWith("L_eye")))) {
						answerAnchorEyeL = _anchor;
					} else if ((_anchor.name.EndsWith("eye_R") || (_anchor.name.EndsWith("R_eye")))) {
						answerAnchorEyeR = _anchor;
					} else if ((_anchor.name.EndsWith("eyebrow_R") || (_anchor.name.EndsWith("R_eyebrow")))) {
						answerAnchorEyebrowR = _anchor;
					} else if ((_anchor.name.EndsWith("eyebrow_L") || (_anchor.name.EndsWith("L_eyebrow")))) {
						answerAnchorEyebrowL = _anchor;
					} else if (_anchor.name.EndsWith("mouth")) {
						answerAnchorMouth = _anchor;
					} else if (_anchor.name.EndsWith("nose")) {
						answerAnchorNose = _anchor;
					} else if ((_anchor.name.EndsWith("other_R") || (_anchor.name.EndsWith("R_other")))) {
						answerAnchorOtherR = _anchor;
					} else if ((_anchor.name.EndsWith("other_L") || (_anchor.name.EndsWith("L_other")))) {
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
			GenerateFaceItem();
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
			mAnswers.Add(p_meshRenderer);

			var _answerView = Instantiate(p_meshRenderer, p_answerAnchor);
			_answerView.gameObject.layer = LayerMask.NameToLayer("Answer");
			_answerView.transform.localScale = Vector3.one;
			_answerView.transform.localPosition = Vector3.zero;
			_answerView.transform.localRotation = Quaternion.identity;

		}

		void GenerateAnswer() {
			mAnswers ??= new List<MeshRenderer> ();
			mAnswers.Clear();

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

		void GenerateFaceItem() {
			HashSet<MeshRenderer> _meshSet = new HashSet<MeshRenderer>();

			mouthTable.AddToSet(_meshSet);
			noseTable.AddToSet(_meshSet);
			eyeTable.AddToSet(_meshSet);
			eyebrowTable.AddToSet(_meshSet);
			otherTable.AddToSet(_meshSet);

			foreach(var _answer in mAnswers) {
				_meshSet.Remove(_answer);
				GenerateFaceItem(_answer);
			}

			List<MeshRenderer> _otherMeshs = new List<MeshRenderer>(_meshSet);
			_otherMeshs.Shuffle();

			int _otherCount = Mathf.Min(_meshSet.Count, mAnswers.Count);

			for(int f=0; f < _otherCount; f++) {
				GenerateFaceItem(_otherMeshs[f]);
			}

		}

		void GenerateFaceItem(MeshRenderer p_mesh) {
			var _item = Instantiate(faceItemPrefab);
			var _mesh = Instantiate(p_mesh, _item.transform);
			_mesh.name = p_mesh.name;
			_item.ApplyCollider();
			_item.Popup();
		}

		[Button]
		List<FaceItemResult> CalculateScore() {
			var _results = new List<FaceItemResult>();
			HashSet<string> _answerSet = new HashSet<string>();
			foreach (var _answer in mAnswers) {
				_answerSet.Add(_answer.name);
			}

			foreach (var _item in attachItems) {
				if (!_answerSet.Contains(_item.name)) {
					_results.Add(new FaceItemResult() {
						item = _item,
						score = -0.5f,
					});
				} else {
					float _score = 0;
					switch (_item.tag) {
					case "Eye_L":		_score = CalculateScore(_item, anchorEyeL);     break;
					case "Eye_R":		_score = CalculateScore(_item, anchorEyeR);     break;
					case "Eyebrow_L":	_score = CalculateScore(_item, anchorEyebrowL); break;
					case "Eyebrow_R":	_score = CalculateScore(_item, anchorEyebrowR); break;
					case "Mouth":		_score = CalculateScore(_item, anchorMouth);    break;
					case "Nose":		_score = CalculateScore(_item, anchorNose);     break;
					case "Other_L":		_score = CalculateScore(_item, anchorOtherL);   break;
					case "Other_R":		_score = CalculateScore(_item, anchorOtherR);   break;
					}

					_results.Add(new FaceItemResult() {
						item = _item,
						score = _score,
					});
				}
			}

			float _totalScore = 0;
			string _log = "";
			foreach (FaceItemResult _result in _results) {
				_totalScore += _result.score;
				_log += _result.item.name + " " + Mathf.Floor(_result.score*100) + "%\n";
			}

			_totalScore = Mathf.Clamp01(_totalScore/ mAnswers.Count);

			Debug.Log("Score : " + Mathf.Floor(_totalScore*100) + "% \n" + _log);

			return _results;
		}

		private float CalculateScore(FaceItem p_item, Transform p_anchor) {
			var _distance = Vector3.Distance(p_item.transform.position, p_anchor.position);
			return 1- Mathf.Clamp01(_distance/0.3f);

		}
	}
}