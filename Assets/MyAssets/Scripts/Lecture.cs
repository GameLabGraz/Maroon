using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Lecture 
{
	/// <summary>
	/// The name of the lecture.
	/// </summary>
	private string name;
	/// <summary>
	/// The URLs of the web content.
	/// </summary>
	private List<string> urls;
	/// <summary>
	/// The list of web content objects.
	/// </summary>
	private List<WWW> webContents;

	public string Name { get{ return this.name; } }
	public List<string> ContentUrls { get{ return this.urls; } }
	public List<WWW> WebContents { get{ return this.webContents; } }

	public Lecture(string name, List<string> urls) {
		this.name = name;
		this.webContents = new List<WWW>();
		this.StartLoadingWebContents (urls);
	}

	public bool IsDone()
	{
		foreach (WWW www in this.webContents) {
			if(null != www) {
				if(!www.isDone)
					return false;
			}
		}
		return true;
	}

	/// <summary>
	/// Asynchronous method that starts loading the web contents.
	/// </summary>
	/// <param name="urls">Urls.</param>
	public void StartLoadingWebContents(List<string> urls)
	{
		this.urls = urls;
		this.webContents.Clear ();
		foreach (string url in urls) {
			this.webContents.Add(new WWW(url));
		}
	}

	/// <summary>
	/// Asynchronous method that starts reloading web contents of
	/// which downloads finished but contained an error.
	/// </summary>
	public void StartReloadingFailedWebContents()
	{
		foreach (WWW www in this.webContents) 
		{
			if(www.isDone && !www.error.Equals(string.Empty))
			{
				//WWW newWww = new WWW(www.url);
				// TODO: replace WWW object in the webContent list with new one
			}
		}
	}
}
