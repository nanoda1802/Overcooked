using System;
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
    [SF, Range(0f, 5f)] private float detectOffset = 0.8f;
    [SF] private Vector3 detectRange; // 0.7 0.7 0.4
    [SF] private LayerMask targetLayer = 1<<7;
    private readonly Collider[] _detectedItems = new Collider[5];
    private Item _grabbedItem;
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
        
        if (_grabbedItem is not null)
        {
            Ungrab();
            return;
        }
        
        if (Detect()) Grab();
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

    private void OnDrawGizmos()
    {
        Vector3 offset = (transform.forward + Vector3.up) * detectOffset;
        Matrix4x4 boxMatrix = Matrix4x4.TRS(transform.position+offset, transform.rotation, Vector3.one);
        Gizmos.matrix = boxMatrix;
        Gizmos.DrawWireCube(Vector3.zero, detectRange*2);
        Gizmos.matrix = Matrix4x4.identity; // 매트릭스를 리셋하여 다른 Gizmos에 영향 안 미치도록 함
    }

    private bool Detect()
    {
        Array.Clear(_detectedItems,0,_detectedItems.Length);
        Vector3 offset = (transform.forward + Vector3.up) * detectOffset;
        int hits = Physics.OverlapBoxNonAlloc(transform.position + offset, detectRange, _detectedItems, transform.rotation,targetLayer);
        return hits > 0;
    }

    private void Grab()
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
        
        if (closest is not null && closest.TryGetComponent(out _grabbedItem))
        {
            closest.transform.SetParent(pivot);
            closest.transform.localPosition = Vector3.zero;
            closest.transform.localRotation = Quaternion.identity;
            _grabbedItem.rb.isKinematic = _grabbedItem.isGrabbed = true;
        }
    }

    private void Ungrab()
    { 
        _grabbedItem.transform.SetParent(null);
        _grabbedItem.rb.isKinematic = _grabbedItem.isGrabbed = false;
        _grabbedItem = null;
    }
}
