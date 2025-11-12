using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Test : MonoBehaviour
{
    public Rigidbody rigid;

    public float moveSpeed = 5f;
    private Vector3 moveDir;
    public float rotSpeed = 0.5f;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
    }

    public void OnMove(InputAction.CallbackContext ctx)
    {
        var input = ctx.ReadValue<Vector2>();
        moveDir.x = input.x;
        moveDir.z = input.y;
        rigid.velocity = moveSpeed * moveDir;
        
        if (moveDir.sqrMagnitude > 0.001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(moveDir);
            Quaternion smoothRot = Quaternion.Slerp(
                rigid.rotation,
                targetRot,
                rotSpeed
            );
            rigid.MoveRotation(smoothRot);
        }
    }
}
