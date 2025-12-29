using UnityEngine;
using DG.Tweening;
using System.Collections;

public class CameraManager : MonoBehaviour
{
    private static CameraManager instance;
    public static CameraManager Instance {get => instance; private set => instance = value; }
    private Quaternion defaultRotation; 
    private Coroutine ShakeCoroutine;

    private const  float duration = 0.2f;
    private const float strength = 0.3f;
    private const int vibrato = 40; 
    private const float randomness = 90;
    private const bool fadeout = true;

    void Awake()
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

    void Start()
    {
        defaultRotation = transform.rotation;
    }

    public void ShakeCam()
    {
        if(ShakeCoroutine == null)
        {
            //todo : 실패 사운드 
            SoundManager.Instance.PlaySFX(ClipName.Fail_sound);
            ShakeCoroutine = StartCoroutine(StartShake());
        }
    }

    private IEnumerator StartShake()
    {
        WaitForSeconds wfs = new WaitForSeconds(duration); 
        
        transform.DOShakeRotation(duration, strength, vibrato, randomness, fadeout);
        yield return wfs;
        
        if(transform.rotation != defaultRotation)
        {
            transform.rotation = defaultRotation;
        }
        ShakeCoroutine = null;
    }
}
