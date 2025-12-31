using System.Collections;
using Cinemachine;
using Sfx;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using SF = UnityEngine.SerializeField;

public class PlayerController_Lobby : MonoBehaviour
{
    /* Cam */
    [Header("[ Camera ]")]
    [SF] private Camera mainCam;
    [SF] private CinemachineBrain cineBrain;
    [SF] private int maxCamPriority;
    /* Navmesh & Coroutine */
    [Header("[ Select Landmark ]")]
    [SF] private NavMeshAgent agent;
    [SF] private float arrivalDistanceThreshold;
    [SF] private Landmark selectedLandmark;
    private Coroutine _coSelectLandmark;
    private WaitUntil _waitPathPending;
    private WaitUntil _waitAgentArrival;
    private WaitUntil _waitVCamBlendingStart;
    private WaitUntil _waitVCamBlendingEnd;
    /* Sound */
    [Header("[ Sound ]")]
    [SF] private SfxInfo bgm;
    /* Anim */
    private Animator _anim;
    private AnimParams _animParams;
    
    #region Unity Event Methods
    private void Awake()
    {
        InitComponents();
        InitFields();
    }

    private void Start()
    {
        GameManager.Instance.SoundManager.ChangeBgm(bgm);
        Time.timeScale = 1; // [임시]
        
        StartPoseAnim(1); // [임시]
        
        if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 3f, agent.areaMask))
        {
            transform.position = hit.position;
        }
    }
    #endregion

    #region Initialize Methods
    private void InitComponents()
    {
        if (!TryGetComponent(out agent))
        {
            #if UNITY_EDITOR
            Debug.LogError("본인에게 부착된 NavmeshAgent가 없슴다. [PlayerController_Lobby.InitComponents]");
            #endif
            return;
        }
        
        mainCam = Camera.main;
        
        if (mainCam is null)
        {
            #if UNITY_EDITOR
            Debug.LogError("Main으로 설정된 Camera가 없슴다. [PlayerController_Lobby.InitComponents]");
            #endif
            return;
        }

        if (!mainCam.TryGetComponent(out cineBrain))
        {
            #if UNITY_EDITOR
            Debug.LogError("MainCamera에 CineBrain이 없슴다. [PlayerController_Lobby.InitComponents]");
            #endif
            return;
        }
        
        if (this.TryGetComponentInChildren(out _anim)) // 확장 메서드 사용부
        {
            _animParams = new AnimParams(_anim);
            // _anim.GetBehaviour<CheckAfk>().Init(_animParams.GetHash("AFK"),3);
        }
        else
        {
            #if UNITY_EDITOR
            Debug.LogError("본인과 모든 자식들 중, 발견된 Animator가 없슴다. [PlayerController_Lobby.InitComponents]");
            #endif
        }
        
        // [메모] 따로 플레이어 agentData니 해서 SO로 빼든 하자
        // if (!TryGetComponent(out agent))
        // {
        //     agent = gameObject.AddComponent<NavMeshAgent>();
        //     agent.speed = 20; // [임시]
        //     agent.angularSpeed = 500; // [임시]
        //     agent.acceleration = 400; // [임시]    
        // }
    }

    private void InitFields()
    {
        _waitPathPending = new WaitUntil(() => !agent.pathPending);
        _waitAgentArrival = new WaitUntil(() => agent.remainingDistance <= arrivalDistanceThreshold);
        _waitVCamBlendingStart = new WaitUntil(() => cineBrain.IsBlending);
        _waitVCamBlendingEnd = new WaitUntil(() => !cineBrain.IsBlending);
    }
    #endregion

    #region Input Event Methods
    private void OnLeftClickPerformed(InputAction.CallbackContext ctx)
    {
        if (selectedLandmark is not null) return;
        if (EventSystem.current.IsPointerOverGameObject()) return;
        
        switch (ctx.interaction)
        {
            case PressInteraction:
                if (!TryDetectLandmark(GameManager.Instance.InputManager.GetCursorPosition())) break;
                if (_coSelectLandmark is not null) StopCoroutine(_coSelectLandmark);
                StartCoroutine(SelectLandmark());
                break;
        }
    }

    private void OnLeftClickCanceled(InputAction.CallbackContext ctx) { } // [보류]
    private void OnScrollPerformed(InputAction.CallbackContext ctx) { }  // [보류]

    public void SubscribeOutStageInputEvents(PlayerInput.OutStageActions actionMap)
    {
        actionMap.LeftClick.performed += OnLeftClickPerformed;
    }

    public void UnsubscribeOutStageInputEvents(PlayerInput.OutStageActions actionMap)
    {
        actionMap.LeftClick.performed -= OnLeftClickPerformed;
    }
    #endregion

    #region Landmark Selecting Methods
    private bool TryDetectLandmark(Vector2 cursorPos)
    {
        Ray ray = mainCam.ScreenPointToRay(cursorPos);
        if (!Physics.Raycast(ray, out RaycastHit hit)) return false;
        if (!hit.collider.TryGetComponent(out selectedLandmark)) return false;
        return true;
    }

    private IEnumerator SelectLandmark()
    {
        if (selectedLandmark is null) yield break;
        if (!NavMesh.SamplePosition(selectedLandmark.Marker.position, out NavMeshHit hit, 3f, agent.areaMask)) yield break;
        
        StopPoseAnim();
        StartAnim(_animParams.GetHash("Run"));
        
        agent.SetDestination(hit.position);
        yield return _waitPathPending;
        yield return _waitAgentArrival;
        
        StopAnim(_animParams.GetHash("Run"));
        StartPoseAnim();
        
        if (selectedLandmark.IsDummy()) // [임시]
        {
            selectedLandmark = null;
            yield break;
        }
        
        selectedLandmark.SetVCamPriority(maxCamPriority);
        LookCam(selectedLandmark.GetVCamPos());
        
        yield return _waitVCamBlendingStart;
        yield return _waitVCamBlendingEnd;
        
        selectedLandmark.ActivatePopUpUI(this);
    }
    
    public void DeselectLandmark()
    {
        if (selectedLandmark is null) return;
        selectedLandmark.SetVCamPriority(0);
        selectedLandmark.DeactivatePopUpUI(this);
        selectedLandmark = null;
    }
    
    private void LookCam(Vector3 camPos)
    {
        transform.LookAt(camPos);
        transform.rotation = Quaternion.Euler(new Vector3(0, transform.rotation.eulerAngles.y, 0));
    }
    #endregion

    #region Animation Methods
    private void StartAnim(int hash)
    {
        _anim.SetBool(hash, true);
    }
    
    private void StopAnim(int hash)
    {
        _anim.SetBool(hash, false);
    }

    private void StartPoseAnim(int index = 0)
    {
        _anim.SetInteger(_animParams.GetHash("Pose"), index);
    }

    private void StopPoseAnim()
    {
        _anim.SetInteger(_animParams.GetHash("Pose"), -1);
    }
    #endregion
}
