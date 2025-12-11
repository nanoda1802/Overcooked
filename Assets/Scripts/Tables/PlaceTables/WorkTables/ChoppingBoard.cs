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

        PlaceItem(item);
    }

    public override void PlaceItem(Item item)
    {
        if (item.IsMaxDone())
        {
            item.ActivatePhysics();
            return;
        }
        base.PlaceItem(item);
    }

    public override Item DisplaceItem()
    {
        DeactivateUI();
        return base.DisplaceItem();
    }

    public override bool BeginWork(InStagePlayerController inStagePlayer)
    {
        if (placedItem is null) return false;
        
        IsWorking = true;
        if (!fillBarCanvas.gameObject.activeSelf) ActivateUI();
        
        inStagePlayer.OnWorkStopped += StopWork;
        _onFinished += inStagePlayer.GetHandledItem;
        _onFinished += inStagePlayer.FinishWork;
        
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
        StopWork();
        
        DeactivateUI();
    }
}
