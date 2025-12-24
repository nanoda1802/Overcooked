using UnityEngine;
using UnityEngine.AI;
using SF = UnityEngine.SerializeField;

public class VehicleMobPool : ObjPool<VehicleMob>
{
    private int _areaMask;
    [SF] private float vehicleMinSpeed;
    [SF] private float vehicleMaxSpeed;
    [SF] private float vehicleAcceleration;
    
    public override void InitPool()
    {
        base.InitPool();
        _areaMask = 1 << NavMesh.GetAreaFromName("Vehicle") | 1 << NavMesh.GetAreaFromName("Both");
    }
    
    protected override VehicleMob Create()
    {
        VehicleMob mob = base.Create();
        mob.Init(_areaMask,m => Pool.Release(m as VehicleMob));
        return mob;
    }

    protected override void OnGet(VehicleMob obj)
    {
        base.OnGet(obj);
        obj.SetAgentInfo(Random.Range(vehicleMinSpeed, vehicleMaxSpeed), vehicleAcceleration);
    }

    protected override void OnRelease(VehicleMob obj)
    {
        base.OnRelease(obj); // 비활성화
        // 풀로 넣을 때 해줄 초기화
        obj.ResetAgentInfo();
    }
}
