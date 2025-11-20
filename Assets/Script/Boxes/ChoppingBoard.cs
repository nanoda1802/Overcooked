using System;
using UnityEngine;
using SF = UnityEngine.SerializeField;

public class ChoppingBoard : Box
{
    [SF,Range(0f,1f)] private float scaleOffset; // 0.6 ?
    [SF] private bool isWorking;
    private Action _onStopped;
    
    private void Update()
    {
        if (!isWorking) return;
        Work();
    }

    public override bool BeginWork(PlayerController player)
    {
        if (placedItem is null) return false;
        
        Debug.Log("Begin Work");
        // 도마에 아이템이 있으면! 작업 상태로 전환!
        isWorking = true;
        _onStopped = player.StopWork;
        return true;
    }

    public override void StopWork(PlayerController player)
    {
        Debug.Log("Stop Work");
        // 작업X 상태로 전환!
        isWorking = false;
    }

    private void Work()
    {
        bool isDone = placedItem.Cook();
        isWorking = !isDone;
        if (isDone) _onStopped.Invoke();
    }

    protected override void AttachItem(Item item)
    {
        base.AttachItem(item);
        pivot.localScale *= scaleOffset;
    }

    protected override void DetachItem()
    {
        pivot.localScale = Vector3.one;
        base.DetachItem();
    }
}
