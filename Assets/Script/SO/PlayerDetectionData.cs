using UnityEngine;
using SF = UnityEngine.SerializeField;

[CreateAssetMenu(fileName = "DetectionData", menuName = "SO/Player/Detection")]
public class PlayerDetectionData : ScriptableObject
{
    [Header("[ Detect Table ]")]
    [SF] private LayerMask tableLayer;
    [SF, Range(0f, 2f)] private float detectRayDistance;
    [SF, Range(0f, 2f)] private float detectRayOffsetY;
    public LayerMask TableLayer => tableLayer;
    public float DetectRayDistance => detectRayDistance;
    public float DetectRayOffsetY => detectRayOffsetY;
    
    [Header("[ Detect Item ]")]
    [SF] private LayerMask itemLayer;
    [SF] private Vector3 detectBoxSize;
    [SF, Range(0f, 5f)] private float detectBoxOffset;
    public LayerMask ItemLayer => itemLayer;
    public Vector3 DetectBoxSize => detectBoxSize;
    public float DetectBoxOffset => detectBoxOffset;
}
