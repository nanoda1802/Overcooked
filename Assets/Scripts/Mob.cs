using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using SF = UnityEngine.SerializeField;

public class Mob : MonoBehaviour
{
    // Mob에서 공통적으로 해야할 일
    // 시작점으로 이동 후 방향에 맞게 회전
    // 목적지 설정
    // 목적지에 도착하면 pool에 복귀
    
    [SF] protected NavMeshAgent agent;
    private float _speed;
    
    [SF] private float arrivalDistanceThreshold;
    private Coroutine _moveCoroutine;
    private WaitUntil _waitPathPending;
    private WaitUntil _waitAgentArrival;
    
    private Action<Mob> _onArrived;

    public void Init(int areaMask, Action<Mob> onArrived)
    {
        if (!TryGetComponent(out agent))
        {
            agent = gameObject.AddComponent<NavMeshAgent>();
            agent.areaMask = areaMask;
        }
        
        _onArrived = onArrived;
        
        _waitPathPending = new WaitUntil(()=>!agent.pathPending);
        _waitAgentArrival = new WaitUntil(()=>agent.remainingDistance <= arrivalDistanceThreshold);
    }

    public void DeactivateAgent()
    {
        agent.enabled = false; // 이걸 끄고 키는 것 만으로도 path 같은 정보가 클리어 된다는디?
        // 모르겠엄ㄴㄹ
    }

    public bool TryReadyFromEntryPoint(Transform entryPoint)
    {
        if (NavMesh.SamplePosition(entryPoint.position, out NavMeshHit hit, 1f, agent.areaMask))
        {
            agent.Warp(hit.position);
            transform.LookAt(entryPoint.forward); 
            return true;
        }
        
        _onArrived?.Invoke(this);
        return false;
    }

    public void SetAgentInfo(int priority, float angularSpeed, float acceleration)
    {
        agent.avoidancePriority = priority;
        agent.angularSpeed = angularSpeed;
        agent.acceleration = acceleration;
    }

    public void ActivateAgent(float speed)
    {
        agent.enabled = true;
        _speed = speed;
    }

    public void SetEndPoint(Transform endPoint) // [임시]
    {
        if (_moveCoroutine is not null) StopCoroutine(_moveCoroutine);
        _moveCoroutine = StartCoroutine(CoMove(endPoint));
    }

    private IEnumerator CoMove(Transform endPoint) // [작성중]
    {
        agent.speed = _speed;
        agent.SetDestination(endPoint.position);
        yield return _waitPathPending;
        yield return _waitAgentArrival;
        _onArrived?.Invoke(this);
    }

    protected IEnumerator CoSmoothBreak()
    {
        agent.speed = _speed;
        while (agent.speed > 0)
        {
            agent.speed = Mathf.Lerp(_speed, 0, agent.speed - Time.deltaTime * 0.2f);
            yield return null;
        }
        agent.isStopped = true;
    }

    protected IEnumerator CoSmoothAccelerate()
    {
        agent.isStopped = false;
        agent.speed = 0;
        while (agent.speed < _speed)
        {
            agent.speed = Mathf.Lerp(0, _speed, agent.speed + Time.deltaTime * 0.2f);
            yield return null;
        }
    }
}
