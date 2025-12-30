using Sfx;
using UnityEngine;
using SF = UnityEngine.SerializeField;

[CreateAssetMenu(fileName = "SfxData", menuName = "SO/Player/Sfx")]
public class PlayerSfxData : ScriptableObject
{
    [SF] private SfxInfo moveSfx;
    [SF] private SfxInfo actionBlockedSfx;
    [SF] private SfxInfo despawnSfx;
    [SF] private SfxInfo dashSfx;
    [SF] private SfxInfo attachSfx;
    [SF] private SfxInfo throwSfx;
    
    public SfxInfo MoveSfx => moveSfx;
    public SfxInfo ActionBlockedSfx => actionBlockedSfx;
    public SfxInfo DespawnSfx => despawnSfx;
    public SfxInfo DashSfx => dashSfx;
    public SfxInfo AttachSfx => attachSfx;
    public SfxInfo ThrowSfx => throwSfx;
}
