using UnityEngine.UI;
using SF = UnityEngine.SerializeField;

public class Stove : Box
{
    [SF] private Image[] fillBars;
    
    private void Update()
    {
        if (!IsWorking || placedItem is null) return;
        Work();
    }

    public override void AttachItem(Item item)
    {
        if (item.IsDone()) return;
        
        ActivateCanvas();
        base.AttachItem(item);
        IsWorking = true;
    }

    protected override void DetachItem()
    {
        DeactivateCanvas();
        base.DetachItem();
    }

    protected override void Work()
    {
        base.Work();
        float progress = placedItem.Handle();
        FillImg(progress);
    }
    
    protected override void ActivateCanvas()
    {
        base.ActivateCanvas();
        InitFillAmount();
    }

    private void InitFillAmount()
    {
        foreach (Image fillBar in fillBars) fillBar.fillAmount = 0;
    }
    
    private void FillImg(float ratio)
    {
        if (ratio >= fillBars.Length) return;
        fillBars[(int)ratio].fillAmount = ratio % 1;
    }
}
