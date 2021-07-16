using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

public class WhiteboardController : MonoBehaviour 
{
	private List<Lecture> lectures = new List<Lecture>();
	private Lecture selectedLecture = null;
	private int currentContentIndex = -1;
	private Renderer myRenderer;
	private Object lockObject = new Object();

	public List<Lecture> Lectures{ get{ return lectures; } }
	public Lecture SelectedLecture{ get{ return selectedLecture; } }
	public int CurrentContentIndex{ get{ return currentContentIndex; } }

    public UnityEvent OnLastLectureContent;

    public UnityEvent OnFirstLectureContent;

	public void Start()
    {
		myRenderer = GetComponent<Renderer> ();
        lectures.AddRange(GetComponents<Lecture>());

        if (lectures.Count > 0)
        {
            currentContentIndex = 0;
            selectedLecture = lectures[0];

            if(selectedLecture.Contents.Count > 0)
                myRenderer.material.mainTexture = selectedLecture.Contents[currentContentIndex];
        }
    }

	public void SelectLecture(int lectureIndex)
	{
		if (lectureIndex < 0 || lectureIndex >= lectures.Count)
			throw new  System.IndexOutOfRangeException(string.Format("Selected Lecture Index {0} out of range", lectureIndex));

		selectedLecture = lectures [lectureIndex];
		currentContentIndex = 0;
	}

    public void Refresh()
    {
        if (selectedLecture.Contents == null)
            throw new System.NullReferenceException("Contents list in selected lecture is null");
        lock (lockObject)
        {
            if (currentContentIndex >= 0 && selectedLecture.Contents.Count > currentContentIndex)
                myRenderer.material.mainTexture = selectedLecture.Contents[currentContentIndex];
        }
    }

	public void Next() 
	{
		if (selectedLecture.Contents == null)
			throw new System.NullReferenceException("Contents list in selected lecture is null");
		lock (lockObject)
        {
			if (selectedLecture.Contents.Count > currentContentIndex + 1)
				myRenderer.material.mainTexture = selectedLecture.Contents[++currentContentIndex];
            else
            {
				OnLastLectureContent?.Invoke();
            }
		}
	}

	public void Previous() 
	{
		if (selectedLecture.Contents == null)
			throw new System.NullReferenceException("Contents list in selected lecture is null");
		lock (lockObject)
        {
			if (currentContentIndex > 0)
                myRenderer.material.mainTexture = selectedLecture.Contents[--currentContentIndex];
            else
            {
                OnFirstLectureContent?.Invoke();
            }
		}
	}
}
