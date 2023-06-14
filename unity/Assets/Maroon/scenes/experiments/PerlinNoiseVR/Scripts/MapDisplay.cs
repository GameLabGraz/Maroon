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
		textureRenderer.sharedMaterial.mainTexture = texture;
		textureRenderer.transform.localScale = new Vector3(texture.width, 1, texture.height);
	}

	public void DrawMesh(MeshData meshData, Texture2D texture)
	{
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
