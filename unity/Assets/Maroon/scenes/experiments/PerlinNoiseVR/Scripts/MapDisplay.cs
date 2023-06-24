using System.Collections;
using UnityEngine;

public class MapDisplay : MonoBehaviour
{
	public Renderer textureRenderer;
	public MeshFilter meshFilter;
	public MeshRenderer meshRenderer;
	public MeshCollider meshCollider;

	public void DrawTexture(Texture2D texture)
	{
		meshRenderer.gameObject.SetActive(false);
		textureRenderer.gameObject.SetActive(true);
		textureRenderer.sharedMaterial.mainTexture = texture;
	}

	public void DrawMesh(MeshData meshData, Texture2D texture)
	{
		textureRenderer.gameObject.SetActive(false);
		meshRenderer.gameObject.SetActive(true);
		meshFilter.sharedMesh = meshData.CreateMesh();
		meshRenderer.sharedMaterial.mainTexture = texture;
		meshCollider.sharedMesh = null;
	}

	public void DrawMeshWithCollider(MeshData meshData, Texture2D texture, MeshData colliderMeshData)
	{
		meshFilter.sharedMesh = meshData.CreateMesh();
		meshRenderer.sharedMaterial.mainTexture = texture;
		meshCollider.sharedMesh = colliderMeshData.CreateMesh();
	}
}
