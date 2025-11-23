using UnityEngine;
using SF = UnityEngine.SerializeField;

public enum ItemType { Bun, Cabbage, Cheese, Meat, Tomato, Plate }

public enum ItemStatus { Undone, WellDone, Overdone }

public class Item : MonoBehaviour
{
    #region 필드와 프로퍼티
    /* 컴포넌트 */
    private Rigidbody _rb;
    private Collider _col;
    private TrailRenderer _trail;
    private MeshRenderer _mesh;
    public MeshRenderer Mesh { get => _mesh; }
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
    public bool IsPlaced { get; set; }
    /* 아이템 요리조리 */
    [Header("[ Doneness ]")] 
    public ItemStatus doneness = ItemStatus.Undone;
    [SF] private ItemStatus maxDoneness;
    [SF] private float curProgress;
    [SF, Range(1f,5f)] private float maxProgress;
    [SF] private Material[] mats;
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
        InitProgress();
        SetMaterial();
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
        else if (IsFalling) IsFalling = false;
    }
    #endregion

    #region 요리조리 메서드
    public float Handle()
    {
        if (IsDone()) return (float) maxDoneness;
        
        curProgress += Time.deltaTime;
        
        float ratio = curProgress / maxProgress;
        doneness = (ItemStatus) ratio;
        SetMaterial(); // 부하 심하려나 이거?
        
        return ratio;
    }

    public void InitProgress()
    {
        curProgress = 0;
        doneness = ItemStatus.Undone;
    }

    public bool IsDone()
    {
        return doneness == maxDoneness;
    }

    public void SetMaterial()
    {
        _mesh.material = mats[(int)doneness];
    }

    #endregion

    #region 던지기 메서드
    public void SetThrowValues(Vector3 origin, Vector3 dir, float force)
    {
        _trail.enabled = IsThrown = true;
        throwOrigin = origin;
        throwDir = dir;
        throwForce = force;
        ActivatePhysics();
    }

    private void Throwing()
    {
        _rb.velocity = throwForce * throwDir;
        float curDist = (throwOrigin - _rb.position).sqrMagnitude;
        if (curDist >= maxThrowDist) StopThrowing();
    }

    private void StopThrowing()
    {
        _rb.velocity *= throwDamp;
        
        _trail.enabled = IsThrown = false;
        _trail.Clear();
        IsFalling = true;
    }

    public void DisposeItem()
    {
        _trail.enabled = false;
        _trail.Clear();
        Destroy(_trail);
        _trail = null;
        Destroy(gameObject); // 임시... 추후 Pool로
    }
    
    public void SetParent(Transform parent)
    {
        _trail.enabled = IsThrown = IsFalling = false;
        _trail.Clear();
        
        _rb.velocity = Vector3.zero;
        _rb.isKinematic = true;
        _col.enabled = false;
        
        transform.SetParent(parent);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }

    public void RemoveParent()
    {
        transform.SetParent(null);
    }

    public void ActivatePhysics()
    {
        _rb.isKinematic = false;
        _col.enabled = true;
    }

    #endregion
}