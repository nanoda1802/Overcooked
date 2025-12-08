using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SF = UnityEngine.SerializeField;

public interface IPoolable
{
    public void Activate();
    public void Deactivate();
}

public interface IPoolBase
{
    public void InitPool();
}

public interface IPool<T> : IPoolBase
{
    public bool TryGetItem(out T poolable);
    public void ReturnToPool(T poolable);
}

public class PoolManager : MonoBehaviour
{
    // 현 스테이지의 모든 pool을 알고 있어야 해
    // 어떻게 알고 있지...? 흠....
    // StageManager의 호출을 통해 모든 pool의 Init을 해줘야해
    private List<IPoolBase> _pools;

    public void Init() // 흠...
    {
        foreach (IPoolBase pool in _pools)    
        {
            pool.InitPool();
        }
    }
}
