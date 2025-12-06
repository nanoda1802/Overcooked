using System.Collections.Generic;
using UnityEngine;
using SF = UnityEngine.SerializeField;

public class MovableUIPool : MonoBehaviour, IPool<IngredientsInfo>
{
    [SF] private GameObject ingredientsInfoPrefab;
    [SF] private int ingredientsInfoPoolSize;
    private Queue<IngredientsInfo> _ingredientsInfoPool;
    
    [SF] private GameObject respawnTimerPrefab;
    [SF] private int respawnTimerPoolSize;
    private Queue<RespawnTimer> _respawnTimerPool;
    
    private void Awake()
    {
        InitPool();
    }
    
    public bool TryGetItem(out IngredientsInfo ui)
    {
        if (_ingredientsInfoPool.TryDequeue(out IngredientsInfo poolUi))
        {
            ui = poolUi;
            return true;
        }
        
        GameObject uiObj = Instantiate(ingredientsInfoPrefab, transform);
        if (!uiObj.TryGetComponent(out IngredientsInfo instantUi))
        {
            Destroy(uiObj);
            ui = null;
            return false;
        }
        uiObj.name = $"IngredientsInfo_Instant";
        
        instantUi.Init(this);
        ui = instantUi;
        return true;
    }
    
    public bool TryGetItem(out RespawnTimer ui)
    {
        if (_respawnTimerPool.TryDequeue(out RespawnTimer poolUi))
        {
            ui = poolUi;
            return true;
        }
        
        GameObject uiObj = Instantiate(respawnTimerPrefab, transform);
        if (!uiObj.TryGetComponent(out RespawnTimer instantUi))
        {
            Destroy(uiObj);
            ui = null;
            return false;
        }
        uiObj.name = $"RespawnTimer_Instant";
        
        instantUi.Init(this);
        ui = instantUi;
        return true;
    }
    
    public void ReturnToPool(IngredientsInfo ui)
    {
        _ingredientsInfoPool.Enqueue(ui);
    }
    
    public void ReturnToPool(RespawnTimer ui)
    {
        _respawnTimerPool.Enqueue(ui);
    }
    
    public void InitPool()
    {
        _ingredientsInfoPool = new Queue<IngredientsInfo>(ingredientsInfoPoolSize);
        for (int i = 0; i < ingredientsInfoPoolSize; i++)
        {
            GameObject uiObj = Instantiate(ingredientsInfoPrefab, transform);
            if (!uiObj.TryGetComponent(out IngredientsInfo ui))
            {
                Destroy(uiObj);
                continue;
            }
            uiObj.name = $"IngredientsInfo_{i}";
            ui.Init(this);
            ui.Deactivate();
        }
        
        _respawnTimerPool = new Queue<RespawnTimer>(respawnTimerPoolSize);
        for (int i = 0; i < respawnTimerPoolSize; i++)
        {
            GameObject uiObj = Instantiate(respawnTimerPrefab, transform);
            if (!uiObj.TryGetComponent(out RespawnTimer ui))
            {
                Destroy(uiObj);
                continue;
            }
            uiObj.name = $"RespawnTimer_{i}";
            ui.Init(this);
            ui.Deactivate();
        }
    }
}
