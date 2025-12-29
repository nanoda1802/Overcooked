using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using SF = UnityEngine.SerializeField;

public class SfxManager : MonoBehaviour
{
    private static SfxManager _instance;
    public static SfxManager Instance;
    
    private IObjectPool<SoundEmitter> emitterPool;
    private readonly List<SoundEmitter> activeEmitters = new();
    public readonly Queue<SoundEmitter> FrequentSoundEmitters = new();
    
    [SF] private SoundEmitter emitterPrefab;
    [SF] private bool collectionCheck = true;
    [SF] private int defaultCapacity = 10;
    [SF] private int maxPoolSize = 100;
    [SF] private int maxSoundInstances = 30;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
            InitPool();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public SoundBuilder CreateSound()
    {
        return new(this);
    }

    public bool CanPlaySfx(SoundDataasdf dataasdf)
    {
        if (!dataasdf.isFrequent) return true;
        if (FrequentSoundEmitters.Count >= maxSoundInstances && FrequentSoundEmitters.TryDequeue(out var soundEmitter))
        {
            try
            {
                soundEmitter.Stop();
                return true;
            }
            catch
            {
                Debug.Log("SoundEmitter is already released");
            }
            return false;
        }
        
        return true;
    }

    public SoundEmitter GetSfx()
    {
        return emitterPool.Get();
    }

    public void ReleaseSfx(SoundEmitter emitter)
    {
        emitterPool.Release(emitter);
    }

    private void InitPool()
    {
        emitterPool = new ObjectPool<SoundEmitter>(CreateSfx, OnGot, OnReleased, OnDestroyed,
            collectionCheck, defaultCapacity, maxPoolSize);
    }

    private SoundEmitter CreateSfx()
    {
        var emitter = Instantiate(emitterPrefab,transform);
        emitter.gameObject.SetActive(false);
        return emitter;
    }

    private void OnGot(SoundEmitter emitter)
    {
        emitter.gameObject.SetActive(true);
        activeEmitters.Add(emitter);
    }

    private void OnReleased(SoundEmitter emitter)
    {
        emitter.gameObject.SetActive(false);
        activeEmitters.Remove(emitter);
    }

    private void OnDestroyed(SoundEmitter emitter)
    {
        Destroy(emitter.gameObject);
    }
}
