using System.Linq;

public class Countertop : Box
{
    // item이 attach 됨
    // plate -> 다른 박스와 똑같이 작동
    // others -> plate의 pivot에 재료 추가, 원본 오브젝트는... 일단 비활성화?

    // placedItem is null 조건식 하나 때문에 오버라이드는 좀 아닌 거 같아...
    public override bool Interact(PlayerController player)
    {
        if (player.pickedItem is null && placedItem is not null)
        {
            Item item = placedItem; // detach에서 참조를 끊어서 필요한 변수
            DetachItem();
            player.AttachItem(item);
            return true;
        }
        
        if (player.pickedItem is not null && availableItems.Contains(player.pickedItem.type))
        {
            Item item = player.pickedItem; // detach에서 참조를 끊어서 필요한 변수
            player.DetachItem();
            AttachItem(item);
            return true;
        }
        
        return false;
    }

    public override void AttachItem(Item item)
    {
        if (item is Plate)
        {
            if (item.IsDone()) base.AttachItem(item);
        }
        else
        {
            if (placedItem is not Plate plate) return;
            item.SetParent(pivot); // [임시]
            item.gameObject.SetActive(false); // [임시]
            plate.StackIngredient(item);
        }
    }
}
