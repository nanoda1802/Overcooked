using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SF = UnityEngine.SerializeField;

public class PlateCount : MonoBehaviour
{
    [SF] private Text sinkText;
    [SF] private float offsetY = 1.1f;
    private RectTransform _rect;
    private Camera _mainCam;
    
    public void Init()
    {
        sinkText ??= GetComponentInChildren<Text>();
        _rect ??= GetComponent<RectTransform>();
        _mainCam ??= Camera.main;
    }
    
    public void SetScreePos(Vector3 worldPos)
    {
        worldPos.y += offsetY;
        Vector3 screenPoint = _mainCam.WorldToScreenPoint(worldPos);
        _rect.position = screenPoint;
    }
    
    public void UpdateText(int count)
    {
        sinkText.text = $"{count}";
    }
}
