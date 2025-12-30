using System.Collections.Generic;
using UnityEngine;
using SF = UnityEngine.SerializeField;

public class AnimParams
{
    public readonly string AnimatorName;
    private readonly Dictionary<string, int> _hashes = new Dictionary<string, int>();

    public AnimParams(Animator animator)
    {
        AnimatorName = animator.name;
        
        foreach (var param in animator.parameters) 
            _hashes.Add(param.name, param.nameHash);
    }

    public int GetHash(string paramName)
    {
        if (!_hashes.TryGetValue(paramName, out int hash))
        {
            #if UNITY_EDITOR
            Debug.LogWarning($"{AnimatorName} 에는 \"{paramName}\"(이)란 파라미터가 존재하지 않슴다. [AnimParam.GetHash]");
            #endif
            return 0;
        }
        return hash;
    }

    // [메모] Animator Transition 지식 하나
    
    // interruption source는 해당 트랜지션이 진행 중일 때,
    // 그 다음 트랜지션의 조건이 만족된 경우, 어떻게 할 것인지에 대한 설정
    // 예를 들어 Next State면, 이번 트랜지션의 목적 State를 재생하지 않고 그 다음 State로 향함
    // 또는 Current State Then Next State면, 이번 트랜지션의 목적 State를 재생하고서 그 다음 State로 향함
    
    // A->B 트랜지션의 interruption source가 Next State고, B->C로의 트랜지션이 있을 때,
    // A에서 B로 전환되던 중 C로의 조건이 만족되면 B를 건너뛰고 A->C로 전환...!
}
