using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
	public enum DrawMode { Mesh, ColorMap, NoiseMap, FalloffMap };

	[Header("Experiment Parameters")] //these are what the user will be able to adjust and can be reset to their initial inspector values upon ResetParameters() being called
	public DrawMode drawMode;
	[Range(0, 6)]
	public int editorPreviewLOD;
	[Range(10, 100)]
	public float noiseScale;
	[Range(0, 1)]
	public float persistance;
	[Range(1, 6)]
	public float lacunarity;
	public bool useFalloff;
	

	[Header("VR Control Parameters")]
	public float traversalSpeedMultiplier = 0.1f;
	public float scaleSpeed = 2f;
	public float maxScale = 200f;
	public float minScale = 10f;
	public float scaleHeightMultiplier = 0.5f;
	public bool speedChangeWithScale = true;

	[Header("Miscellaneous")]
	[Range(1, 12)]
	public int octaves;
	public bool parallaxEffect;
	public Noise.NormalizeMode normalizeMode;
	public const int mapChunkSize = 239;
	public int seed;
	public Vector2 offset;
	public bool generateCollider;
	[Range(0, 6)]
	public int colliderMeshLOD;
	public float meshHeightMultiplier;
	public AnimationCurve meshHeightCurve;
	public bool autoUpdate;
	public TerrainType[] regions;

	ExperimentParameters initialParameters;
	float[,] falloffMap;

	Queue<MapThreadInfo<MapData>> mapDataThreadInfoQueue = new Queue<MapThreadInfo<MapData>>();
	Queue<MapThreadInfo<MeshData>> meshDataThreadInfoQueue = new Queue<MapThreadInfo<MeshData>>();

	void Awake()
	{
		falloffMap = FalloffGenerator.GenerateFalloffMap(mapChunkSize);
	}

	private void Start()
	{
		DrawMapInEditor();
		initialParameters = new ExperimentParameters(drawMode, noiseScale, octaves, persistance, lacunarity, useFalloff, parallaxEffect, editorPreviewLOD);
	}

	public void DrawMapInEditor()
	{
		MapData mapData = GenerateMapData(Vector2.zero);

		MapDisplay display = FindObjectOfType<MapDisplay>();
		if (drawMode == DrawMode.NoiseMap)
		{
			display.DrawTexture(TextureGenerator.TextureFromHeightMap(mapData.heightMap));
		}
		else if (drawMode == DrawMode.ColorMap)
		{
			display.DrawTexture(TextureGenerator.TextureFromColorMap(mapData.colorMap, mapChunkSize, mapChunkSize));
		}
		else if (drawMode == DrawMode.Mesh)
		{
			if (generateCollider)
			{
				display.DrawMeshWithCollider(MeshGenerator.GenerateTerrainMesh(mapData.heightMap, meshHeightMultiplier, meshHeightCurve, editorPreviewLOD), TextureGenerator.TextureFromColorMap(mapData.colorMap, mapChunkSize, mapChunkSize), MeshGenerator.GenerateTerrainMesh(mapData.heightMap, meshHeightMultiplier, meshHeightCurve, colliderMeshLOD));
			}
			else
			{
				display.DrawMesh(MeshGenerator.GenerateTerrainMesh(mapData.heightMap, meshHeightMultiplier, meshHeightCurve, editorPreviewLOD), TextureGenerator.TextureFromColorMap(mapData.colorMap, mapChunkSize, mapChunkSize));
			}
		}
		else if (drawMode == DrawMode.FalloffMap)
		{
			display.DrawTexture(TextureGenerator.TextureFromHeightMap(FalloffGenerator.GenerateFalloffMap(mapChunkSize)));
		}
	}

	public void RequestMapData(Vector2 center, Action<MapData> callback)
	{
		ThreadStart threadStart = delegate {
			MapDataThread(center, callback);
		};

		new Thread(threadStart).Start();
	}

	void MapDataThread(Vector2 center, Action<MapData> callback)
	{
		MapData mapData = GenerateMapData(center);
		lock (mapDataThreadInfoQueue)
		{
			mapDataThreadInfoQueue.Enqueue(new MapThreadInfo<MapData>(callback, mapData));
		}
	}

	public void RequestMeshData(MapData mapData, int lod, Action<MeshData> callback)
	{
		ThreadStart threadStart = delegate {
			MeshDataThread(mapData, lod, callback);
		};

		new Thread(threadStart).Start();
	}

	void MeshDataThread(MapData mapData, int lod, Action<MeshData> callback)
	{
		MeshData meshData = MeshGenerator.GenerateTerrainMesh(mapData.heightMap, meshHeightMultiplier, meshHeightCurve, lod);
		lock (meshDataThreadInfoQueue)
		{
			meshDataThreadInfoQueue.Enqueue(new MapThreadInfo<MeshData>(callback, meshData));
		}
	}

	void Update()
	{
		if (mapDataThreadInfoQueue.Count > 0)
		{
			for (int i = 0; i < mapDataThreadInfoQueue.Count; i++)
			{
				MapThreadInfo<MapData> threadInfo = mapDataThreadInfoQueue.Dequeue();
				threadInfo.callback(threadInfo.parameter);
			}
		}

		if (meshDataThreadInfoQueue.Count > 0)
		{
			for (int i = 0; i < meshDataThreadInfoQueue.Count; i++)
			{
				MapThreadInfo<MeshData> threadInfo = meshDataThreadInfoQueue.Dequeue();
				threadInfo.callback(threadInfo.parameter);
			}
		}
	}

	MapData GenerateMapData(Vector2 center)
	{
		float[,] noiseMap = Noise.GenerateNoiseMap(mapChunkSize + 2, mapChunkSize + 2, seed, noiseScale, octaves, persistance, lacunarity, center + offset, normalizeMode, parallaxEffect);

		Color[] colorMap = new Color[mapChunkSize * mapChunkSize];
		for (int y = 0; y < mapChunkSize; y++)
		{
			for (int x = 0; x < mapChunkSize; x++)
			{
				if (useFalloff)
				{
					noiseMap[x, y] = Mathf.Clamp01(noiseMap[x, y] - falloffMap[x, y]);
				}

				float currentHeight = noiseMap[x, y];

				for (int i = 0; i < regions.Length; i++)
				{
					if (currentHeight >= regions[i].height)
					{
						colorMap[y * mapChunkSize + x] = regions[i].color;
					}
					else
					{
						break;
					}
				}
			}
		}

		return new MapData(noiseMap, colorMap);
	}

	void OnValidate()
	{
		if (lacunarity < 1) lacunarity = 1;
		if (octaves < 0) octaves = 0;
		meshHeightMultiplier = noiseScale * scaleHeightMultiplier;
		falloffMap = FalloffGenerator.GenerateFalloffMap(mapChunkSize);
	}

	struct MapThreadInfo<T>
	{
		public readonly Action<T> callback;
		public readonly T parameter;

		public MapThreadInfo(Action<T> callback, T parameter)
		{
			this.callback = callback;
			this.parameter = parameter;
		}
	}

	public void ChangeOffset(Vector3 value)
	{
		Vector2 valueV2 = new Vector2(value.x, value.y);
		valueV2 = valueV2 * 2 - Vector2.one;

		if (speedChangeWithScale)
		{
			offset += minScale * traversalSpeedMultiplier * valueV2 / noiseScale;
		}
		else
		{
			offset += traversalSpeedMultiplier * valueV2;
		}
		
		DrawMapInEditor();
	}

	public void ChangeScale(float value)
	{
		noiseScale = value;
		meshHeightMultiplier = noiseScale * scaleHeightMultiplier;
		DrawMapInEditor();
	}

	public void ChangeLOD(int value)
	{
		int newLOD = 6 - value;
		editorPreviewLOD = newLOD;
		DrawMapInEditor();
	}

	public void ChangePersistance(float value)
	{
		persistance = value;
		DrawMapInEditor();
	}

	public void ChangeLacunarity(float value)
	{
		lacunarity = value;
		DrawMapInEditor();
	}

	public void SetFalloff(bool value)
	{
		useFalloff = value;
		DrawMapInEditor();
	}

	public void ResetParameters()
	{
		drawMode = initialParameters.drawMode;
		octaves = initialParameters.octaves;
		persistance = initialParameters.persistance;
		lacunarity = initialParameters.lacunarity;
		useFalloff = initialParameters.useFalloff;
		parallaxEffect = initialParameters.parallaxEffect;
		editorPreviewLOD = initialParameters.editorPreviewLOD;

		noiseScale = initialParameters.noiseScale;
		meshHeightMultiplier = noiseScale * scaleHeightMultiplier;
		DrawMapInEditor();
	}
}

[System.Serializable]
public struct TerrainType
{
	public string name;
	public float height;
	public Color color;
}

public struct MapData
{
	public readonly float[,] heightMap;
	public readonly Color[] colorMap;

	public MapData(float[,] heightMap, Color[] colorMap)
	{
		this.heightMap = heightMap;
		this.colorMap = colorMap;
	}
}

public class ExperimentParameters
{
	public MapGenerator.DrawMode drawMode;
	public float noiseScale;
	public int octaves;
	public float persistance;
	public float lacunarity;
	public bool useFalloff;
	public bool parallaxEffect;
	public int editorPreviewLOD;

	public ExperimentParameters(MapGenerator.DrawMode drawMode, float noiseScale, int octaves, float persistance, float lacunarity, bool useFalloff, bool parallaxEffect, int editorPreviewLOD)
	{
		this.drawMode = drawMode;
		this.noiseScale = noiseScale;
		this.octaves = octaves;
		this.persistance = persistance;
		this.lacunarity = lacunarity;
		this.useFalloff = useFalloff;
		this.parallaxEffect = parallaxEffect;
		this.editorPreviewLOD = editorPreviewLOD;
	}
}