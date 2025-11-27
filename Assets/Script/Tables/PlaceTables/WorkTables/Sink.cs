using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SF = UnityEngine.SerializeField;

public class Sink : WorkTable
{
    // 편의상 깨끗한 접시 넣어도 같은 동작 (누가 설거지 두 번하래?)
    // 플레이어가 직접 갖다 넣거나, 던지거나, 음식이 제출되고 일정 시간이 지나면 Sink에 접시 들어옴
    // 들어온 접시는 피벗에 자식으로 들어가고 비활성화
    // 이때 접시의 doneness를 undone으로 변경하고 item 컴포넌트를 Sink의 큐에 enqueue 
    // UI로 현재 접시 개수 갱신 (큐의 count)
    // 큐에 접시가 한개 이상 있을 때, 홀드 상호작용 시도하면 dequeue된 접시가 placedItem에 할당
    // Cook하듯이 진행, 작업 완료되면 접시 오브젝트 활성화 시키고 DishRack의 pivot으로 옮김

    [SF] private DishRack dishRack;
    [SF] private Canvas sinkCanvas;
    [SF] private Text sinkText;
    private Action _onFinished;
    private readonly Queue<Item> _plates = new(10);

    private void OnTriggerEnter(Collider other)
    {
        if (!CheckTriggeredItem(other, out var item)) return;
        if (item is not Plate plate) return;
        if (plate.HasIngredient()) return;
        
        PlaceItem(item);
    }

    public override bool Interact(PlayerController player)
    {
        if (player.pickedItem is not Plate plate) return false;
        if (plate.HasIngredient()) return false;
        
        PlaceItem(player.DetachItem());
        return true;
    }
    
    public override void PlaceItem(Item item)
    {
        item.SetParent(pivot);
        item.IsPlaced = true;
        item.gameObject.SetActive(false);
        
        item.InitProgress();
        item.SetMaterial();
        
        _plates.Enqueue(item);

        if (!sinkCanvas.gameObject.activeSelf) ActivatePlateCount();
        UpdatePlateCount();
    }
    
    public override Item DisplaceItem() // 다시 오자
    {
        dishRack.PlaceItem(placedItem);
        placedItem = null;
        
        UpdatePlateCount();
        if(_plates.Count <= 0) DeactivatePlateCount();
        
        return null;
    }
    
    public override bool BeginWork(PlayerController player)
    {
        if (placedItem is null) 
        {
            if (!_plates.TryDequeue(out placedItem)) return false;
            placedItem.gameObject.SetActive(true);
            ActivateUI();
        }

        IsWorking = true;
        
        player.OnWorkStopped += StopWork;
        _onFinished = player.FinishWork;
        
        return true;
    }

    protected override void StopWork()
    {
        base.StopWork();
        _onFinished = null;
    }

    protected override void FinishWork()
    {
        _onFinished.Invoke();
        StopWork();
        
        DeactivateUI();
        DisplaceItem();
    }

    private void ActivatePlateCount() // [임시]
    {
        sinkCanvas?.gameObject.SetActive(true);
    }

    private void DeactivatePlateCount() // [임시]
    {
        sinkCanvas?.gameObject.SetActive(false);
    }
    
    private void UpdatePlateCount() // [임시]
    {
        sinkText.text = $"{_plates.Count}";
    }
}
