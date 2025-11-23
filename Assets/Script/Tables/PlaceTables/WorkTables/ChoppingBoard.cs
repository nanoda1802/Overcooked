using System;
using UnityEngine;
using SF = UnityEngine.SerializeField;

public class ChoppingBoard : WorkTable
{
    private Action _onFinished;

    private void OnTriggerEnter(Collider other)
    {
        if (placedItem is not null || !CheckTriggeredItem(other, out var item)) return;
        PlaceItem(item);
    }

    protected override void PlaceItem(Item item)
    {
        if (item.IsDone())
        {
            item.ActivatePhysics();
            return;
        }
        base.PlaceItem(item);
    }

    protected override Item DisplaceItem()
    {
        DeactivateUI();
        return base.DisplaceItem();
    }

    public override bool BeginWork(PlayerController player)
    {
        bool hasBegun = base.BeginWork(player);
        if (hasBegun && !canvas.gameObject.activeSelf)
        {
            ActivateUI();
            player.OnWorkStopped += StopWork;
            _onFinished += player.GetHandledItem;
            _onFinished += player.FinishWork;
            player.handledItem = placedItem;
        }
        return hasBegun;
    }

    protected override void StopWork()
    {
        base.StopWork();
        _onFinished = null;
    }

    protected override void FinishWork()
    {
        DeactivateUI();
        _onFinished?.Invoke();
        _onFinished = null;
        base.FinishWork();
    }
}
