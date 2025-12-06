using UnityEngine;

public class FallZone : MonoBehaviour
{
    private Canvas subCanvas;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Item") && other.TryGetComponent(out Item item))
        {
            item.Deactivate();
            return;
        }

        if (other.CompareTag("Player") && other.TryGetComponent(out PlayerController player))
        {
            
            Vector3 respawnPos = player.CalculateRespawnPosition();
            
            // 그리고 해당 블록 위치에 타이머 UI 
        }
    }
}
