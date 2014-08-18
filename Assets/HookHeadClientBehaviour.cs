using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HookHeadClientBehaviour : MonoBehaviour 
{

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
		if( stream.isWriting )
		{
			syncPosition = transform.position;
			syncRotation = transform.rotation;
			
			stream.Serialize( ref syncPosition );
			stream.Serialize( ref syncRotation );
		}
		else if( stream.isReading )
		{
			stream.Serialize( ref syncPosition );
			stream.Serialize( ref syncRotation );
			
			transform.position = syncPosition;
			transform.rotation = syncRotation;
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
			links[i].GetComponent<HookLinkBehaviour>().target 			= links[i + 1].transform;
			links[i].GetComponent<HookLinkBehaviour>().targetOffset 	= Vector3.zero;
		}
		links[links.Count - 1].GetComponent<HookLinkBehaviour>().target 			= serverScript.PlayerTransform;
		links[links.Count - 1].GetComponent<HookLinkBehaviour>().targetOffset 		= serverScript.hookOffset;
	}
	
	
	
	//
	// PushLink
	// Add a new link to the back of the hook.
	//
	[RPC] public void PushLink( Vector3 spawnPosition, Quaternion spawnRotation )
	{
		GameObject		newLink = Instantiate( serverScript.linkPrefab, spawnPosition, spawnRotation ) as GameObject;
		newLink.GetComponent<HookLinkBehaviour>().target = links[links.Count - 1].transform;
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
		links[links.Count - 1].GetComponent<HookLinkBehaviour>().target = serverScript.PlayerTransform;
		links[links.Count - 1].GetComponent<HookLinkBehaviour>().targetOffset = serverScript.hookOffset;
	}



	private HookHeadServerBehaviour 			serverScript	= null;
}
