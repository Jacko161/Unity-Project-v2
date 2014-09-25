using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public class HookHeadServerBehaviour : MonoBehaviour 
{

	public GameObject			linkPrefab;
	public float				speed 		= 10.0f;
	public int					maxLinks 	= 10;



	//
	// Start
	//
	void Start () 
	{
		clientScript 	= GetComponent<HookHeadClientBehaviour>();
		clientLinks		= clientScript.links;
	}



	//
	// Update
	//
	void Update () 
	{
		if( IsFiring )
		{
			// Check if the hook has been full extended.
			if( clientLinks.Count >= maxLinks )
			{
				// Flag that we are now retracting. Reverse the link targets in order to go back along the same path.
				extending = false;
				networkView.RPC( "ReverseLinkTargets", RPCMode.All );
			}
			
			
			// Distance between the last spawned link and the origin.
			float 		distance = Vector3.Distance( clientLinks[clientLinks.Count - 1].GetComponent<HookLinkBehaviour>().CurrentPivot.transform.position, clientScript.hookOrigin.transform.position );
			if( extending )
			{
				// Keep creating links until the empty space is full. Only create a link if the hook has been extended enough.
                for( int i = 0; i < ( int ) ( distance / ( HookLinkBehaviour.gap ) ); i += 1 )
				{
					networkView.RPC( "PushLink", RPCMode.All, clientScript.hookOrigin.transform.position, Quaternion.LookRotation( ( clientLinks[clientLinks.Count - 1].GetComponent<HookLinkBehaviour>().CurrentPivot.transform.position - clientScript.hookOrigin.transform.position ).normalized ) );
				}
			}
			else 
			{
				// Retract links as they return to the origin.
				if( distance < HookLinkBehaviour.gap )
				{
					if( clientLinks.Count > 1 )
					{
						networkView.RPC( "PopLink", RPCMode.All );
					}
					// This is the head-link. The hook has fully retracted, so we reset some state and deactivate it.
					else
					{
						EndHook();
					}
				}
			}
		}
	}



	//
	// OnCollisionEnter
	//
	void OnCollisionEnter( Collision other )
	{
		if( IsFiring )
		{
			// We've hit the level mesh, reflect off the contact normal.
			if( other.gameObject.tag == "Level" )
			{
				if( other.contacts.Length > 0 )
				{
					transform.forward =  Vector3.Reflect( transform.forward, other.contacts[0].normal );	
				}
			}
			if( other.gameObject.tag == "Player" && other.gameObject.transform != parent )
			{
				if( extending )
				{
					extending = false;
					networkView.RPC( "ReverseLinkTargets", RPCMode.All );
				}
			}
		}
	}



	//
	// FireHook
	//
	public void FireHook()
	{
		if( !IsFiring )
		{
			// Set the speed for all the links. Target is set to null so that the head link just follows it's initial normal.
			GetComponent<HookLinkBehaviour>().target = null;
			GetComponent<HookLinkBehaviour>().speed = speed;


			parent			   									= transform.parent;
			clientScript.hookOrigin.transform.parent 			= parent;
			clientScript.hookOrigin.transform.localPosition 	= transform.localPosition;
			transform.parent   									= null;

			
			networkView.RPC( "AddHeadLink", RPCMode.All );

			GetComponent<HookLinkBehaviour>().enabled = true;
			firing = true;
		}
	}


	//
	// EndHook
	//
	public void EndHook()
	{
		extending = true;
		networkView.RPC( "RemoveHeadLink", RPCMode.All );
		
		transform.parent 								= parent;
		transform.localRotation							= Quaternion.identity;
		transform.localPosition							= clientScript.hookOrigin.transform.localPosition;
		clientScript.hookOrigin.transform.parent 		= transform;
		clientScript.hookOrigin.transform.localPosition	= Vector3.zero;


		GetComponent<HookLinkBehaviour>().enabled = false;
		firing = false;
	}



	//
	// IsFiring
	//
	public bool IsFiring
	{
		get
		{
			return firing;
		}
	}



	private bool						extending 		= true;
	private bool						firing			= false;
	private	Transform					parent			= null;
	private HookHeadClientBehaviour		clientScript	= null;
	private List<GameObject>			clientLinks		= null;
}
