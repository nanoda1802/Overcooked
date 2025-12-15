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
    
    public string CurStage => curStage;
    public RuntimeAnimatorController PreviewAnimController => previewAnimController;
    public int PageCount => pageCount;

    public string GetCurrentTitle(int curPageIdx) // 인덱싱 방어 조건 추가해야
    {
        return titles[curPageIdx];
    }

    public string GetCurrentDescription(int curPageIdx) // 인덱싱 방어 조건 추가해야
    {
        return descriptions[curPageIdx];
    }
}
