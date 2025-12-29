using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SF = UnityEngine.SerializeField;

public class LoadingDisplay : MonoBehaviour
{
    [SF] private CanvasGroup canvasGroup; // [임시] 추후 트윈으로 변경
    [SF] private Text tipsText;
    [SF] private Image progressImage;

    [SF] private string[] tips; // [임시] 추후 데이터로 따로 빼기
    
    public void Activate()
    {
        SetTipText();
        canvasGroup.alpha = 1f;
        progressImage.fillAmount = 0f;
        gameObject.SetActive(true);
    }

    private void Deactivate()
    {
        gameObject.SetActive(false);
    }

    private void SetTipText()
    {
        int rnd = Random.Range(0, tips.Length);
        tipsText.text = tips[rnd];
    }

    public void UpdateProgressImage(float progress)
    {
        progressImage.fillAmount = progress;
    }

    public IEnumerator Fade(float start, float end)
    {
        canvasGroup.alpha = start;
        
        float ratio = 0;
        while (ratio <= 1)
        {
            canvasGroup.alpha = Mathf.Lerp(start, end, ratio);
            ratio += Time.unscaledDeltaTime * 2f;
            yield return null;
        }

        canvasGroup.alpha = end;
        
        if (end < 1) Deactivate();
    }
}
