using UnityEngine;
using SF = UnityEngine.SerializeField;

public class MobManager : MonoBehaviour
{
    [SF] private VehicleMobPool vehiclePool; // 자식으로 두기
    [SF] private PedsMobPool pedsPool; // 자식으로 두기

    private float _timer;
    [SF] private float vehicleSpawnDuration;
    [SF] private float pedsSpawnDuration;
    
    // 알고 있어야할 것들 (일단 Transform 인디, point 스크립트 만들어 변경)
    [SF] private Transform[] vehicleEntryPoints;
    [SF] private Transform[] vehicleEndPoints;
    [SF] private Transform[] pedsWayPoints;

    private void Awake() // [임시] 얘도 어디선가 상위의 녀석이 호출하도록... awake 말구
    {
        vehiclePool.InitPool(); // 이런 구조가 맞지비
        pedsPool.InitPool(); // 이런 구조가 맞지비
    }

    private void OnEnable()
    {
        _timer = 0;
    }

    private void OnDestroy()
    {
        vehiclePool?.DisposePool();
        pedsPool?.DisposePool();
    }
    
    private void SpawnVehicle()
    {
        // 시작 위치 정하고
        // 목적지 정하고
        vehiclePool.Pool.Get(); // 가져오고
        // mob 시작위치로 이동시키고,
        // mob한테 목적지 전달해주기
    }

    private void SpawnPeds()
    {
        // 시작 위치 정하고
        // 목적지 정하고
        pedsPool.Pool.Get(); // 가져오고
        // mob 시작위치로 이동시키고,
        // mob한테 목적지 전달해주기
    }
}
