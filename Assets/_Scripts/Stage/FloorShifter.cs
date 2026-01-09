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
    Idle,
    Shifting,
    RespawnReserved
}

public class FloorShifter : MonoBehaviour
{
    [SF] private FloorData floorData;
    public Floor DefaultRespawnPoint { get; private set; }

    /* Find Floors */
    [Header("[ Find Shiftable Floors ]")]
    [SF] private Transform floorParent;
    [SF] private string floorTagName = "Floor";
    /* Shifting */
    [Header("[ Shifting Values ]")] 
    [SF] private MinMax<int> shiftableCount = new MinMax<int>(6,10);
    [SF] private float shiftInterval = 12;
    [SF,Range(0f,0.1f)] private float submergeDelay = 0.02f;
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

    #endregion

    #region Initialize Methods
    private void Init()
    {
        if (floorParent is null) 
        {
            Debug.LogError("floorParent가 설정되지 않았슴다. [FloorShifter.Init]");
            return;
        }

        _waitInterval = new WaitForSeconds(shiftInterval);
        _shiftableFloors = new List<Floor>(floorParent.childCount);
        _submergedFloors = new Queue<Floor>(shiftableCount.Max);
        
        foreach (Transform floorTransform in floorParent)
        {
            if (!floorTransform.CompareTag(floorTagName)) continue;
            if (!floorTransform.TryGetComponent(out Floor floor)) continue;
            if (floorData.HasTable(floorTransform))
            {
                floorTransform.gameObject.layer = floorData.IgnoreRaycastLayerIdx;
                floor.enabled = false;
                continue;
            }
            
            floor.Init(floorData);
            
            if (floorData.HasPlayer(floorTransform))
            {
                DefaultRespawnPoint = floor;
                Debug.Log($"Default Floor Selected {floor.name}({floor.transform.position})");
                continue;
            }
            
            _shiftableFloors.Add(floor);
        }
    }
    #endregion

    #region Shifting Methods
    private IEnumerator CycleShifting()
    {
        yield return _waitInterval; // TimeScale 정상 가동하기 전에는 시작 않도록...
        
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
        ShuffleFloors();
        
        int randomCount = Random.Range(shiftableCount.Min, shiftableCount.Max);
        int startIdx = _shiftableFloors.Count - 1;
        int endIdx = Mathf.Clamp(startIdx - randomCount, 0, startIdx);

        for (int i = startIdx; i >= endIdx; i--)
        {
            Floor targetFloor = _shiftableFloors[i];
            if (!targetFloor.CanShift) continue;
            
            targetFloor.Submerge(i * submergeDelay);
            _shiftableFloors.RemoveAt(i);
            _submergedFloors.Enqueue(targetFloor);
        }
    }

    private void EmergeFloors()
    {
        while (_submergedFloors.Count > 0)
        {
            Floor targetFloor = _submergedFloors.Dequeue();
            targetFloor.Emerge();
            _shiftableFloors.Add(targetFloor);
        }
    }
    #endregion

    #region Helper Methods
    private void ShuffleFloors()
    {
        int floorCount = _shiftableFloors.Count;
        if (floorCount <= 1) return;
        
        for (int i = floorCount - 1; i > 0; i--) // Fisher-Yates는 뒤에서부터 섞는게 정석이래유 (한번 결정된 자리를 다시 조작하는 경우를 최소화하기 위함!)
        {
            int rndIdx = Random.Range(0, i+1); // 완전 무작위로 하려면 본인 포함 (i+1)이 맞고, 이전 경우를 완벽 제거하고 싶으면 본인 제외 (i)가 맞고...
            (_shiftableFloors[i], _shiftableFloors[rndIdx]) = (_shiftableFloors[rndIdx], _shiftableFloors[i]);
        }
    }

    public Floor GetSafeFloor()
    {
        foreach (Floor floor in _shiftableFloors)
        {
            if (floor.CurState == FloorState.Idle) return floor;
        }   
        return DefaultRespawnPoint; // 그냥 바로 default 줄까...?
    }
    #endregion
}
