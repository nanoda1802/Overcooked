using UnityEngine;
using SF = UnityEngine.SerializeField;

public enum ItemType { Bun, Cabbage, Cheese, Meat, Tomato, Plate }

public enum ItemStatus { Undone, WellDone, Overdone }

public class Item : MonoBehaviour, IPoolable
{
    #region 필드와 프로퍼티
    /* 컴포넌트 */
    private Rigidbody _rb;
    private Collider _col;
    private TrailRenderer _trail;
    private MeshRenderer _mesh;
    public MeshRenderer Mesh { get => _mesh; }
    private Pantry _pantry;
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
    [SF, Range(0.1f,5f)] private float maxProgress;
    [SF] private Material[] mats;
    #endregion

    #region 유니티 이벤트 메서드
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
        if (IsMaxDone()) return (float) maxDoneness;
        
        curProgress += Time.deltaTime;
        
        float ratio = curProgress / maxProgress;
        doneness = (ItemStatus) ratio;
        SetMaterial();
        
        return ratio;
    }

    public void InitProgress()
    {
        curProgress = 0;
        doneness = type is ItemType.Bun ? ItemStatus.WellDone : ItemStatus.Undone;
    }

    public bool IsMaxDone()
    {
        return doneness == maxDoneness;
    }

    public bool IsWellDone()
    {
        return doneness == ItemStatus.WellDone;
    }

    public void SetMaterial()
    {
        _mesh.material = mats[(int)doneness];
    }

    #endregion

    #region 던지기 메서드
    public void SetThrowValues(Vector3 origin, Vector3 dir, float force)
    {
        IsThrown = true;
        throwOrigin = origin;
        throwDir = dir;
        throwForce = force;
        ActivateTrail();
        ActivatePhysics();
    }

    private void Throwing()
    {
        _rb.velocity = throwForce * throwDir;
        
        float dist = (throwOrigin - _rb.position).sqrMagnitude;
        if (dist >= maxThrowDist) StopThrowing();
    }

    private void StopThrowing()
    {
        _rb.velocity *= throwDamp;
        
        IsThrown = false;
        IsFalling = true;
        DeactivateTrail();
    }
    
    public void SetParent(Transform parent)
    {
        IsThrown = IsFalling = false;
        
        DeactivateTrail();
        DeactivatePhysics();
        
        transform.SetParent(parent);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }

    public void RemoveParent()
    {
        transform.SetParent(null);
    }

    private void ActivateTrail()
    {
        _trail.enabled = true;
    }

    protected void DeactivateTrail()
    {
        if (!_trail.enabled)  return;
        _trail.enabled = false;
        _trail.Clear();
    }

    public void ActivatePhysics()
    {
        _rb.isKinematic = false;
        _col.enabled = true;
    }

    private void DeactivatePhysics()
    {
        if (_rb.isKinematic) return;
        _rb.velocity = Vector3.zero;
        _rb.isKinematic = true;
        _col.enabled = false;
    }

    #endregion

    public void InitComponents(Pantry pantry)
    {
        if (!TryGetComponent(out _rb))
        {
            _rb = gameObject.AddComponent<Rigidbody>();
            _rb.angularDrag = 5f;
        }
        _col = GetComponent<Collider>();
        _mesh = GetComponentInChildren<MeshRenderer>();
        _trail = GetComponent<TrailRenderer>();
        _pantry = pantry;
    }

    public void Activate()
    {
        InitProgress();
        SetMaterial();
        gameObject.SetActive(true);
    }

    public void Deactivate()
    {
        _pantry.ReturnToPool(this);
        gameObject.SetActive(false);
    }
}