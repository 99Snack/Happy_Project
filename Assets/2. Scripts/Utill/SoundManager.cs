using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private SoundManager instance;
    public SoundManager Instance {get => instance; private set => instance = value;}

    [SerializeField] private AudioSource bgmPlayer;
    [SerializeField] private AudioSource effectPlayer;

    private void Awake()
    {
        if(instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
            
        }
        else
        {
            instance = this;
        }
    }

    public void PlayBGM(AudioClip audioClip)
    {
        if(bgmPlayer == null || audioClip == null) return;
        
        bgmPlayer.clip = audioClip;
        bgmPlayer.Play();
    }

    public void PlayEffect(AudioClip audioClip)
    {
        if(effectPlayer == null || audioClip == null) return;

        effectPlayer.PlayOneShot(audioClip);
        transform.position = Camera.main.transform.position;
    }

}
