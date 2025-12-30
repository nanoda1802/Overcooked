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

    [SF] private PlayerController_Stage player_stage;
    [SF] private PlayerController_Lobby player_lobby;
    
    private void Awake()
    {
        _inputs = new PlayerInput();
        _inStageActionMap = _inputs.InStage;
        _outStageActionMap = _inputs.OutStage;
    }

    private void OnEnable()
    {
        _inputs.Enable();
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
        player_stage = GameObject.FindWithTag("Player").GetComponent<PlayerController_Stage>();
        player_stage.SubscribeStageInputEvents(_inStageActionMap);
    }

    public void ExitInStage()
    {
        player_stage.UnsubscribeStageInputEvents(_inStageActionMap);
        player_stage = null;
        SetDisableInStageActionMap();
    }

    public void EnterOutStage() // [임시]
    {
        SetEnableOutStageActionMap();
        player_lobby = GameObject.FindWithTag("Player").GetComponent<PlayerController_Lobby>();
        player_lobby.SubscribeOutStageInputEvents(_outStageActionMap);
    }

    public void ExitOutStage() // [임시]
    {
        player_lobby.UnsubscribeOutStageInputEvents(_outStageActionMap);
        player_lobby = null;
        SetDisableOutStageActionMap();
    }
}
