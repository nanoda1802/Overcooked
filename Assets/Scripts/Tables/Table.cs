using UnityEngine;
using SF = UnityEngine.SerializeField;

public abstract class Table : MonoBehaviour
{
    [SF] protected Transform pivot;
    public virtual bool Interact(InStagePlayerController inStagePlayer) { return false; }
}
