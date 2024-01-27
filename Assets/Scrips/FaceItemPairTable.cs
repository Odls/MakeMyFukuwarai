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

	}
}