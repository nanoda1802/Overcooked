using UnityEngine;
using SF = UnityEngine.SerializeField;

public class ChoppingBoard : WorkTable
{
    [SF] private Transform knife;
    [SF] private Vector3 knifeLocalPos;
    
    private void OnTriggerEnter(Collider other)
    {
        if (placedItem is not null) return;
        if (!CheckTriggeredItem(other, out var item)) return;
        if (item.IsMaxDone()) return;
        
        PlaceItem(item);
    }

    public override bool Interact(PlayerController_Stage player)
    {
        if (player.pickedItem is not null && player.pickedItem.IsMaxDone()) return false;
        return base.Interact(player);
    }

    public override void PlaceItem(Item item)
    {
        base.PlaceItem(item);
        knife.gameObject.SetActive(false);
    }

    public override Item DisplaceItem()
    {
        DeactivateUI();
        return base.DisplaceItem();
    }

    public override bool BeginWork(PlayerController_Stage player = null)
    {
        if (player is null) return false;
        if (placedItem is null) return false;
        
        base.BeginWork();
        
        if (!fillBarCanvas.gameObject.activeSelf) ActivateUI();
        
        player.GrabKnife(knife);
        
        player.PlayAnim(animHash);
        OnStopped += () => player.StopAnim(animHash);
        OnStopped += ReturnKnife;
        
        player.OnWorkStopped += StopWork;
        OnFinished += player.GetHandledItem;
        OnFinished += player.FinishWork;
        
        return true;
    }

    protected override void StopWork()
    {
        base.StopWork();
    }

    protected override void FinishWork()
    {
        base.FinishWork();
        DeactivateUI();
    }

    private void ReturnKnife()
    {
        knife.SetParent(transform);
        knife.SetLocalPositionAndRotation(knifeLocalPos, Quaternion.identity);
    }
}
