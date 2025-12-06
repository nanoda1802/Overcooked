using UnityEngine;
using SF = UnityEngine.SerializeField;

public class FallZone : MonoBehaviour
{
    [SF] private MovableUIPool uiPool;

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

        if (other.CompareTag("Player") && other.TryGetComponent(out PlayerController player))
        {
            player.WaitForRespawn();
            Vector3 respawnPos = player.CalculateRespawnPosition();
            
            if (!uiPool.TryGetItem(out RespawnTimer ui)) return;
            ui.Activate();
            ui.SetRespawnValues(respawnPos, player);
        }
    }
}
