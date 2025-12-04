using UnityEngine;
using UnityEngine.Animations;

public class LookAt : MonoBehaviour
{
    private AimConstraint lookAim;
    private ConstraintSource source;
    [SerializeField] private Transform target;
    
    private void Awake()
    {
        lookAim = GetComponent<AimConstraint>();
        target = transform.parent.GetChild(3).transform;
    }

    private void Start()
    {
        SetCameraToSource();
    }


    private void SetCameraToSource()
    {
        lookAim.constraintActive = true;
        lookAim.rotationAxis = Axis.X | Axis.Y; // 비트 마스크 형식, 여러 축 허용하려면 Axis.X | Axis.Y 같은 느낌
        // lookAim.worldUpType = AimConstraint.WorldUpType.ObjectUp;
        // lookAim.worldUpObject = source.sourceTransform = target;
        source.sourceTransform = Camera.main.transform;
        source.weight = 1;
        lookAim.AddSource(source);
    }
}
