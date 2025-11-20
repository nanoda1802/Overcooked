using UnityEngine;
using SF = UnityEngine.SerializeField;

public enum ItemType { Bun, Cabbage, Cheese, Meat, Tomato }

public class Item : MonoBehaviour
{
    private Rigidbody _rb;
    private Collider _col;
    private TrailRenderer _trail;

    public ItemType type;
    
    [SF] private Vector3 throwOrigin;
    [SF] private Vector3 throwDir;
    [SF] private float throwForce;
    [SF, Range(0f, 1f)] private float throwDamp; // 0.4
    [SF, Range(5f, 50f)] private float maxThrowDist; // 17
    public bool IsThrown { get; private set; }
    public bool IsFalling { get; private set; }

    private void Awake()
    {
        if (!TryGetComponent(out _rb)) _rb = gameObject.AddComponent<Rigidbody>();
        TryGetComponent(out _col);
        _trail = GetComponent<TrailRenderer>();
    }

    private void Start()
    {
        _trail.enabled = false;
    }

    private void FixedUpdate()
    {
        if (!IsThrown) return;
        Throwing();
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player")) return; // 임시
        if (IsThrown) StopThrowing();
        if (IsFalling) IsFalling = false;
    }

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

    private void StopThrowing()
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
    
    // 플레이어와 충돌 -> 플레이어가 하던 동작 멈추고 아이템 들게 됨 (이미 들고 있는 아이템 있으면 떨어뜨림)
}