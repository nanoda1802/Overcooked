using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using SF = UnityEngine.SerializeField;

// 참고 링크 https://rito15.github.io/posts/unity-rigidbody-move-and-jump/

public class PlayerController_Stage : MonoBehaviour
{
    [SF] private StageManager stageManager;
    /* 이동 */
    [Header("[ Move ]")] 
    private Rigidbody _rb;
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
    [SF] private Transform pivot; // spine03의 child, pos : (-0.15, 0.7, 0), rot : (0, 0, 110)
    [SF] private Transform rightHand;
    public Item pickedItem;
    /* 작업 */
    public event Action OnWorkStopped;
    private bool _isWorking;
    /* SFX */
    [SF] private PlayerSfxData sfxData;
    /* Anim */
    private Animator _anim;
    private AnimParams _animParams;
    private HandIK _handIK;
    // [SF] private PlayerAnimData animData;
    
    #region Unity Event Methods
    private void Awake()
    {
        InitComponents();
    }

    private void OnEnable()
    {
        // _anim.SetFloat(animData.MoveSpeedHash, _moveSpeedModifier);
        ApplyMoveSpeedToAnim();
    }

    private void Start()
    {
        // animData.Init();
        InitFields();
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

    #region Initialize Methods

    private void InitComponents()
    {
        if (!TryGetComponent(out _rb))
        {
            #if UNITY_EDITOR
            Debug.LogError("본인에게 부착된 RigidBody가 없슴다. [PlayerController_Stage.InitComponents]");
            #endif
        }

        if (this.TryGetComponentInChildren(out _anim)) // 확장 메서드 사용부
        {
            _animParams = new AnimParams(_anim);
            _anim.GetBehaviour<CheckAfk>()?.Init(_animParams.GetHash("AFK"), 3);
            _handIK = _anim.GetBehaviour<HandIK>();
        }
        else
        {
            #if UNITY_EDITOR
            Debug.LogError("본인과 모든 자식들 중, 발견된 Animator가 없슴다. [PlayerController_Stage.InitComponents]");
            #endif
            return;
        }
        
        // [메모] 이거 Player RB Data든 뭐든 해서 빼놓자
        // if (!TryGetComponent(out _rb))
        // {
        //     _rb = gameObject.AddComponent<Rigidbody>();
        //     _rb.freezeRotation = true;
        //     _rb.mass = 100;
        //     _rb.drag = 1.5f;
        //     _rb.angularDrag = 0.05f;
        // }
    }

    private void InitFields()
    {
        _waitInertiaDecay = new WaitForSeconds(moveData.InertiaDecayTime);
    }

    #endregion
    
    #region Input Event Methods
    private void OnMoveStarted(InputAction.CallbackContext ctx)
    {
        if (!gameObject.activeSelf) return;
        // StopAnim(animData.DashHash);
        // PlayAnim(animData.MoveHash);
        StopAnim(_animParams.GetHash("Dash"));
        StartAnim(_animParams.GetHash("Move"));
    }

    private void OnMovePerformed(InputAction.CallbackContext ctx)
    {
        if (!gameObject.activeSelf) return;
        Vector2 input = ctx.ReadValue<Vector2>();  
        _moveDir.x = input.x;  
        _moveDir.z = input.y;
    }

    private void OnMoveCanceled(InputAction.CallbackContext ctx)
    {
        _moveDir = Vector3.zero;
        StopMoveImmediately();
        // StopAnim(animData.MoveHash);
        StopAnim(_animParams.GetHash("Move"));
    }

    private void OnDashStarted(InputAction.CallbackContext ctx)
    {
        if (!gameObject.activeSelf) return;
        if (_dashCoroutine is not null) StopCoroutine(_dashCoroutine);  
        _dashCoroutine = StartCoroutine(Dash());  
        
        _moveSpeedModifier = moveData.RunSpeedMultiplier;
        // _anim.SetFloat(animData.MoveSpeedHash, _moveSpeedModifier);
        ApplyMoveSpeedToAnim();
    }

    private void OnDashCanceled(InputAction.CallbackContext ctx)
    {
        _moveSpeedModifier = 1f;
        // _anim.SetFloat(animData.MoveSpeedHash, _moveSpeedModifier);
        ApplyMoveSpeedToAnim();
    }

    private void OnInteractStarted(InputAction.CallbackContext ctx)
    {
        if (!gameObject.activeSelf) return;
        // StopAnim(animData.DashHash);
        StopAnim(_animParams.GetHash("Dash"));
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
        if (!_isWorking) return; 
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
                Throw(transform.forward);
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

    private void OnPauseStarted(InputAction.CallbackContext ctx)
    {
        if (stageManager.PauseUI.IsPopping()) return;
        
        if (stageManager.PauseUI.gameObject.activeSelf)
        {
            stageManager.ResumeStage();
            stageManager.PauseUI.PopDown();
        }
        else
        {
            stageManager.PauseUI.PopUp();
        }
    }

    public void SubscribeStageInputEvents(PlayerInput.InStageActions actionMap)
    {
        actionMap.Move.started += OnMoveStarted;
        actionMap.Move.performed += OnMovePerformed;
        actionMap.Move.canceled += OnMoveCanceled;
        actionMap.Dash.started += OnDashStarted;
        actionMap.Dash.canceled += OnDashCanceled;
        actionMap.Interact.started += OnInteractStarted;
        actionMap.Interact.performed += OnInteractPerformed;
        actionMap.Interact.canceled += OnInteractCanceled;
        actionMap.Throw.performed += OnThrowPerformed;
        actionMap.Throw.canceled += OnThrowCanceled;
        actionMap.Pause.started += OnPauseStarted;
    }

    public void UnsubscribeStageInputEvents(PlayerInput.InStageActions actionMap)
    {
        actionMap.Move.started -= OnMoveStarted;
        actionMap.Move.performed -= OnMovePerformed;
        actionMap.Move.canceled -= OnMoveCanceled;
        actionMap.Dash.started -= OnDashStarted;
        actionMap.Dash.canceled -= OnDashCanceled;
        actionMap.Interact.started -= OnInteractStarted;
        actionMap.Interact.performed -= OnInteractPerformed;
        actionMap.Interact.canceled -= OnInteractCanceled;
        actionMap.Throw.performed -= OnThrowPerformed;
        actionMap.Throw.canceled -= OnThrowCanceled;
        actionMap.Pause.started -= OnPauseStarted;
    }
    #endregion

    #region Movement Methods
    private void Move()
    {
        if (_isWorking) return;  
        Vector3 moveOffset = (moveData.MoveSpeed * _moveSpeedModifier * Time.fixedDeltaTime) * _moveDir;
        _rb.MovePosition(_rb.position + moveOffset);
        // [sfx] 걷는 소리
    }

    private void Rotate()
    {
        if (_isWorking) return;  
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
        
        // PlayAnim(animData.DashHash);
        StartAnim(_animParams.GetHash("Dash"));
        
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
        // StopAnim(animData.DashHash);
        StopAnim(_animParams.GetHash("Dash"));
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

    #region ReSpawn/Despawn Methods
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
    
    #region Table Handling Methods
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
        
        StopMoveImmediately();
        
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
    }

    private void BeginWork(WorkTable table)
    {
        StopMoveImmediately();
        // RotateImmediately(table.transform.position);
        _isWorking = table.BeginWork(this);
    }

    private void StopWork()
    {
        OnWorkStopped?.Invoke();
        FinishWork();   
    }

    public void FinishWork()
    {
        _isWorking = false;
        _detectedTable = null;
        OnWorkStopped = null;
    }
    #endregion

    #region Item Handling Methods
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

    public void GrabKnife(Transform knife)
    {
        knife.SetParent(rightHand);
        knife.SetLocalPositionAndRotation(0.15f * Vector3.forward, Quaternion.Euler(new Vector3(0,0,180))); // [임시]
        knife.gameObject.SetActive(true);
    }

    public void AttachItem(Item item)
    {
        if (item.Data.ItemType != ItemType.Cheese) item.SetParent(pivot); // [임시......]
        else item.SetParent(pivot,Vector3.zero,new Vector3(0,90,0)); // 이렇게 하긴 정말 싫은데 달리 말끔한 방법ㅇ...
        
        pickedItem = item;
        
        _handIK.SetHandPoints(item.LeftHandPoint, item.RightHandPoint);
        // PlayAnim(animData.PickHash);
        StartAnim(_animParams.GetHash("Pick"));
        
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

        _handIK.ClearHandPoints();
        // StopAnim(animData.PickHash);
        StopAnim(_animParams.GetHash("Pick"));
        
        return item;
    }

    public void GetHandledItem()
    {
        if (_detectedTable is not PlaceTable table) return;
        AttachItem(table.DisplaceItem());
    }
    #endregion

    #region Animation Methods
    public void StartAnim(int hash)
    {
        StopAnim(_animParams.GetHash("AFK"));
        _anim.SetBool(hash, true);
    }
    
    public void StopAnim(int hash)
    {
        _anim.SetBool(hash, false);
    }

    private void TriggerAnim(int hash)
    {
        _anim.SetTrigger(hash);
    }

    private void ApplyMoveSpeedToAnim()
    {
        _anim.SetFloat(_animParams.GetHash("MoveSpeed"), _moveSpeedModifier);
    }
    #endregion
}