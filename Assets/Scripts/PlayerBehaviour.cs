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
		else if( stream.isReading )
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
		target 				= gameObject.transform.position;
	}



	//
	// Update
	// 
	void Update () 
	{
		if( Input.GetMouseButtonDown( 1 ) && networkView.isMine )
		{
			if( !hookHead.GetComponent<HookHeadBehvaiour>().IsFiring )
			{
				hookOrigin.transform.parent = transform;
				hookOrigin.transform.localPosition = hookHead.transform.localPosition;
				hookOrigin.transform.localRotation = hookHead.transform.localRotation;

				hookHead.GetComponent<HookHeadBehvaiour>().originTransform = hookOrigin.transform;

				Vector3 direction = Utility.DirectionToMousePosition( Camera.main, hookHead.transform.position );
				hookHead.transform.forward = new Vector3( direction.x, hookHead.transform.forward.y, direction.z );
				hookHead.GetComponent<HookHeadBehvaiour>().FireHook();
			}
		}
        if( attachment != null )
        {
            // Move with the attached grapple.
            Vector3         normalToTarget = ( attachment.transform.position - transform.position ).normalized;
            transform.Translate( normalToTarget * hookHead.GetComponent<HookHeadBehvaiour>().speed * Time.deltaTime, Space.World );
        }
		if( agent.enabled && networkView.isMine )
		{
			agent.SetDestination( target );
			if ( Input.GetMouseButtonDown( 0 ) )
			{
				Move();
			}
		}
	}



	//
	// OnTriggerEnter
	//
	void OnTriggerEnter( Collider other )
	{
		if( other.tag == "PlayerBoundry" && other.transform.parent.gameObject.GetComponent<PlayerBehaviour>().hookHead == attachment  )
		{
			DetachFromPlayer( attachment );
		}
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
			target = transform.position;
        }
    }


	private GameObject				hookOrigin = null;
	private NavMeshAgent			agent	   = null;
	private Vector3 				target;



	//
	// Get the point of impact of the ray and move to it.
	//
	private void Move()
	{
		RaycastHit 	hit;
		// set ray conditions to from camera to mouse
		Ray 		ray 	= Camera.main.ScreenPointToRay( Input.mousePosition );
		
		Physics.Raycast( ray, out hit );
		
		if ( hit.collider != null && hit.collider.tag == "Level" )
		{
			target 			= hit.point;
		}
	}
}
