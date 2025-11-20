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
    [SF] private float dashSpeedModifier = 1f;
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
    [SF] private LayerMask boxLayer; // 1<<6
    [SF, Range(0f, 2f)] private float boxDetectDist; // 0.8f
    [SF, Range(0f, 2f)] private float boxDetectOffset; // 1f
    private IInteractable _detectedBox;
    public bool isWorking;
    #endregion

    #region 유니티 이벤트 메서드
    private void Awake()
    {
        if (!TryGetComponent(out _rb))
        {
            _rb = gameObject.AddComponent<Rigidbody>();
            _rb.freezeRotation = true;
        }

        Application.targetFrameRate = 60; // 임시
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
        Matrix4x4 boxMatrix = Matrix4x4.TRS(transform.position + offset, transform.rotation, Vector3.one);
        Gizmos.matrix = boxMatrix;
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
        if (ctx.performed) dashSpeedModifier = 1.5f;
        if (ctx.canceled) dashSpeedModifier = 1f;
    }

    public void OnInteract(InputAction.CallbackContext ctx)
    {
        switch (ctx.interaction)
        {
            case HoldInteraction when ctx.performed:
                if (DetectBox()) BeginWork();
                break;
            case HoldInteraction when ctx.canceled:
                if (_detectedBox is not null) StopWork();
                break;
            case PressInteraction when ctx.started:
            {
                if (DetectBox()) Interact();
                else if (pickedItem is not null) Drop();
                else if (DetectItem()) Pick();
                break;
            }
        }
    }

    public void OnThrow(InputAction.CallbackContext ctx)
    {
        if (pickedItem is null) return;
        if (ctx.interaction is HoldInteraction)
        {
            // 홀드 -> 던질 방향 나오고 떼면 해당 방향으로 던짐
            // 홀드 시간 조정해야함 input map 에서
            // 홀드 시간 동안 방향 조절, 시간 초과시 마지막 방향으로 던짐
        }
        else if (ctx.interaction is PressInteraction)
        {
            Throw(pivot.forward);
        }
    }
    #endregion

    #region 이동 메서드
    private void Move()
    {
        Vector3 moveOffset = (moveSpeed * dashSpeedModifier * Time.fixedDeltaTime) * _moveDir;
        _rb.MovePosition(_rb.position + moveOffset);
    }

    private void Rotate()
    {
        Quaternion smoothRot = Quaternion.Slerp(_rb.rotation, Quaternion.LookRotation(_moveDir), rotRatio);
        _rb.MoveRotation(smoothRot);
    }
    #endregion

    #region 박스 상호작용 메서드
    private bool DetectBox()
    {
        Vector3 offset = Vector3.up * boxDetectOffset;
        bool isHit = Physics.Raycast(transform.position + offset, transform.forward, out RaycastHit hit, boxDetectDist,
            boxLayer);
        Debug.DrawRay(transform.position + offset, transform.forward, isHit ? Color.red : Color.green);
        return isHit && hit.collider.gameObject.TryGetComponent(out _detectedBox);
    }

    private void Interact()
    {
        _detectedBox.Interact(this);
        _detectedBox = null;
    }

    private void BeginWork()
    {
        isWorking = _detectedBox.BeginWork(this);
    }

    public void StopWork()
    {
        if (!isWorking || _detectedBox is null) return;
        
        isWorking = false;
        _detectedBox.StopWork(this);
        _detectedBox = null;
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
            if (col is null) continue;
            float dist = (_rb.position - col.attachedRigidbody.position).sqrMagnitude;
            if (dist < minDist)
            {
                minDist = dist;
                closest = col.gameObject;
            }
        }

        if (closest is not null && closest.TryGetComponent(out Item item))
        {
            AttachItem(item);
        }
    }

    private void Drop()
    {
        DetachItem();
    }

    private void Throw(Vector3 dir)
    {
        pickedItem.SetThrowValues(pivot.position, dir, throwForce * dashSpeedModifier);
        DetachItem();
        // 근데 바로 플레이어와 충돌해서 안 던져질 수 있음... 플레이어랑도 충돌할 거니까
    }
    
    public void AttachItem(Item item)
    {
        item.SetParent(pivot);
        pickedItem = item;
    }

    public void DetachItem()
    {
        pickedItem.RemoveParent();
        pickedItem = null;
    }
    #endregion
}