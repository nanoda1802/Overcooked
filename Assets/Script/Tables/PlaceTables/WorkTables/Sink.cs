using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SF = UnityEngine.SerializeField;

public class Sink : WorkTable, IPool<Item>
{
    [SF] private DishRack dishRack;
    [SF] private Canvas sinkCanvas;
    [SF] private Text sinkText;
    private Action _onFinished;

    [SF] private GameObject platePrefab;
    [SF] private Transform poolPivot;
    [SF] private int poolSize;
    private Queue<Item> _pool;

    private void Awake()
    {
        InitPool();
    }

    private void Start()
    {
        UpdatePlateCount();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!CheckTriggeredItem(other, out var item)) return;
        if (item is not Plate plate) return;
        if (plate.HasIngredient()) return;
        if (IsFull()) return;
        
        PlaceItem(item);
    }

    public override bool Interact(PlayerController player)
    {
        if (player.pickedItem is not Plate plate) return false;
        if (plate.HasIngredient()) return false;
        if (IsFull()) return false;
        
        PlaceItem(player.DetachItem());
        return true;
    }
    
    public override void PlaceItem(Item item)
    {
        if (item is not Plate plate) return;
        
        plate.IsPlaced = true;
        
        plate.Deactivate();

        if (!sinkCanvas.gameObject.activeSelf) ActivatePlateCount();
        UpdatePlateCount();
    }
    
    public override Item DisplaceItem()
    {
        dishRack.PlaceItem(placedItem);
        placedItem = null;
        
        UpdatePlateCount();
        if(_pool.Count <= 0) DeactivatePlateCount();
        
        return null;
    }
    
    public override bool BeginWork(PlayerController player)
    {
        if (placedItem is null) 
        {
            if (!TryGetItem(out placedItem)) return false;
            placedItem.Activate();
            ActivateUI();
        }

        IsWorking = true;
        
        player.OnWorkStopped += StopWork;
        _onFinished = player.FinishWork;
        
        return true;
    }

    protected override void StopWork()
    {
        base.StopWork();
        _onFinished = null;
    }

    protected override void FinishWork()
    {
        _onFinished.Invoke();
        StopWork();
        
        DeactivateUI();
        DisplaceItem();
    }

    private void ActivatePlateCount() // [임시]
    {
        sinkCanvas?.gameObject.SetActive(true);
    }

    private void DeactivatePlateCount() // [임시]
    {
        sinkCanvas?.gameObject.SetActive(false);
    }
    
    private void UpdatePlateCount() // [임시]
    {
        sinkText.text = $"{_pool.Count}";
    }

    private bool IsFull()
    {
        return _pool.Count >= poolSize;
    }

    public void InitPool()
    {
        _pool = new Queue<Item>(poolSize);
        for (int i = 0; i < poolSize; i++)
        {
            GameObject plateObj = Instantiate(platePrefab, poolPivot);
            if (!plateObj.TryGetComponent(out Plate plate))
            {
                Destroy(plateObj);
                continue;
            }
            plateObj.name = $"Plate_{i}";
            plate.InitComponents(this);
            plate.Deactivate();
        }
    }

    public bool TryGetItem(out Item item)
    {
        if (_pool.TryDequeue(out Item poolItem))
        {
            item = poolItem;
            return true;
        }

        item = null;
        return false;
    }

    public void ReturnToPool(Item item)
    {
        item.SetParent(poolPivot);
        _pool.Enqueue(item);
        UpdatePlateCount();
    }
}
