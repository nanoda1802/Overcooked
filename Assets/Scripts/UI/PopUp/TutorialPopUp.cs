using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using SF = UnityEngine.SerializeField;

public class TutorialPopUp : MonoBehaviour
{
    [SF] private CanvasGroup canvasGroup;
    [SF] private RectTransform popUpRect;
    [SF, Range(0, 1)] private float originalScaleRatio; // 0.6;
    private Sequence _popUpSequence;
    
    [SF] private Text titleTxt;
    [SF] private Text descriptionTxt;
    [SF] private Animator previewAnim;
    private readonly int _paramHash = Animator.StringToHash("PageIdx");

    [SF] private Text pageTxt;
    [SF] private CustomButton prevPageBtn;
    [SF] private CustomButton nextPageBtn;
    
    [SF] private CustomButton exitBtn;
    [SF] private Toggle dontShowTutorialToggle;
    
    [SF] private TutorialData tutorialInfo;
    private int _curPageIdx;
    
    private InStageManager _inStageManager;

    private void OnEnable()
    {
        SubscribeEvents();
        DoEnableSequence();
    }

    private void OnDisable()
    {
        _inStageManager.StageCueUI.Activate(CueType.Start);

        UnsubscribeEvents();
        
        _popUpSequence?.Kill();
        _popUpSequence = null;
    }

    public void Init(InStageManager inStageManager)
    {
        previewAnim.runtimeAnimatorController = tutorialInfo.PreviewAnimController;
        previewAnim.updateMode = AnimatorUpdateMode.UnscaledTime;
        _inStageManager = inStageManager;
    }
    
    public void Activate()
    {
        _curPageIdx = 0;
        UpdateContents();
        popUpRect.localScale = Vector3.zero;
        gameObject.SetActive(true);
    }

    private void SubscribeEvents()
    {
        prevPageBtn.SubscribeEvent(OnPrevButton);
        nextPageBtn.SubscribeEvent(OnNextButton);
        exitBtn.SubscribeEvent(OnExitButton);
        dontShowTutorialToggle.onValueChanged.AddListener(OnToggleChanged);
    }

    private void UnsubscribeEvents()
    {
        prevPageBtn.UnsubscribeEvent(OnPrevButton);
        nextPageBtn.UnsubscribeEvent(OnNextButton);
        exitBtn.UnsubscribeEvent(OnExitButton);
        dontShowTutorialToggle.onValueChanged.RemoveAllListeners();
    }

    private void DoEnableSequence()
    {
        _popUpSequence?.Kill();

        _popUpSequence = DOTween.Sequence()
            .Append(canvasGroup.DOFade(1,0.2f).From(0))
            .Join(popUpRect.DOScaleX(originalScaleRatio, 0.2f).From(0).SetEase(Ease.OutCubic))
            .Append(popUpRect.DOScaleY(originalScaleRatio, 0.3f).From(0.01f).SetEase(Ease.OutBack))
            .SetUpdate(true)
            .OnKill(()=>_popUpSequence=null);
    }

    private void DoDisableSequence()
    {
        _popUpSequence?.Kill();
        
        _popUpSequence = DOTween.Sequence()
            .Append(popUpRect.DOScaleY(0.01f, 0.2f).From(originalScaleRatio).SetEase(Ease.InBack))
            .Append(popUpRect.DOScaleX(0f, 0.15f).From(originalScaleRatio).SetEase(Ease.InCubic))
            .Join(canvasGroup.DOFade(0,0.15f).From(1))
            .SetUpdate(true)
            .OnComplete(()=>gameObject.SetActive(false))
            .OnKill(()=>_popUpSequence=null);
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

    private void OnPrevButton()
    {
        if (_curPageIdx == 0) return;
        _curPageIdx--;
        UpdateContents();
    }

    private void OnNextButton()
    {
        if (_curPageIdx == tutorialInfo.PageCount - 1) return;
        _curPageIdx++;
        UpdateContents();
    }

    private void OnToggleChanged(bool isOn)
    {
        if (!isOn) return; // 애초에 저게 꺼져있으면 튜토리얼 ui를 볼 수가 없엉
        
        _inStageManager.StageInfo.SetShowTutorial(false);
        DoDisableSequence();
    }

    private void OnExitButton()
    {
        DoDisableSequence();
    }
}
