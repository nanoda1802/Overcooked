using System;
using UnityEngine;
using SF = UnityEngine.SerializeField;

public class ChoppingBoard : WorkTable
{
    private event Action OnFinished;

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
        OnFinished += player.GetHandledItem;
        OnFinished += player.FinishWork;
        
        return true;
    }

    protected override void StopWork()
    {
        base.StopWork();
        OnFinished = null;
    }

    protected override void FinishWork()
    {
        OnFinished?.Invoke();
        base.FinishWork();
        
        DeactivateUI();
    }
}
