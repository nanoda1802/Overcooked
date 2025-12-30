using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;
using SF = UnityEngine.SerializeField;

[CreateAssetMenu(fileName = "AnimationData", menuName = "SO/Player/Animation")]
public class PlayerAnimData : ScriptableObject
{
    [Header("[ Param Hash ]")]
    [SF] private string moveParam;
    [SF] private string moveSpeedParam;
    [SF] private string dashParam;
    [SF] private string pickParam;
    [SF] private string afkParam;
    [SF] private string chopParam;
    [SF] private string washDishParam;
    
    public int MoveHash { get; private set; }
    public int MoveSpeedHash { get; private set; }
    public int DashHash { get; private set; }
    public int PickHash { get; private set; }
    public int AfkHash { get; private set; }
    public int ChopHash { get; private set; }
    public int WashDishHash { get; private set; }
    
    public void Init()
    {
        MoveHash = Animator.StringToHash(moveParam);
        MoveSpeedHash = Animator.StringToHash(moveSpeedParam);
        DashHash = Animator.StringToHash(dashParam);
        PickHash = Animator.StringToHash(pickParam);
        AfkHash = Animator.StringToHash(afkParam);
        ChopHash = Animator.StringToHash(chopParam);
        WashDishHash = Animator.StringToHash(washDishParam);
    }
}
