using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SF = UnityEngine.SerializeField;

[CreateAssetMenu(menuName = "SO/Stage/Floor", fileName = "FloorData")]
public class FloorData : ScriptableObject
{
    public Floor DefaultFloor { get; set; }
    
    /* Find Floors */
    [Header("[ Detect ]")]
    [SF] private LayerMask tableLayer = 1 << 6;
    [SF] private float tableDetectDistance = 5f;
    [SF] private LayerMask playerLayer = 1 << 8;
    [SF] private Vector3 playerDetectBoxSize = new Vector3(0.25f, 1f, 0.25f);
    /* Tween */
    [Header("[ Tween Values ]")]
    [SF] private float originY = 0f;
    [SF] private float targetY = -5f;
    [SF,Range(1,10)] private float tweenDuration = 2f;
    
    public float OriginY => originY;
    public float TargetY => targetY;
    public float TweenDuration => tweenDuration;
    
    public bool HasTable(Transform floorTransform)
    {
        return Physics.Raycast(floorTransform.position, Vector3.up, tableDetectDistance, tableLayer);
    }

    public bool HasPlayer(Transform floorTransform)
    {
        return Physics.CheckBox(floorTransform.position, playerDetectBoxSize, Quaternion.identity, playerLayer);
    }
}
