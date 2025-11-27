using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using SF = UnityEngine.SerializeField;

// 참고 링크 https://rito15.github.io/posts/unity-rigidbody-move-and-jump/

public class PlayerController : MonoBehaviour
{
    #region 필드와 프로퍼티
    /* 컴포넌트 */
    private Rigidbody _rb;
    /* 이동 */
    [Header("[ Move ]")] 
    [SF, Range(0f, 10f)] private float moveSpeed = 5f;
    [SF, Range(0f, 1f)] private float rotRatio = 0.4f;
    [SF, Range(0f, 10f)] private float dashForce = 5f;
    [SF, Range(0f, 5f)] private float dashSpeed = 1.5f;
    [SF] private float moveSpeedModifier = 1f;
    private Vector3 _moveDir;
    /* 물체 잡기 놓기 */
    [Header("[ Pick & Drop ]")] 
    [SF] private Transform pivot;
    [SF] private LayerMask itemLayer; // 1<<7
    [SF] private Vector3 itemDetectRange; // (0.8,0.7,0.4)
    [SF, Range(0f, 5f)] private float itemDetectOffset; // 0.8;
    private readonly Collider[] _detectedItems = new Collider[5];
    [HideInInspector] public Item pickedItem;
    /* 물체 던지기 */
    [Header("[ Throw ]")] 
    [SF, Range(1f, 20f)] private float throwForce; // 12f
    /* 상호작용 */
    [Header("[ Interact ]")] 
    [SF] private LayerMask tableLayer; // 1<<6
    [SF, Range(0f, 2f)] private float tableDetectDist; // 0.8f
    [SF, Range(0f, 2f)] private float tableDetectOffset; // 1f
    private Table _detectedTable;
    public bool isWorking;
    public Action OnWorkStopped;
    #endregion

    #region 유니티 이벤트 메서드
    private void Awake()
    {
        if (!TryGetComponent(out _rb))
        {
            _rb = gameObject.AddComponent<Rigidbody>();
            _rb.freezeRotation = true;
        }

        Application.targetFrameRate = 60; // [임시]
    }

    private void FixedUpdate()
    {
        if (_moveDir.sqrMagnitude <= 0.001f) return;
        Move();
        Rotate();
    }

    private void OnDrawGizmos()
    {
        Vector3 offset = (transform.forward + Vector3.up) * itemDetectOffset;
        Matrix4x4 tableMatrix = Matrix4x4.TRS(transform.position + offset, transform.rotation, Vector3.one);
        Gizmos.matrix = tableMatrix;
        Gizmos.DrawWireCube(Vector3.zero, itemDetectRange * 2);
        Gizmos.matrix = Matrix4x4.identity; // 매트릭스를 리셋하여 다른 Gizmos에 영향 안 미치도록 함
    }
    #endregion

    #region 인풋 이벤트 메서드
    public void OnMove(InputAction.CallbackContext ctx)
    {
        if (isWorking) return;
        
        Vector2 input = ctx.ReadValue<Vector2>();
        _moveDir.x = input.x;
        _moveDir.z = input.y;
    }

    public void OnDash(InputAction.CallbackContext ctx)
    {
        if (ctx.started) _rb.AddForce(dashForce * _moveDir, ForceMode.VelocityChange);
        if (ctx.performed) moveSpeedModifier = dashSpeed;
        if (ctx.canceled) moveSpeedModifier = 1f;
    }

    public void OnInteract(InputAction.CallbackContext ctx)
    {
        switch (ctx.interaction)
        {
            case HoldInteraction when ctx.performed:
                if (DetectTable() && _detectedTable is WorkTable table) BeginWork(table);
                break;
            case HoldInteraction when ctx.canceled:
                if (isWorking) StopWork();
                break;
            case PressInteraction when ctx.started:
                if (DetectTable() && Interact()) break;
                if (pickedItem is not null) Drop();
                else if (DetectItem()) Pick();
                break;
        }
    }

    public void OnThrow(InputAction.CallbackContext ctx)
    {
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
    #endregion

    #region 이동 메서드
    private void Move()
    {
        Vector3 moveOffset = (moveSpeed * moveSpeedModifier * Time.fixedDeltaTime) * _moveDir;
        _rb.MovePosition(_rb.position + moveOffset);
    }

    private void Rotate()
    {
        Quaternion smoothRot = Quaternion.Slerp(_rb.rotation, Quaternion.LookRotation(_moveDir), rotRatio);
        _rb.MoveRotation(smoothRot);
    }
    #endregion

    #region 박스 상호작용 메서드
    private bool DetectTable()
    {
        Vector3 offset = Vector3.up * tableDetectOffset;
        bool isHit = Physics.Raycast(transform.position + offset, transform.forward, out RaycastHit hit, tableDetectDist,
            tableLayer);
        Debug.DrawRay(transform.position + offset, transform.forward, isHit ? Color.red : Color.green);
        return isHit && hit.collider.gameObject.TryGetComponent(out _detectedTable);
    }

    private bool Interact()
    {
        bool hasInteraction = _detectedTable.Interact(this);
        _detectedTable = null;
        return hasInteraction;
    }

    private void BeginWork(WorkTable table)
    {
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
        
        Vector3 offset = (transform.forward + Vector3.up) * itemDetectOffset;
        int hits = Physics.OverlapBoxNonAlloc(transform.position + offset, itemDetectRange, _detectedItems,
            transform.rotation, itemLayer);
        return hits > 0;
    }

    private void Pick()
    {
        float minDist = float.MaxValue;
        GameObject closest = null;
        
        foreach (Collider col in _detectedItems)
        {
            if (col is null) break; // DetectItem에서 0번 인덱스부터 순서대로 채워지기 때문에, null이 등장했다면 이후는 모두 null
            
            float dist = (_rb.position - col.attachedRigidbody.position).sqrMagnitude;
            if (dist >= minDist) continue;
            
            minDist = dist;
            closest = col.gameObject;
        }

        if (closest is null) return;
        if (!closest.TryGetComponent(out Item item)) return;
        
        AttachItem(item);
    }

    private void Drop()
    {
        Item item = DetachItem();
        item.ActivatePhysics();
    }

    private void Throw(Vector3 dir)
    {
        DetachItem().SetThrowValues(pivot.position, dir, throwForce * moveSpeedModifier);
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