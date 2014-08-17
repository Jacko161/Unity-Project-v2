using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HookHeadBehvaiour : MonoBehaviour 
{
	public float				speed 		= 10.0f;
	public int					maxLinks 	= 10;
	public GameObject			linkPrefab;
	public Transform			originTransform;



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
		if( IsFiring )
		{
			// Check if the hook has been full extended.
			if( links.Count >= maxLinks )
			{
				// Flag that we are now retracting. Reverse the link targets in order to go back along the same path.
				extending = false;
				ReverseLinkTargets();
			}


			// Distance between the last spawned link and the origin.
			float 		distance = Vector3.Distance( links[links.Count - 1].transform.position, originTransform.position );
			if( extending )
			{
				// Keep creating links until the empty space is full. Only create a link if the hook has been extended enough.
				for( int i = 0; i < ( int ) ( distance / linkPrefab.transform.localScale.z ); i += 1 )
				{
					PushLink();
				}
			}
			else 
			{
				// Retract links as they return to the origin.
				if( distance < linkPrefab.transform.localScale.z )
				{
	                if( links.Count > 1 )
					{
						PopLink();
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
    			transform.forward = Vector3.Reflect( transform.forward, other.contacts[0].normal );
    		}
            // We've collided with another player, start to retract, and bring the player with us.
            else if( other.gameObject.tag == "Player" && other.gameObject != player )
            {
                if( extending )
                {
                    extending = false;
                    ReverseLinkTargets();
                }
                other.gameObject.GetComponent<PlayerBehaviour>().AttachToPlayer( gameObject );
                attachedPlayers.Add( other.gameObject );
				Debug.Log( "fdsfds" );
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
			
			// Remove the parent transform of the hook head. It is no longer directly connected to it.
            player             = transform.parent.gameObject;
			transform.parent   = null;
			
			// Add the head as a link.
			links.Add( gameObject );

			firing = true;
		}
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
    


	private List<GameObject>	links 			= new List<GameObject>();
    private List<GameObject>    attachedPlayers = new List<GameObject>();
	private bool				extending 		= true;
	private bool				firing			= false;
    private GameObject          player          = null;



	//
	// ReverseLinkTargets
	// Just set each link's target to the link before it. Set the last link's
	// target to the origin.
	//
	private void ReverseLinkTargets()
	{
		for( int i = 0; i < links.Count - 1; i++ )
		{
			links[i].GetComponent<HookLinkBehaviour>().target = links[i + 1].transform;
		}
		links[links.Count - 1].GetComponent<HookLinkBehaviour>().target = originTransform;
    }



	//
	// PushLink
	// Add a new link to the back of the hook.
	//
	private void PushLink()
	{
		GameObject		newLink = Network.Instantiate( linkPrefab, originTransform.position, originTransform.rotation, 0 ) as GameObject;
		newLink.GetComponent<HookLinkBehaviour>().target = links[links.Count - 1].transform;
		newLink.GetComponent<HookLinkBehaviour>().speed = speed;
		links.Add( newLink );
    }



	//
	// PopLink
	// Remove a link from the back of the hook.
	//
	private void PopLink()
	{
		Network.Destroy( links[links.Count - 1].networkView.viewID );
		links.RemoveAt( links.Count - 1 );
		links[links.Count - 1].GetComponent<HookLinkBehaviour>().target = originTransform;
    }



    //
    // EndHook
    //
    private void EndHook()
    {
        extending = true;
        links.RemoveAt( links.Count - 1 );
        transform.parent = originTransform.parent;
        transform.position = originTransform.position;
        firing = false;

        // Remove all attachments.
        foreach( GameObject attachments in attachedPlayers )
        {
            attachments.GetComponent<PlayerBehaviour>().DetachFromPlayer( gameObject );
        }
    }
}
