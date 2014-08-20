using UnityEngine;
using System.Collections;

public class HookLinkBehaviour : MonoBehaviour 
{
	// The link will follow it's target.
	public Transform			target			= null;
	public Vector3				targetOffset 	= Vector3.zero;
	public float				speed			= 0.0f;
	public GameObject			endPivot;



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
			Vector3			normalToTarget = ( Origin - transform.position ).normalized;
			transform.rotation = Quaternion.LookRotation( normalToTarget );
			if( Vector3.Distance( Origin, transform.position ) > 0.8f )
			{
				transform.Translate( Vector3.forward * speed * Time.deltaTime );
			}
		}
		else
		{
			transform.Translate( Vector3.forward * speed * Time.deltaTime );
		}
	}



	//
	// Origin
	//
	private Vector3 Origin
	{
		get
		{
			return target.position + targetOffset;
		}
	}
}
