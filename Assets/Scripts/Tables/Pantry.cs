using System.Collections.Generic;
using UnityEngine;
using SF = UnityEngine.SerializeField;

public class Pantry : Table, IPool<Item>
{
    [SF] private ItemType type;
    [SF] private GameObject[] items;
    [SF] private Transform poolPivot;
    [SF] private int poolSize;
    private Queue<Item> _pool;

    private void Awake()
    {
        InitPool();
    }

    private void Start()
    {
        pivot.GetChild((int)type).gameObject.SetActive(true);
    }

    public override bool Interact(InStagePlayerController player)
    {
        if (player.pickedItem is not null) return false;
        if (!TryGetItem(out Item item)) return false;
        
        item.Activate();
        player.AttachItem(item);
        
        return true;
    }

    public void InitPool()
    {
        _pool = new Queue<Item>(poolSize);
        for (int i = 0; i < poolSize; i++)
        {
            GameObject itemObj = Instantiate(items[(int)type], poolPivot);
            if (!itemObj.TryGetComponent(out Item item))
            {
                Destroy(itemObj);
                continue;
            }
            itemObj.name = $"{type}_{i}";
            item.InitComponents(this);
            item.Deactivate();
        }
    }

    public bool TryGetItem(out Item item)
    {
        if (_pool.TryDequeue(out Item poolItem))
        {
            item = poolItem;
            return true;
        }
        
        GameObject itemObj = Instantiate(items[(int)type]);
        if (!itemObj.TryGetComponent(out Item instantiatedItem))
        {
            Destroy(itemObj);
            item = null;
            return false;
        }
        
        itemObj.name = $"{type}_Instant";
        instantiatedItem.InitComponents(this);
        item = instantiatedItem;
        return true;
    }

    public void ReturnToPool(Item item)
    {
        item.SetParent(poolPivot);
        _pool.Enqueue(item);
    }
}