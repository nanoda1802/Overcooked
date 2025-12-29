using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using SF = UnityEngine.SerializeField;

// 참고 링크 https://rito15.github.io/posts/unity-rigidbody-move-and-jump/

public class InStagePlayerController : MonoBehaviour
{
    /* 컴포넌트 */
    private Rigidbody _rb;
    private Animator _anim;
    [SF] private InStageManager inStageManager;
    /* 이동 */
    [Header("[ Move ]")] 
    [SF] private PlayerMovementData moveData;
    [SF] private ParticleSystem dashVfx;
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
    /* SFX */
    [SF] private PlayerSfxData sfxData;
    /* Anim */
    private readonly int _moveHash = Animator.StringToHash("Move"); // [임시] SO로 뺄 것임, 테스트용
    private readonly int _dashHash = Animator.StringToHash("Dash"); // [임시] SO로 뺄 것임, 테스트용
    
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
        
        _anim = GetComponentInChildren<Animator>(); // [임시]
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
        
        _anim.SetBool(_moveHash,true); // [임시] 테스트용
    }

    private void OnMoveCanceled(InputAction.CallbackContext ctx)
    {
        _moveDir = Vector3.zero;
        StopMoveImmediately();
        _anim.SetBool(_moveHash,false); // [임시] 테스트용
    }

    private void OnDashStarted(InputAction.CallbackContext ctx)
    {
        if (!gameObject.activeSelf) return;
        if (_dashCoroutine is not null) StopCoroutine(_dashCoroutine);  
        _dashCoroutine = StartCoroutine(Dash());  
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
        if (inStageManager.PauseUI.IsPopping()) return;
        
        if (inStageManager.PauseUI.gameObject.activeSelf)
        {
            inStageManager.ResumeStage();
            inStageManager.PauseUI.PopDown();
        }
        else
        {
            inStageManager.PauseUI.PopUp();
        }
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

    private void RotateImmediately(Vector3 lookPos)
    {
        Quaternion rotDir = Quaternion.LookRotation(lookPos);
        _rb.MoveRotation(rotDir);
    }

    private IEnumerator Dash()
    {
        if (_moveDir == Vector3.zero) yield break;
        
        StopMoveImmediately();
        _rb.AddForce(moveData.DashForce * _moveDir, ForceMode.VelocityChange);
        
        GameManager.Instance.SoundManager.BuildSfx()
            .WithSfxInfo(sfxData.DashSfx)
            .WithPos(transform.position)
            .WithRandomPitch()
            .Play();
        
        PlayDashVfx();
        
        yield return _waitInertiaDecay;
        StopMoveImmediately();
    }

    private void StopMoveImmediately()
    {
        _rb.velocity = _rb.angularVelocity = Vector3.zero;
    }

    private void PlayDashVfx()
    {
        if (dashVfx is null) return;
        if (dashVfx.isPlaying) StopDashVfxSmoothly();
        dashVfx.Play();
    }

    private void StopDashVfxSmoothly()
    {
        dashVfx?.Stop(true,ParticleSystemStopBehavior.StopEmitting);
    }

    #endregion

    #region 리스폰 메서드
    public Vector3 DespawnPlayer()
    {
        _rb.Sleep();
        gameObject.SetActive(false);
        
        GameManager.Instance.SoundManager.BuildSfx()
            .WithSfxInfo(sfxData.DespawnSfx)
            .WithPos(transform.position)
            .Play();

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
        if (!_detectedTable.Interact(this))
        {
            GameManager.Instance.SoundManager.BuildSfx()
                .WithSfxInfo(sfxData.ActionBlockedSfx)
                .WithPos(transform.position)
                .WithRandomPitch()
                .Play();
            return false;
        }
        
        // RotateImmediately(_detectedTable.transform.position);
        _detectedTable = null;
        return true;
        
        // bool hasInteraction = _detectedTable.Interact(this);
        // if (!hasInteraction)
        // {
        //     GameManager.Instance.SoundManager.BuildSfx()
        //         .WithSfxInfo(interactBlockSfx)
        //         .WithPos(transform.position)
        //         .WithRandomPitch()
        //         .Play();
        // }
        // RotateImmediately(_detectedTable.transform.position);
        // _detectedTable = null;
        // return hasInteraction;
    }

    private void BeginWork(WorkTable table)
    {
        StopMoveImmediately();
        // RotateImmediately(table.transform.position);
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
        GameManager.Instance.SoundManager.BuildSfx()
            .WithSfxInfo(sfxData.ThrowSfx)
            .WithPos(transform.position)
            .WithRandomPitch()
            .Play();
    }
    
    public void AttachItem(Item item)
    {
        item.SetParent(pivot);
        pickedItem = item;
        GameManager.Instance.SoundManager.BuildSfx()
            .WithSfxInfo(sfxData.AttachSfx)
            .WithPos(transform.position)
            .WithRandomPitch()
            .Play();
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