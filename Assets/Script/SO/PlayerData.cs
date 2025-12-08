using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SF = UnityEngine.SerializeField;

[CreateAssetMenu(fileName = "MovementData", menuName = "SO/Player/Movement")]
public class PlayerMovementData : ScriptableObject
{
    [Header("[ Move & Rotate ]")]
    [SF, Range(0f, 10f)] private float moveSpeed;
    [SF, Range(0f, 1f)] private float rotRatio;
    public float MoveSpeed => moveSpeed;
    public float RotRatio => rotRatio;
    
    [Header("[ Dash & Run ]")]
    [SF, Range(0f, 10f)] private float dashForce;
    [SF, Range(0f, 5f)] private float runSpeedMultiplier;
    public float DashForce => dashForce;
    public float RunSpeedMultiplier => runSpeedMultiplier;
}

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