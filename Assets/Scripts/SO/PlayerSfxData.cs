using Sfx;
using UnityEngine;
using SF = UnityEngine.SerializeField;

[CreateAssetMenu(fileName = "SfxData", menuName = "SO/Player/Sfx")]
public class PlayerSfxData : ScriptableObject
{
    [SF] private ClipInfo moveSfx;
    [SF] private ClipInfo actionBlockedSfx;
    [SF] private ClipInfo despawnSfx;
    [SF] private ClipInfo dashSfx;
    [SF] private ClipInfo attachSfx;
    [SF] private ClipInfo throwSfx;
    
    public ClipInfo MoveSfx => moveSfx;
    public ClipInfo ActionBlockedSfx => actionBlockedSfx;
    public ClipInfo DespawnSfx => despawnSfx;
    public ClipInfo DashSfx => dashSfx;
    public ClipInfo AttachSfx => attachSfx;
    public ClipInfo ThrowSfx => throwSfx;
}
