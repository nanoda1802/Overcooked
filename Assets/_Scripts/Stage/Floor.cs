using System;
using DG.Tweening;
using UnityEngine;
using SF = UnityEngine.SerializeField;

public class Floor : MonoBehaviour
{
    public FloorState CurState { get; private set; }
    private FloorData _floorData;
    private Transform _tr;
    
    private Vector3 _originPos;
    private Vector3 _targetPos;
    
    private Sequence _shiftSeq;
    
    public bool CanShift => (CurState == FloorState.Idle) && (!_floorData.HasPlayer(_tr));
    
    public void Init(FloorData data)
    {
        CurState = FloorState.Idle;
    
        _floorData = data;
        _tr = this.transform;
        
        _originPos = _tr.localPosition;
        _targetPos = new Vector3(_tr.localPosition.x, _tr.localPosition.y + _floorData.TargetY, _tr.localPosition.z);
    }

    public void Deactivate()
    {
        _shiftSeq?.Kill();
        _shiftSeq = null;
    }

    public void Submerge(float delay)
    {        
        _shiftSeq?.Kill();
        
        _shiftSeq = DOTween.Sequence()
            .AppendCallback(SwitchToShiftingState)
            .Append(_tr.DOShakePosition(_floorData.TweenDuration, 0.1f))
            .Append(_tr.DOLocalMoveY(_floorData.TargetY, _floorData.TweenDuration)
                .SetEase(Ease.InBack, 0.7f)
                .SetDelay(delay))
            .OnKill(OnKillSubmergeSequence);
    }
    
    public void Emerge()
    {
        _shiftSeq?.Kill();
        
        _shiftSeq = DOTween.Sequence()
            .Append(_tr.DOLocalMoveY(_floorData.OriginY, _floorData.TweenDuration)
                .SetEase(Ease.OutBack, 0.7f))
            .OnComplete(SwitchToIdleState)
            .OnKill(OnKillEmergeSequence);
    }

    private void OnKillSubmergeSequence()
    {
        _tr.localPosition = _targetPos;
        _shiftSeq = null;
    }

    private void OnKillEmergeSequence()
    {
        _tr.localPosition = _originPos;
        _shiftSeq = null;
    }

    public Vector3 GetRespawnPosition()
    {
        Vector3 offset = Vector3.up * _tr.lossyScale.y; // [메모] 월드 기준 최종 Scale은 lossyScale
        return _tr.position + offset;
    }

    private void SwitchToShiftingState()
    {
        CurState = FloorState.Shifting;
        gameObject.layer = _floorData.IgnoreRaycastLayerIdx;
    }

    public void SwitchToRespawnReservedState()
    {
        CurState = FloorState.RespawnReserved;
        gameObject.layer = _floorData.IgnoreRaycastLayerIdx;
    }

    public void SwitchToIdleState()
    {
        CurState = FloorState.Idle;
        gameObject.layer = _floorData.FloorLayerIdx;
    }
}
