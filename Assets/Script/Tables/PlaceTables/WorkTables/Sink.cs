using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SF = UnityEngine.SerializeField;

public class Sink : WorkTable
{
    [SF] private DishRack dishRack;
    [SF] private Canvas sinkCanvas;
    [SF] private Text sinkText;
    private Action _onFinished;
    private readonly Queue<Item> _plates = new(10);

    private void OnTriggerEnter(Collider other)
    {
        if (!CheckTriggeredItem(other, out var item)) return;
        if (item is not Plate plate) return;
        if (plate.HasIngredient()) return;
        
        PlaceItem(item);
    }

    public override bool Interact(PlayerController player)
    {
        if (player.pickedItem is not Plate plate) return false;
        if (plate.HasIngredient()) return false;
        
        PlaceItem(player.DetachItem());
        return true;
    }
    
    public override void PlaceItem(Item item)
    {
        item.SetParent(pivot);
        item.gameObject.SetActive(false);
        item.IsPlaced = true;
        
        item.InitProgress();
        item.SetMaterial();
        
        _plates.Enqueue(item);

        if (!sinkCanvas.gameObject.activeSelf) ActivatePlateCount();
        UpdatePlateCount();
    }
    
    public override Item DisplaceItem() // 다시 오자
    {
        dishRack.PlaceItem(placedItem);
        placedItem = null;
        
        UpdatePlateCount();
        if(_plates.Count <= 0) DeactivatePlateCount();
        
        return null;
    }
    
    public override bool BeginWork(PlayerController player)
    {
        if (placedItem is null) 
        {
            if (!_plates.TryDequeue(out placedItem)) return false;
            placedItem.gameObject.SetActive(true);
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
        sinkText.text = $"{_plates.Count}";
    }
}
