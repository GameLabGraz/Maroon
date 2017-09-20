using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using AssemblyCSharp;

public class Appear : MonoBehaviour {

	private List<SpriteRenderer> vImages;
	public float valpha = 0f;
	public bool vchoice = true;
	public float vTimer = 5f;
	private float cTime = 0f;
	public bool needtoclick = false;

	// Use this for initialization
	void Start () {

		vImages = new List<SpriteRenderer> ();

		//get all image below the main Object
		foreach (Transform child in transform)
		{
			SpriteRenderer vRenderer = child.GetComponent<SpriteRenderer> ();
			if (vRenderer != null)
				vImages.Add (vRenderer);
		}
	}

	IEnumerator WaitInSeconds(float vseconds, string vChoice) {
		yield return new WaitForSeconds(vseconds);
		switch (vChoice) {
			case "False":
				vchoice = false;
			break;
		}
	}

	//make the alpha appear
	public void ImageAppear()
	{
		foreach (SpriteRenderer vRenderer in vImages)
			vRenderer.color = new Color (vRenderer.color.r, vRenderer.color.g, vRenderer.color.b, valpha);

		if (vchoice)
			valpha+=5f;
		else 
			valpha-=5f;
	}
	
	// Update is called once per frame
	void Update () {
		if ((vchoice && valpha < 255) || (!vchoice && valpha > 0))
			ImageAppear ();
		else if (!vchoice && valpha<= 0)
		{
			DialogBubble vCharacter = transform.parent.GetComponent<DialogBubble>();

			//before deleting himself, we tell the character this buble is no more
			foreach (PixelBubble vBubble in transform.parent.GetComponent<DialogBubble>().vBubble)
				if (vCharacter.vCurrentBubble == this.gameObject && !vBubble.vClickToCloseBubble) //remove current bubble ONLY if it must dissappear by itself
				{
					vCharacter.vCurrentBubble = null; //remove it
					vCharacter.IsTalking = false;
				}

			//destroy itself
			GameObject.Destroy (this.gameObject); 
		}
		else if ((valpha == 255f) &&(!needtoclick))
		{
			valpha = 254f;
			StartCoroutine(WaitInSeconds(3f, "False"));
		}
	}		
}
