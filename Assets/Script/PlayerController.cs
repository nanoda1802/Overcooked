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
    [SF] private PlayerMovementData moveData;
    private Vector3 _moveDir;
    private float _moveSpeedModifier = 1f;
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
        }
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
    public void OnMove(InputAction.CallbackContext ctx)
    {
        if (isWorking) return;
        
        Vector2 input = ctx.ReadValue<Vector2>();
        _moveDir.x = input.x;
        _moveDir.z = input.y;
    }

    public void OnDash(InputAction.CallbackContext ctx)
    {
        if (ctx.started) _rb.AddForce(moveData.DashForce * _moveDir, ForceMode.VelocityChange);
        if (ctx.performed) _moveSpeedModifier = moveData.RunSpeedMultiplier;
        if (ctx.canceled) _moveSpeedModifier = 1f;
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
            default:
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
            default:
                break;
        }
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

    public void WaitForRespawn()
    {
        gameObject.SetActive(false);

        if (pickedItem is null) return;
        
        Item item = DetachItem();
        item.Deactivate();
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