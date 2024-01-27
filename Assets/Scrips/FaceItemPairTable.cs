using Sirenix.OdinInspector;

using System.Collections.Generic;

using UnityEngine;

namespace MakeMyFukuwarai {

	[CreateAssetMenu]
	public class FaceItemPairTable : FaceItemTableBase<FaceItemPairTable.FaceItemPair> {

		[System.Serializable]
		public struct FaceItemPair {
			public MeshRenderer left;
			public MeshRenderer right;
		}

		internal override void AddToSet(HashSet<MeshRenderer> p_meshSet) {
			foreach (FaceItemPair _pair in items) {
				p_meshSet.Add(_pair.left);
				p_meshSet.Add(_pair.right);
			}
		}
	}
}