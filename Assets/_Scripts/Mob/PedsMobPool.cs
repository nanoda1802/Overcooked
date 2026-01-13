using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Pool;
using SF = UnityEngine.SerializeField;

public class PedsMobPool : ObjPool<PedsMob>
{
    private int _areaMask;
    [SF] private int agentPriorityOffset;
    [SF] private float pedsMinSpeed;
    [SF] private float pedsMaxSpeed;
    [SF] private float pedsAngularSpeed;
    [SF] private float pedsAcceleration;
    
    
    public override ObjectPool<PedsMob> InitPool()
    {
        base.InitPool();
        _areaMask = 1 << NavMesh.GetAreaFromName("Peds") | 1 << NavMesh.GetAreaFromName("Both");
        return Pool;
    }

    protected override PedsMob CreateObj()
    {
        PedsMob mob = base.CreateObj();
        mob.Init(_areaMask, m => Pool.Release(m as PedsMob));
        mob.SetAgentInfo(agentPriorityOffset + objIdx++, pedsAngularSpeed, pedsAcceleration);
        return mob;
    }

    protected override void OnGot(PedsMob obj)
    {
        base.OnGot(obj);
        obj.ActivateAgent(Random.Range(pedsMinSpeed, pedsMaxSpeed));
    }

    protected override void OnReleased(PedsMob obj)
    {
        base.OnReleased(obj); // 비활성화
        // 풀로 넣을 때 해줄 초기화
        obj.DeactivateAgent();
    }
}
