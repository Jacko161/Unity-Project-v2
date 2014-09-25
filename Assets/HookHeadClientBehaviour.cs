using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HookHeadClientBehaviour : MonoBehaviour 
{

	public GameObject				hookOrigin;
	public List<GameObject>			links 			= new List<GameObject>();



	//
	// Start
	//
	void Start()
	{
		serverScript = GetComponent<HookHeadServerBehaviour>();
	}







	//
	// OnSerializeNetworkView
	//
	void OnSerializeNetworkView( BitStream stream, NetworkMessageInfo info )
	{
		Vector3		syncPosition 	= Vector3.zero;
		Quaternion	syncRotation	= Quaternion.identity;
		Vector3		syncHookOrigin	= Vector3.zero;

		if( stream.isWriting )
		{
			syncPosition 	= transform.position;
			syncRotation 	= transform.rotation;
			syncHookOrigin 	= hookOrigin.transform.position;
			
			stream.Serialize( ref syncPosition );
			stream.Serialize( ref syncRotation );
			stream.Serialize( ref syncHookOrigin );
		}
		else if( stream.isReading )
		{
			stream.Serialize( ref syncPosition );
			stream.Serialize( ref syncRotation );
			stream.Serialize( ref syncHookOrigin );

			transform.position 				= syncPosition;
			transform.rotation 				= syncRotation;
			hookOrigin.transform.position	= syncHookOrigin;
		}
	}

	
	
	//
	// AddLinkAll
	//
	[RPC] public void AddHeadLink()
	{
		// Add the head as a link.
		links.Add( gameObject );
	}



	//
	// RemoveHeadLink
	//
	[RPC] public void RemoveHeadLink()
	{
		links.Remove( gameObject );
	}
	
	
	
	//
	// ReverseLinkTargets
	// Just set each link's target to the link before it. Set the last link's
	// target to the origin.
	//
	[RPC] public void ReverseLinkTargets()
	{
		for( int i = 0; i < links.Count - 1; i++ )
		{
			links[i].GetComponent<HookLinkBehaviour>().target 			= links[i + 1].GetComponent<HookLinkBehaviour>();
			links[i].GetComponent<HookLinkBehaviour>().IsExtending 		= false;
		}
		links[links.Count - 1].GetComponent<HookLinkBehaviour>().target  			= hookOrigin.GetComponent<HookLinkBehaviour>();
		links[links.Count - 1].GetComponent<HookLinkBehaviour>().IsExtending 		= false;
	}
	
	
	
	//
	// PushLink
	// Add a new link to the back of the hook.
	//
	[RPC] public void PushLink( Vector3 spawnPosition, Quaternion spawnRotation )
	{
		GameObject		newLink = Instantiate( serverScript.linkPrefab, spawnPosition, spawnRotation ) as GameObject;
		newLink.GetComponent<HookLinkBehaviour>().target = links[links.Count - 1].GetComponent<HookLinkBehaviour>();
		newLink.GetComponent<HookLinkBehaviour>().speed = serverScript.speed;
		links.Add( newLink );
	}
	
	
	
	//
	// PopLink
	// Remove a link from the back of the hook.
	//
	[RPC] public void PopLink()
	{
		Destroy( links[links.Count - 1] );
		links.RemoveAt( links.Count - 1 );
		links[links.Count - 1].GetComponent<HookLinkBehaviour>().target = hookOrigin.GetComponent<HookLinkBehaviour>();
	}



	private HookHeadServerBehaviour 			serverScript	= null;
}
