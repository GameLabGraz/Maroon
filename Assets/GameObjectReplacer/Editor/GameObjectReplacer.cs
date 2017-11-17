using UnityEngine;
using UnityEditor;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace GOReplacer
{
	public class GameObjectReplacer : EditorWindow 
	{
		private static GameObjectReplacer Window;
		
		private static GameObject[] GameObjects;
		private static GameObject GameObjectReplacement;
		private static bool CopyRotation, CopyScale;
		
		private static Rect m_PrefabIconRect;
		private static Texture2D m_PrefabIcon;
		private static bool m_ShowInSceneView;
				
		[MenuItem ("GameObject/GameObject Replacer")]
		[MenuItem ("Window/GameObject Replacer")]
		private static void Init () 
		{
			Window = (GameObjectReplacer)EditorWindow.GetWindow (typeof (GameObjectReplacer),false, "GameObject Replacer");
			Vector2 windowMinSize = Window.minSize;
			windowMinSize.x = 250;
			windowMinSize.y = 125;
			Window.minSize = windowMinSize;
			GameObjectReplacement = null;
		}
		
		private void OnEnable()
		{
			Init();
		}

		private void OnScene(SceneView sceneView)
		{
			GameObjects = Selection.gameObjects;

			GUI.skin.font = ((GUIStyle)"ShurikenLabel").font;
			
			
			Handles.BeginGUI();
			GUILayout.BeginArea(new Rect(10, Screen.height-148, 216,100), "GameObject Replacer", GUI.skin.window);
			GetLayoutFields(true);
			Repaint();
			GUILayout.EndArea();
			Handles.EndGUI();
        }
    
        public void OnGUI () 
		{
			GameObjects = Selection.gameObjects;
			GUI.skin.font = ((GUIStyle)"ShurikenLabel").font;
			
			GetLayoutHeader();
					
			GUILayout.Space(15);
            GUILayout.BeginHorizontal();
			GUILayout.Space(10);
			GUILayout.BeginVertical();
			
			GetLayoutFields();
			
			GUILayout.Space(5);
			GUILayout.EndVertical();
			GUILayout.Space(10);
			GUILayout.EndHorizontal();
			EditorGUILayout.EndVertical();
			SceneView.RepaintAll();
		}
		
		private void Replace()
		{
			
			string prefabType = PrefabUtility.GetPrefabType(GameObjectReplacement).ToString();
			
			List<GameObject> newSelection = new List<GameObject>();
			foreach(GameObject gameObject in GameObjects)
			{
				if(gameObject == GameObjectReplacement)
				{
					newSelection.Add(gameObject);
					continue;
				}
					
				GameObject newGameObject = null;
				Object newPrefab = PrefabUtility.GetPrefabParent(GameObjectReplacement);
				
				switch(prefabType)
				{
				case "PrefabInstance":
					newGameObject =	PrefabUtility.InstantiatePrefab(newPrefab) as GameObject;
					PrefabUtility.SetPropertyModifications(newGameObject, PrefabUtility.GetPropertyModifications(GameObjectReplacement));
					break;
				case "Prefab":
					newGameObject = PrefabUtility.InstantiatePrefab(GameObjectReplacement) as GameObject;
					break;
				case "None":
					newGameObject = GameObject.Instantiate(GameObjectReplacement) as GameObject;
					newGameObject.name = GameObjectReplacement.name;
                    break;
				}
			
				Undo.RegisterCreatedObjectUndo(newGameObject, "created object");
				
				newGameObject.transform.position = gameObject.transform.position;
				
				if(CopyRotation)
					newGameObject.transform.rotation = gameObject.transform.rotation;

				if(CopyScale)
					newGameObject.transform.localScale = gameObject.transform.localScale;

				newGameObject.transform.parent = gameObject.transform.parent;
				
				Undo.DestroyObjectImmediate(gameObject);
				newSelection.Add(newGameObject);
			}
			Selection.objects = newSelection.ToArray();
			
			string goString = (newSelection.Count > 1) ? " GameObjects have " : " GameObject has ";
			Debug.Log(newSelection.Count.ToString() + goString + "been replaced with: " + GameObjectReplacement.name + "\nPrefab Type: " + prefabType);
		}
		
		private void GetLayoutHeader()
		{
			GUILayout.Space(5);
			EditorGUILayout.BeginVertical((GUIStyle)"HelpBox");
			EditorGUILayout.LabelField("GameObject Replacer", (GUIStyle)"ShurikenEmitterTitle");
            if(Event.current.type == EventType.Repaint)
			{
	            if(m_PrefabIcon == null)
	            {
					m_PrefabIcon = EditorGUIUtility.FindTexture("PrefabNormal Icon");
					m_PrefabIconRect = GUILayoutUtility.GetLastRect();
					m_PrefabIconRect.x += 4;
					m_PrefabIconRect.y += 1;
					m_PrefabIconRect.width = m_PrefabIcon.width/3;
	                m_PrefabIconRect.height = m_PrefabIcon.height/3;
	            }
	            else
	            {
					GUI.DrawTexture(m_PrefabIconRect, m_PrefabIcon);
	            }
            }
        }
        
		private void GetLayoutFields(bool isSceneView = false)
		{
			EditorGUIUtility.labelWidth = 100;
			
			GameObjectReplacement = EditorGUILayout.ObjectField("Replacement", GameObjectReplacement, typeof(GameObject),true) as GameObject;
			CopyRotation = EditorGUILayout.Toggle("Copy Rotation", CopyRotation);
			CopyScale = EditorGUILayout.Toggle("Copy Scale", CopyScale);
			
			EditorGUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			if(GUILayout.Button("Replace", GUILayout.Width(75)))
			{
				if(GameObjects.Length != 0 && GameObjectReplacement != null)
					Replace();
				else 
					EditorUtility.DisplayDialog("Missing GameObjects!", "Make sure you have both a Replacement GameObject and have selected 1 or more GameObjects in the scene.", "OK");
            }
            if(!m_ShowInSceneView)
			{
				if(GUILayout.Button("Scene View", GUILayout.Width(100)))
				{
					SceneView.onSceneGUIDelegate += OnScene;
					m_ShowInSceneView = true;
					
					BindingFlags bindings = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;
					MethodInfo isDocked = typeof(EditorWindow).GetProperty("docked", bindings).GetGetMethod(true);
					
					if((bool)isDocked.Invoke(this, null) == false) 
						Window.Close();
				}
			}
            
            if(isSceneView)
            {
				if(GUILayout.Button("Close", GUILayout.Width(50)))
				{
					m_ShowInSceneView = false;
					SceneView.onSceneGUIDelegate -= OnScene;
				}
            }
            
            EditorGUILayout.EndHorizontal();
        }
    }
}