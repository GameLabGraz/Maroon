using System;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using VRStandardAssets.Utils;

public class VRMovementController : MonoBehaviour
{
    public float rayLength = 500f;
    public LayerMask floorLayer;
    public Transform cam, character;
    public GameObject dummyHuman;
    public CanvasGroup tutorialCanvas;
    public VRInput vrInput;
    public float doubleClickDelay = .3f;
    GameObject humanInstance;

    bool isLookingAtFloor = false;
    float lastClickTime = 0f;
    Vector3 hitPoint = new Vector3();

	// Use this for initialization
	void Start ()
	{
	    humanInstance = Instantiate(dummyHuman);
        humanInstance.SetActive(false);
	    initInputEvents();
	    tutorialCanvas.alpha = 0f;
	}

    void initInputEvents()
    {
        vrInput.OnSwipe += onSwipe;
    }

    void onSwipe(VRInput.SwipeDirection swipeDirection)
    {
        if (swipeDirection != VRInput.SwipeDirection.NONE)
        {
            Debug.Log(swipeDirection);
        }
    }

    bool checkFloorHit()
    {
        // Create a ray that points forwards from the camera.
        Ray ray = new Ray(cam.position, cam.forward);
        RaycastHit hit;

        // Do the raycast forwards to see if we hit the floor
        if (Physics.Raycast(ray, out hit, rayLength) && ((1 << hit.transform.gameObject.layer) & floorLayer) != 0)
        {
            hitPoint = hit.point;
            humanInstance.transform.position = hit.point;
            humanInstance.SetActive(true);
            return true;
        }
        else
        {
            humanInstance.SetActive(false);
            return false;
        }
    }

    void rotateHuman(float degrees)
    {
        Vector3 rotation = humanInstance.transform.localEulerAngles;
        rotation.y += degrees;
        humanInstance.transform.localEulerAngles = rotation;
    }

    void teleport()
    {
        Vector3 pos = character.position;
        pos.x = hitPoint.x;
        pos.z = hitPoint.z;
        character.position = pos;

        Vector3 rot = humanInstance.transform.localEulerAngles;
        rot.x = 0f;
        rot.z = 0f;
        character.localEulerAngles = rot;
    }

    IEnumerator fadeInTutorial()
    {
        yield return new WaitForSeconds(1f);
        yield return fade(.5f, 1f);
    }

    IEnumerator fadeOutTutorial()
    {
        yield return fade(.5f, 0f);
    }

    IEnumerator fade(float maxDuration, float targetAlpha)
    {
        float duration = 0f;
        float startAlpha = tutorialCanvas.alpha;

        while (duration < maxDuration)
        {
            tutorialCanvas.alpha = (targetAlpha - startAlpha) * duration / maxDuration + startAlpha;
            yield return null;
            duration += Time.deltaTime;
        }

        tutorialCanvas.alpha = targetAlpha;
    }

    // Update is called once per frame
    void Update()
    {
        if (checkFloorHit())
        {
            if (!isLookingAtFloor)
            {
                humanInstance.transform.LookAt(cam);
                Vector3 rotation = humanInstance.transform.localEulerAngles;
                rotation.x = 0f;
                rotation.z = 0f;
                rotation.y += 180f;
                isLookingAtFloor = true;
                humanInstance.transform.localEulerAngles = rotation;
                StopAllCoroutines();
                StartCoroutine(fadeInTutorial());
            }
            else
            {

                if (Input.GetMouseButtonDown(0))
                {
                    if (Time.time - lastClickTime < doubleClickDelay)
                    {
                        teleport();
                        lastClickTime = 0f;
                    }
                    else
                    {
                        lastClickTime = Time.time;
                    }
                }
                else
                {

                    float rotationAngle;
#if !UNITY_EDITOR
                    rotationAngle = Input.GetAxis("Mouse X") * 5;
#else
                    rotationAngle = Input.GetAxis("Mouse ScrollWheel") * 50;
#endif
                    rotateHuman(rotationAngle);
                }
            }
        }
        else
        {
            if (isLookingAtFloor)
            {
                StopAllCoroutines();
                StartCoroutine(fadeOutTutorial());
            }
            isLookingAtFloor = false;
            lastClickTime = 0f;
        }
    }
}
