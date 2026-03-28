using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class VideoController : MonoBehaviour
{
    public VideoPlayer video;
    public SceneChanger sceneChanger;

    void Start()
    {
        video.loopPointReached += FinVideo;
    }

    void FinVideo(VideoPlayer vp)
    {
        sceneChanger.ChangeScene(1);
    }
}

