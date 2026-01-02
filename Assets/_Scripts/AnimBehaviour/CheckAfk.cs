using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SF = UnityEngine.SerializeField;

public class CheckAfk : StateMachineBehaviour
{
    private bool _isInit;
    private int _targetParamHash;
    private float _transitionThreshold;
    private int _conditionParamHash;
    private float _timer;

    public void Init(int targetParamHash, float  transitionThreshold, int conditionParamHash = 0)
    {
        _targetParamHash = targetParamHash;
        _transitionThreshold = transitionThreshold;
        _conditionParamHash = conditionParamHash;
        _isInit = true;
    }

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _timer = 0;
        animator.SetBool(_targetParamHash, false);
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!_isInit) return;

        _timer += Time.deltaTime;
        
        if (_timer > _transitionThreshold)
        {
            _timer = 0;
            animator.SetBool(_targetParamHash, CanTransition(animator));
        }
    }

    private bool CanTransition(Animator anim)
    {
        if (_conditionParamHash <= 0) return true;
        return !anim.GetBool(_conditionParamHash);
    }

    // [메모] 상속받는 메서드 5종
    // OnStateEnter, OnStateUpdate, OnStateExit, OnStateMove, OnStateIK
    
    // [메모] 위 메서드들의 공통 매개변수 3종
    // Animator animator, AnimatorStateInfo stateInfo, int layerIndex
}
