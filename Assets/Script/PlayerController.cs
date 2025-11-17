using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using SF = UnityEngine.SerializeField;

// 참고 링크 https://rito15.github.io/posts/unity-rigidbody-move-and-jump/

public class PlayerController : MonoBehaviour
{
    #region 필드와 프로퍼티
    /* 컴포넌트 */
    private Rigidbody _rb;
    /* 이동 */
    [Header("Move")]
    [SF, Range(0f, 10f)] private float moveSpeed = 5f;
    [SF, Range(0f, 1f)] private float rotRatio = 0.4f;
    [SF, Range(0f, 10f)] private float dashForce = 5f;
    [SF] private float dashSpeedModifier = 1f;
    private Vector3 _moveDir;
    /* 물체 잡기 놓기 */
    [Header("Grab Item")]
    [SF] private Transform pivot;
    [SF, Range(60f, 180f)] private float fanAngle = 120f;
    [SF, Range(1, 10)] private int rayCount = 7;
    [SF, Range(0f, 5f)] private float rayDist = 1.5f;
    [SF, Range(0f, 1f)] private float rayOffsetY = 0.1f;
    [SF] private LayerMask targetLayer = 1<<7;
    private Ray _detectRay;
    private readonly HashSet<Transform> _detectedItems = new();
    private Rigidbody _itemRb;
    #endregion

    #region 유니티 이벤트 메서드
    private void Awake()
    {
        if (!TryGetComponent(out _rb))
        {
            _rb = gameObject.AddComponent<Rigidbody>();
            _rb.freezeRotation = true;
        }
        
        Application.targetFrameRate = 60;
    }

    private void FixedUpdate()
    {
        Debug.Log($"현재 속도 {_rb.velocity.sqrMagnitude:F1}");
        if (_moveDir.sqrMagnitude <= 0.001f) return;
        Move();
        Rotate();
    }
    #endregion

    #region 인풋 이벤트 메서드
    public void OnMove(InputAction.CallbackContext ctx)
    {
        Vector2 input = ctx.ReadValue<Vector2>();
        _moveDir.x = input.x;
        _moveDir.z = input.y;
    }

    public void OnDash(InputAction.CallbackContext ctx)
    {
        if (ctx.started) _rb.AddForce(dashForce * _moveDir, ForceMode.VelocityChange);
        if (ctx.performed) dashSpeedModifier = 2f;
        if (ctx.canceled) dashSpeedModifier = 1f;
    }
    
    public void OnGrab(InputAction.CallbackContext ctx)
    {
        if (!ctx.started) return;
        if (pivot.childCount > 0)
        {
            Ungrab();
            return;
        }

        Detect();
        if (_detectedItems.Count > 0) Grab();
    }
    #endregion
    
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
    
    private void Detect()
    {
        _detectedItems.Clear();
        
        float angleStep = fanAngle / (rayCount - 1); // Ray 간 간격 (각도)
        for (int i = 0; i < rayCount; i++)
        {
            float angle = -fanAngle / 2 + angleStep * i; // 이번 Ray의 Degree
            _detectRay.origin = transform.position + rayOffsetY * Vector3.up;
            _detectRay.direction = Quaternion.Euler(0, angle, 0) * transform.forward;

            if (Physics.Raycast(_detectRay, out RaycastHit hit, rayDist, targetLayer))
            {
                _detectedItems.Add(hit.collider.transform);
                Debug.DrawRay(_detectRay.origin, _detectRay.direction * hit.distance, Color.red);
                Debug.Log("Hit: " + hit.collider.gameObject.name);
            }
            else
            {
                Debug.DrawRay(_detectRay.origin, _detectRay.direction * rayDist, Color.green);
            }
        }
    }

    private void Grab()
    {
        float minDist = float.MaxValue;
        Transform closest = null;

        foreach (Transform t in _detectedItems)
        {
            float dist = (t.position - transform.position).sqrMagnitude;
            if (dist < minDist)
            {
                minDist = dist;
                closest = t;
            }
        }

        if (closest is not null && closest.gameObject.TryGetComponent(out _itemRb))
        {
            closest.SetParent(pivot);
            closest.localPosition = Vector3.zero;
            closest.localRotation = Quaternion.identity;
            _itemRb.isKinematic = true;
        }
    }

    private void Ungrab()
    { 
        Transform curItem = pivot.GetChild(0);
        curItem.SetParent(null);
        _itemRb.isKinematic = false;
        _itemRb = null;
    }
}
