using System;
using UnityEngine;
using SF = UnityEngine.SerializeField;

public class ChoppingBoard : WorkTable
{
    private Action _onFinished;

    private void OnTriggerEnter(Collider other)
    {
        if (placedItem is not null) return;
        if (!CheckTriggeredItem(other, out var item)) return;
        if (item.IsMaxDone()) return;
        
        PlaceItem(item);
    }

    public override bool Interact(InStagePlayerController player)
    {
        if (player.pickedItem is not null && player.pickedItem.IsMaxDone()) return false;
        return base.Interact(player);
    }

    public override void PlaceItem(Item item)
    {
        // if (item.IsMaxDone())
        // {
        //     item.ActivatePhysics();
        //     return;
        // }
        base.PlaceItem(item);
    }

    public override Item DisplaceItem()
    {
        DeactivateUI();
        return base.DisplaceItem();
    }

    public override bool BeginWork(InStagePlayerController player = null)
    {
        if (player is null) return false;
        if (placedItem is null) return false;
        base.BeginWork();
        
        if (!fillBarCanvas.gameObject.activeSelf) ActivateUI();
        
        player.OnWorkStopped += StopWork;
        _onFinished += player.GetHandledItem;
        _onFinished += player.FinishWork;
        
        return true;
    }

    protected override void StopWork()
    {
        base.StopWork();
        _onFinished = null;
    }

    protected override void FinishWork()
    {
        _onFinished?.Invoke();
        base.FinishWork();
        
        DeactivateUI();
    }
}
