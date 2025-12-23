using UnityEngine;
using UnityEngine.Pool;
using SF = UnityEngine.SerializeField;

public class ObjPool<T> : MonoBehaviour where T : Component
{
    public ObjectPool<T> Pool { get; private set; }

    private bool _isPrewarming;

    private int _objIdx;
    [SF] private string objName;
    [SF] private int poolMaxSize;
    [SF] private int poolCapacity;
    // collectionCheck; true 면 "중복 반납" 발생 시 에러 발생시켜줌...
    
    [SF] private T prefab;

    public void InitPool()
    {
        // mobPrefab = Resources.Load<GameObject>("Prefabs/Mob");
        Pool = new ObjectPool<T>(Create, OnGet, OnRelease, OnDestroyObj, true, poolCapacity,
            poolMaxSize);
        
        _objIdx = 0;
        
        Prewarm();
    }
    
    public void DisposePool()
    {
        Pool?.Dispose();
    }
    
    private void Prewarm()
    {
        _isPrewarming = true;

        for (int i = 0; i < poolCapacity; i++)
        {
            Pool.Release(Pool.Get());
        }
        
        _isPrewarming = false;
    }

    protected virtual T Create()
    {
        T mob = Instantiate(prefab, transform);
        mob.name = _isPrewarming ? $"{objName}_{_objIdx++}" : $"{objName}_Instant";
        return mob;
    }

    protected virtual void OnGet(T obj)
    {
        obj.gameObject.SetActive(true);
    }

    protected virtual void OnRelease(T obj)
    {
        obj.gameObject.SetActive(false);
    }

    private void OnDestroyObj(T obj)
    {
        Destroy(obj.gameObject);
    }
}
