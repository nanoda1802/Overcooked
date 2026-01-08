using System;
using System.Linq;
using UnityEngine;
using SF = UnityEngine.SerializeField;

public class FallZone : MonoBehaviour
{
    [SF] private MovableUIPool uiPool;
    [SF] private StageManager stageManager;

    [SF] private FloorShifter floorShifter;
    
    [SF] private float floorDetectRadius = 1.5f; // 너무 넓으면 안 돼
    [SF] private LayerMask floorLayer = 1<<3;
    private readonly Collider[] _detectedFloors = new Collider[5];
    
    [SF] private RespawnTimer2 respawnTimerPrefab;
    
    private void Start()
    {
        if (uiPool is not null) return;
        uiPool = GameObject.Find("SubCanvas").GetComponent<MovableUIPool>();
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
            Vector3 despawnPos = player.DespawnPlayer();
            // Vector3 respawnPos = stageManager.StageInfo.GetClosestRespawnPoint(despawnPos);
            //
            // if (!uiPool.TryGetItem(out RespawnTimer ui)) return;
            // ui.Activate();
            // ui.SetRespawnValues(respawnPos, player);
            
            // 반환 받은 Floor의 position에 y localScale 더한 벡터값이 respawnPos
            // 리스폰타이머2는 Floor의 자식으로 넣고 Activate
            
            Floor respawnPoint = FindRespawnPoint(despawnPos);
            respawnPoint.ReserveRespawn();
            RespawnTimer2 respawnTimer = Instantiate(respawnTimerPrefab, respawnPoint.transform); // [임시]
            respawnTimer.SubscribeEvent(respawnPoint.OnRespawnDone);
            respawnTimer.SubscribeEvent(()=>player.Respawn(respawnPoint.GetRespawnPosition()));
        }
    }

    private Floor FindRespawnPoint(Vector3 despawnPos)
    {
        // RaycastHit는 구조체라 Clear는 굳이 default 값으로 갱신하는 작업이 추가되는 것
        // 게다가 SphereCastNonAlloc는 애초에 감지된 값들만 갱신하고,
        // size 변수를 활용해 순회 범위를 제한한다면 이전 감지 데이터에 접근할 일이 없음
        // Array.Clear(_detectedFloors,0,_detectedFloors.Length);
        
        // 가장 가까운 floor 반환, 설마 감지 실패했으면(size가 0보다 작으면) FloorShifter가 그냥 안전한 위치 찾아서 반환 
        // int size = Physics.SphereCastNonAlloc(despawnPos, floorDetectRadius, Vector3.zero, _detectedFloors,0,floorMask);
        
        int size = Physics.OverlapSphereNonAlloc(despawnPos, floorDetectRadius, _detectedFloors, floorLayer);
        Debug.Log(size);
        float minDist = float.MaxValue;
        int minIdx = -1;
        
        for (int i = 0; i < size; i++)
        {
            if (_detectedFloors[i] is null) continue;
            
            float dist = (despawnPos - _detectedFloors[i].transform.position).sqrMagnitude;
            
            if (minDist <= dist) continue;
            minDist = dist;
            minIdx = i;
        }
        
        return minIdx < 0 ? floorShifter.GetSafeFloor() : _detectedFloors[minIdx].GetComponent<Floor>();
    }
}
