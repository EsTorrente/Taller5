using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class VideoController : MonoBehaviour
{
    public VideoPlayer video;
    public SceneChanger sceneChanger;
    public int idScene;

    void Start()
    {
        video.loopPointReached += FinVideo;

        //para que funcione el skip de frames
        video.skipOnDrop = false;
    }

    void Update()
    {
        SkipCinematic();
    }

    void SkipCinematic()
    {

        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.S))
        {
            video.time = video.length;
        }
    }

    void FinVideo(VideoPlayer vp)
    {
        sceneChanger.ChangeScene(idScene);
    }
}