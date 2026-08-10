using UnityEngine;
using System.IO;
using UnityEngine.Video;

// Application-streamingAssetsPath example.
//
// Play a video and let the user stop/start it.
// The video location is StreamingAssets. The video is
// played on the camera background.

public class VideoPlayer : MonoBehaviour
{
    private UnityEngine.Video.VideoPlayer videoPlayer;

    void Start()
    {
        videoPlayer = this.GetComponent<UnityEngine.Video.VideoPlayer>();

        // Obtain the location of the video clip.
        videoPlayer.url = Path.Combine(Application.streamingAssetsPath, "maroon-main-menu-background.webm");

        // Restart from beginning when done.
        videoPlayer.isLooping = true;
    }
}
