using UnityEngine.AI;
using UnityEngine.Pool;
using SF = UnityEngine.SerializeField;

public class VehicleMobPool : ObjPool<VehicleMob>
{
    private int _areaMask;
    [SF] private int agentPriorityOffset;
    [SF] private float vehicleSpeed;
    [SF] private float vehicleAngularSpeed;
    [SF] private float vehicleAcceleration;
    
    public override ObjectPool<VehicleMob> InitPool()
    {
        base.InitPool();
        _areaMask = 1 << NavMesh.GetAreaFromName("Vehicle") | 1 << NavMesh.GetAreaFromName("Both");
        return Pool;
    }
    
    protected override VehicleMob CreateObj()
    {
        VehicleMob mob = base.CreateObj();
        mob.Init(_areaMask,m => Pool.Release(m as VehicleMob));
        mob.SetAgentInfo(agentPriorityOffset + objIdx++, vehicleAngularSpeed, vehicleAcceleration);
        return mob;
    }

    protected override void OnGot(VehicleMob obj)
    {
        if (isPrewarming) return;
        base.OnGot(obj);
        obj.ActivateAgent(vehicleSpeed);
    }

    protected override void OnReleased(VehicleMob obj)
    {
        if (isPrewarming) return;
        base.OnReleased(obj); // 비활성화
        // 풀로 넣을 때 해줄 초기화
        obj.DeactivateAgent();
    }
}
