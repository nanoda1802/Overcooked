using System;
using UnityEngine;
using UnityEngine.InputSystem;
using SF = UnityEngine.SerializeField;

// 참고 링크 https://rito15.github.io/posts/unity-rigidbody-move-and-jump/
public class PlayerController : MonoBehaviour
{
    private Rigidbody rb;
    private Vector3 moveDir;
    
    [SF, Range(0f, 100f)] private float moveSpeed = 5f;
    [SF, Range(0f, 200f)] private float rotSpeed = 100f;
    [SF, Range(0f, 100f)] private float dashForce = 5f;
    [SF, Range(1f, 2f)] private float dashSpeedModifier = 1f;
    
    private void Awake()
    {
        if (!TryGetComponent(out rb)) rb = gameObject.AddComponent<Rigidbody>();
        rb.freezeRotation = true;
        Application.targetFrameRate = 60;
    }

    private void FixedUpdate()
    {
        if (moveDir.sqrMagnitude <= 0.001f) return;
        Move();
        Rotate();
    }

    private void Move()
    {
        Vector3 moveOffset = moveSpeed * dashSpeedModifier * Time.fixedDeltaTime * moveDir;
        rb.MovePosition(rb.position + moveOffset);
    }

    private void Rotate()
    {
        Quaternion smoothRot = Quaternion.Slerp(rb.rotation, Quaternion.LookRotation(moveDir), rotSpeed * Time.fixedDeltaTime);
        rb.MoveRotation(smoothRot);
    }

    public void OnMove(InputAction.CallbackContext ctx)
    {
        Vector2 input = ctx.ReadValue<Vector2>();
        moveDir.x = input.x;
        moveDir.z = input.y;
    }

    public void OnDash(InputAction.CallbackContext ctx)
    {
        // 알아보고 다시 짜기!
        // 입력시 짧게 대시, 누르고있으면 달리기 상태, 떼면 해제?
        if (ctx.started) rb.AddForce(dashForce * moveDir, ForceMode.VelocityChange);
        if (ctx.performed) dashSpeedModifier = 2f;
        if (ctx.canceled) dashSpeedModifier = 1f;
    }
}
