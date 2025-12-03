using UnityEngine;
using UnityEngine.Animations;

public class LookAtCam : MonoBehaviour
{
    private LookAtConstraint look;
    private ConstraintSource source = new ConstraintSource(); 

    private void Awake()
    {
        look = GetComponent<LookAtConstraint>();
    }

    private void Start()
    {
        SetCameraToSource();
    }

    private void SetCameraToSource()
    {
        source.sourceTransform = Camera.main.transform;
        source.weight = 1;
        look.AddSource(source);
        look.useUpObject = true;
        
        look.worldUpObject = GameObject.FindWithTag("Respawn").transform;
    }
}
