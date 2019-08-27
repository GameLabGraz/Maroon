using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Maroon.Util
{
  public static class RemoteTexture
  {
    public static async Task<Texture2D> GetTexture(string url)
    {
      using (var www = UnityWebRequestTexture.GetTexture(url))
      {
        var asyncOp = www.SendWebRequest();
        while (asyncOp.isDone == false) { await Task.Delay(10); }

        if (!www.isNetworkError && !www.isHttpError)
          return DownloadHandlerTexture.GetContent(www);

        Debug.LogError($"{ www.error }, URL:{ www.url }");
        return null;
      }
    }

    public static async Task<Sprite> GetSprite(string url)
    {
      var texture = await GetTexture(url);
      return texture != null ? Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0, 0)) : null;
    }
  }
}
