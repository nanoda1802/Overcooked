using System.Collections.Generic;
using UnityEngine;
using SF = UnityEngine.SerializeField;

public class MovableUIPool : MonoBehaviour, IPool<IngredientsInfo>
{
    [SF] private GameObject ingredientsInfoPrefab;
    [SF] private int ingredientsInfoPoolSize;
    private Queue<IngredientsInfo> _ingredientsInfoPool;
    
    private void Awake()
    {
        InitPool();
    }
    
    public bool TryGetItem(out IngredientsInfo ui)
    {
        if (_ingredientsInfoPool.TryDequeue(out ui)) return true;
        
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
    
    public void ReturnToPool(IngredientsInfo ui)
    {
        _ingredientsInfoPool.Enqueue(ui);
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
    }
}
