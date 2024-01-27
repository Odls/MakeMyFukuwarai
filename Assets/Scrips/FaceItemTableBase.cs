using Sirenix.OdinInspector;

using System.Collections.Generic;
using UnityEngine;

namespace MakeMyFukuwarai {
	public abstract class FaceItemTableBase<T> : ScriptableObject {
		//[TableList]
		public List<T> items;

		public T GetRandomItem() {
			return items[Random.Range(0, items.Count)];
		}


	}
}