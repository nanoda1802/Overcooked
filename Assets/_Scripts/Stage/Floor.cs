using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using SF = UnityEngine.SerializeField;

public class Floor : MonoBehaviour
{
    private FloorData _floorData;
    // private int _instanceId;
    private Vector3 _originPos;
    private Vector3 _targetPos;
    private Sequence _shiftSeq;
    
    public FloorState CurState { get; private set; }
    
    public void Init(FloorData data)
    {
        _floorData = data;
        // _instanceId = GetInstanceID();
        _originPos = transform.localPosition;
        _targetPos = new Vector3(transform.localPosition.x, transform.localPosition.y + _floorData.TargetY, transform.localPosition.z);
        CurState = _floorData.HasPlayer(transform) ? FloorState.Default : FloorState.Idle;
        
        if (CurState == FloorState.Default)
        {
            _floorData.DefaultFloor = this;
        }
    }

    public void Deactivate()
    {
        _shiftSeq?.Kill();
        _shiftSeq = null;
        transform.localPosition = _originPos;
    }

    public void Submerge(float delay)
    {        
        if (_floorData.HasPlayer(transform))
        {
            Debug.Log($"{this.name} 위엔 플레이어가 서있슴다.");
            return;
        }

        if (CurState == FloorState.RespawnReserved)
        {
            Debug.Log($"{this.name}은 플레이어 리스폰 대기중임다.");
            return;
        }

        _shiftSeq?.Kill(true);
        
        _shiftSeq = DOTween.Sequence()
            .AppendCallback(() => CurState = FloorState.Shifting)
            .Append(transform.DOShakePosition(2, 0.1f))
            .Append(transform.DOLocalMoveY(_floorData.TargetY, _floorData.TweenDuration)
                .SetEase(Ease.InBack, 0.7f)
                .SetDelay(delay))
            .OnComplete(() => gameObject.SetActive(false))
            .OnKill(() => OnKillShiftSequence(_targetPos));
    }
    
    public void Emerge()
    {
        if (gameObject.activeSelf) return;
        
        _shiftSeq?.Kill(true);
        
        _shiftSeq = DOTween.Sequence()
            .AppendCallback(()=>gameObject.SetActive(true))
            .Append(transform.DOLocalMoveY(_floorData.OriginY, _floorData.TweenDuration)
                .SetEase(Ease.OutBack, 0.7f))
            .OnComplete(() => CurState = FloorState.Idle)
            .OnKill(()=>OnKillShiftSequence(_originPos));
    }
    
    private void OnKillShiftSequence(Vector3 pos)
    {
        transform.localPosition = pos;
        _shiftSeq = null;
    }

    public Vector3 GetRespawnPosition()
    {
        Vector3 offset = Vector3.up * transform.localScale.y;
        return _originPos + offset;
    }

    public void ReserveRespawn()
    {
        if (CurState == FloorState.Default) return;
        CurState = FloorState.RespawnReserved;
    }

    public void OnRespawnDone()
    {
        if (CurState == FloorState.Default) return;
        CurState = FloorState.Idle;
    }
}
