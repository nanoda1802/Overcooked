using UnityEngine;
using SF = UnityEngine.SerializeField;

public class InputManager : MonoBehaviour
{
    // DontDestroyOnLoad 예정
    // 스테이지 입장 시 구독 받으려면 playerController 참조가 필요한디...
    // 일단 스테이지에서 시작한다 가정하고 하자
    // 이외 마우스 조작 구독은 또 어디서...
    
    private PlayerInput _inputs;
    private PlayerInput.InStageActions _inStageActionMap;
    private PlayerInput.OutStageActions _outStageActionMap;
    public PlayerInput.OutStageActions OutStageActionMap => _outStageActionMap;

    [SF] private InStagePlayerController inStagePlayerInStage;
    [SF] private OutStagePlayerController playerOutStage;
    
    private void Awake()
    {
        _inputs = new PlayerInput();
        _inStageActionMap = _inputs.InStage;
        _outStageActionMap = _inputs.OutStage;
    }

    private void OnEnable()
    {
        _inputs.Enable();
        // EnterOutStage(); // [임시]
    }

    private void OnDisable()
    {
        _inputs.Disable();
        SetDisableInStageActionMap();
        SetDisableOutStageActionMap();
    }

    private void SetEnableInStageActionMap()
    {
        _inStageActionMap.Enable();
    }

    private void SetDisableInStageActionMap()
    {
        _inStageActionMap.Disable();
    }

    private void SetEnableOutStageActionMap()
    {
        _outStageActionMap.Enable();
    }

    private void SetDisableOutStageActionMap()
    {
        _outStageActionMap.Disable();
    }

    public Vector2 GetCursorPosition()
    {
        return _outStageActionMap.CursorPos.ReadValue<Vector2>();
    }

    public void EnterInStage()
    {
        SetEnableInStageActionMap();
        inStagePlayerInStage = GameObject.FindWithTag("Player").GetComponent<InStagePlayerController>();
        inStagePlayerInStage.SubscribeInStageInputEvents(_inStageActionMap);
    }

    public void ExitInStage()
    {
        inStagePlayerInStage.UnsubscribeInStageInputEvents(_inStageActionMap);
        inStagePlayerInStage = null;
        SetDisableInStageActionMap();
    }

    public void EnterOutStage() // [임시]
    {
        SetEnableOutStageActionMap();
        playerOutStage = GameObject.FindWithTag("Player").GetComponent<OutStagePlayerController>();
        playerOutStage.SubscribeOutStageInputEvents(_outStageActionMap);
    }

    public void ExitOutStage() // [임시]
    {
        playerOutStage.UnsubscribeOutStageInputEvents(_outStageActionMap);
        playerOutStage = null;
        SetDisableOutStageActionMap();
    }
}
