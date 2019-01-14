using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Lecture  : MonoBehaviour
{
    [SerializeField]
    private string _name;
    
    [SerializeField]
    private List<Texture> _contents;

    private bool isDone = true;

    public string Name { get { return this._name; } }

    public List<Texture> Contents { get { return _contents; } }

    public bool IsDone { get { return isDone; } }

    public void AddContentTexture(Texture content)
    {
        _contents.Add(content);
    }

    public void LoadWebContents(List<string> urls)
    {
        isDone = false;
        StartCoroutine(LoadWebContentTextures(urls));
    }

    private IEnumerator LoadWebContentTextures(IEnumerable<string> urls)
    {
        foreach (var url in urls)
        {
            var webContent = new WWW(url);
            yield return webContent;
            _contents.Add(webContent.texture);
        }
        isDone = true;
    }
}
