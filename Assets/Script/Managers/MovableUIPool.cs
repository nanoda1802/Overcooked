using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SF = UnityEngine.SerializeField;

public class MovableUIPool : MonoBehaviour
{
    [SF] private GameObject ingredientsInfoPrefab;
    [SF] private int poolSize;
    private Queue<IngredientsInfo> _pool;
    
    private void Awake()
    {
        InitPool();
    }
    
    public bool TryGetUI(out IngredientsInfo ui)
    {
        if (_pool.TryDequeue(out IngredientsInfo poolUI))
        {
            ui = poolUI;
            return true;
        }
        
        GameObject uiObj = Instantiate(ingredientsInfoPrefab, transform);
        if (!uiObj.TryGetComponent(out IngredientsInfo instantUI))
        {
            Destroy(uiObj);
            ui = null;
            return false;
        }
        uiObj.name = $"IngredientsInfo_Instant";
        
        instantUI.Init(this);
        ui = instantUI;
        return true;
    }
    
    public void ReturnToPool(IngredientsInfo ui)
    {
        _pool.Enqueue(ui);
    }
    
    private void InitPool()
    {
        _pool = new Queue<IngredientsInfo>(poolSize);
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
