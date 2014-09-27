using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public class HookHeadServerBehaviour : MonoBehaviour 
{



	//
	// State
	//
	[System.Flags]
	public enum State
	{
		idle   		= 0,
		firing 		= 1,
		fading 		= 2,
		attached 	= 4,
		extending	= 8,
	}



	public GameObject			linkPrefab;
	public float				speed 					= 10.0f;
	public int					maxLinks 				= 10;
	public float				fadeTimeSeconds			= 1.0f;



	//
	// Start
	//
	void Start () 
	{
		clientScript 	= GetComponent<HookHeadClientBehaviour>();
		clientLinks		= clientScript.links;
		attachments		= new List<GameObject>();
	}



	//
	// Update
	//
	void Update () 
	{
		if( ( state & State.firing ) == State.firing )
		{
			// Check if the hook has been full extended.
			if( clientLinks.Count >= maxLinks )
			{
				// Flag that we are now retracting. Reverse the link targets in order to go back along the same path.
				state &= ~State.extending;
				networkView.RPC( "ReverseLinkTargets", RPCMode.All );
			}
			
			
			// Distance between the last spawned link and the origin.
			float 		distance = Vector3.Distance( clientLinks[clientLinks.Count - 1].GetComponent<HookLinkBehaviour>().CurrentPivot.transform.position, clientScript.hookOrigin.transform.position );
			if( ( state & State.extending ) == State.extending )
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
		// If we're fading, we just want to fade the mesh. No other logic is needed at all.
		else if( ( state & State.fading ) == State.fading )
		{
			float		deltaFadeTime =  Time.time - fadeStartTime;
			networkView.RPC( "SetMeshAlpha", RPCMode.All, 1.0f - ( deltaFadeTime / fadeTimeSeconds ) );

			Debug.Log(  1.0f - ( deltaFadeTime / fadeTimeSeconds ) );
			if( deltaFadeTime > fadeTimeSeconds )
			{
				EndFade();
			}
		}
	}



	//
	// OnCollisionEnter
	//
	void OnCollisionEnter( Collision other )
	{
		if( ( state & State.firing ) == State.firing )
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
				if( ( state & State.extending ) == State.extending )
				{
					attachments.Add( other.gameObject );

					// We hit a player. Attach to them and start retracting.
					state &= ~State.extending;
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
		if( !( ( state & State.firing ) == State.firing ) )
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

			// Set flags.
			state = State.firing;
			state |= State.extending;
		}
	}


	//
	// EndHook
	//
	public void EndHook()
	{
		networkView.RPC( "RemoveHeadLink", RPCMode.All );
		
		transform.parent 								= parent;
		transform.localRotation							= Quaternion.identity;
		transform.localPosition							= clientScript.hookOrigin.transform.localPosition;
		clientScript.hookOrigin.transform.parent 		= transform;
		clientScript.hookOrigin.transform.localPosition	= Vector3.zero;
		
		GetComponent<HookLinkBehaviour>().enabled = false;

		state = State.idle;
	}


	//
	// StartFade
	//
	public void StartFade()
	{
		// Free all players from this hook.
		foreach( GameObject player in attachments )
		{
			player.GetComponent<PlayerServerBehaviour>().DetachFromPlayer( gameObject );
		}
		attachments.Clear();

		fadeStartTime = Time.time;
		state = State.fading;
	}



	//
	// EndFade
    //
	public void EndFade()
	{
		networkView.RPC( "SetMeshAlpha", RPCMode.All, 1.0f );
		networkView.RPC( "RemoveAllLinks", RPCMode.All );

		transform.parent 								= parent;
		transform.localRotation							= Quaternion.identity;
		transform.localPosition							= clientScript.hookOrigin.transform.localPosition;
		clientScript.hookOrigin.transform.parent 		= transform;
		clientScript.hookOrigin.transform.localPosition	= Vector3.zero;

		GetComponent<HookLinkBehaviour>().enabled = false;
		
		state = State.idle;
    }



	//
	// IsFiring
	//
	public bool IsFiring
	{
		get
		{
			return ( state & State.firing ) == State.firing;
		}
	}



	private	Transform					parent			= null;
	private HookHeadClientBehaviour		clientScript	= null;
	private List<GameObject>			clientLinks		= null;
	private List<GameObject>			attachments		= null;
	private State						state			= State.idle;
	private float						fadeStartTime	= 0;



}
