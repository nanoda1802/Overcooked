using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using SF = UnityEngine.SerializeField;

public class Sink : Box
{
    // 편의상 깨끗한 접시 넣어도 같은 동작 (누가 설거지 두 번하래?)
    // 플레이어가 직접 갖다 넣거나, 던지거나, 음식이 제출되고 일정 시간이 지나면 Sink에 접시 들어옴
    // 들어온 접시는 피벗에 자식으로 들어가고 비활성화
    // 이때 접시의 doneness를 undone으로 변경하고 item 컴포넌트를 Sink의 큐에 enqueue 
    // UI로 현재 접시 개수 갱신 (큐의 count)
    // 큐에 접시가 한개 이상 있을 때, 홀드 상호작용 시도하면 dequeue된 접시가 placedItem에 할당
    // Cook하듯이 진행, 작업 완료되면 접시 오브젝트 활성화 시키고 DishRack의 pivot으로 옮김

    private Action _onStopped;
    private readonly Queue<Item> _plates = new();
    [SF] private Image fillBar;
    [SF] private DishRack dishRack;
    private void Update()
    {
        if (!IsWorking || placedItem is null) return;
        Work();
    }
    
    public override void Interact(PlayerController player)
    {
        if (player.pickedItem is null) return;
        
        if (availableItems.Contains(player.pickedItem.type))
        {
            Item item = player.pickedItem; // detach에서 참조를 끊어서 필요한 변수
            player.DetachItem();
            AttachItem(item);
        }
    }

    protected override void AttachItem(Item item)
    {
        // Sink 만의 attach
        if (item.type is not ItemType.Plate) return;
        item.SetParent(pivot);
        item.gameObject.SetActive(false);
        item.InitProgress();
        _plates.Enqueue(item);
        Debug.Log($"현재 안 닦은 접시 개수 : {_plates.Count}");
    }

    protected override void DetachItem()
    {
        // Sink 만의 Detach
        DeactivateCanvas();
        dishRack.AttachItem(placedItem);
        placedItem = null;
        Debug.Log($"현재 안 닦은 접시 개수 : {_plates.Count}");
    }
    
    public override bool BeginWork(PlayerController player)
    {
        if (_plates.Count == 0 && placedItem is null) return false;
        if (!canvas.gameObject.activeSelf) ActivateCanvas();
        if (placedItem is null) 
        {
            placedItem = _plates.Dequeue(); // ??= 라는 연산자로 가능한가봐
            placedItem.gameObject.SetActive(true);
        }
        
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
        if (!IsWorking)
        {
            _onStopped?.Invoke();
            DetachItem();
        }
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
