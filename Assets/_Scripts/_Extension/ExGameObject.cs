using UnityEngine;

public static class ExGameObject
{
    /// <summary>
    /// 호출한 게임오브젝트의 자식 오브젝트들 대상으로, 특정 컴포넌트를 탐색 시도하는 확장 메서드 
    /// </summary>
    /// <param name="go">탐색의 기준이 될 게임오브젝트</param>
    /// <param name="targetComponent">찾고자 하는 대상 컴포넌트</param>
    /// <param name="includeInactive">비활성화 상태인 자식오브젝트도 탐색할 것인지?</param>
    /// <typeparam name="T">찾을 대상 컴포넌트의 타입</typeparam>
    /// <returns>탐색 성공 여부</returns>
    public static bool TryGetComponentInChildren<T>(this GameObject go, out T targetComponent, bool includeInactive = false) where T : Component
    {
        targetComponent = null;
        if (go == null)
        {
            Debug.LogError($"\"{go.name}\"은(는) null 이거나, 이미 파괴된 GameObject 임다. [TryGetComponentInChildren]");
            return false;
        }
        
        targetComponent = go.GetComponentInChildren<T>(includeInactive);
        return targetComponent != null;
    }
    
    /// <summary>
    /// 호출한 컴포넌트가 부착된, 게임오브젝트의 자식 오브젝트들 대상으로, 특정 컴포넌트를 탐색 시도하는 확장 메서드 
    /// </summary>
    /// <param name="comp">탐색의 기준이 될 컴포넌트</param>
    /// <param name="targetComponent">찾고자 하는 대상 컴포넌트</param>
    /// <param name="includeInactive">비활성화 상태인 자식오브젝트도 탐색할 것인지?</param>
    /// <typeparam name="T">찾을 대상 컴포넌트의 타입</typeparam>
    /// <returns>탐색 성공 여부</returns>
    public static bool TryGetComponentInChildren<T>(this Component comp, out T targetComponent, bool includeInactive = false) where T : Component
    {
        targetComponent = null;
        if (comp == null)
        {
            Debug.LogError($"\"{comp.name}\"은(는) null 이거나, 이미 파괴된 Component 임다. [TryGetComponentInChildren]");
            return false;
        }
        
        targetComponent = comp.GetComponentInChildren<T>(includeInactive);
        return targetComponent != null;
    }
    
    // 자식 중에 특정 오브젝트 찾는 것도 있음 좋겠다 그래서 짜보았으나
    /*
    public static bool TryGetChildInImmediate(this GameObject go, string childName, out GameObject child, bool includeInactive = false)
    {
        child = null;
        if (go == null)
        {
            Debug.LogError($"\"{go.name}\"은(는) null 이거나, 이미 파괴된 GameObject 임다. [TryGetChild]");
            return false;
        }

        foreach (Transform t in go.transform) // transform은 이미 Enumerable이라 순회 가능, 매번 GetChild보다 이게 낫지비
        {
            if (!includeInactive && !t.gameObject.activeSelf) continue;
            if (!t.name.Equals(childName)) continue;
            
            child = t.gameObject;
            return true;
        }

        // 이건 직계 자식밖에 못 돌아잉
        return false;
    }
    */
    
    // 아래는 제미나이가 보완했다는 버전
    /*
    private static List<Transform> _tempList = new List<Transform>(64);

    public static bool TryGetChildRecursive(this GameObject go, string childName, out GameObject child)
    {
        child = null;
        _tempList.Clear(); // 이전 데이터 비우기

        // 중요: 배열을 새로 만들지 않고 기존 리스트에 담습니다. (성능 최적화 핵심)
        go.GetComponentsInChildren<Transform>(true, _tempList);

        for (int i = 0; i < _tempList.Count; i++)
        {
            var t = _tempList[i];
            if (t.gameObject != go && t.name == childName)
            {
                child = t.gameObject;
                return true;
            }
        }
        return false;
    }
    */
}
