using UnityEngine;
using UnityEngine.Video;
using TMPro; 

public class VideoUIController : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public TMP_Text botonTexto; 

    void Start()
    {
        ActualizarIcono();
    }

    public void TogglePlayPause()
    {
        if (videoPlayer.isPlaying)
        {
            videoPlayer.Pause();
        }
        else
        {
            videoPlayer.Play();
        }

        ActualizarIcono();
    }

    void ActualizarIcono()
    {
        if (videoPlayer.isPlaying)
        {
            botonTexto.text = "Pausar";
        }
        else
        {
            botonTexto.text = "Reproducir";
        }
    }
}