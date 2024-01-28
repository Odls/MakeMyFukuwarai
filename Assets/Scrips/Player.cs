using Sirenix.OdinInspector;

using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace MakeMyFukuwarai {
	public class Player : MonoBehaviour {
		enum State {
			Pick,
			Picking,
			WaitThrow,
		}

		[SerializeField] public Camera camera;
		[SerializeField] Vector2 rotaRange;
		[SerializeField] float waitOffset;
		[SerializeField] Transform waitPoint, shootPoint;
		[SerializeField] Transform lineTop;
		[ReadOnly, ShowInInspector] State state = State.Pick;

		[ReadOnly, ShowInInspector] FaceItem nowItem;

		public Vector3 waitPos => waitPoint.position;
		public Vector3 shootPos => shootPoint.position;
		static public Player instance { get; private set; }

		private void Awake() {
			instance = this;
		}

		private void Update() {
			switch (state) {
			case State.Pick:

				break;
			case State.Picking:
				state = State.WaitThrow;
				lineTop.gameObject.SetActive(false);
				break;
			case State.WaitThrow:
				lineTop.gameObject.SetActive(true);

				Vector3 _mousePos = Input.mousePosition;
				_mousePos.z = 0.5f;
				var _pos = camera.ScreenToWorldPoint(_mousePos);
				transform.position = _pos;

				var _viewportPos = camera.ScreenToViewportPoint(_mousePos);
				Vector3 _rota = new Vector3(
					Mathf.Lerp(rotaRange.y, -rotaRange.y, _viewportPos.y),
					Mathf.Lerp(-rotaRange.x, rotaRange.x, _viewportPos.x),
					0
				);

				transform.eulerAngles = _rota;

				var _waitPos = new Vector3(
					Mathf.Lerp(-waitOffset, waitOffset, _viewportPos.x),
					Mathf.Lerp(-waitOffset, waitOffset, _viewportPos.y),
					0
				);

				waitPoint.localPosition = _waitPos;

				if (Input.GetMouseButtonDown(0)) {
					state = State.Pick;
					lineTop.gameObject.SetActive(false);

					nowItem.Shoot(shootPoint.position);
					nowItem = null;
				}
				if (Input.GetMouseButtonDown(1)) {
					state = State.Pick;
					lineTop.gameObject.SetActive(false);

					nowItem.Drop();
					nowItem = null;
				}
				break;
			}
		}

		internal void Pick(FaceItem p_faceItem) {
			if(state == State.Pick) {
				state = State.Picking;
				nowItem?.Drop();
				nowItem = p_faceItem;
				p_faceItem.OnPick();
			}
		}

		private void OnDrawGizmos() {
			RaycastHit _hit;
			Gizmos.DrawLine(transform.position, transform.position + transform.forward * 10);
			if (Physics.Raycast(transform.position, transform.forward, out _hit, 10, LayerMask.GetMask("Face"))) {
				Gizmos.DrawWireSphere(_hit.point, 0.1f);
			}
		}

		internal Transform GetNearAnchor() {
			float _minSqrMagnitude = float.MaxValue;
			Transform _nearAnchor = null;
			foreach (var _anchor in Face.instance.anchors) {
				float _sqrMagnitude = Vector3.SqrMagnitude(_anchor.position - transform.position);
				if (_sqrMagnitude < _minSqrMagnitude) {
					_minSqrMagnitude = _sqrMagnitude;
					_nearAnchor = _anchor;
				}
			}
			return _nearAnchor;
		}
	}
}