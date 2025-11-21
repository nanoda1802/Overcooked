using System;
using UnityEngine;
using SF = UnityEngine.SerializeField;

public class ChoppingBoard : Box
{
    private Action _onStopped;
    
    private void Update()
    {
        if (!IsWorking || placedItem is null) return;
        Work();
    }

    protected override void AttachItem(Item item)
    {
        if (item.IsDone()) return;
        
        base.AttachItem(item);
    }

    public override bool BeginWork(PlayerController player)
    {
        if (placedItem is null) return false;
        Debug.Log("Begin Work");
        IsWorking = true;
        _onStopped = player.StopWork; // ??
        return IsWorking;
    }

    public override void StopWork(PlayerController player)
    {
        Debug.Log("Stop Work");
        IsWorking = false;
        _onStopped = null;
    }

    private void Work()
    {
        placedItem.Cook();
        IsWorking = !placedItem.IsDone();
        if (!IsWorking)
        {
            _onStopped?.Invoke();
            Debug.Log($"{placedItem.name} 조리 완료!");
        }
    }
}
