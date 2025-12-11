using System;
using UnityEngine;

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

    private PlayerController _playerInStage;
    private TempPlayer _playerOutStage;
    
    private void Awake()
    {
        _inputs = new PlayerInput();
        _inStageActionMap = _inputs.InStage;
        _outStageActionMap = _inputs.OutStage;
    }

    private void OnEnable()
    {
        _inputs.Enable();
        EnterOutStage(); // [임시]
    }

    private void OnDisable()
    {
        _inputs.Disable();
        SetDisableInStageActionMap();
        SetDisableOutStageActionMap();
    }

    public void SetEnableInStageActionMap()
    {
        _inStageActionMap.Enable();
    }

    public void SetDisableInStageActionMap()
    {
        _inStageActionMap.Disable();
    }

    public void SetEnableOutStageActionMap()
    {
        _outStageActionMap.Enable();
    }

    public void SetDisableOutStageActionMap()
    {
        _outStageActionMap.Disable();
    }

    public void EnterInStage()
    {
        SetEnableInStageActionMap();
        _playerInStage = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        _playerInStage.SubscribeInStageInputEvents(_inStageActionMap);
    }

    public void ExitInStage()
    {
        _playerInStage.UnsubscribeInStageInputEvents(_inStageActionMap);
        _playerInStage = null;
        SetDisableInStageActionMap();
    }

    public void EnterOutStage() // [임시]
    {
        SetEnableOutStageActionMap();
        _playerOutStage = GameObject.FindWithTag("Player").GetComponent<TempPlayer>();
        _playerOutStage.SubscribeOutStageInputEvents(_outStageActionMap);
        _playerOutStage.inputManager = this;
    }

    public void ExitOutStage() // [임시]
    {
        _playerOutStage.UnsubscribeOutStageInputEvents(_outStageActionMap);
        _playerOutStage = null;
        SetDisableOutStageActionMap();
    }
}
