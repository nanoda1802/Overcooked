using UnityEngine;
using SF = UnityEngine.SerializeField;

public class FallZone : MonoBehaviour
{
    [SF] private MovableUIPool uiPool;
    [SF] private InStageManager inStageManager;
    
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

        if (other.CompareTag("Player") && other.TryGetComponent(out InStagePlayerController player))
        {
            Vector3 despawnPos = player.DespawnPlayer();
            Vector3 respawnPos = inStageManager.StageInfo.GetClosestRespawnPoint(despawnPos);
            
            if (!uiPool.TryGetItem(out RespawnTimer ui)) return;
            ui.Activate();
            ui.SetRespawnValues(respawnPos, player);
        }
    }
}
