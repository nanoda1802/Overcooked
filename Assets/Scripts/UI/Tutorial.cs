using UnityEngine;
using UnityEngine.UI;
using SF = UnityEngine.SerializeField;

public class Tutorial : MonoBehaviour
{
    [SF] private Text titleTxt;
    [SF] private Text descriptionTxt;
    [SF] private Animator previewAnim;
    private readonly int _paramHash = Animator.StringToHash("PageIdx");

    [SF] private Button exitBtn;
    [SF] private Text pageTxt;
    [SF] private Button prevPageBtn;
    [SF] private Button nextPageBtn;
    [SF] private Toggle dontShowTutorialToggle;
    
    [SF] private TutorialData tutorialInfo;
    private int _curPageIdx;
    
    private InStageManager _inStageManager;

    public void Init(InStageManager inStageManager)
    {
        previewAnim.runtimeAnimatorController = tutorialInfo.PreviewAnimController;
        previewAnim.updateMode = AnimatorUpdateMode.UnscaledTime;
        _inStageManager = inStageManager;
        Activate();
    }

    private void Activate()
    {
        gameObject.SetActive(true);
        _curPageIdx = 0;
        UpdateContents();
    }

    private void UpdateContents()
    {
        titleTxt.text = tutorialInfo.GetCurrentTitle(_curPageIdx);
        descriptionTxt.text = tutorialInfo.GetCurrentDescription(_curPageIdx);
        previewAnim.SetInteger(_paramHash, _curPageIdx);
        
        UpdatePageText();
        
        if (_curPageIdx == 0) OnFirstPage();
        else ActivatePrevButton();

        if (_curPageIdx == tutorialInfo.PageCount - 1) OnLastPage();
        else ActivateNextButton();
    }

    private void UpdatePageText()
    {
        pageTxt.text = $"{_curPageIdx+1} / {tutorialInfo.PageCount}";
    }

    private void OnFirstPage()
    {
        DeactivatePrevButton();
        DeactivateExitButton();
    }

    private void OnLastPage()
    {
        DeactivateNextButton();
        ActivateExitButton();
    }

    private void ActivateExitButton()
    {
        if (exitBtn.gameObject.activeSelf) return;
        exitBtn.gameObject.SetActive(true);
    }

    private void DeactivateExitButton()
    {
        if (!exitBtn.gameObject.activeSelf) return;
        exitBtn.gameObject.SetActive(false);
    }

    private void ActivatePrevButton()
    {
        if (prevPageBtn.gameObject.activeSelf) return;
        prevPageBtn.gameObject.SetActive(true);
    }

    private void DeactivatePrevButton()
    {
        if (!prevPageBtn.gameObject.activeSelf) return;
        prevPageBtn.gameObject.SetActive(false);
    }

    private void ActivateNextButton()
    {
        if (nextPageBtn.gameObject.activeSelf) return;
        nextPageBtn.gameObject.SetActive(true);
    }

    private void DeactivateNextButton()
    {
        if (!nextPageBtn.gameObject.activeSelf) return;
        nextPageBtn.gameObject.SetActive(false);
    }

    public void OnPrevButton()
    {
        if (_curPageIdx == 0) return;
        _curPageIdx--;
        UpdateContents();
    }

    public void OnNextButton()
    {
        if (_curPageIdx == tutorialInfo.PageCount - 1) return;
        _curPageIdx++;
        UpdateContents();
    }

    public void OnToggleChanged() // [임시] 추후 글로벌 설정과 연계
    {
        if (!dontShowTutorialToggle.isOn) return; // 애초에 저게 꺼져있으면 튜토리얼 ui를 볼 수가 없엉
        gameObject.SetActive(false);
        _inStageManager.StageInfo.SetShowTutorial(false);
        _inStageManager.ResumeStage();
    }

    public void OnExitButton()
    {
        gameObject.SetActive(false);
        _inStageManager.ResumeStage();
    }
}
