using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;

public class VideoUIController : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public Image botonImagen;

    public Sprite iconoPlay;
    public Sprite iconoPause;

    void Start()
    {
        botonImagen.sprite = iconoPause;
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
            botonImagen.sprite = iconoPause;
        }
        else
        {
            botonImagen.sprite = iconoPlay;
        }
    }
     public void Adelantar()
    {
        videoPlayer.time += 10;

        if (videoPlayer.time > videoPlayer.length)
        {
            videoPlayer.time = videoPlayer.length;
        }
    }

    public void Retroceder()
    {
        videoPlayer.time -= 10;

        if (videoPlayer.time < 0)
        {
            videoPlayer.time = 0;
        }
    }
}