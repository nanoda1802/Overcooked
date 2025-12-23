using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using SF = UnityEngine.SerializeField;

// 참고 링크 https://rito15.github.io/posts/unity-rigidbody-move-and-jump/

public class InStagePlayerController : MonoBehaviour
{
    #region 필드와 프로퍼티
    /* 컴포넌트 */
    private Rigidbody _rb;
    [SF] private InStageManager inStageManager;
    /* 이동 */
    [Header("[ Move ]")] 
    [SF] private PlayerMovementData moveData;
    private Vector3 _moveDir;
    private float _moveSpeedModifier = 1f;
    private Coroutine _dashCoroutine;
    private WaitForSeconds _waitInertiaDecay;
    /* 아이템 및 테이블 감지 */
    [Header("[ Detect ]")]
    [SF] private PlayerDetectionData detectData;
    private readonly Collider[] _detectedItems = new Collider[5];
    private Table _detectedTable;
    /* 아이템 줍기 내려놓기 */
    [Header("[ Pick & Drop ]")] 
    [SF] private Transform pivot;
    public Item pickedItem;
    /* 작업 */
    [HideInInspector] public bool isWorking;
    public event Action OnWorkStopped;

    [SF] private AudioClip interactBlockSoundClip; // [임시]
    [SF] private AudioClip despawnSoundClip; // [임시]
    [SF] private AudioClip dashSoundClip; // [임시]
    [SF] private AudioClip attachSoundClip; // [임시]
    [SF] private AudioClip throwSoundClip; // [임시]
    #endregion

    #region 유니티 이벤트 메서드
    private void Awake()
    {
        if (!TryGetComponent(out _rb))
        {
            _rb = gameObject.AddComponent<Rigidbody>();
            _rb.freezeRotation = true;
            _rb.mass = 100;
            _rb.drag = 1.5f;
            _rb.angularDrag = 0.05f;
        }
    }

    private void Start()
    {
        _waitInertiaDecay = new WaitForSeconds(moveData.InertiaDecayTime);
    }

    private void FixedUpdate()
    {
        if (_moveDir == Vector3.zero) return;
        Move();
        Rotate();
    }

    private void OnDisable()
    {
        if (_dashCoroutine is not null)
        {
            StopCoroutine(_dashCoroutine);
            _dashCoroutine = null;
            StopMoveImmediately();
        }

        _moveDir = Vector3.zero;
    }

    private void OnDrawGizmos()
    {
        Vector3 offset = (transform.forward + Vector3.up) * detectData.DetectBoxOffset;
        Matrix4x4 tableMatrix = Matrix4x4.TRS(transform.position + offset, transform.rotation, Vector3.one);
        Gizmos.matrix = tableMatrix;
        Gizmos.DrawWireCube(Vector3.zero, detectData.DetectBoxSize * 2);
        Gizmos.matrix = Matrix4x4.identity; // 매트릭스를 리셋하여 다른 Gizmos에 영향 안 미치도록 함
    }
    #endregion

    #region 인풋 이벤트 메서드
    private void OnMovePerformed(InputAction.CallbackContext ctx)
    {
        if (!gameObject.activeSelf) return;
        if (isWorking) return;  
        Vector2 input = ctx.ReadValue<Vector2>();  
        _moveDir.x = input.x;  
        _moveDir.z = input.y;
    }

    private void OnMoveCanceled(InputAction.CallbackContext ctx)
    {
        _moveDir = Vector3.zero;
    }

    private void OnDashStarted(InputAction.CallbackContext ctx)
    {
        if (!gameObject.activeSelf) return;
        if (_dashCoroutine is not null) StopCoroutine(_dashCoroutine);  
        _dashCoroutine = StartCoroutine(CoDash());  
        _moveSpeedModifier = moveData.RunSpeedMultiplier;
    }

    private void OnDashCanceled(InputAction.CallbackContext ctx)
    {
        _moveSpeedModifier = 1f;
    }

    private void OnInteractPerformed(InputAction.CallbackContext ctx)
    {
        if (!gameObject.activeSelf) return;
        
        switch (ctx.interaction)
        {
            case HoldInteraction:
                if (!DetectTable()) return;
                if (_detectedTable is not WorkTable table) return;
                BeginWork(table);
                return;
            case PressInteraction:
                if (TryPick()) return;
                if (DetectTable()) TryInteract();
                else Drop();
                return;
        }
    }

    private void OnInteractCanceled(InputAction.CallbackContext ctx)
    {
        if (!gameObject.activeSelf) return;
        if (!isWorking) return; 
        StopWork();
    }

    private void OnThrowPerformed(InputAction.CallbackContext ctx)
    {
        if (!gameObject.activeSelf) return;
        if (pickedItem is null) return;
        
        switch (ctx.interaction)
        {
            case HoldInteraction:
                // 홀드 시간 동안 방향 조절, 키 떼거나 시간 초과시 마지막 방향으로 던짐
                break;
            case PressInteraction:
                Throw(pivot.forward);
                break;
        }
    }

    private void OnThrowCanceled(InputAction.CallbackContext ctx)
    {
        if (!gameObject.activeSelf) return;
        if (pickedItem is null) return;
        if (ctx.interaction is not HoldInteraction) return;
        // 홀드 동안 계산된 throwDir로 던지기
        // Throw(throwDir);
    }

    private void OnPauseStated(InputAction.CallbackContext ctx)
    {
        if (inStageManager.IsStagePaused) inStageManager.ResumeStage();  
        else inStageManager.PauseStage(true);
    }

    public void SubscribeInStageInputEvents(PlayerInput.InStageActions actionMap)
    {
        actionMap.Move.performed += OnMovePerformed;
        actionMap.Move.canceled += OnMoveCanceled;
        actionMap.Dash.started += OnDashStarted;
        actionMap.Dash.canceled += OnDashCanceled;
        actionMap.Interact.performed += OnInteractPerformed;
        actionMap.Interact.canceled += OnInteractCanceled;
        actionMap.Throw.performed += OnThrowPerformed;
        actionMap.Throw.canceled += OnThrowCanceled;
        actionMap.Pause.started += OnPauseStated;
    }

    public void UnsubscribeInStageInputEvents(PlayerInput.InStageActions actionMap)
    {
        actionMap.Move.performed -= OnMovePerformed;
        actionMap.Move.canceled -= OnMoveCanceled;
        actionMap.Dash.started -= OnDashStarted;
        actionMap.Dash.canceled -= OnDashCanceled;
        actionMap.Interact.performed -= OnInteractPerformed;
        actionMap.Interact.canceled -= OnInteractCanceled;
        actionMap.Throw.performed -= OnThrowPerformed;
        actionMap.Throw.canceled -= OnThrowCanceled;
        actionMap.Pause.started -= OnPauseStated;
    }
    #endregion

    #region 이동 메서드
    private void Move()
    {
        Vector3 moveOffset = (moveData.MoveSpeed * _moveSpeedModifier * Time.fixedDeltaTime) * _moveDir;
        _rb.MovePosition(_rb.position + moveOffset);
        // [sfx] 걷는 소리
    }

    private void Rotate()
    {
        Quaternion smoothRot = Quaternion.Slerp(_rb.rotation, Quaternion.LookRotation(_moveDir), moveData.RotRatio);
        _rb.MoveRotation(smoothRot);
    }

    private IEnumerator CoDash()
    {
        if (_moveDir == Vector3.zero) yield break;
        
        StopMoveImmediately();
        _rb.AddForce(moveData.DashForce * _moveDir, ForceMode.VelocityChange);
        // [sfx] 대시 소리
        GameManager.Instance.SoundManager.PlaySfx(dashSoundClip);
        yield return _waitInertiaDecay;
        StopMoveImmediately();
    }

    private void StopMoveImmediately()
    {
        _rb.velocity = _rb.angularVelocity = Vector3.zero;
    }
    #endregion

    #region 리스폰 메서드
    public Vector3 DespawnPlayer()
    {
        _rb.Sleep();
        gameObject.SetActive(false);
        
        // [sfx] 떨어지는 소리
        GameManager.Instance.SoundManager.PlaySfx(despawnSoundClip);

        if (pickedItem is not null) DetachItem().Deactivate();
        
        return transform.position;
    }

    public void Respawn(Vector3 respawnPos)
    {
        gameObject.SetActive(true);
        transform.position = respawnPos;
        _rb.WakeUp();
    }
    #endregion
    
    #region 테이블 상호작용 메서드
    private bool DetectTable()
    {
        Vector3 offset = Vector3.up * detectData.DetectRayOffsetY;
        bool isHit = Physics.Raycast(transform.position + offset, transform.forward, out RaycastHit hit, detectData.DetectRayDistance,
            detectData.TableLayer);
        Debug.DrawRay(transform.position + offset, transform.forward, isHit ? Color.red : Color.green);
        return isHit && hit.collider.gameObject.TryGetComponent(out _detectedTable);
    }

    private bool TryInteract()
    {
        if (!DetectTable()) return false;
        
        bool hasInteraction = _detectedTable.Interact(this);
        if (!hasInteraction)
        {
            // [sfx] 상호작용 블락 소리   
            GameManager.Instance.SoundManager.PlaySfx(interactBlockSoundClip);
        }
        _detectedTable = null;
        return hasInteraction;
    }

    private void BeginWork(WorkTable table)
    {
        StopMoveImmediately();
        isWorking = table.BeginWork(this);
    }

    private void StopWork()
    {
        OnWorkStopped?.Invoke();
        FinishWork();   
    }

    public void FinishWork()
    {
        isWorking = false;
        _detectedTable = null;
        OnWorkStopped = null;
    }
    #endregion

    #region 아이템 들기 놓기 던지기 메서드
    private bool DetectItem()
    {
        Array.Clear(_detectedItems, 0, _detectedItems.Length);
        
        Vector3 offset = (transform.forward + Vector3.up) * detectData.DetectBoxOffset;
        int hits = Physics.OverlapBoxNonAlloc(transform.position + offset, detectData.DetectBoxSize, _detectedItems,
            transform.rotation, detectData.ItemLayer);
        return hits > 0;
    }

    private bool TryPick()
    {
        if (pickedItem is not null) return false;
        if (!DetectItem())  return false;
        
        float minDist = float.MaxValue;
        GameObject closestObj = null;
        
        foreach (Collider col in _detectedItems)
        {
            if (col is null) break; // DetectItem에서 0번 인덱스부터 순서대로 채워지기 때문에, null이 등장했다면 이후는 모두 null
            
            float dist = (_rb.position - col.transform.position).sqrMagnitude;
            if (dist >= minDist) continue;
            
            minDist = dist;
            closestObj = col.gameObject;
        }

        if (closestObj is null) return false;
        if (!closestObj.TryGetComponent(out Item item)) return false;
        if (item.IsPlaced) return false; // 버그 해결 핵심 분기...!
        
        AttachItem(item);
        return true;
    }

    private void Drop()
    {
        if (pickedItem is null) return;
        Item item = DetachItem();
        item.ActivatePhysics();
    }

    private void Throw(Vector3 dir)
    {
        DetachItem().SetThrowValues(pivot.position, dir, _moveSpeedModifier);
        // 근데 바로 플레이어와 충돌해서 안 던져질 수 있음... 플레이어랑도 충돌할 거니까
        // [sfx] 던지는 소리
        GameManager.Instance.SoundManager.PlaySfx(throwSoundClip);
    }
    
    public void AttachItem(Item item)
    {
        item.SetParent(pivot);
        pickedItem = item;
        // [sfx] 재료 줍는 소리
        GameManager.Instance.SoundManager.PlaySfx(attachSoundClip);
    }

    public Item DetachItem()
    {
        Item item = pickedItem;
        item.RemoveParent();
        pickedItem = null;
        return item;
    }

    public void GetHandledItem()
    {
        if (_detectedTable is not PlaceTable table) return;
        AttachItem(table.DisplaceItem());
    }
    #endregion
}