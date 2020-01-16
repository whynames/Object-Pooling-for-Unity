using UnityEngine;
using UnityEngine.Events;

namespace ToolBox.Pools
{
	[DisallowMultipleComponent]
	public class Poolable : MonoBehaviour
	{
		public Pool Pool { get; private set; } = null;

		[SerializeField] private UnityEvent OnBackToPool = null;
		[SerializeField] private UnityEvent OnBackFromPool = null;

		private bool isPooled = false;
		private bool isEnabled = true;
		private bool wasSpawnedBefore = false;

		public void ReturnToPool()
		{
			if (!isEnabled)
				return;

			OnBackToPool?.Invoke();

			Pool.ReturnEntity(this);
			isEnabled = false;
		}

		public void ReturnFromPool()
		{
			if (wasSpawnedBefore)
				OnBackFromPool?.Invoke();
			else
				wasSpawnedBefore = true;

			isEnabled = true;
		}

		public void SetPool(Pool pool)
		{
			if (!isPooled)
			{
				Pool = pool;
				isPooled = true;
			}
		}
	}
}
