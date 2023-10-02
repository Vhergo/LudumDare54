using UnityEngine;

public class PlaySoundOnUIInteraction : MonoBehaviour
{
    [SerializeField] private AudioClip selectSound;
    [SerializeField] private AudioClip hoverSound;
    [SerializeField] private AudioClip releaseSound;
    [SerializeField] private AudioClip exitSound;

    
    public void PlayOnSelect() {
        if (SoundManager.Instance == null) return;
        SoundManager.Instance.PlaySound(selectSound);
    }

    public void PlayOnHover() {
        if (SoundManager.Instance == null) return;
        SoundManager.Instance.PlaySound(hoverSound);
    }

    public void PlayOnRelease() {
        if (SoundManager.Instance == null) return;
        SoundManager.Instance.PlaySound(releaseSound);
    }

    public void PlayOnExit() {
        if (SoundManager.Instance == null) return;
        SoundManager.Instance.PlaySound(hoverSound);
    }
}
