using UnityEngine;

namespace ToolBox.Pools
{
    [DefaultExecutionOrder(-9999), DisallowMultipleComponent]
    internal sealed class PoolInstaller : MonoBehaviour
    {
        [SerializeField] private PoolContainer[] _pools;

        private void Awake()
        {
            for (var i = 0; i < _pools.Length; i++)
                _pools[i].Populate();
        }

        [System.Serializable]
        private struct PoolContainer
        {
            [SerializeField] private GameObject _prefab;
            [Sirenix.OdinInspector.PropertyRange(0, "@isMaxCapacity? _maxCapacity : 100 "), Sirenix.OdinInspector.PropertyOrder(3)]
            [Sirenix.OdinInspector.InfoBox("Start Count must be lower than Max Capacity", "@isMaxCapacity && (_maxCapacity < $value)")]
            [SerializeField, Min(1)] private int _startCount;
            [SerializeField] private bool isMaxCapacity;

            [Sirenix.OdinInspector.EnableIf("isMaxCapacity")]
            [SerializeField, Min(1)] private int _maxCapacity;

            public void Populate()
            {

                if (isMaxCapacity)
                {
                    _prefab.PopulateWithMaxCap(_startCount, _maxCapacity);
                    return;
                }
                _prefab.Populate(_startCount);
            }

            public void ReUse()
            {
                GameObject p = _prefab.Reuse();
                p.transform.position = Vector3.zero;
            }
        }

        private void OnGUI()
        {
            if (GUI.Button(new Rect(50, 100, 75, 75), "Click"))
            {
                foreach (var item in _pools)
                {
                    item.ReUse();
                }
                // var options = new ModalOptions(ResourceKey.TopScreenPrefab(), true, loadAsync: true, poolingPolicy: ZBase.UnityScreenNavigator.Core.PoolingPolicy.EnablePooling);
                // Launcher.ContainerManager.Find<ModalContainer>(ContainerKey.Modals).PushAsync(options);
            }
        }
    }
}
