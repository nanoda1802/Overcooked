using UnityEngine;
using SF = UnityEngine.SerializeField;

public enum ItemType { Bun, Cabbage, Cheese, Meat, Tomato }

public enum CookStatus { Raw, WellCooked, OverCooked }

public class Item : MonoBehaviour
{
    #region 필드와 프로퍼티
    /* 컴포넌트 */
    private Rigidbody _rb;
    private Collider _col;
    private TrailRenderer _trail;
    private MeshRenderer _mesh;
    /* 아이템 종류 */
    public ItemType type;
    /* 아이템 던지기 */
    [Header("[ Throw ]")] 
    [SF] private Vector3 throwOrigin;
    [SF] private Vector3 throwDir;
    [SF] private float throwForce;
    [SF, Range(0f, 1f)] private float throwDamp; // 0.4
    [SF, Range(5f, 50f)] private float maxThrowDist; // 17
    public bool IsThrown { get; private set; }
    public bool IsFalling { get; private set; }
    /* 아이템 요리조리 */
    [Header("[ Cook ]")] 
    public CookStatus status = CookStatus.Raw;
    [SF] private float curProgress; // 0f?
    [SF, Range(1f,5f)] private float maxProgress; // 1.5f?
    [SF] private Material[] mats;
    [SF] private int matIndex;
    #endregion

    #region 유니티 이벤트 메서드
    private void Awake()
    {
        if (!TryGetComponent(out _rb)) _rb = gameObject.AddComponent<Rigidbody>();
        _col = GetComponent<Collider>();
        _mesh = GetComponentInChildren<MeshRenderer>();
        _trail = GetComponent<TrailRenderer>();
    }

    private void Start()
    {
        _trail.enabled = false;
        _mesh.material = mats[matIndex];
    }

    private void FixedUpdate()
    {
        if (!IsThrown) return;
        Throwing();
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player")) return; // [임시]
        if (IsThrown) StopThrowing();
        if (IsFalling) IsFalling = false;
    }
    #endregion

    #region 요리조리 메서드
    public bool Cook()
    {
        if (status == CookStatus.WellCooked) return true;
        curProgress += Time.deltaTime;
        Debug.Log($"{name} 진행도 : {curProgress / maxProgress * 100}%");
        if (curProgress >= maxProgress)
        {
            Debug.Log($"{name} 조리 완료!");
            status = CookStatus.WellCooked;
            _mesh.material = mats[Mathf.Clamp(matIndex+1,0,mats.Length-1)];
        }
        return curProgress >= maxProgress;
    }
    #endregion

    #region 던지기 메서드
    public void SetThrowValues(Vector3 origin, Vector3 dir, float force)
    {
        _trail.enabled = IsThrown = true;
        throwOrigin = origin;
        throwDir = dir;
        throwForce = force;
    }

    private void Throwing()
    {
        _rb.velocity = throwForce * throwDir;
        float curDist = (throwOrigin - _rb.position).sqrMagnitude;
        if (curDist >= maxThrowDist) StopThrowing();
    }

    public void StopThrowing()
    {
        _rb.velocity *= throwDamp;
        _trail.enabled = IsThrown = false;
        IsFalling = true;
    }

    public void SetParent(Transform parent)
    {
        _trail.enabled = IsThrown = IsFalling = false;
        _rb.isKinematic = true;
        _col.enabled = false;
        transform.SetParent(parent);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }

    public void RemoveParent()
    {
        transform.SetParent(null);
        _rb.isKinematic = false;
        _col.enabled = true;
    }
    #endregion
}