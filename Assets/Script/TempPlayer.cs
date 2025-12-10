using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using SF = UnityEngine.SerializeField;

public class TempPlayer : MonoBehaviour
{
    [SF] private NavMeshAgent agent;
    
    
    
    
    public void OnLeftClick(InputAction.CallbackContext ctx)
    {
        
        
        
        agent.SetDestination(transform.position);
    }
}
