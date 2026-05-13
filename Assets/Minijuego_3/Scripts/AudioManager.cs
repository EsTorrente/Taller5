using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;

    private void OnEnable()
    {
        PhotoManager.OnPhotoSolved += PlaySuccessSound; // se suscribe al aviso de foto resulta
    }

    private void OnDisable() // cuando el objeto se apaga o destruye
    {
        PhotoManager.OnPhotoSolved -= PlaySuccessSound; // se desuscribe para no tirar errores.          puntos extra por favor
    }

    private void PlaySuccessSound(string clue) // se activa cuando se resuelve la foto
    {
        if (audioSource != null)
        {
            audioSource.Play();
        }
    }
}