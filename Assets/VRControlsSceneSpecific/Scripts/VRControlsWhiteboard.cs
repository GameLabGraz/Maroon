using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class VRControlsWhiteboard : MonoBehaviour
{
    public WhiteboardController whiteboardController;
    public GameObject slidesControls, lectureList;
    public Button previousButton, nextButton;

    bool showSlidesControls = false;
    
	void Start () {
	    if (whiteboardController == null)
	    {
	        Debug.LogError("Whiteboard controller not set");
	        enabled = false;
	    }
        slidesControls.SetActive(false);
        lectureList.SetActive(true);
	}

    public void showLectures()
    {
        if (lectureList.activeSelf)
            return;

        lectureList.SetActive(true);
        slidesControls.SetActive(false);
        showSlidesControls = false;
    }

    public void selectLecture(int index)
    {
        whiteboardController.SelectLecture(index);
        whiteboardController.Refresh();
        showSlidesControls = true;
        lectureList.SetActive(false);
        slidesControls.SetActive(true);
    }

    public void updateSlideControls()
    {
        previousButton.interactable = whiteboardController.currentSlideIndex > 0;
        int slidesCount = whiteboardController.selectedLecture.usingLocalTextures
            ? whiteboardController.selectedLecture.textures.Count
            : whiteboardController.selectedLecture.WebContents.Count;
        nextButton.interactable = whiteboardController.currentSlideIndex < slidesCount - 1;
    }
	
	void Update () {
	    if (showSlidesControls)
	    {
            updateSlideControls();
	    }
	}
}
