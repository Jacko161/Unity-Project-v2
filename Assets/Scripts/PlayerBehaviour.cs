using UnityEngine;
using System.Collections;

public class PlayerBehaviour : MonoBehaviour 
{
	public GameObject				hookHead;

    // The gameObject of any hookhead that may be attached to this player. If null, there is
    // no attachment. If not null and another hook trys to attach itself, the player dies.
    public GameObject               attachment;



	//
	// OnSerializeNetworkView
	//
	void OnSerializeNetworkView( BitStream stream, NetworkMessageInfo info ) 
	{
		Vector3			syncPosition	= Vector3.zero;	
		Quaternion		syncRotation	= Quaternion.identity;
		if( stream.isWriting )
		{
			syncPosition = transform.position;
			syncRotation = transform.rotation;
			stream.Serialize( ref syncPosition );
			stream.Serialize( ref syncRotation );
		}
		else
		{
			stream.Serialize( ref syncPosition );
			stream.Serialize( ref syncRotation );
			transform.position = syncPosition;
			transform.rotation = syncRotation;
		}
	}



	//
	// Start
	//
	void Start () 
	{
		hookOrigin 			= new GameObject( "HookOrigin" );
		agent      			= GetComponent<NavMeshAgent>();
	}



	//
	// Update
	// 
	void Update () 
	{
		// Specific to a single client. Invokes RPC methods.
		if( Network.player == clientPlayerId )
		{
			if( Input.GetMouseButtonDown( 1 ) && !hookHead.GetComponent<HookHeadBehvaiour>().IsFiring )
			{
				Vector3 direction = Utility.DirectionToMousePosition( Camera.main, hookHead.transform.position );

				if( Network.isServer )
				{
					FireHook( direction );
				}
				else
				{
					networkView.RPC( "FireHook", RPCMode.Server, direction );
				}
			}
			if( Input.GetMouseButtonDown( 0 ) )
			{
				if( Network.isServer )
				{
					SetAgentDestination( ( Camera.main.ScreenToWorldPoint( new Vector3( Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane ) ) - Camera.main.transform.position ).normalized, Camera.main.transform.position );
				}
				else
				{
					networkView.RPC( "SetAgentDestination", RPCMode.Server, ( Camera.main.ScreenToWorldPoint( new Vector3( Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane ) ) - Camera.main.transform.position ).normalized, Camera.main.transform.position );	
				}
			}
		}
		if( attachment != null )
		{
			if( Network.isServer )
			{
				MoveWithAttachment();
			}
			else
			{
				networkView.RPC( "MoveWithAttachment", RPCMode.Server );
			}
		}
	}



	//
	// OnTriggerEnter
	//
	void OnTriggerEnter( Collider other )
	{
		if( Network.isServer )
		{
			if( other.tag == "PlayerBoundry" && other.transform.parent.gameObject.GetComponent<PlayerBehaviour>().hookHead == attachment )
			{
				DetachFromPlayer( attachment );
			}
		}
	}



	//
	// OnTriggerStay
	//
	void OnTriggerStay( Collider other )
	{
		if( Network.isServer )
		{
			if( other.tag == "PlayerBoundry" && other.transform.parent.gameObject.GetComponent<PlayerBehaviour>().hookHead == attachment )
			{
				DetachFromPlayer( attachment );
			}
		}
	}



	//
	// OnCollisionEnter
	//
	void OnCollisionEnter( Collision collision )
	{
		if( Network.isServer )
		{
			if( collision.gameObject.tag == "HookHead" && collision.gameObject != hookHead && collision.gameObject.GetComponent<HookHeadBehvaiour>().IsFiring )
			{
				AttachToPlayer( collision.gameObject );
			}
		}
	}



	//
	// SetPlayerID
	//
	public void SetPlayerID( NetworkPlayer id )
	{
		networkView.RPC( "SetNetworkPlayerID", RPCMode.All, id );
	}



    //
    // AttachToPlayer
    // Returns true if the attachment was successful.
    //
    public bool AttachToPlayer( GameObject otherHook )
    {
        if( attachment == null )
        {
            attachment = otherHook;
			agent.enabled = false;
            return true;
        }
        return false;
    }



    //
    // DetachFromPlayer
    //
    public void DetachFromPlayer( GameObject otherHook )
    {
		if( otherHook == attachment )
        {
            attachment = null;
			agent.enabled = true;
			agent.SetDestination( transform.position );
        }
    }



	//
	// FireHook
	//
	[RPC] private void FireHook( Vector3 direction )
	{
		networkView.RPC( "SetHookOrigin", RPCMode.All );

		hookHead.transform.forward = new Vector3( direction.x, hookHead.transform.forward.y, direction.z );
		hookHead.GetComponent<HookHeadBehvaiour>().FireHook();
	}



	//
	// SetHookOrigin
	//
	[RPC] private void SetHookOrigin()
	{
		hookOrigin.transform.parent = transform;
		hookOrigin.transform.localPosition = hookHead.transform.localPosition;
		hookOrigin.transform.localRotation = hookHead.transform.localRotation;
		hookHead.GetComponent<HookHeadBehvaiour>().originTransform = hookOrigin.transform;
	}



	//
	// SetAgentDestination
	//
	[RPC] private void SetAgentDestination( Vector3 direction, Vector3 cameraPosition )
	{	
		Ray			ray = new Ray( cameraPosition, direction );
		RaycastHit 	hit;
		Physics.Raycast( ray, out hit );

		if ( hit.collider != null && hit.collider.tag == "Level" )
		{
			agent.SetDestination( hit.point );
		}
	}



	//
	// MoveWithAttachment
	//
	[RPC] private void MoveWithAttachment()
	{			
		// Move with the attached grapple.
		Vector3         normalToTarget = ( attachment.transform.position - transform.position ).normalized;
		transform.Translate( normalToTarget * hookHead.GetComponent<HookHeadBehvaiour>().speed * Time.deltaTime, Space.World );
	}



	//
	// SetPlayerNetworkID
	//
	[RPC] private void SetNetworkPlayerID( NetworkPlayer id )
	{
		clientPlayerId = id;
	}
	

	private GameObject				hookOrigin = null;
	private NavMeshAgent			agent	   = null;
	private NetworkPlayer			clientPlayerId;

}
