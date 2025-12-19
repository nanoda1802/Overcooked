using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using SF = UnityEngine.SerializeField;

public class CustomSlider : MonoBehaviour, IBeginDragHandler,IDragHandler,IEndDragHandler,IScrollHandler
{
    [Range(0,1)] private float _value;
    [SF] private Image imgHandle;
    [SF] private Image imgFillBar;

    [SF] private float scrollSpeed;
    
    // Handle
    // pointEnter -> 살짝 커짐
    // pointExit -> 살짝 작아짐
    // pointDown -> 색 달라짐
    // pointUp -> 색 돌아옴

    // Slider
    // BeginDrag
    // Drag
    // EndDrag
    // Scroll (EventData로 스크롤 중엔 Handle을 움직이고, 스크롤 시작과 끝에만 값 변하게)

    public void OnBeginDrag(PointerEventData eventData)
    {
        // 필요한 거
        // Handle의 Image
        // Handle의 RectTransform
        
        // 할 일
        // imgHandle의 명도 조금 낮추고
        // 크기 살짝 줄임
        
        Debug.Log($"Begin drag {eventData.position}");
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        // 필요한 거
        // Handle의 RectTransform
        // FillBar의 Image
        
        // 할 일
        // 위치는 핸들의 anchor의 x 값을 조절하네 0~1 (fillAmount도 0~1이라 같은 값 해주면 될 덧)
        // imgHandle의 위치를 커서 위치와 맞춤 (x축만, imgFillBar를 넘어서지 않도록) (앵커 조절이라 알아서 됨)
        // imgFillBar의 fillAmount를 커서 위치와 맞춤 (Tween으로 부드럽게)
        Debug.Log($"Drag 중!");
    }
    
    public void OnEndDrag(PointerEventData eventData)
    {
        // 필요한 거
        // Handle의 Image
        // Handle의 RectTransform
        
        // 할 일
        // imgHandle의 명도와 크기 원상복구
        // imgHandle 위치에 따라 _value 갱신
        Debug.Log($"End drag {eventData.position}");
    }

    public void OnScroll(PointerEventData eventData)
    {
        // 필요한 거
        // Handle의 RectTransform
        // FillBar의 Image
        
        // 할 일
        // 위치는 핸들의 anchor의 x 값을 조절하네 0~1 (fillAmount도 0~1이라 같은 값 해주면 될 덧)
        // scrollSpeed에 따라 imgHandle의 위치와 imgFillBar의 fillAmount 조절
        // 조절 방향은 eventData 참고 (scrollDelta.y 값)
        // 스크롤이 멈췄으면 imgHandle 위치에 따라 _value 갱신 (IsScrolling 확인)
        Debug.Log($"Scrolling {eventData.scrollDelta.y}");
    }
}
