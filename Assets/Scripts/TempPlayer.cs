using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using SF = UnityEngine.SerializeField;

public class TempPlayer : MonoBehaviour
{
    [SF] private NavMeshAgent agent;

    public Vector3 cursorPos;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    public void OnCursorPos(InputAction.CallbackContext ctx)
    {
        cursorPos = ctx.ReadValue<Vector2>();
    }

    public void OnLeftClick(InputAction.CallbackContext ctx)
    {
        

        // cursorPos = Input.mousePosition;
        //
        // Debug.Log($"OnLeftClick {cursorPos.x},{cursorPos.y}");
        // if (cursorPos == Vector3.zero) return;
        //
        // Physics.Raycast(cursorPos, , out RaycastHit hit);
        // Debug.Log($"Hit {hit.point.x},{hit.point.y},{hit.point.z}");
        //
        // Vector3 pos = Camera.main.ScreenToWorldPoint(hit.point);
        // agent.SetDestination(pos);
    }
}
