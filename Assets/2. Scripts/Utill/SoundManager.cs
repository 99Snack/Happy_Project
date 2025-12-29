using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ClipName
{
    Main_bgm,
    Ingame_bgm,
    Btn_sound,
    Success_sound,
    Fail_sound,
    Wave_sound,
    Win_sound,
    Lose_sound,
    Spawn_sound,
    Clear_sound,
    Cannon_sound,
    Magic_sound,
    Ice_sound,
    Spear_sound,
    Knight_sound,
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

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            instance = this;
        }
    }

    [SerializeField] private AudioSource bgmPlayer;

    [SerializeField] private int poolSizePerSound = 5;
    [SerializeField] private List<ClipInfo> list = new List<ClipInfo>();

    private Dictionary<ClipName, AudioClip> audioClipDic = new Dictionary<ClipName, AudioClip>();
    private Dictionary<ClipName, Queue<AudioSource>> soundPoolDict = new Dictionary<ClipName, Queue<AudioSource>>();

    private void Start()
    {
        foreach (var item in list)
        {
            if (!audioClipDic.ContainsKey(item.name))
            {
                audioClipDic.Add(item.name, item.source);
                CreatePool(item.name);
            }
        }

        //todo : 첫 타이틀에서 bgm 실행 (로비, 인게임도 동일) 다 돌아가도록
        //PlayBGM(ClipName.Main_bgm);
    }

    //효과음 재생 (2D/기본)
    public void PlaySFX(ClipName name)
    {
        if (!audioClipDic.ContainsKey(name)) return;

        AudioSource source = GetAudioSourceFromPool(name);

        source.clip = audioClipDic[name];
        source.gameObject.SetActive(true);
        source.Play();

        StartCoroutine(ReturnToPool(name, source, audioClipDic[name].length));
    }

    //배경음 재생 (ClipName 사용 버전)
    public void PlayBGM(ClipName name, bool loop = true)
    {
        if (!audioClipDic.ContainsKey(name)) return;

        bgmPlayer.clip = audioClipDic[name];
        bgmPlayer.loop = loop;
        bgmPlayer.Play();
    }


    private void CreatePool(ClipName name)
    {
        Queue<AudioSource> pool = new Queue<AudioSource>();
        GameObject poolRoot = new GameObject($"Pool_{name}");
        poolRoot.transform.parent = this.transform;

        for (int i = 0; i < poolSizePerSound; i++)
        {
            pool.Enqueue(CreateNewAudioSource(name, poolRoot.transform));
        }
        soundPoolDict.Add(name, pool);
    }

    private AudioSource CreateNewAudioSource(ClipName name, Transform parent)
    {
        GameObject go = new GameObject($"AudioSource_{name}");
        go.transform.parent = parent;
        AudioSource source = go.AddComponent<AudioSource>();
        source.playOnAwake = false;
        go.SetActive(false);
        return source;
    }

    private AudioSource GetAudioSourceFromPool(ClipName name)
    {
        if (soundPoolDict[name].Count > 0)
        {
            return soundPoolDict[name].Dequeue();
        }

        //풀이 비었을 경우 새로 생성하여 반환
        return CreateNewAudioSource(name, transform.Find($"Pool_{name}"));
    }

    private IEnumerator ReturnToPool(ClipName name, AudioSource source, float delay)
    {
        yield return new WaitForSeconds(delay);

        source.Stop();
        source.gameObject.SetActive(false);
        soundPoolDict[name].Enqueue(source);
    }
}