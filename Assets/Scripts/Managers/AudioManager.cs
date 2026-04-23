using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("玩家引用")]
    public Transform player;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        InitPool();              // 初始化音效池
        InitBGMSource();         // 初始化 bgm 播放器
    }

    private void Start()
    {
        PlayBGM(0);
    }

    private void Update()
    {
        // 每帧检查是否有音效播放完毕，回收 AudioSource
        for (int i = activeSources.Count - 1; i >= 0; i--)
        {
            if (!activeSources[i].isPlaying)
            {
                RecycleSource(activeSources[i]);
                activeSources.RemoveAt(i);
            }
        }
    }

    #region SFX模块
    [Header("音效设置")]
    public AudioClip[] sfxClips;           // 音效列表
    public int poolSize = 20;              // 对象池大小
    public float minDistance = 1f;         // 最小距离
    public float maxDistance = 15f;        // 最大距离
    public int maxSimultaneousSFX = 10;    // 限制同一帧最多播放的数量
    public float clipCooldown = 0.3f;      // 同一音效冷却时间

    [Header("音效音量")]
    [Range(0f, 1f)]
    public float sfxVolume = 0.5f;         // 默认音效音量（50%）

    private Queue<AudioSource> audioPool = new Queue<AudioSource>();
    private List<AudioSource> activeSources = new List<AudioSource>();
    private Dictionary<int, float> clipCooldownDict = new Dictionary<int, float>();

    private void InitPool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject go = new GameObject("PooledAudioSource");
            go.transform.parent = transform;

            AudioSource source = go.AddComponent<AudioSource>();
            source.spatialBlend = 1f;
            source.rolloffMode = AudioRolloffMode.Linear;
            source.minDistance = minDistance;
            source.maxDistance = maxDistance;
            go.SetActive(false);

            audioPool.Enqueue(source);
        }
    }

    public void PlaySFX(int index, Vector3? position = null)
    {
        if (index < 0 || index >= sfxClips.Length || sfxClips[index] == null || player == null)
            return;

        AudioClip clip = sfxClips[index];

        // 冷却检测
        if (clipCooldownDict.TryGetValue(index, out float lastTime))
        {
            if (Time.time - lastTime < clipCooldown)
                return;
        }
        clipCooldownDict[index] = Time.time;

        // 限制同帧播放数量
        if (activeSources.Count >= maxSimultaneousSFX)
            return;

        AudioSource source = GetPooledSource();
        if (source == null) return;

        bool isPlayerSound = !position.HasValue;

        if (isPlayerSound)
        {
            source.spatialBlend = 0f; // 2D播放
            source.transform.position = player.position;
        }
        else
        {
            Vector3 pos = position.Value;
            float dist = Vector3.Distance(pos, player.position);
            if (dist > maxDistance)
            {
                RecycleSource(source);
                return;
            }

            source.spatialBlend = 1f; // 3D播放
            source.transform.position = pos;
        }

        source.clip = clip;
        source.volume = sfxVolume; // 改为可调节音量
        source.gameObject.SetActive(true);
        source.Play();

        activeSources.Add(source);
    }

    private Dictionary<int, AudioSource> loopSFXDict = new Dictionary<int, AudioSource>();

    public void PlayLoopSFX(int index, Vector3? position = null)
    {
        if (loopSFXDict.ContainsKey(index))
            return;

        if (index < 0 || index >= sfxClips.Length || sfxClips[index] == null)
            return;

        AudioSource source = GetPooledSource();
        if (source == null) return;

        bool isPlayerSound = !position.HasValue;

        if (isPlayerSound)
        {
            source.spatialBlend = 0f;
            source.transform.position = player.position;
        }
        else
        {
            Vector3 pos = position.Value;
            float dist = Vector3.Distance(pos, player.position);
            if (dist > maxDistance)
            {
                RecycleSource(source);
                return;
            }
            source.spatialBlend = 1f;
            source.transform.position = pos;
        }

        source.clip = sfxClips[index];
        source.loop = true;
        source.volume = sfxVolume; // 改为可调节音量
        source.gameObject.SetActive(true);
        source.Play();

        loopSFXDict[index] = source;
    }

    public void StopLoopSFX(int index)
    {
        if (loopSFXDict.TryGetValue(index, out AudioSource source))
        {
            source.loop = false;
            source.Stop();
            RecycleSource(source);
            loopSFXDict.Remove(index);
        }
    }

    private AudioSource GetPooledSource()
    {
        if (audioPool.Count > 0)
            return audioPool.Dequeue();
        else
        {
            Debug.LogWarning("音效池已满，忽略播放");
            return null;
        }
    }

    private void RecycleSource(AudioSource source)
    {
        source.Stop();
        source.clip = null;
        source.gameObject.SetActive(false);
        audioPool.Enqueue(source);
    }
    #endregion


    #region BGM模块
    [Header("背景音乐设置")]
    public AudioClip[] bgmClips;
    [Range(0f, 1f)]
    public float bgmVolume = 0.5f;      // 默认音量 50%
    public float fadeDuration = 1.5f;

    private AudioSource bgmSource;
    private Coroutine bgmFadeCoroutine;
    private int currentBGMIndex = -1;

    private void InitBGMSource()
    {
        bgmSource = gameObject.AddComponent<AudioSource>();
        bgmSource.loop = true;
        bgmSource.playOnAwake = false;
        bgmSource.volume = 0f;
        bgmSource.spatialBlend = 0f;
    }

    public void ChangeBGM(int index)
    {
        StopBGM();
        PlayBGM(index);
    }

    private void PlayBGM(int index)
    {
        if (index < 0 || index >= bgmClips.Length || bgmClips[index] == null)
        {
            Debug.LogWarning("BGM 索引无效或音频为空");
            return;
        }

        if (index == currentBGMIndex && bgmSource.isPlaying)
            return;

        currentBGMIndex = index;
        bgmSource.clip = bgmClips[index];
        bgmSource.volume = 0f;
        bgmSource.Play();

        if (bgmFadeCoroutine != null)
            StopCoroutine(bgmFadeCoroutine);

        bgmFadeCoroutine = StartCoroutine(FadeInBGM());
    }

    private void StopBGM()
    {
        if (bgmFadeCoroutine != null)
            StopCoroutine(bgmFadeCoroutine);

        bgmFadeCoroutine = StartCoroutine(FadeOutBGM());
    }

    private IEnumerator FadeInBGM()
    {
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            bgmSource.volume = Mathf.Lerp(0f, bgmVolume, t / fadeDuration);
            yield return null;
        }
        bgmSource.volume = bgmVolume;
    }

    private IEnumerator FadeOutBGM()
    {
        float startVolume = bgmSource.volume;
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            bgmSource.volume = Mathf.Lerp(startVolume, 0f, t / fadeDuration);
            yield return null;
        }
        bgmSource.Stop();
        bgmSource.volume = bgmVolume;
    }
    #endregion


    #region 动态音量控制接口
    public void SetBGMVolume(float value)
    {
        bgmVolume = Mathf.Clamp01(value);
        bgmSource.volume = bgmVolume;
    }

    public void SetSFXVolume(float value)
    {
        sfxVolume = Mathf.Clamp01(value);
        foreach (var src in activeSources)
        {
            if (src != null)
                src.volume = sfxVolume;
        }
    }
    #endregion
}
