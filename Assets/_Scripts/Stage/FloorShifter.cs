using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using SF = UnityEngine.SerializeField;

[Serializable] // [임시 위치]
public struct MinMax<T> where T : IComparable<T>
{
    [SF] private T min;
    [SF] private T max;

    public T Min => min;
    public T Max => max;

    public MinMax(T min, T max)
    {
        if (min.CompareTo(max) > 0)
        {
            Debug.LogWarning($"입력하신 최소값 {min}가 최대값 {max}보다 큽니다. [MinMax<{nameof(T)}>]");
            min = max;
        }
        this.min = min;
        this.max = max;
    }
}

public enum FloorState
{
    Default,
    Idle,
    Shifting,
    RespawnReserved
}

public class FloorShifter : MonoBehaviour
{
    [SF] private FloorData floorData;
    /* Find Floors */
    [Header("[ Find Shiftable Floors ]")]
    [SF] private Transform floorParent;
    [SF] private string floorTagName = "Floor";
    // [SF] private LayerMask tableLayer = 1 << 6;
    // [SF] private float tableDetectDistance = 5f;
    // [SF] private LayerMask playerLayer = 1 << 8;
    // [SF] private Vector3 playerDetectBoxSize = new Vector3(0.25f, 2f, 0.25f);
    // /* Tween */
    // [Header("[ Tween Values ]")]
    // [SF] private float originY = 0f;
    // [SF] private float targetY = -10f;
    // [SF,Range(1,10)] private float tweenDuration = 8f;
    // private int _instanceId;
    /* Shifting */
    [Header("[ Shifting Values ]")] 
    [SF] private MinMax<int> shiftingCount = new MinMax<int>(6,10);
    [SF] private float shiftInterval = 12;
    private List<Floor> _shiftableFloors;
    private Queue<Floor> _submergedFloors;
    /* Coroutine */
    private Coroutine _coShiftingCycle;
    private WaitForSeconds _waitInterval;

    #region Unity Event Methods
    private void Awake() 
    {
        Init();
    }

    private void Start() 
    {
        _coShiftingCycle = StartCoroutine(CycleShifting());
    }

    private void OnDisable()
    {
        if (_coShiftingCycle is not null)
        {
            StopCoroutine(_coShiftingCycle);
            _coShiftingCycle = null;
        }

        foreach (Floor floor in _shiftableFloors)
        {
            floor?.Deactivate();
        }
        
        _shiftableFloors.Clear();
        _submergedFloors.Clear();
    }

    private void OnDrawGizmos()
    {
        // foreach (Transform floor in _shiftableFloors)
        // {
        //     Gizmos.color = Color.red;
        //     Gizmos.DrawCube(floor.position, playerDetectBoxSize*2);
        // }
    }

    #endregion

    #region Initialize Methods
    private void Init()
    {
        if (floorParent is null) 
        {
            Debug.LogError("floorParent가 설정되지 않았슴다. [FloorShifter.Init]");
            return;
        }

        // _instanceId = GetInstanceID();
        _waitInterval = new WaitForSeconds(shiftInterval);
        _shiftableFloors = new List<Floor>(floorParent.childCount);
        _submergedFloors = new Queue<Floor>(shiftingCount.Max);
        
        foreach (Transform floorTransform in floorParent)
        {
            if (!floorTransform.CompareTag(floorTagName)) continue;
            if (floorData.HasTable(floorTransform)) continue;
            if (!floorTransform.TryGetComponent(out Floor floor)) continue;
            // if (Physics.Raycast(floor.position, Vector3.up, tableDetectDistance,tableLayer)) continue;
            
            floor.Init(floorData);
            _shiftableFloors.Add(floor);
        }
    }
    #endregion

    #region Shifting Methods
    private IEnumerator CycleShifting()
    {
        yield return _waitInterval; // 게임 시작 전에 발동 막기 위한...!
        
        while (gameObject.activeSelf)
        {
            SubmergeRandomFloors();
            yield return _waitInterval;
            EmergeFloors();
            yield return _waitInterval;
        }
    }

    private void SubmergeRandomFloors()
    {
        Shuffle();
        
        int rnd = Random.Range(shiftingCount.Min, shiftingCount.Max);

        // for (int i = 0; i < rnd; i++)
        // {
        //     int lastIdx = _shiftableFloors.Count - 1; // 뒤에서부터 추출하기 위함!
        //     Floor targetFloor = _shiftableFloors[lastIdx];
        //
        //     targetFloor.Submerge(i * 0.02f);
        //     // Submerge(targetFloor, i * 0.02f);
        //
        //     _shiftableFloors.RemoveAt(lastIdx);
        //     _submergedFloors.Enqueue(targetFloor);
        // }
        //
        // foreach (Floor floor in _shiftableFloors)
        // {
        //     if (floor.CurState != FloorState.Idle) continue;
        //     floor.Submerge(rnd * 0.02f);
        //     _submergedFloors.Enqueue(floor);
        //     rnd--;
        // }

        while (rnd > 0)
        {
            if (rnd >= _shiftableFloors.Count)
            {
                Debug.LogWarning($"움직일 Floor의 개수 {rnd}가 전체 Floor의 개수 {_shiftableFloors.Count} 보다 큽니다. [FloorShifter.SubmergeRandomFloors]");
                break;
            }
            Floor targetFloor = _shiftableFloors[rnd];
            if (targetFloor.CurState != FloorState.Idle) continue;
            targetFloor.Submerge(rnd * 0.02f);
            _submergedFloors.Enqueue(targetFloor);
            rnd--;
        }
    }

    private void EmergeFloors()
    {
        while (_submergedFloors.Count > 0)
        {
            Floor targetFloor = _submergedFloors.Dequeue();
            targetFloor.Emerge();
            // Emerge(targetFloor);
            _shiftableFloors.Add(targetFloor);
        }
    }

    // private void Submerge(Transform floor, float delay)
    // {        
    //     if (Physics.CheckBox(floor.position, playerDetectBoxSize, Quaternion.identity, playerLayer))
    //     {
    //         Debug.Log($"{floor.name}({floor.GetInstanceID()}) 위엔 플레이어가 서있슴다.");
    //         return;
    //     }
    //     
    //     DOTween.Sequence()
    //         .Append(floor.DOShakePosition(2,0.1f))
    //         .Append(floor.DOLocalMoveY(targetY, tweenDuration)
    //             .SetEase(Ease.InBack,0.7f)
    //             .SetDelay(delay))
    //         .SetId(_instanceId)
    //         .OnComplete(()=>floor.gameObject.SetActive(false));
    //     
    //     // floor.DOLocalMoveY(targetY, tweenDuration)
    //     //     .SetEase(Ease.InBack,0.7f)
    //     //     .SetDelay(delay)
    //     //     .SetId(_instanceId)
    //     //     .OnComplete(()=>floor.gameObject.SetActive(false));
    //     
    //     // DOTween.Sequence()
    //     //     .Append(floor.DOLocalMoveY(targetY, tweenDuration).SetEase(Ease.InBack,0.5f))
    //     //     .SetDelay(delay)
    //     //     .SetId(_instanceId)
    //     //     .OnComplete(()=>floor.gameObject.SetActive(false));
    // }
    //
    // private void Emerge(Transform floor)
    // {
    //     if (floor.gameObject.activeSelf) return;
    //     floor.gameObject.SetActive(true);
    //
    //     floor.DOLocalMoveY(originY, tweenDuration)
    //         .SetEase(Ease.OutBack, 0.7f)
    //         .SetId(_instanceId);
    //     
    //     // DOTween.Sequence()
    //     //     .Append(floor.DOLocalMoveY(originY, tweenDuration).SetEase(Ease.OutBack,0.5f))
    //     //     .SetId(_instanceId);
    // }
    #endregion

    #region Helper Methods
    private void Shuffle()
    {
        int floorCount = _shiftableFloors.Count;
        if (floorCount <= 1) return;
        
        for (int i = floorCount - 1; i > 0; i--) // Fisher-Yates는 뒤에서부터 섞는게 정석이래유 (한번 결정된 자리를 다시 조작하는 경우를 최소화하기 위함!)
        {
            int rndIdx = Random.Range(0, i + 1); // 완전 무작위로 하려면 본인 포함 (i+1)이 맞고, 이전 경우를 완벽 제거하고 싶으면 본인 제외 (i)가 맞고...
            (_shiftableFloors[i], _shiftableFloors[rndIdx]) = (_shiftableFloors[rndIdx], _shiftableFloors[i]);
        }
    }

    public Floor GetSafeFloor()
    {
        foreach (Floor floor in _shiftableFloors)
        {
            if (floor.CurState == FloorState.Idle)
            {
                Debug.Log("FloorShifter에서 찾아줌!");
                return floor;
            }
        }   
        Debug.Log("FloorShifter에서도 못 찾음!");
        return null; // [임시] StageManger나 StageData에 default Respawn Point를 해두고, 그거 반환하자
    }

    #endregion
}
