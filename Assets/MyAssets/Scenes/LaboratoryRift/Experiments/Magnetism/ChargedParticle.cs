using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargedParticle : MonoBehaviour
{
	public float charge = 1f;
	private Renderer _renderer;
	public Color PositiveChargedColor = Color.green;
	public Color NegativeChargedColor = Color.red;

	public void Awake()
	{
		_renderer = GetComponent<Renderer>();
	}

	public void Start()
	{
		UpdateColor();
	}

	public void UpdateColor()
	{
		Color color = charge > 0 ? PositiveChargedColor : NegativeChargedColor;
		if (!_renderer)
			_renderer = GetComponent<Renderer>();
		_renderer.material.color = color;
	}

	private void OnValidate()
	{
		UpdateColor();
	}
}
