using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;
using SF = UnityEngine.SerializeField;


public class FloorShifter : MonoBehaviour
{
    /* Find Floors */
    [Header("[ Find Shiftable Floors ]")]
    [SF] private Transform floorParent;
    [SF] private string floorTagName = "Floor";
    [SF] private float detectDistance = 5f;
    [SF] private LayerMask tableLayer = 1 << 6;
    [SF] private LayerMask playerLayer = 1 << 8;
    /* Tween */
    [Header("[ Tween Values ]")]
    [SF] private float originY = 0f;
    [SF] private float targetY = -10f;
    [SF,Range(1,10)] private float tweenDuration = 8f;
    private int _instanceId;
    /* Shifting */
    [Header("[ Shifting Values ]")]
    [SF] private int maxShiftingCount = 12;
    [SF] private Vector3 playerDetectBoxSize = new Vector3(0.25f, 2f, 0.25f);
    private List<Transform> _shiftableFloors;
    private Queue<Transform> _submergedFloors;
    /* Coroutine */
    private Coroutine _coShiftingCycle;
    private WaitForSeconds _shiftInterval;

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
        
        DOTween.Kill(_instanceId);
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

        _instanceId = GetInstanceID();
        _shiftInterval = new WaitForSeconds(tweenDuration * 1.2f);
        _shiftableFloors = new List<Transform>(floorParent.childCount);
        _submergedFloors = new Queue<Transform>(maxShiftingCount);
        
        foreach (Transform floor in floorParent)
        {
            if (!floor.CompareTag(floorTagName)) continue;
            if (Physics.Raycast(floor.position, Vector3.up, out var hit, detectDistance,tableLayer)) continue;
            
            _shiftableFloors.Add(floor);
        }
    }
    #endregion

    #region Shifting Methods
    private IEnumerator CycleShifting()
    {
        yield return _shiftInterval; // 게임 시작 전에 발동 막기 위한...!
        
        while (gameObject.activeSelf)
        {
            SubmergeRandomFloors();
            yield return _shiftInterval;
            EmergeFloors();
            yield return _shiftInterval;
        }
    }

    private void SubmergeRandomFloors()
    {
        Shuffle();
        
        int rnd = Random.Range(maxShiftingCount - 3, maxShiftingCount);

        for (int i = 0; i < rnd; i++)
        {
            int lastIdx = _shiftableFloors.Count - 1; // 뒤에서부터 추출하기 위함!
            Transform targetFloor = _shiftableFloors[lastIdx];
            
            Submerge(targetFloor, i * 0.02f);

            _shiftableFloors.RemoveAt(lastIdx);
            _submergedFloors.Enqueue(targetFloor);
        }       
    }

    private void EmergeFloors()
    {
        while (_submergedFloors.Count > 0)
        {
            Transform targetFloor = _submergedFloors.Dequeue();
            Emerge(targetFloor);
            _shiftableFloors.Add(targetFloor);
        }
    }

    private void Submerge(Transform floor, float delay)
    {        
        if (Physics.CheckBox(floor.position, playerDetectBoxSize, Quaternion.identity, playerLayer))
        {
            Debug.Log($"{floor.name}({floor.GetInstanceID()}) 위엔 플레이어가 서있슴다.");
            return;
        }

        DOTween.Sequence()
            .Append(floor.DOLocalMoveY(targetY, tweenDuration))
            .Join(floor.DOScale(0.2f, tweenDuration))
            .SetEase(Ease.InBack)
            .SetDelay(delay)
            .SetId(_instanceId)
            .OnComplete(()=>floor.gameObject.SetActive(false));
    }

    private void Emerge(Transform floor)
    {
        if (floor.gameObject.activeSelf) return;
        floor.gameObject.SetActive(true);

        DOTween.Sequence()
            .Append(floor.DOScale(1f, tweenDuration))
            .Join(floor.DOLocalMoveY(originY, tweenDuration))
            .SetEase(Ease.OutBack)
            .SetId(_instanceId);
    }
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
    #endregion
}
