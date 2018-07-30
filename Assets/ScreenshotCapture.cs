using UnityEngine;
 
public class ScreenshotCapture : MonoBehaviour {
	public int resWidth = 1920; 
	public int resHeight = 1080;
 
	private bool takeHiResShot = false;
	private Camera _camera;
	
//	private void Awake()
//	{
//		_camera = Camera.current; // GetComponent<Camera>();
//	}

	public static string ScreenShotName(int width, int height) {
		return string.Format("{0}/screenshots/screen_{1}x{2}_{3}.png", 
			Application.dataPath, 
			width, height, 
			System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"));
	}
 
	public void TakeHiResShot() {
		takeHiResShot = true;
	}
 
	void LateUpdate()
	{
//		_camera = Camera.current;
		_camera = GameObject.FindGameObjectWithTag("ScreenshotCamera").GetComponent<Camera>();
		takeHiResShot |= Input.GetKeyDown("k");
		if (takeHiResShot) {
			RenderTexture rt = new RenderTexture(resWidth, resHeight, 24);
			_camera.targetTexture = rt;
			Texture2D screenShot = new Texture2D(resWidth, resHeight, TextureFormat.RGB24, false);
			_camera.Render();
			RenderTexture.active = rt;
			screenShot.ReadPixels(new Rect(0, 0, resWidth, resHeight), 0, 0);
			_camera.targetTexture = null;
			RenderTexture.active = null; // JC: added to avoid errors
			Destroy(rt);
			byte[] bytes = screenShot.EncodeToPNG();
			string filename = ScreenShotName(resWidth, resHeight);
			System.IO.File.WriteAllBytes(filename, bytes);
			Debug.Log(string.Format("Took screenshot to: {0}", filename));
			takeHiResShot = false;
		}
	}
}