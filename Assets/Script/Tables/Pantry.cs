using System.Collections.Generic;
using UnityEngine;
using SF = UnityEngine.SerializeField;

public interface IPoolable
{
    public void Activate();
    public void Deactivate();
}

public class Pantry : Table
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

    public override bool Interact(PlayerController player)
    {
        if (player.pickedItem is not null) return false;
        if (!TryGetItem(out Item item)) return false;
        
        item.Activate();
        player.AttachItem(item);
        
        return true;
    }

    private void InitPool()
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

    private bool TryGetItem(out Item item)
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
        if (item.type != type)
        {
            Destroy(item.gameObject);
            return;
        }
        item.SetParent(poolPivot);
        _pool.Enqueue(item);
    }
}