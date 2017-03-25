using UnityEngine;
using System.Collections;

public class CustomTeleporter : MonoBehaviour
{
	//teleport instantly upon entering trigger?
	public bool instantTeleport;
	//teleport to a random pad?
	public bool randomTeleport;
	//teleport when a button is pressed?
	public bool buttonTeleport;
	//the name of the button
	public string buttonName;
	//teleport delayed?
	public bool delayedTeleport;
	//time it takes to teleport, if not instant
	public float teleportTime = 3;
	//only allow specific tag object? if left empty, any object can teleport
	public string objectTag = "if empty, any object will tp";
	//one or more destination pads
	public Transform[] destinationPad;
	//height offset
	public float teleportationHeightOffset = 1;
	//a private float counting down the time
	private float curTeleportTime;
	//private bool checking if you entered the trigger
	private bool inside;
	//check to wait for arrived object to leave before enabling teleporation again
	[HideInInspector]
	public bool arrived;
	//gameobjects inside the pad
	private Transform subject;
	//add a sound component if you want the teleport playing a sound when teleporting
	public AudioSource teleportSound;
	//add a sound component for the teleport pad, vibrating for example, or music if you want :D
	//also to make it more apparent when the pad is off, stop this component playing with "teleportPadSound.Stop();"
	//PS the distance is set to 10 units, so you only hear it standing close, not from the other side of the map
	public AudioSource teleportPadSound;
	//simple enable/disable function in case you want the teleport not working at some point
	//without disabling the entire script, so receiving objects still works
	public bool teleportPadOn = true;

	void Start ()
	{
		//Set the countdown ready to the time you chose
		curTeleportTime = teleportTime;
	}


	void Update ()
	{
		//check if theres something/someone inside
		if(inside)
		{
			//if that object hasnt just arrived from another pad, teleport it
			if(!arrived && teleportPadOn)
			Teleport();
		}
	}

	void Teleport()
	{
		//if you chose to teleport instantly
		if(instantTeleport)
		{
			//if you chose instant + random teleport, teleport to a random pad from the array
			if(randomTeleport)
			{
				//choose a random pad from the array
				int chosenPad = Random.Range(0,destinationPad.Length);
				//set arrived to true in that array, so it doesnt teleport the subject back
				destinationPad[chosenPad].GetComponent<CustomTeleporter>().arrived = true;
				//and teleport the subject
				subject.transform.position = destinationPad[chosenPad].transform.position + new Vector3(0,teleportationHeightOffset,0);
				//play teleport sound
				teleportSound.Play();
			}
			else //if not random, teleport to the first one in the array list
			{
				//set arrived to true in that array, so it doesnt teleport the subject back
				destinationPad[0].GetComponent<CustomTeleporter>().arrived = true;
				//and teleport the subject
				subject.transform.position = destinationPad[0].transform.position + new Vector3(0,teleportationHeightOffset,0);
				//play teleport sound
				teleportSound.Play();
			}
		}
		else if(delayedTeleport) //if its a delayed teleport
		{
			//start the countdown
			curTeleportTime-=1 * Time.deltaTime;
			//if the countdown reaches zero
			if(curTeleportTime <= 0)
			{
				//reset the countdown
				curTeleportTime = teleportTime;
				//if you chose delayed + random teleport, teleport to random pad from array, after the delay
				if(randomTeleport)
				{
					//choose a random pad from the array
					int chosenPad = Random.Range(0,destinationPad.Length);
					//set arrived to true in that array, so it doesnt teleport the subject back
					destinationPad[chosenPad].GetComponent<CustomTeleporter>().arrived = true;
					//teleport
					subject.transform.position = destinationPad[chosenPad].transform.position + new Vector3(0,teleportationHeightOffset,0);
					//play teleport sound
					teleportSound.Play();
				}
				else //if not random, teleport to the first one in the array list
				{
					//set arrived to true in that array, so it doesnt teleport the subject back
					destinationPad[0].GetComponent<CustomTeleporter>().arrived = true;
					//teleport
					subject.transform.position = destinationPad[0].transform.position + new Vector3(0,teleportationHeightOffset,0);
					//play teleport sound
					teleportSound.Play();
				}
			}
		}
		else if(buttonTeleport) //if you selected a teleport activated by a button
		{
			//checks if the button you set in the inspector is being pressed
			if(Input.GetButtonDown(buttonName))
			{
				if(delayedTeleport) //if you set it to button + delayed
				{
					//start the countdown
					curTeleportTime-=1 * Time.deltaTime;
					//if the countdown reaches zero
					if(curTeleportTime <= 0)
					{
						//reset the countdown
						curTeleportTime = teleportTime;
						//if you chose delayed + random teleport + button, teleport to random pad from array, after the delay
						// and button press
						if(randomTeleport)
						{
							//choose a random pad from the array
							int chosenPad = Random.Range(0,destinationPad.Length);
							//set arrived to true in that array, so it doesnt teleport the subject back
							destinationPad[chosenPad].GetComponent<CustomTeleporter>().arrived = true;
							//teleport
							subject.transform.position = destinationPad[chosenPad].transform.position + new Vector3(0,teleportationHeightOffset,0);
							//play teleport sound
							teleportSound.Play();
						}
						else //if not random, teleport to the first one in the array list
						{
							//set arrived to true in that array, so it doesnt teleport the subject back
							destinationPad[0].GetComponent<CustomTeleporter>().arrived = true;
							//teleport
							subject.transform.position = destinationPad[0].transform.position + new Vector3(0,teleportationHeightOffset,0);
							//play teleport sound
							teleportSound.Play();
						}
					}
				}
				//if you chose button + random teleport, teleport to random pad from array, on button down
				else if(randomTeleport)
				{
					//choose a random pad from the array
					int chosenPad = Random.Range(0,destinationPad.Length);
					//set arrived to true in that array, so it doesnt teleport the subject back
					destinationPad[chosenPad].GetComponent<CustomTeleporter>().arrived = true;
					//teleport
					subject.transform.position = destinationPad[chosenPad].transform.position + new Vector3(0,teleportationHeightOffset,0);
					//play teleport sound
					teleportSound.Play();
				}
				else
				{
					//set arrived to true in that array, so it doesnt teleport the subject back
					destinationPad[0].GetComponent<CustomTeleporter>().arrived = true;
					//teleport
					subject.transform.position = destinationPad[0].transform.position + new Vector3(0,teleportationHeightOffset,0);
					//play teleport sound
					teleportSound.Play();
				}
			}
		}
	}

	void OnTriggerEnter(Collider trig)
	{
		//when an object enters the trigger
		//if you set a tag in the inspector, check if an object has that tag
		//otherwise the pad will take in and teleport any object
		if(objectTag != "")
		{
			//if the objects tag is the same as the one allowed in the inspector
			if(trig.gameObject.tag == objectTag)
			{
				//set the subject to be the entered object
				subject = trig.transform;
				//and check inside, ready for teleport
				inside = true;
				//if its a button teleport, the pad should be set to be ready to teleport again
				//so the player/object doesn't have to leave the pad, and re enter again, we do that here
				if(buttonTeleport)
				{
					arrived = false;
				}
			}
		}
		else
		{
			//set the subject to be the entered object
			subject = trig.transform;
			//and check inside, ready for teleport
			inside = true;
			//if its a button teleport, the pad should be set to be ready to teleport again
			//so the player/object doesn't have to leave the pad, and re enter again, we do that here
			if(buttonTeleport)
			{
				arrived = false;
			}
		}
	}

	void OnTriggerExit(Collider trig)
	{
		//////////////if you set a tag for the entering pad, you should also set it for the exiting pad////////
		//when an object exists the trigger
		//if you set a tag in the inspector, check if an object has that tag
		//otherwise the pad will register any object as the one leaving 
		if(objectTag != "")
		{
			//if the objects tag is the same as the one allowed in the inspector
			if(trig.gameObject.tag == objectTag)
			{
				//set that the object left
				inside = false;
				//reset countdown time
				curTeleportTime = teleportTime;
				//if the object that left the trigger is the same object as the subject
				if(trig.transform == subject)
				{
					//set arrived to false, so that the pad can be used again
					arrived = false;
				}
				//remove the subject from the pads memory
				subject = null;
			}
		}
		else //if tags arent important
		{
			//set that the object left
			inside = false;
			//reset countdown time
			curTeleportTime = teleportTime;
			//if the object that left the trigger is the same object as the subject
			if(trig.transform == subject)
			{
				//set arrived to false, so that the pad can be used again
				arrived = false;
			}
			//remove the subject from the pads memory
			subject = null;
		}
	}
}
