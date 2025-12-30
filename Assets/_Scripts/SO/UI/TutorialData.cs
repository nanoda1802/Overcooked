using UnityEngine;
using SF = UnityEngine.SerializeField;

[CreateAssetMenu(menuName = "SO/UI/Tutorial", fileName = "TutorialData")]
public class TutorialData : ScriptableObject
{
    [SF] private string curStage;
    [SF] private string[] titles;
    [SF] private string[] descriptions;
    [SF] private RuntimeAnimatorController previewAnimController; // 스테이지 별로 다른 컨트롤러
    [SF] private int pageCount;
    
    public string CurStage => curStage; // 쓸 일이 있을 것...
    public RuntimeAnimatorController PreviewAnimController => previewAnimController;
    public int PageCount => pageCount;

    public string GetCurrentTitle(int curPageIdx)
    {
        if (curPageIdx < 0 || curPageIdx >= titles.Length) return string.Empty;
        return titles[curPageIdx];
    }

    public string GetCurrentDescription(int curPageIdx)
    {
        if (curPageIdx < 0 || curPageIdx >= descriptions.Length) return string.Empty;
        return descriptions[curPageIdx];
    }
}
