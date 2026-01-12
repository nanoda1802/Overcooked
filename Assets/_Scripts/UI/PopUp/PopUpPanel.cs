using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PopUpPanel : MonoBehaviour, IPointerClickHandler
{
    private bool isClicked = false;
    public event Action OnClicked;
    
    public void OnPointerClick(PointerEventData eventData)
    {
        if (isClicked) return;
        isClicked = true;
        
        OnClicked?.Invoke();
    }
}
