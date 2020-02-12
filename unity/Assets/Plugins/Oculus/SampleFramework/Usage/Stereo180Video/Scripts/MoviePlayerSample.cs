/************************************************************************************

Copyright (c) Facebook Technologies, LLC and its affiliates. All rights reserved.  

See SampleFramework license.txt for license terms.  Unless required by applicable law 
or agreed to in writing, the sample code is provided “AS IS” WITHOUT WARRANTIES OR 
CONDITIONS OF ANY KIND, either express or implied.  See the license for specific 
language governing permissions and limitations under the license.

************************************************************************************/

using UnityEngine;
using System;
using System.IO;

public class MoviePlayerSample : MonoBehaviour
{
    private bool    videoPausedBeforeAppPause = false;

	private UnityEngine.Video.VideoPlayer videoPlayer = null;
	private OVROverlay          overlay = null;
	private Renderer 			mediaRenderer = null;

    public bool isPlaying { get; private set; }

    private RenderTexture copyTexture;
    private Material externalTex2DMaterial;

    public string MovieName;

    /// <summary>
    /// Initialization of the movie surface
    /// </summary>
    void Awake()
    {
        Debug.Log("MovieSample Awake");

        mediaRenderer = GetComponent<Renderer>();

        videoPlayer = GetComponent<UnityEngine.Video.VideoPlayer>();
        if (videoPlayer == null)
            videoPlayer = gameObject.AddComponent<UnityEngine.Video.VideoPlayer>();

        overlay = GetComponent<OVROverlay>();
        if (overlay == null)
            overlay = gameObject.AddComponent<OVROverlay>();

        // only mobile has Equirect shape
        overlay.enabled = Application.platform == RuntimePlatform.Android;
        // only can use external surface with native plugin
        overlay.isExternalSurface = NativeVideoPlayer.IsAvailable;
    }

    private void Start()
    {

        if (mediaRenderer.material == null)
		{
			Debug.LogError("No material for movie surface");
            return;
		}

        if (!string.IsNullOrEmpty(MovieName))
        {
#if UNITY_EDITOR
            var guids = UnityEditor.AssetDatabase.FindAssets(Path.GetFileNameWithoutExtension(MovieName));

            if (guids.Length > 0)
            {
                string video = UnityEditor.AssetDatabase.GUIDToAssetPath(guids[0]);
                Play(video);
            }
#else
            Play(Application.streamingAssetsPath +"/" + MovieName);
#endif
        }
    }

    public void Play(string moviePath)
    {
        if (moviePath != string.Empty)
        {
            Debug.Log("Playing Video: " + moviePath);
            if (overlay.isExternalSurface)
            {
                OVROverlay.ExternalSurfaceObjectCreated surfaceCreatedCallback = () =>
                {
                    Debug.Log("Playing ExoPlayer with SurfaceObject");
                    NativeVideoPlayer.PlayVideo(moviePath, overlay.externalSurfaceObject);
                };

                if (overlay.externalSurfaceObject == IntPtr.Zero)
                {
                    overlay.externalSurfaceObjectCreated = surfaceCreatedCallback;
                }
                else
                {
                    surfaceCreatedCallback.Invoke();
                }
            }
            else
            {
                Debug.Log("Playing Unity VideoPlayer");
                videoPlayer.url = moviePath;
                videoPlayer.Play();
            }

            Debug.Log("MovieSample Start");
            isPlaying = true;
        }
        else
        {
            Debug.LogError("No media file name provided");
        }
    }

    public void Play()
    {
        if (overlay.isExternalSurface)
        {
            NativeVideoPlayer.Play();
        }
        else
        {
            videoPlayer.Play();
        }
        isPlaying = true;
    }

    public void Pause()
    {
        if (overlay.isExternalSurface)
        {
            NativeVideoPlayer.Pause();
        }
        else
        {
            videoPlayer.Pause();
        }
        isPlaying = false;
    }

	void Update()
	{
        if (!isPlaying)
            return;

        if (!overlay.isExternalSurface)            
        {
            if (videoPlayer.texture != null)
            {
                if (overlay.enabled)
                {
                    overlay.textures[0] = videoPlayer.texture;
                    overlay.textures[1] = null;
                }
                else
                {
                    mediaRenderer.material.mainTexture = videoPlayer.texture;
                    mediaRenderer.material.SetVector("_SrcRectLeft", overlay.srcRectLeft.ToVector());
                    mediaRenderer.material.SetVector("_SrcRectRight", overlay.srcRectRight.ToVector());
                }
            }
            else
            {
                overlay.textures[0] = null;
                overlay.textures[1] = null;
                mediaRenderer.material.mainTexture = Texture2D.blackTexture;   
            }
        }
	}

    public void Rewind()
    {
        if (videoPlayer != null)
			videoPlayer.Stop();
    }
    
    public void Stop()
    {
        if (overlay.isExternalSurface)
        {
            NativeVideoPlayer.Stop();
        }
        else
        {
            videoPlayer.Stop();
        }

        isPlaying = false;
    }

    /// <summary>
    /// Pauses video playback when the app loses or gains focus
    /// </summary>
    void OnApplicationPause(bool appWasPaused)
    {
        Debug.Log("OnApplicationPause: " + appWasPaused);
        if (appWasPaused)
        {
            videoPausedBeforeAppPause = !isPlaying;
        }
        
        // Pause/unpause the video only if it had been playing prior to app pause
        if (!videoPausedBeforeAppPause)
        {
            if (appWasPaused)
            {
                Pause();
            }
            else
            {
                Play();
            }
        }
    }
}
