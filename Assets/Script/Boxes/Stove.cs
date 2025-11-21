using UnityEngine;
using SF = UnityEngine.SerializeField;

public class Stove : Box
{
    private void Update()
    {
        if (!IsWorking || placedItem is null) return;
        Work();
    }

    protected override void AttachItem(Item item)
    {
        if (item.IsDone()) return;
        
        base.AttachItem(item);
        IsWorking = true;
        Debug.Log("Begin Work");
    }

    private void Work()
    {
        placedItem.Cook();
        IsWorking = !placedItem.IsDone();
        if (!IsWorking) Debug.Log($"{placedItem.name} 조리 완료!");
    }
}
