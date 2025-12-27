using System;
using UnityEngine;
using Random = UnityEngine.Random;
using SF = UnityEngine.SerializeField;

[Serializable]
public struct WaypointPair
{
    public Transform entryPoint;
    public Transform endPoint;
}

public class MobManager : MonoBehaviour
{
    [SF] private VehicleMobPool vehiclePool; // 자식으로 두기
    [SF] private PedsMobPool pedsPool; // 자식으로 두기

    private float _vehicleSpawnTimer;
    private float _pedsSpawnTimer;
    [SF] private float vehicleSpawnDuration;
    [SF] private float pedsSpawnDuration;
    
    // 알고 있어야할 것들 (일단 Transform 인디, point 스크립트 만들어 변경)
    [SF] private WaypointPair[] vehicleWaypoints;
    [SF] private Transform[] pedsWaypoints;
    
    private void Awake() // [임시] 얘도 어디선가 상위의 녀석이 호출하도록... awake 말구
    {
        vehiclePool.InitPool(); // 이런 구조가 맞지비
        pedsPool.InitPool(); // 이런 구조가 맞지비
    }

    private void OnEnable()
    {
        _vehicleSpawnTimer = _pedsSpawnTimer = 0;
    }

    private void Update()
    {
        _vehicleSpawnTimer += Time.deltaTime;
        if (_vehicleSpawnTimer >= vehicleSpawnDuration) SpanwVehiclesOnAllPoints();
        
        _pedsSpawnTimer += Time.deltaTime;
        if (_pedsSpawnTimer >= pedsSpawnDuration) SpawnPedsOnAllPoints();
    }

    private void OnDestroy()
    {
        vehiclePool?.DisposePool();
        pedsPool?.DisposePool();
    }

    private Transform GetRandomEndPoint(int entryPointIdx)
    {
        int pointCnt = pedsWaypoints.Length;
        if (pointCnt <= 1) return null;
        
        int offset = Random.Range(1, pointCnt); 
        int endPointIdx = (entryPointIdx + offset) % pointCnt;
        return pedsWaypoints[endPointIdx];
    }

    private void SpawnPedsOnAllPoints()
    {
        _pedsSpawnTimer = 0f;
        
        for (int i = 0; i < pedsWaypoints.Length; i++)
        {
            if (pedsPool.Pool.CountActive >= pedsPool.MaxPoolSize) break;
            
            float rnd = Random.Range(0f, 10f);
            if (rnd < 5f) continue;
            SpawnPeds(pedsWaypoints[i], GetRandomEndPoint(i));
        }
    }

    private void SpanwVehiclesOnAllPoints()
    {
        _vehicleSpawnTimer = 0f;
        
        for (int i = 0; i < vehicleWaypoints.Length; i++)
        {
            if (vehiclePool.Pool.CountActive >= vehiclePool.MaxPoolSize) break;
            
            SpawnVehicle(vehicleWaypoints[i].entryPoint, vehicleWaypoints[i].endPoint);
        }
    }

    /* Fisher-Yates 셔플 방식... */
    // private (Transform,Transform) GetRandomWayPointPair() 
    // {
    //     if (pedsWayPoints.Length <= 1) return (null, null);
    //     
    //     for (int i = 0; i < pedsWayPoints.Length-2; i++)
    //     {
    //         int rndIdx = Random.Range(i, pedsWayPoints.Length-1);
    //         if (rndIdx == i) continue;
    //         (pedsWayPoints[i],pedsWayPoints[rndIdx]) = (pedsWayPoints[rndIdx], pedsWayPoints[i]);
    //     }
    //     return (pedsWayPoints[0], pedsWayPoints[1]);
    // }

    private void SpawnVehicle(Transform entryPoint, Transform endPoint)
    {
        VehicleMob vehicle = vehiclePool.Pool.Get();
        if (!vehicle.TryReadyFromEntryPoint(entryPoint)) return;
        vehicle.SetEndPoint(endPoint);
    }
  
    private void SpawnPeds(Transform entryPoint, Transform endPoint)
    {
        PedsMob peds = pedsPool.Pool.Get();
        if (!peds.TryReadyFromEntryPoint(entryPoint)) return;
        peds.SetEndPoint(endPoint);
    }
}
