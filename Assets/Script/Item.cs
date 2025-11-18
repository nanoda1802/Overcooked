using UnityEngine;
using SF = UnityEngine.SerializeField;

public class Item : MonoBehaviour
{
    public Rigidbody rb;
    private TrailRenderer _trail;
    
    public bool isGrabbed = false;

    private bool _isThrown;
    [SF] private Vector3 throwOrigin;
    [SF] private Vector3 throwDir;
    [SF,Range(0f, 1f)] private float throwDamp; // 0.4
    [SF,Range(5f, 20f)] private float maxThrowDist; // 12
    private float _throwForce = 10f;
    
    private void Awake()
    {
        if (!TryGetComponent(out rb)) rb = gameObject.AddComponent<Rigidbody>();
        _trail = GetComponent<TrailRenderer>();
    }

    private void Start()
    {
        _trail.enabled = false;
    }

    private void FixedUpdate()
    {
        if (_isThrown) Throwing();
    }

    public void SetThrowValues(Vector3 origin, Vector3 dir, float force)
    {
        _trail.enabled = _isThrown = true;
        throwOrigin = origin;
        throwDir = dir;
        _throwForce =  force;
    }

    private void Throwing()
    {
        rb.velocity = _throwForce * throwDir;
        float curDist = (throwOrigin - rb.position).sqrMagnitude;
        if (curDist >= maxThrowDist)
        {
            rb.velocity *= throwDamp;
            _trail.enabled = _isThrown = false;
        }
    }
    // 그냥 던지기 누르면 바라보는 방향으로 던지고, 꾹 누르면 던질 방향 설정할 수 있네
    // 플레이어가 달리면서 던지면 좀 더 멀리 간대
    // 대략 5칸 정도 날아가고 떨어지는 듯? 
    
    // 날아가다가 박스 or 플레이어와 충돌 (이건 박스와 플레이어에서 해야겠다 isThrown인 Item과 충돌했을 시 ~~)
    // 박스와 충돌 -> 빈 박스면 위에 올려짐 아니면 평범하게 충돌 (트리거와 콜라이더 둘다 있어야함...)
    // 용도에 맞는 박스 -> 설치?됨
    // 플레이어와 충돌 -> 플레이어가 하던 동작 멈추고 아이템 들게 됨 (이미 들고 있는 아이템 있으면 떨어뜨림)
}
