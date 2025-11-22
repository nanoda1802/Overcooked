using System;
using UnityEngine.UI;
using SF = UnityEngine.SerializeField;

public class ChoppingBoard : Box
{
    private Action _onStopped;
    [SF] private Image fillBar;

    private void Update()
    {
        if (!IsWorking || placedItem is null) return;
        Work();
    }

    public override void AttachItem(Item item)
    {
        if (item.IsDone()) return;
        base.AttachItem(item);
    }

    protected override void DetachItem()
    {
        DeactivateCanvas();
        base.DetachItem();
    }

    public override bool BeginWork(PlayerController player)
    {
        if (placedItem is null) return false;
        if (!canvas.gameObject.activeSelf) ActivateCanvas();
        IsWorking = true;
        _onStopped = player.StopWork; // ??
        return true;
    }

    public override void StopWork(PlayerController player)
    {
        IsWorking = false;
        _onStopped = null;
    }

    protected override void Work()
    {
        base.Work();
        float progress = placedItem.Handle();
        FillImg(progress);
        if (!IsWorking) _onStopped?.Invoke();
    }
    
    protected override void ActivateCanvas()
    {
        base.ActivateCanvas();
        InitFillAmount();
    }

    private void InitFillAmount()
    {
        fillBar.fillAmount = 0;
    }

    private void FillImg(float ratio)
    {
        if (fillBar is null) return;
        fillBar.fillAmount = ratio;
    }
}
