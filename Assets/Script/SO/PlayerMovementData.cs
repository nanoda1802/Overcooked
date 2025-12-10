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
    [SF, Range(0f, 2f)] private float inertiaDecayTime;
    [SF, Range(0f, 5f)] private float runSpeedMultiplier;
    public float DashForce => dashForce;
    public float InertiaDecayTime => inertiaDecayTime;
    public float RunSpeedMultiplier => runSpeedMultiplier;
}