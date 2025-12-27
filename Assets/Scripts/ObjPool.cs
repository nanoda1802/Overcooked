using UnityEngine;
using UnityEngine.Pool;
using SF = UnityEngine.SerializeField;

public class ObjPool<T> : MonoBehaviour where T : Component
{
    public ObjectPool<T> Pool { get; private set; }
    
    protected bool isPrewarming;

    protected int objIdx;
    [SF] protected string objName;
    [SF] protected bool collectionCheck = true;
    [SF] protected int defaultCapacity = 10;
    [SF] protected int maxPoolSize = 20;
    public int MaxPoolSize => maxPoolSize;
    // collectionCheck; true 면 "중복 반납" 발생 시 에러 발생시켜줌...
    
    [SF] private T prefab;

    public virtual void InitPool()
    {
        // mobPrefab = Resources.Load<GameObject>("Prefabs/Mob");
        Pool = new ObjectPool<T>(CreateObj, OnGot, OnReleased, OnDestroyed, collectionCheck, defaultCapacity,
            maxPoolSize);
        objIdx = 0;
        Prewarm();
    }
    
    public void DisposePool()
    {
        Pool?.Dispose();
    }
    
    private void Prewarm()
    {
        isPrewarming = true;
        
        T[] prePool = new T[defaultCapacity];
        for (int i = 0; i < defaultCapacity; i++) prePool[i] = Pool.Get();
        for (int i = 0; i < defaultCapacity; i++) Pool.Release(prePool[i]);
        
        isPrewarming = false;
    }

    protected virtual T CreateObj()
    {
        T mob = Instantiate(prefab, transform);
        mob.name = isPrewarming ? $"{objName}_{objIdx}" : $"{objName}_{objIdx}_Instant";
        mob.gameObject.SetActive(false);
        return mob;
    }

    protected virtual void OnGot(T obj)
    {
        obj.gameObject.SetActive(true);
    }

    protected virtual void OnReleased(T obj)
    {
        obj.gameObject.SetActive(false);
    }

    private void OnDestroyed(T obj)
    {
        Destroy(obj.gameObject);
    }
}
