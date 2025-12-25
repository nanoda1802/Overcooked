using UnityEngine;
using UnityEngine.Pool;
using SF = UnityEngine.SerializeField;

public class ObjPool<T> : MonoBehaviour where T : Component
{
    public ObjectPool<T> Pool { get; private set; }
    
    private bool _isPrewarming;

    protected int objIdx;
    [SF] private string objName;
    [SF] private int poolSize;
    public int PoolSize => poolSize;
    // collectionCheck; true 면 "중복 반납" 발생 시 에러 발생시켜줌...
    
    [SF] private T prefab;

    public virtual void InitPool()
    {
        // mobPrefab = Resources.Load<GameObject>("Prefabs/Mob");
        Pool = new ObjectPool<T>(Create, OnGet, OnRelease, OnDestroyObj, true, poolSize,
            poolSize);
        
        objIdx = 0;
        
        Prewarm();
    }
    
    public void DisposePool()
    {
        Pool?.Dispose();
    }
    
    private void Prewarm()
    {
        _isPrewarming = true;
        
        T[] prePool = new T[poolSize];
        for (int i = 0; i < poolSize; i++) prePool[i] = Pool.Get();
        for (int i = 0; i < poolSize; i++) Pool.Release(prePool[i]);
        
        _isPrewarming = false;
    }

    protected virtual T Create()
    {
        T mob = Instantiate(prefab, transform);
        mob.name = _isPrewarming ? $"{objName}_{objIdx}" : $"{objName}_Instant";
        mob.gameObject.SetActive(false);
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
