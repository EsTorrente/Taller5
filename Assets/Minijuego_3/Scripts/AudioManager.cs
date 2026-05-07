using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;

    private void OnEnable()
    {
        PhotoManager.OnPhotoSolved += PlaySuccessSound;
    }

    private void OnDisable()
    {
        PhotoManager.OnPhotoSolved -= PlaySuccessSound;
    }

    private void PlaySuccessSound(string clue)
    {
        if (audioSource != null)
        {
            audioSource.Play();
            Debug.Log("Sonido de éxito reproducido");
        }
    }
}