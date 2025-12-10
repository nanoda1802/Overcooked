using UnityEngine;

public class InputManager : MonoBehaviour
{
    // DontDestroyOnLoad 예정
    // 스테이지 입장 시 구독 받으려면 playerController 참조가 필요한디...
    // 이외 마우스 조작 구독은 또 어디서...
    
    private PlayerInput _inputs;
    private PlayerInput.InStageActions _inStageActionMap;
    private PlayerInput.GlobalActions _globalActionMap;

    private void Awake()
    {
        _inputs = new PlayerInput();
    }

    private void OnEnable()
    {
        _inputs.Enable();
        SetEnableGlobalActionMap();
    }

    private void OnDisable()
    {
        _inputs.Disable();
        SetDisableInStageActionMap();
        SetDisableGlobalActionMap();
    }

    public void SetEnableInStageActionMap()
    {
        _inStageActionMap.Enable();
    }

    public void SetDisableInStageActionMap()
    {
        _inStageActionMap.Disable();
    }

    public void SetEnableGlobalActionMap()
    {
        _globalActionMap.Enable();
    }

    public void SetDisableGlobalActionMap()
    {
        _globalActionMap.Disable();
    }
    
}
