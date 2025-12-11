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
    [SF] private Canvas canvas;
    [SF] private GameObject uiPrefab;
    
    [SF] private NavMeshAgent agent;
    private Camera _mainCam;
    public InputManager inputManager; // [임시]
    
    [SF] private float zoomSpeed;
    [SF] private float zoomInLimit;
    [SF] private float zoomOutLimit;

    [SF] private Vector3 swipeStartPos;
    [SF] private Vector3 swipeEndPos;
    
    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        _mainCam = Camera.main;
    }

    public void OnLeftClickPerformed(InputAction.CallbackContext ctx)
    {
        // IsPointerOverGameObject의 인자인 pointerId의 default 값은 -1, 모바일 기기는 0부터 시작하니 직접 인자를 넣어줘야 한대
        // Graphic Raycaster 컴포넌트를 가진 Canvas가 필요하고,
        // Raycast Target이 활성화된 오브젝트에만 반응함
        if (EventSystem.current.IsPointerOverGameObject()) return;
        
        Vector2 cursorPos = inputManager.OutStageActionMap.CursorPos.ReadValue<Vector2>(); 
        
        switch (ctx.interaction)
        {
            case HoldInteraction:
                swipeStartPos.x = cursorPos.x;
                swipeStartPos.z = cursorPos.y;
                break;
            case PressInteraction:
                // ray 발사
                // 오브젝트 감지
                // 거기로 이동
                if (!TryDetectObject(cursorPos, out Transform target)) break;
                agent.SetDestination(target.position);
                GameObject ui = Instantiate(uiPrefab, canvas.transform);
                ui.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(-150, 30, 0);
                
                break;
        }
    }

    public void OnLeftClickCanceled(InputAction.CallbackContext ctx)
    {
        if (ctx.interaction is not HoldInteraction) return;
        if (swipeStartPos == Vector3.zero) return;
        // Drag();
    }

    public void OnScrollPerformed(InputAction.CallbackContext ctx)
    {
        float scrollAmount = ctx.ReadValue<Vector2>().y;
        float originalFOV = _mainCam.fieldOfView; // 시네머신이라서 현재 활성화 중인 virtual cam의 virtual FOV를 조절해야 할 듯?
        _mainCam.fieldOfView = Mathf.Clamp(originalFOV - scrollAmount * zoomSpeed, zoomInLimit, zoomOutLimit);
    }

    public void SubscribeOutStageInputEvents(PlayerInput.OutStageActions actionMap)
    {
        actionMap.LeftClick.performed += OnLeftClickPerformed;
        actionMap.LeftClick.canceled += OnLeftClickCanceled;
        actionMap.Scroll.performed += OnScrollPerformed;
    }

    public void UnsubscribeOutStageInputEvents(PlayerInput.OutStageActions actionMap)
    {
        actionMap.LeftClick.performed -= OnLeftClickPerformed;
        actionMap.LeftClick.canceled -= OnLeftClickCanceled;
        actionMap.Scroll.performed -= OnScrollPerformed;
    }

    private bool TryDetectObject(Vector2 cursorPos, out Transform target)
    {
        Ray ray = _mainCam.ScreenPointToRay(cursorPos);
        bool isHit = Physics.Raycast(ray, out RaycastHit hit);
        target = isHit ? hit.transform : null;
        return isHit;
    }

    private void Drag() // 이거 아니야..........
    {
        Vector2 cursorPos = inputManager.OutStageActionMap.CursorPos.ReadValue<Vector2>();
        swipeEndPos.x = cursorPos.x;
        swipeEndPos.z = cursorPos.y;
        StartCoroutine(CoDrag());
        swipeStartPos = Vector3.zero;
    }

    private IEnumerator CoDrag()
    {
        Vector3 targetPos = swipeEndPos - swipeStartPos;
        targetPos.y = 0;
        Vector3 origin = _mainCam.transform.position;

        float time = 1f;

        while (time > 0)
        {
            _mainCam.transform.position = Vector3.Lerp(origin, origin + targetPos, time);
            time -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }
}
