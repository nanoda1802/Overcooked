using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SF = UnityEngine.SerializeField;

public class ProgressBar : MonoBehaviour
{
    [SF] private Image[] fillBars;
    [SF] private float offsetY = 1.1f;
    private RectTransform _rect;
    private Camera _mainCam;

    public void Init()
    {
        if (fillBars == null || fillBars.Length == 0)
        {
            Transform imagesParent = transform.GetChild(0);
            fillBars = new Image[imagesParent.childCount];

            for (int i = 0; i < imagesParent.childCount; i++)
            {
                fillBars[i] = imagesParent.GetChild(i).GetComponent<Image>();
            }    
        }
        
        _rect ??= GetComponent<RectTransform>();
        _mainCam ??= Camera.main;
    }

    public void SetScreePos(Vector3 worldPos)
    {
        worldPos.y += offsetY;
        Vector3 screenPoint = _mainCam.WorldToScreenPoint(worldPos);
        _rect.position = screenPoint;
    }

    public void InitFillAmount()
    {
        foreach (Image img in fillBars) img.fillAmount = 0;
    }

    public void Fill(float ratio)
    {
        if (ratio >= fillBars.Length) return;
        fillBars[(int)ratio].fillAmount = Mathf.Lerp(0, 1, ratio % 1);
    }
}
