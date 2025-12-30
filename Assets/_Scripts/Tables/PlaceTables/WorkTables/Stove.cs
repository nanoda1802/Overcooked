using UnityEngine;
using SF = UnityEngine.SerializeField;

public class Stove : WorkTable
{
    // [SF] private ParticleSystem fireVfx;
    
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
        // if (item.IsMaxDone())
        // {
        //     item.ActivatePhysics();
        //     return;
        // }
        base.PlaceItem(item);
        BeginWork();
        ActivateUI();
    }

    public override Item DisplaceItem()
    {
        DeactivateUI();
        StopWork();
        // StopFireVfxSmoothly();
        return base.DisplaceItem();
    }

    public override bool BeginWork(PlayerController_Stage player = null)
    {
        bool hasBegun = base.BeginWork(player);
        // if (hasBegun) PlayFireVfx();
        return hasBegun;
    }

    protected override void StopWork()
    {
        base.StopWork();
    }

    protected override void FinishWork()
    {
        // 근데 이거 maxDone 기준이라 finish는 다 탔을 때 호출되는디
        base.FinishWork();
    }
    
    // private void PlayFireVfx()
    // {
    //     if (fireVfx is null) return;
    //     if (fireVfx.isPlaying) StopFireVfxSmoothly();
    //     fireVfx.Play();
    // }
    //
    // private void StopFireVfxSmoothly()
    // {
    //     fireVfx?.Stop(true, ParticleSystemStopBehavior.StopEmitting);
    // }
}
