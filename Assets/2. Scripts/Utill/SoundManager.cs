using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ClipName
{
    //사운드 이름 추가
}

[System.Serializable]
public struct ClipInfo
{
    public ClipName name;
    public AudioClip source;
}

public class SoundManager : MonoBehaviour
{
    private static SoundManager instance;
    public static SoundManager Instance { get => instance; private set => instance = value; }

    [SerializeField] private AudioSource bgmPlayer;
    [SerializeField] private AudioSource effectPlayer;

    [SerializeField] private List<ClipInfo> list = new List<ClipInfo>();
    private Dictionary<ClipName, AudioClip> dic = new Dictionary<ClipName, AudioClip>();

    private Dictionary<ClipName, Queue<AudioSource>> soundPoolDict = new Dictionary<ClipName, Queue<AudioSource>>();
    [SerializeField] private int poolSizePerSound = 5;

    private void Awake()
    {
        // 싱글톤 초기화
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            instance = this;
        }

        foreach (var item in list)
        {
            if (!dic.ContainsKey(item.name))
            {
                dic.Add(item.name, item.source);
                CreatePool(item.name); 
            }
        }
    }

    public void Play(ClipName name)
    {
        if (!dic.ContainsKey(name)) return;

        AudioSource source = GetAudioSourceFromPool(name);

        source.transform.position = Camera.main.transform.position;

        source.clip = dic[name];
        source.gameObject.SetActive(true);
        source.Play();

        StartCoroutine(ReturnToPool(name, source, dic[name].length));
    }

    public void PlayBGM(AudioClip audioClip)
    {
        if (bgmPlayer == null || audioClip == null) return;
        bgmPlayer.clip = audioClip;
        bgmPlayer.Play();
    }

    private void CreatePool(ClipName name)
    {
        Queue<AudioSource> pool = new Queue<AudioSource>();

        for (int i = 0; i < poolSizePerSound; i++)
        {
            GameObject go = new GameObject($"AudioSource_{name}_{i}");
            go.transform.parent = transform;
            AudioSource source = go.AddComponent<AudioSource>();
            source.playOnAwake = false;
            go.SetActive(false);
            pool.Enqueue(source);
        }
        soundPoolDict.Add(name, pool);
    }

    private AudioSource GetAudioSourceFromPool(ClipName name)
    {
        if (soundPoolDict[name].Count > 0)
        {
            return soundPoolDict[name].Dequeue();
        }

        // 풀 부족 시 추가 생성
        GameObject go = new GameObject($"AudioSource_{name}_Extra");
        go.transform.parent = transform;
        AudioSource source = go.AddComponent<AudioSource>();
        source.playOnAwake = false;
        return source;
    }

    private IEnumerator ReturnToPool(ClipName name, AudioSource source, float delay)
    {
        yield return new WaitForSeconds(delay);
        source.gameObject.SetActive(false);
        soundPoolDict[name].Enqueue(source);
    }
}