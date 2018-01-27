using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WhiteboardController : MonoBehaviour 
{
	private List<Lecture> lectures;
	private Lecture selectedLecture;
	private int currentWebContentIndex;
	private Renderer myRenderer;
	private Object lockObject = new Object();

	public List<Lecture> Lectures{ get{ return this.lectures; } }
	public Lecture SelectedLecture{ get{ return this.selectedLecture; } }
	public int CurrentWebContentIndex{ get{ return this.currentWebContentIndex; } }

	public void Start() {
		this.myRenderer = this.GetComponent<Renderer> ();
		this.currentWebContentIndex = -1;

		// ---- REMOVE WHEN DONE
		// Create some dummy lectures - TO BE REPLACED with dynamic loading from XML
		this.lectures = new List<Lecture> ();
//        if (lectures != null)
//        {
//            Debug.Log("WhiteboardController created a new list Lectures");
//        }
//        else
//        {
//            Debug.Log("WhiteboardController - list Lectures is NULL");
//        }
       
		List<string> webContentList1 = new List<string> ();
		webContentList1.Add ("https://upload.wikimedia.org/wikipedia/commons/b/bd/Westinghouse_Van_de_Graaff_atom_smasher_-_cutaway.png");
		webContentList1.Add ("http://www.kshitij-iitjee.com/Study/Physics/Part4/Chapter25/57.jpg");
		webContentList1.Add ("http://www.atmo.arizona.edu/students/courselinks/spring13/atmo589/ATMO489_online/lecture_2/van_de_graaff_sketch.jpg");
		webContentList1.Add ("http://www.atmo.arizona.edu/students/courselinks/spring08/atmo336s1/courses/spring11/atmo589/lecture_notes/jan18/no_points.jpg");
		Lecture lecture1 = new Lecture ("Van de Graaff Generator", webContentList1);
		this.lectures.Add (lecture1);

		List<string> webContentList2 = new List<string> ();
		webContentList2.Add ("http://www.nuffieldfoundation.org/sites/default/files/images/Experiments%20with%20a%20Van%20de%20Graaff%20generator3_480.jpg");
		webContentList2.Add ("http://www.nuffieldfoundation.org/sites/default/files/images/counting%20matches%20van%201015.jpg");
		webContentList2.Add ("http://demoweb.physics.ucla.edu/sites/default/files/instructional_lab_manuals_physics_6b_experiment_5_experiment_5_files/instructionallab_manualsphysics6bexperiment_56b-exp4_fig9.png");
		Lecture lecture2 = new Lecture ("Van de Graaf Experiment", webContentList2);
		this.lectures.Add (lecture2);

		List<string> webContentList3 = new List<string> ();
		webContentList3.Add ("http://images2.fanpop.com/images/photos/3300000/Tesla-Reading-by-the-Light-of-the-Tesla-Coil-nikola-tesla-3363025-390-561.jpg");
		webContentList3.Add ("http://freie-energie-projekt.de/wp-content/uploads/2013/07/Wardenclyffe-Tower.jpg");
		webContentList3.Add ("https://upload.wikimedia.org/wikipedia/commons/0/01/Original_Tesla_Coil.png");
		webContentList3.Add ("http://www.amazing1.com/content/Graphics/btc30-3.jpg");
		webContentList3.Add ("http://www.gfx3.de/konstantin/wordpress/wp-content/uploads/2012/09/tesla_cad.png");
		webContentList3.Add ("http://www.stellarproducts.com/about/personal/papers/tcfig1.jpg");
		Lecture lecture3 = new Lecture ("Tesla Transformator", webContentList3);
		this.lectures.Add (lecture3);

		List<string> webContentList4 = new List<string> ();
		webContentList4.Add ("https://qph.is.quoracdn.net/main-qimg-a044ef142c78706813aeff66efca5b4c?convert_to_webp=true");
		webContentList4.Add ("https://lh4.googleusercontent.com/-TybeZ_pWwkQ/TYez0f0r-4I/AAAAAAAAEzs/DKitANPemPQ/s1600/VFPt_charges_plus_plus-morris.png");
		Lecture lecture4 = new Lecture ("Electric Field", webContentList4);
		this.lectures.Add (lecture4);
		
		this.selectedLecture = lecture1;
		// ----------------------
	}
	
	public void Update () {

	}

	public void SelectLecture(int lectureIndex)
	{
		if (lectureIndex < 0 || lectureIndex >= this.lectures.Count) {
			throw new  System.IndexOutOfRangeException(string.Format("Selected Lecture Index {0} out of range", lectureIndex));
		}
		this.selectedLecture = this.lectures [lectureIndex];
		this.currentWebContentIndex = 0;
	}

	public void Refresh()
	{
		if (null == this.selectedLecture.WebContents)
			throw new System.NullReferenceException("Web contents list in selected lecture is null");
		lock (this.lockObject) {
			if (this.currentWebContentIndex >= 0 && this.selectedLecture.WebContents.Count > this.currentWebContentIndex) {
				this.LoadTexture (this.selectedLecture.WebContents [this.currentWebContentIndex]);
			}
		}
	}

	public void Next() 
	{
		if (null == this.selectedLecture.WebContents)
			throw new System.NullReferenceException("Web contents list in selected lecture is null");
		lock (this.lockObject) {
			if (this.selectedLecture.WebContents.Count > this.currentWebContentIndex + 1) {
				this.LoadTexture (this.selectedLecture.WebContents [++this.currentWebContentIndex]);
			}
		}
	}

	public void Previous() 
	{
		if (null == this.selectedLecture.WebContents)
			throw new System.NullReferenceException("Web contents list in selected lecture is null");
		lock (this.lockObject) {
			if (this.currentWebContentIndex > 0) {
				this.LoadTexture (this.selectedLecture.WebContents [--this.currentWebContentIndex]);
			}
		}
	}

	private bool LoadTexture(WWW webContent)
	{
		if (null == webContent || !webContent.isDone || null == webContent.texture) {
			return false;
		}
		this.myRenderer.material.mainTexture = webContent.texture;
		return true;
	}
}
