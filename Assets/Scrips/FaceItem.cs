using Sirenix.OdinInspector;

using System.Collections;
using System.Collections.Generic;

using Unity.VisualScripting;

using UnityEngine;
using OdlsExtend;

namespace MakeMyFukuwarai {
	
	public class FaceItem : MonoBehaviour {

		enum State {
			Idle,
			WaitThrow,
			Fly,
			Attach,
		}

		[ReadOnly, ShowInInspector] State state = State.Idle;
		[SerializeField] float flySpeed = 1;
		[SerializeField] float waitThrowSpeed = 1;
		[SerializeField] Rigidbody rigidbody;
		[SerializeField] List<Collider> colliders;

		int faceMask;
		void Start() {
			colliders = new List<Collider>(GetComponentsInChildren<Collider>());
			faceMask = LayerMask.GetMask("Face");
			Popup();
		}

		bool trigger {
			set {
				foreach (var _collider in colliders) {
					_collider.isTrigger = value;
				}
			}
		}

		void Update() {
			switch (state) {
			case State.Idle:
				CheckOut();

				break;
			case State.WaitThrow:
				transform.position = Vector3.MoveTowards(transform.position, Player.instance.waitPos, waitThrowSpeed * Time.deltaTime);
				transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.identity, 360 * Time.deltaTime);
				break;
			case State.Fly:
				transform.position += shootDir * flySpeed * Time.deltaTime;

				RaycastHit _hit;
				if (Physics.Raycast(startPos, shootDir, out _hit, 10, faceMask)) {
					float _hitSqrMagnitude = Vector3.SqrMagnitude(startPos-_hit.point);
					float _flySqrMagnitude = Vector3.SqrMagnitude(startPos-transform.position);
					if (_flySqrMagnitude >= _hitSqrMagnitude) {
						transform.position = _hit.point - shootDir * 0.03f;
						state = State.Attach;
						Face.instance.Attach(this);
					}
				}
				CheckOut();
				break;
			case State.Attach:
				break;
			}
		}

		void Popup() {
			state = State.Idle;
			rigidbody.useGravity = true;
			trigger = false;
			transform.position = NumberExtend.RandomInBox(popupMin, popupMax);
			transform.transform.rotation = Random.rotation;
		}

		static Vector3 minPos = new Vector3(-1.4f, -0.45f, -2.8f);
		static Vector3 maxPos = new Vector3(1.4f, 1.3f, 0.1f);
		static Vector3 popupMin = new Vector3(-7.7f, 0.45f, -1.65f);
		static Vector3 popupMax = new Vector3(7.7f, 0.45f, -1.45f);

		void CheckOut() {
			Vector3 _pos = transform.position;
			Vector2 _viewportPos = Player.instance.camera.WorldToViewportPoint(_pos);
			if (!_pos.IsInRange(minPos, maxPos) || !_viewportPos.IsInRange(Vector2.zero, Vector2.one)) {
				Popup();
			}
		}

		Vector3 startPos;
		Vector3 shootDir;
		public void Drop() {
			transform.SetParent(null);
			state = State.Idle;
			rigidbody.useGravity = true;
			trigger = false;
			rigidbody.AddForce(Vector3.forward * 10, ForceMode.Impulse);
		}

		internal void Shoot(Vector3 p_position) {
			transform.SetParent(null);
			state = State.Fly;
			trigger = true;
			transform.position = p_position;
			shootDir = Player.instance.transform.forward;
			startPos = transform.position;
			//var _nearAnchor = Player.instance.GetNearAnchor();
			//if (_nearAnchor != null) {
			//	transform.rotation = _nearAnchor.rotation;
			//	transform.Rotate(Vector3.right, 90);
			//} else {
				transform.rotation = Quaternion.identity;
			//}
			rigidbody.useGravity = false;
		}

		private void OnMouseDown() {
			if (state == State.Idle) {
				Player.instance.Pick(this);
			}
		}

		internal void OnPick() {
			state = State.WaitThrow;
			trigger = true;
			rigidbody.useGravity = false;
		}

		[Button]
		void ApplyCollider() {
			foreach (var _meshRenderer in GetComponentsInChildren<MeshRenderer>()) {
				foreach (var _collider in _meshRenderer.GetComponents<Collider>()) {
					DestroyImmediate(_collider);
				}
				var _meshCollider = _meshRenderer.AddComponent<MeshCollider>();
				_meshCollider.convex = true;
				var _boxCollider = _meshRenderer.AddComponent<BoxCollider>();
				_boxCollider.size = _boxCollider.size * 0.5f;
			}
		}
	}
}