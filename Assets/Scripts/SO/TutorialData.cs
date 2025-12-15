using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SF = UnityEngine.SerializeField;

[CreateAssetMenu(menuName = "SO/UI/Tutorial", fileName = "TutorialData")]
public class TutorialData : ScriptableObject
{
    [SF] private string curStage;
    [SF] private string[] titles;
    [SF] private string[] descriptions;
    [SF] private Sprite[] previewSprites;
    [SF] private int pageCount;
    
    public string CurStage => curStage;
    public string[] Titles => titles;
    public string[] Descriptions => descriptions;
    public Sprite[] PreviewSprites => previewSprites;
    public int PageCount => pageCount;

    public string GetCurrentTitle(int curPageIdx) // 인덱싱 방어 조건 추가해야
    {
        return titles[curPageIdx];
    }

    public string GetCurrentDescription(int curPageIdx) // 인덱싱 방어 조건 추가해야
    {
        return descriptions[curPageIdx];
    }

    public Sprite GetPreviewSprite(int curPageIdx) // 인덱싱 방어 조건 추가해야
    {
        return previewSprites[curPageIdx];
    }
}
