using UnityEngine;
using UnityEngine.AI;
using SF = UnityEngine.SerializeField;

public class PedsMobPool : ObjPool<PedsMob>
{
    private int _areaMask;
    [SF] private float pedsMinSpeed;
    [SF] private float pedsMaxSpeed;
    [SF] private float pedsAngularSpeed;
    [SF] private float pedsAcceleration;
    
    
    public override void InitPool()
    {
        base.InitPool();
        _areaMask = 1 << NavMesh.GetAreaFromName("Peds") | 1 << NavMesh.GetAreaFromName("Both");
    }

    protected override PedsMob Create()
    {
        PedsMob mob = base.Create();
        mob.Init(_areaMask, m => Pool.Release(m as PedsMob));
        return mob;
    }

    protected override void OnGet(PedsMob obj)
    {
        base.OnGet(obj);
        obj.SetAgentInfo(Random.Range(pedsMinSpeed, pedsMaxSpeed), pedsAngularSpeed, pedsAcceleration);
    }

    protected override void OnRelease(PedsMob obj)
    {
        base.OnRelease(obj); // 비활성화
        // 풀로 넣을 때 해줄 초기화
        obj.ResetAgentInfo();
    }
}
