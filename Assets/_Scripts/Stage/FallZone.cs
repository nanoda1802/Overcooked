using System;
using UnityEngine;
using Random = UnityEngine.Random;
using SF = UnityEngine.SerializeField;

public class FallZone : MonoBehaviour
{
    [SF] private FloorShifter floorShifter;
    
    [SF,Range(0,2)] private float floorDetectRadius = 1f; // 너무 넓으면 오히려 안 돼
    [SF] private LayerMask floorLayer = 1<<3;
    private readonly Collider[] _detectedFloors = new Collider[5];
    
    [SF] private RespawnTimer respawnTimerPrefab;
    private RespawnTimer _respawnTimer;
    
    private void Start()
    {
        _respawnTimer = Instantiate(respawnTimerPrefab, floorShifter.DefaultRespawnPoint.transform);
        _respawnTimer.Init();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Item") && other.TryGetComponent(out Item item))
        {
            item.Deactivate();
            return;
        }

        if (other.CompareTag("Player") && other.TryGetComponent(out PlayerController_Stage player))
        {
            ReserveRespawn(player);
        }
    }

    private void ReserveRespawn(PlayerController_Stage player)
    {
        Floor respawnFloor = FindRespawnPoint(player.transform.position);
        respawnFloor.SwitchToRespawnReservedState();
        
        player.Despawn(respawnFloor);   
        
        _respawnTimer.Activate(respawnFloor.GetRespawnPosition());
        _respawnTimer.OnTimerDone += respawnFloor.SwitchToIdleState;
        _respawnTimer.OnTimerDone += player.Respawn;
    }

    private Floor FindRespawnPoint(Vector3 despawnPos)
    {
        // [안 해도 되는 이유] -> Array.Clear(_detectedFloors,0,_detectedFloors.Length);
        // RaycastHit는 구조체라 Clear는 굳이 default 값으로 갱신하는 작업이 추가되는 것
        // 게다가 SphereCastNonAlloc는 애초에 감지된 값들만 갱신하고,
        // size 변수를 활용해 순회 범위를 제한한다면 이전 감지 데이터에 접근할 일이 없음
        
        // [SphereCastNonAlloc이 아닌 이유] -> int size = Physics.SphereCastNonAlloc(despawnPos, floorDetectRadius, Vector3.zero, _detectedFloors,0,floorMask);
        // 어차피 maxDistance를 0으로 둘거라면 OverlapSphere와 똑같드ㅏ
        // Cast는 아무튼 특정 방향으로 발사하는 것
        
        int detectedCount = Physics.OverlapSphereNonAlloc(despawnPos, floorDetectRadius, _detectedFloors, floorLayer);
        if (detectedCount <= 0 || detectedCount > _detectedFloors.Length) return floorShifter.GetSafeFloor();
        
        int rndIdx = Random.Range(0, detectedCount);
        if (!_detectedFloors[rndIdx].TryGetComponent(out Floor targetFloor)) return floorShifter.GetSafeFloor();
        
        return targetFloor;
    }
}
