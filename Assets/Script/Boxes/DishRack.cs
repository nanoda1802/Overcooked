using System.Collections.Generic;
using UnityEngine;
using SF = UnityEngine.SerializeField;

public class DishRack : MonoBehaviour, IInteractable
{
    // Sink에서 설거지된 접시가 쌓임
    // Pantry와 유사하지만 보유 중인 접시가 없다면 접시를 주지 않음
    // 일정 offset 만큼 이동해 꽂히도록 해야함! (최대 다섯장이니까...)
    // y축으로 + 이동임
    
    [SF] private ItemType type;
    [SF] private Transform pivot;
    [SF] private float offset;
    private readonly Stack<Item> _plates = new();
    
    public bool Interact(PlayerController player)
    {
        if (player.pickedItem is not null || _plates.Count == 0) return false;
        player.AttachItem(_plates.Pop());
        return true;
    }

    public void AttachItem(Item item)
    {
        if (item.type is not ItemType.Plate) return;
        item.SetParent(pivot);
        Vector3 pos = _plates.TryPeek(out Item peekPlate) ? peekPlate.transform.localPosition : Vector3.zero;
        pos.y += offset;
        item.transform.localPosition = pos;
        _plates.Push(item);
    }

    public bool BeginWork(PlayerController player) { return false; }
    public void StopWork(PlayerController player) { }
}
