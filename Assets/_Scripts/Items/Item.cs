using System.Collections;
using UnityEngine;
using SF = UnityEngine.SerializeField;

public class Item : MonoBehaviour, IPoolable
{
    #region 필드와 프로퍼티
    /* 컴포넌트 */
    private Rigidbody _rb;
    private Collider _col;
    private TrailRenderer _trail;
    private MeshRenderer _mesh;
    [field:SF] public Transform LeftHandPoint { get; private set; }
    [field:SF] public Transform RightHandPoint { get; private set; }
    /* 아이템 데이터 */
    [SF] protected ItemData data;
    public ItemData Data => data;
    /* 오브젝트 풀 */
    private IPool<Item> _pool;
    /* 아이템 던지기 */
    [Header("[ Throw ]")] 
    [SF] private Vector3 throwOrigin;
    [SF] private Vector3 throwDir;
    [SF] private float throwForceModifier;
    [field:SF] public bool IsThrown { get; private set; }
    [field:SF] public bool IsFalling { get; private set; }
    [field:SF] public bool IsPlaced { get; set; }
    /* 아이템 요리조리 */
    [Header("[ Doneness ]")] 
    [SF] private float curProgress;
    [SF] private ItemStatus curDoneness;
    public ItemStatus CurDoneness => curDoneness;
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
        else if (IsFalling)
        {
            IsFalling = false;
            DeactivateTrail();
        }
    }
    #endregion

    #region 요리조리 메서드
    public float Handle()
    {
        if (IsMaxDone()) return (float) data.MaxDoneness;
        
        curProgress += Time.deltaTime;
        
        float ratio = curProgress / data.MaxProgress;
        curDoneness = (ItemStatus) ratio;
        SetMaterial();
        
        return ratio;
    }

    protected void InitProgress()
    {
        curProgress = 0;
        curDoneness = data.InitialDoneness;
    }

    public bool IsMaxDone()
    {
        return curDoneness == data.MaxDoneness;
    }

    public bool IsWellDone()
    {
        return curDoneness == ItemStatus.WellDone;
    }

    protected void SetMaterial()
    {
        _mesh.material = data.Mats[(int)curDoneness];
    }

    #endregion

    #region 던지기 메서드
    public void SetThrowValues(Vector3 origin, Vector3 dir, float forceModifier)
    {
        IsThrown = true;
        throwOrigin = origin;
        throwDir = dir;
        throwForceModifier = forceModifier;
        ActivateTrail();
        ActivatePhysics();
    }

    private void Throwing()
    {
        _rb.velocity = data.ThrowForce * throwForceModifier * throwDir;
        
        float dist = (throwOrigin - _rb.position).sqrMagnitude;
        if (dist >= data.MaxThrowDistance) StopThrowing();
    }

    private void StopThrowing()
    {
        _rb.velocity *= data.ThrowDamp;
        
        IsThrown = false;
        IsFalling = true;
    }
    
    public void SetParent(Transform parent, Vector3 localPos = default, Vector3 localRot = default)
    {
        IsThrown = IsFalling = false;
        
        DeactivateTrail();
        DeactivatePhysics();
        
        transform.SetParent(parent);
        transform.SetLocalPositionAndRotation(localPos, Quaternion.Euler(localRot));
    }

    public void RemoveParent()
    {
        transform.SetParent(null);
    }

    private void ActivateTrail()
    {
        _trail.enabled = true;
    }

    private void DeactivateTrail()
    {
        if (!_trail.enabled) return;
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

    #region 초기화, 활성화, 비활성화
    public virtual void InitComponents(IPool<Item> pool)
    {
        if (!TryGetComponent(out _rb))
        {
            _rb = gameObject.AddComponent<Rigidbody>();
            _rb.angularDrag = 5f;
        }
        _col = GetComponent<Collider>();
        _mesh = GetComponentInChildren<MeshRenderer>();
        _trail = GetComponent<TrailRenderer>();
        _pool = pool;
    }

    public virtual void Activate()
    {
        InitProgress();
        SetMaterial();
        gameObject.SetActive(true);
    }

    public virtual void Deactivate()
    {
        IsThrown = IsFalling = false;
        DeactivateTrail();
        DeactivatePhysics();
        
        _pool.ReturnToPool(this);
        gameObject.SetActive(false);
    }
    #endregion
}