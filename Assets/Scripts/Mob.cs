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
    
    [SF] private NavMeshAgent agent;
    private float _speed;
    
    private Coroutine _moveCoroutine;
    private Action<Mob> _onArrived;
    
    public void SetReleaseEvent(Action<Mob> onArrived)
    {
        _onArrived = onArrived;
    }

    public void ReadyFromEntryPoint(Transform entryPoint)
    {
        if (NavMesh.SamplePosition(entryPoint.position, out NavMeshHit hit, 1f, agent.areaMask))
        {
            transform.position = hit.position;
            transform.LookAt(entryPoint.forward); 
            return;
        }
        
        _onArrived?.Invoke(this);
    }

    public void SetSpeed(float speed)
    {
        _speed = speed;
    }

    public void SetEndPoint(Transform endPoint) // [임시]
    {
        if (_moveCoroutine is not null) StopCoroutine(_moveCoroutine);
        _moveCoroutine = StartCoroutine(CoMove(endPoint));
    }

    private IEnumerator CoMove(Transform endPoint) // [작성중]
    {
        agent.SetDestination(endPoint.position);
        yield return null;
    }

    private IEnumerator CoSmoothBreak()
    {
        while (agent.speed > 0)
        {
            agent.speed = Mathf.Lerp(_speed, 0, Time.deltaTime*3);
            yield return null;
        }
        agent.isStopped = true;
    }

    private IEnumerator CoSmoothAccelerate()
    {
        while (agent.speed < _speed)
        {
            agent.speed = Mathf.Lerp(0, _speed, Time.deltaTime*3);
            yield return null;
        }
        agent.isStopped = false;
    }

    public void OnArrived() // [임시]
    {
        _onArrived?.Invoke(this);
    }
}
