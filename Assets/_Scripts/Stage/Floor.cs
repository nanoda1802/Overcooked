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

    private Transform _tr;

    public FloorState CurState { get; private set; }
    
    public void Init(FloorData data)
    {
        _floorData = data;
        // _instanceId = GetInstanceID();
        _tr = this.transform;
        
        _originPos = _tr.localPosition;
        _targetPos = new Vector3(_tr.localPosition.x, _tr.localPosition.y + _floorData.TargetY, _tr.localPosition.z);
        
        CurState = FloorState.Idle;
    }

    public void Deactivate()
    {
        _shiftSeq?.Kill();
        _shiftSeq = null;
        
        _tr.localPosition = _originPos;
    }

    public void Submerge(float delay)
    {        
        if (_floorData.HasPlayer(_tr))
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
            .Append(_tr.DOShakePosition(2, 0.1f))
            .Append(_tr.DOLocalMoveY(_floorData.TargetY, _floorData.TweenDuration)
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
            .Append(_tr.DOLocalMoveY(_floorData.OriginY, _floorData.TweenDuration)
                .SetEase(Ease.OutBack, 0.7f))
            .OnComplete(() => CurState = FloorState.Idle)
            .OnKill(()=>OnKillShiftSequence(_originPos));
    }
    
    private void OnKillShiftSequence(Vector3 pos)
    {
        _tr.localPosition = pos;
        _shiftSeq = null;
    }

    public Vector3 GetRespawnPosition()
    {
        Vector3 offset = Vector3.up * _tr.lossyScale.y; // [메모] 월드 기준 최종 Scale은 lossyScale
        return _tr.position + offset;
    }

    public void ReserveRespawn()
    {
        CurState = FloorState.RespawnReserved;
    }

    public void OnRespawnDone()
    {
        CurState = FloorState.Idle;
    }
}
