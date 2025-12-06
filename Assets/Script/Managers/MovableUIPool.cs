using System.Collections.Generic;
using UnityEngine;
using SF = UnityEngine.SerializeField;

public class MovableUIPool : MonoBehaviour, IPool<IngredientsInfo>
{
    [SF] private GameObject ingredientsInfoPrefab;
    [SF] private int poolSize;
    private Queue<IngredientsInfo> _ingredientInfoPool;
    
    // RespawnTimer UI 추가 예정
    
    private void Awake()
    {
        InitPool();
    }
    
    public bool TryGetItem(out IngredientsInfo ui)
    {
        if (_ingredientInfoPool.TryDequeue(out IngredientsInfo poolUi))
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
    
    public void ReturnToPool(IngredientsInfo ui)
    {
        _ingredientInfoPool.Enqueue(ui);
    }
    
    public void InitPool()
    {
        _ingredientInfoPool = new Queue<IngredientsInfo>(poolSize);
        for (int i = 0; i < poolSize; i++)
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
