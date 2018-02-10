using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingChargedParticle : ChargedParticle
{

	public float Mass = 1f;
	public Rigidbody Rb;

	private void Awake()
	{
		base.Awake(); // explicitly call awake of base class to assign renderer
		
		Rb = gameObject.GetOrAddComponent<Rigidbody>();
	}

	private void Start()
	{
		base.Start();

		Rb.mass = Mass;
		Rb.useGravity = false;
	}
}
