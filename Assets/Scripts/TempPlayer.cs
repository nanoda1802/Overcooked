using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using SF = UnityEngine.SerializeField;

public class TempPlayer : MonoBehaviour
{
    [SF] private NavMeshAgent agent;
    [SF] private InputManager inputManager; // [임시]
    public Vector2 cursorPos;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        
    }

    public void OnLeftClickPerformed(InputAction.CallbackContext ctx)
    {
        cursorPos = inputManager.OutStageActionMap.CursorPos.ReadValue<Vector2>(); // 그럼 얘는 필요가...
        
        // IsPointerOverGameObject의 인자인 pointerId의 default 값은 -1, 모바일 기기는 0부터 시작하니 직접 인자를 넣어줘야 한대
        // Graphic Raycaster 컴포넌트를 가진 Canvas가 필요하고,
        // Raycast Target이 활성화된 오브젝트에만 반응함
        if (EventSystem.current.IsPointerOverGameObject()) return;
        
        switch (ctx.interaction)
        {
            case HoldInteraction:
                break;
            case PressInteraction:
                break;
        }
    }

    public void OnLeftClickCanceled(InputAction.CallbackContext ctx)
    {
        if (ctx.interaction is not HoldInteraction) return;
        // 카메라 이동 모드 헤제
    }

    public void OnScrollPerformed(InputAction.CallbackContext ctx)
    {
        
    }
}
