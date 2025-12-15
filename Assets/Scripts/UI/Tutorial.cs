using UnityEngine;
using UnityEngine.UI;
using SF = UnityEngine.SerializeField;

public class Tutorial : MonoBehaviour
{
    [SF] private Text titleTxt;
    [SF] private Text descriptionTxt;
    [SF] private Animator previewAnim;
    private readonly int _paramHash = Animator.StringToHash("PageIdx");
    
    [SF] private Text pageTxt;
    [SF] private Button prevPageBtn;
    [SF] private Button nextPageBtn;
    [SF] private Toggle neverShownToggle;
    
    [SF] private TutorialData tutorialInfo;
    private int _curPageIdx;

    private void Awake() // [임시]
    {
        Init();
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Init()
    {
        previewAnim.runtimeAnimatorController = tutorialInfo.PreviewAnimController;
        _curPageIdx = 0;
        UpdateContents();
    }

    public void UpdateContents()
    {
        titleTxt.text = tutorialInfo.GetCurrentTitle(_curPageIdx);
        descriptionTxt.text = tutorialInfo.GetCurrentDescription(_curPageIdx);
        previewAnim.SetInteger(_paramHash, _curPageIdx);
        
        UpdatePageText();
        
        if (_curPageIdx == 0) DeactivatePrevButton();
        else ActivatePrevButton();

        if (_curPageIdx == tutorialInfo.PageCount - 1) DeactivateNextButton(); // [추가] 닫기 버튼 추가하고, 여기서 닫기 버튼 활성화하기
        else ActivateNextButton();
        
    }

    public void UpdatePageText()
    {
        pageTxt.text = $"{_curPageIdx+1} / {tutorialInfo.PageCount}";
    }

    public void ActivatePrevButton()
    {
        if (prevPageBtn.gameObject.activeSelf) return;
        prevPageBtn.gameObject.SetActive(true);
    }

    public void DeactivatePrevButton()
    {
        if (!prevPageBtn.gameObject.activeSelf) return;
        prevPageBtn.gameObject.SetActive(false);
    }

    public void ActivateNextButton()
    {
        if (nextPageBtn.gameObject.activeSelf) return;
        nextPageBtn.gameObject.SetActive(true);
    }

    public void DeactivateNextButton()
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
        if (neverShownToggle.isOn)
        {
            gameObject.SetActive(false);
            // 해당 스테이지의 튜토리얼 보이지 않도록 글로벌 설정
            // 로비 옵션에서도 수정 가능하도록
        }
        else
        {
            
        }
    }
}
