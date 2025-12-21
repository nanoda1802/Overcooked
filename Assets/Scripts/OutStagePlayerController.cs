using System.Collections;
using Cinemachine;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using SF = UnityEngine.SerializeField;

public class OutStagePlayerController : MonoBehaviour
{
    [SF] private GameManager gameManager;
    [SF] private NavMeshAgent agent;
    [SF] private Camera mainCam;
    [SF] private CinemachineBrain cineBrain;
    
    private Coroutine _coSelectStage;
    private WaitUntil _waitPathPending;
    private WaitUntil _waitAgentArrival;
    private WaitUntil _waitVCamBlendingStart;
    private WaitUntil _waitVCamBlendingEnd;
    [SF] private float arrivalDistanceThreshold;

    [SF] private Eatery curTargetEatery;
    [SF] private int maxCamPriority;

    [SF] private AudioClip bgm;
    
    private void Awake()
    {
        gameManager = FindObjectOfType(typeof(GameManager)) as GameManager;
        
        if (!TryGetComponent(out agent))
        {
            agent = gameObject.AddComponent<NavMeshAgent>();
            agent.speed = 20; // [임시]
            agent.angularSpeed = 500; // [임시]
            agent.acceleration = 400; // [임시]    
        }

        mainCam = Camera.main;
        
        if (cineBrain is null)
        {
            cineBrain = mainCam?.GetComponent<CinemachineBrain>();
        }
    }

    private void Start()
    {
        _waitPathPending = new WaitUntil(() => !agent.pathPending);
        _waitAgentArrival = new WaitUntil(IsAgentArrived);
        _waitVCamBlendingStart = new WaitUntil(() => cineBrain.IsBlending);
        _waitVCamBlendingEnd = new WaitUntil(() => !cineBrain.IsBlending);
        
        gameManager.SoundManager.ChangeBgm(bgm);
        
        Time.timeScale = 1;
        
        if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 3f, NavMesh.AllAreas))
        {
            transform.position = hit.position;
        }
    }

    private void OnLeftClickPerformed(InputAction.CallbackContext ctx)
    {
        if (curTargetEatery is not null) return;
        if (EventSystem.current.IsPointerOverGameObject()) return;
        
        switch (ctx.interaction)
        {
            case PressInteraction:
                if (!TryDetectEatery(gameManager.InputManager.GetCursorPosition())) break;
                if (_coSelectStage is not null) StopCoroutine(_coSelectStage);
                StartCoroutine(CoSelectStage());
                break;
        }
    }

    private void OnLeftClickCanceled(InputAction.CallbackContext ctx)
    {
    }

    private void OnScrollPerformed(InputAction.CallbackContext ctx)
    {
        // float scrollAmount = ctx.ReadValue<Vector2>().y;
        // float originalFOV = _mainCam.fieldOfView; // 시네머신이라서 현재 활성화 중인 virtual cam의 virtual FOV를 조절해야 할 듯?
        // _mainCam.fieldOfView = Mathf.Clamp(originalFOV - scrollAmount * zoomSpeed, zoomInLimit, zoomOutLimit);
    }

    public void SubscribeOutStageInputEvents(PlayerInput.OutStageActions actionMap)
    {
        actionMap.LeftClick.performed += OnLeftClickPerformed;
        // actionMap.LeftClick.canceled += OnLeftClickCanceled;
        // actionMap.Scroll.performed += OnScrollPerformed;
    }

    public void UnsubscribeOutStageInputEvents(PlayerInput.OutStageActions actionMap)
    {
        actionMap.LeftClick.performed -= OnLeftClickPerformed;
        // actionMap.LeftClick.canceled -= OnLeftClickCanceled;
        // actionMap.Scroll.performed -= OnScrollPerformed;
    }

    private bool TryDetectEatery(Vector2 cursorPos)
    {
        Ray ray = mainCam.ScreenPointToRay(cursorPos);
        
        if (!Physics.Raycast(ray, out RaycastHit hit)) return false;
        if (!hit.collider.TryGetComponent(out curTargetEatery)) return false;
        return true;
    }

    private bool IsAgentArrived()
    {
        return agent.remainingDistance <= arrivalDistanceThreshold;
    }

    private IEnumerator CoSelectStage()
    {
        if (curTargetEatery is null) yield break;
        if (!NavMesh.SamplePosition(curTargetEatery.Marker.position, out NavMeshHit hit, 3f, NavMesh.AllAreas)) yield break;
        agent.SetDestination(hit.position);
        yield return _waitPathPending;
        yield return _waitAgentArrival;
        
        if (curTargetEatery.IsDummyEatery())
        {
            curTargetEatery = null;
            yield break;
        }
        
        curTargetEatery.SetVCamPriority(maxCamPriority);

        LookCam(curTargetEatery.GetVCamPos());
        
        yield return _waitVCamBlendingStart;
        yield return _waitVCamBlendingEnd;
        
        curTargetEatery.ActivatePopUpUI(this);
    }

    private void LookCam(Vector3 camPos)
    {
        transform.LookAt(camPos);
        transform.rotation = Quaternion.Euler(new Vector3(0, transform.rotation.eulerAngles.y, 0));
    }

    public void DeselectEatery()
    {
        if (curTargetEatery is null) return;
        curTargetEatery.SetVCamPriority(0);
        curTargetEatery.DeactivatePopUpUI(this);
        curTargetEatery = null;
    }
}
