using UnityEngine;
using System.Collections;

public class HookLinkBehaviour : MonoBehaviour 
{
	// The link will follow it's target.
	public Transform			target	= null;
	public float				speed	= 0.0f;



	//
	// Start
	//
	void Start () 
	{
	}



	//
	// Update
	//
	void Update () 
	{
		// If the target is null, just move the link towards it's forward vector. This is intended
		// for the head link only. If it's not null, follow the target.
		if( target != null )
		{
			// Get the direction of this link to it's target, and translate.
			Vector3			normalToTarget = ( target.position - transform.position ).normalized;
			transform.Translate( normalToTarget * speed * Time.deltaTime, Space.World );
		}
		else
		{
			transform.Translate( Vector3.forward * speed * Time.deltaTime );
		}
	}
}
