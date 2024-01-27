using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace MakeMyFukuwarai {

	[CreateAssetMenu]
	public class FaceItemTable : FaceItemTableBase<MeshRenderer> {
		internal override void AddToSet(HashSet<MeshRenderer> p_meshSet) {
			foreach (MeshRenderer _mesh in items) {
				p_meshSet.Add(_mesh);
			}
		}
	}
}