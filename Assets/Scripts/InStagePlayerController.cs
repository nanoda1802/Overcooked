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
    private Transform _recentTile;
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
    public Action OnWorkStopped;
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

    private void OnEnable()
    {
        // inputManager.SetAbleInStageActionMap();
        // 이후 적절한 액션에 맞는 메서드들 구독
    }

    private void OnDisable()
    {
        // inputManager.SetDisableInStageActionMap();
        // 이후 등록해둔 메서드들 구독 해제
    }

    private void Start()
    {
        _waitInertiaDecay = new WaitForSeconds(moveData.InertiaDecayTime);
    }

    private void FixedUpdate()
    {
        if (_moveDir.sqrMagnitude <= 0.001f) return;
        Move();
        Rotate();
    }

    private void OnCollisionEnter(Collision other)
    {
        if (!other.gameObject.CompareTag("Ground")) return;
        
        Transform otherTransform = other.gameObject.transform;
        if (otherTransform.position.y + 0.8f * otherTransform.localScale.y > _rb.position.y) return;
        
        _recentTile = otherTransform;
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
        if (!gameObject.activeSelf) return;
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
        if (!gameObject.activeSelf) return;
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
                if (DetectTable() && TryInteract()) return;
                if (pickedItem is not null) Drop();  
                else if (DetectItem()) Pick();
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
        else inStageManager.PauseStage();
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
        yield return new WaitUntil(IsVelocityZero);
        yield return _waitInertiaDecay;
        StopMoveImmediately();
    }

    private bool IsVelocityZero()
    {
        return _rb.velocity == Vector3.zero;
    }

    private void StopMoveImmediately()
    {
        _rb.velocity = _rb.angularVelocity = Vector3.zero;
    }
    #endregion

    #region 리스폰 메서드
    public void DeactivatePlayer()
    {
        gameObject.SetActive(false);

        if (pickedItem is null) return;
        DetachItem().Deactivate();
    }

    public Vector3 CalculateRespawnPosition()
    {
        Vector3 respawnPos;
        
        if (_recentTile is null)
        {
            respawnPos = Vector3.one;
        }
        else
        {
            Vector3 temp = _recentTile.position;
            temp.y += _recentTile.localScale.y;
            respawnPos = temp;
        }
        
        return respawnPos;
    }

    public void Respawn(Vector3 respawnPos)
    {
        gameObject.SetActive(true);
        _rb.position = respawnPos;
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
        bool hasInteraction = _detectedTable.Interact(this);
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

    private void Pick()
    {
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

        if (closestObj is null) return;
        if (!closestObj.TryGetComponent(out Item item)) return;
        if (item.IsPlaced) return; // 버그 해결 핵심 분기...!
        
        AttachItem(item);
    }

    private void Drop()
    {
        Item item = DetachItem();
        item.ActivatePhysics();
    }

    private void Throw(Vector3 dir)
    {
        DetachItem().SetThrowValues(pivot.position, dir, _moveSpeedModifier);
        // 근데 바로 플레이어와 충돌해서 안 던져질 수 있음... 플레이어랑도 충돌할 거니까
    }
    
    public void AttachItem(Item item)
    {
        item.SetParent(pivot);
        pickedItem = item;
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