using UnityEngine;
using UnityEngine.UI;
using SF = UnityEngine.SerializeField;

public class RespawnTimer : MonoBehaviour, IPoolable
{
    private Camera _mainCam;
    private RectTransform _rect;
    private MovableUIPool _uiPool;
    
    [SF] private Text timerText;
    [SF] private float respawnTime;
    private float _timerCount;

    private PlayerController _player;
    private Vector3 _respawnPos;

    private void Update()
    {
        if (!gameObject.activeSelf) return;
        UpdateTimer();
    }

    private void UpdateTimer()
    {
        _timerCount -= Time.deltaTime;
        timerText.text = $"{_timerCount:F0}";

        if (_timerCount > 0) return;
        _player.Respawn(_respawnPos);
        Deactivate();
    }

    public void Init(MovableUIPool pool)
    {
        _mainCam = Camera.main;
        _rect = GetComponent<RectTransform>();
        _rect.position = Vector3.zero;
        _uiPool = pool;
    }

    public void SetRespawnValues(Vector3 respawnPos, PlayerController player)
    {
        _respawnPos = respawnPos;
        _rect.position = _mainCam.WorldToScreenPoint(_respawnPos);
        _player = player;
    }

    public void Activate()
    {
        gameObject.SetActive(true);
        _timerCount = respawnTime;
    }

    public void Deactivate()
    {
        gameObject.SetActive(false);
        _player = null;
        _uiPool.ReturnToPool(this);
    }
}
