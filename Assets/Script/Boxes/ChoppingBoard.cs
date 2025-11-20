using UnityEngine;
using SF = UnityEngine.SerializeField;

public class ChoppingBoard : Box
{
    [SF,Range(0f,1f)] private float scaleOffset; // 0.6 ?
    
    // 개발 중... 치즈가 공중에 좀 뜨는디... -> 얼추 맞췄음
    // 칼질은 홀드 액션이래 !!! 그럼 구분이 더 쉬울 수도?
    // 프레스는 아이템 들기, 홀드는 칼질 하면 되니,...
    
    public override void Interact()
    {
        // 아예 base는 호출하지 말아야 할지도? 조건이 너무 달라
        base.Interact();
        
        // 1. 플레이어가 재료를 들고있고, 도마에 재료가 없을 경우
        //      -> 도마에 재료 올리고 바로 칼질
        // 2. 플레이어가 재료를 들고있지 않고, 도마에 재료가 있을 경우
        //      손질된 재료 -> 플레이어가 들기
        //      손질되지 않은 재료 -> 칼질
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
