using UnityEngine;
using SF = UnityEngine.SerializeField;

public class FallZone : MonoBehaviour
{
    [SF] private MovableUIPool uiPool; // [안 쓸 듯?]
    [SF] private StageManager stageManager; // [안 쓸 듯?]

    [SF] private FloorShifter floorShifter;
    
    [SF,Range(0,2)] private float floorDetectRadius = 1f; // 너무 넓으면 오히려 안 돼
    [SF] private LayerMask floorLayer = 1<<3;
    private readonly Collider[] _detectedFloors = new Collider[5];
    
    [SF] private RespawnTimer2 respawnTimerPrefab;
    private RespawnTimer2 _respawnTimer;
    
    [SF] private Floor defaultRespawnPoint;
    
    private void Start()
    {
        _respawnTimer = Instantiate(respawnTimerPrefab, defaultRespawnPoint.transform);
        _respawnTimer.Init();
        
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
            // Vector3 respawnPos = stageManager.StageInfo.GetClosestRespawnPoint(despawnPos);
            //
            // if (!uiPool.TryGetItem(out RespawnTimer ui)) return;
            // ui.Activate();
            // ui.SetRespawnValues(respawnPos, player);
            
            // 반환 받은 Floor의 position에 y localScale 더한 벡터값이 respawnPos
            // 리스폰타이머2는 Floor의 자식으로 넣고 Activate
            
            Vector3 despawnPos = player.Despawn();
            Floor respawnFloor = FindRespawnPoint(despawnPos) ?? defaultRespawnPoint;
            
            respawnFloor.ReserveRespawn();
            // RespawnTimer2 respawnTimer = Instantiate(respawnTimerPrefab, respawnFloor.transform); // [임시]
            _respawnTimer.Activate(respawnFloor.transform.position);
            
            _respawnTimer.OnTimerDone += respawnFloor.OnRespawnDone;
            _respawnTimer.OnTimerDone += () => player.Respawn(respawnFloor.GetRespawnPosition());
        }
    }

    private Floor FindRespawnPoint(Vector3 despawnPos)
    {
        // RaycastHit는 구조체라 Clear는 굳이 default 값으로 갱신하는 작업이 추가되는 것
        // 게다가 SphereCastNonAlloc는 애초에 감지된 값들만 갱신하고,
        // size 변수를 활용해 순회 범위를 제한한다면 이전 감지 데이터에 접근할 일이 없음
        // Array.Clear(_detectedFloors,0,_detectedFloors.Length);
        
        // int size = Physics.SphereCastNonAlloc(despawnPos, floorDetectRadius, Vector3.zero, _detectedFloors,0,floorMask);
        // 어차피 maxDistance를 0으로 둘거라면 OverlapSphere와 똑같드ㅏ
        // Cast는 아무튼 특정 방향으로 발사하는 것
        
        int detectedCount = Physics.OverlapSphereNonAlloc(despawnPos, floorDetectRadius, _detectedFloors, floorLayer);
        Debug.Log($"감지된 Floor 수 : {detectedCount}");
        if (detectedCount <= 0 || detectedCount > _detectedFloors.Length) return floorShifter.GetSafeFloor();
        
        int rndIdx = Random.Range(0, detectedCount);
        if (!_detectedFloors[rndIdx].TryGetComponent(out Floor targetFloor)) return floorShifter.GetSafeFloor();
        
        Debug.Log("근처 Floor 감지함!");
        return targetFloor;
    }
}
