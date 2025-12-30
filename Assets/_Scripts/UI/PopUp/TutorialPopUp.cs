using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using SF = UnityEngine.SerializeField;

public class TutorialPopUp : MonoBehaviour
{
    /* Components */
    [SF] private Animator previewAnim; // [임시] GetComponentInChildren 하든가...
    [SF] private TutorialData tutorialData; // [임시] StageManager한테 받아서 init 하기
    [SF] private CanvasGroup canvasGroup; // [임시] GetComponentInChildren 하든가...
    [SF] private RectTransform popUpRect; // [임시] 이거 본인 Rect임
    [SF] private Image bg;
    [SF, Range(0, 1)] private float originalScaleRatio; // 0.6;
    /* UI Elements */ 
    [Header("[ Texts ]")]
    [SF] private Text titleTxt;
    [SF] private Text descriptionTxt;
    [SF] private Text pageTxt;
    [Header("[ Buttons ]")]
    [SF] private CustomButton prevPageBtn;
    [SF] private CustomButton nextPageBtn;
    [SF] private CustomButton exitBtn;
    [Header("[ Toggles ]")]
    [SF] private Toggle dontShowTutorialToggle;
    /* Fields */
    private int _curPageIdx;
    private readonly int _paramHash = Animator.StringToHash("PageIdx");
    private Sequence _popUpSequence;
    private StageManager _stageManager;

    #region Unity Event Methods
    private void OnEnable()
    {
        SubscribeEvents();
        PopUp();
    }

    private void OnDisable()
    {
        _stageManager.StageCueUI.Activate(CueType.Start); // [임시] 더 좋은 흐름이 없을지 고민해보자

        UnsubscribeEvents();
        
        _popUpSequence?.Kill();
        _popUpSequence = null;
    }
    #endregion

    #region Initialize Methods
    public void Init(StageManager stageManager)
    {
        previewAnim.runtimeAnimatorController = tutorialData.PreviewAnimController;
        previewAnim.updateMode = AnimatorUpdateMode.UnscaledTime;
        _stageManager = stageManager;
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
    #endregion

    #region UI Control Methods
    public void Activate()
    {
        _curPageIdx = 0;
        UpdateContents();
        popUpRect.localScale = Vector3.zero;
        if (!bg.gameObject.activeSelf) bg.gameObject.SetActive(true);
        gameObject.SetActive(true);
    }

    private void Deactivate()
    {
        gameObject.SetActive(false);
    }

    private void UpdateContents()
    {
        titleTxt.text = tutorialData.GetCurrentTitle(_curPageIdx);
        descriptionTxt.text = tutorialData.GetCurrentDescription(_curPageIdx);
        previewAnim.SetInteger(_paramHash, _curPageIdx);
        
        UpdatePageText();
        
        if (_curPageIdx == 0) OnFirstPage();
        else ActivatePrevButton();

        if (_curPageIdx == tutorialData.PageCount - 1) OnLastPage();
        else ActivateNextButton();
    }

    private void UpdatePageText()
    {
        pageTxt.text = $"{_curPageIdx+1} / {tutorialData.PageCount}";
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
    #endregion

    #region Tween Methods
    private void PopUp()
    {
        _popUpSequence?.Kill();

        _popUpSequence = DOTween.Sequence()
            .Append(canvasGroup.DOFade(1,0.2f).From(0))
            .Join(popUpRect.DOScaleX(originalScaleRatio, 0.2f).From(0).SetEase(Ease.OutCubic))
            .Append(popUpRect.DOScaleY(originalScaleRatio, 0.3f).From(0.01f).SetEase(Ease.OutBack))
            .SetUpdate(true)
            .OnKill(()=>_popUpSequence=null);
    }

    private void PopDown()
    {
        _popUpSequence?.Kill();
        
        _popUpSequence = DOTween.Sequence()
            .Append(popUpRect.DOScaleY(0.01f, 0.2f).From(originalScaleRatio).SetEase(Ease.InBack))
            .Append(popUpRect.DOScaleX(0f, 0.15f).From(originalScaleRatio).SetEase(Ease.InCubic))
            .Join(canvasGroup.DOFade(0,0.15f).From(1))
            .SetUpdate(true)
            .OnComplete(Deactivate)
            .OnKill(()=>_popUpSequence=null);
    }
    #endregion

    #region UI Event Methods
    private void OnPrevButton()
    {
        if (_curPageIdx == 0) return;
        _curPageIdx--;
        UpdateContents();
    }

    private void OnNextButton()
    {
        if (_curPageIdx == tutorialData.PageCount - 1) return;
        _curPageIdx++;
        UpdateContents();
    }

    private void OnToggleChanged(bool isOn)
    {
        if (!isOn) return; // 애초에 저게 꺼져있으면 튜토리얼 ui를 볼 수가 없엉
        
        _stageManager.StageInfo.SetShowTutorial(false);
        PopDown();
    }

    private void OnExitButton()
    {
        PopDown();
    }
    #endregion
}
