using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonSoundTrigger : MonoBehaviour
{
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(() => {
            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.PlaySFX(ClipName.Btn_sound);
            }
        });
    }
}
