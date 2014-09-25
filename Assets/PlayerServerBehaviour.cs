using UnityEngine;
using System.Collections;

public class PlayerServerBehaviour : MonoBehaviour 
{
	public GameObject				hookHead;
	
	// The gameObject of any hookhead that may be attached to this player. If null, there is
	// no attachment. If not null and another hook trys to attach itself, the player dies.
	public GameObject               attachment;



	//
	// FireHook
	//
	[RPC] public void FireHook( Vector3 direction )
	{
        if( !hookHead.GetComponent<HookHeadServerBehaviour>().IsFiring )
        {
            hookHead.transform.forward = new Vector3( direction.x, hookHead.transform.forward.y, direction.z );
		    hookHead.GetComponent<HookHeadServerBehaviour>().FireHook();
        }
	}

	
	
	//
	// SetAgentDestination
	//
	[RPC] public void SetAgentDestination( Vector3 direction, Vector3 cameraPosition )
	{	
		if( agent.enabled )
		{
			Ray			ray = new Ray( cameraPosition, direction );
			RaycastHit 	hit;
			Physics.Raycast( ray, out hit );
			
			if ( hit.collider != null && hit.collider.tag == "Level" )
			{
				agent.SetDestination( hit.point );
			}
		}
	}



	//
	// Start
	//
	void Start () 
	{
		agent      			= GetComponent<NavMeshAgent>();
	}
	


	//
	// Update
	//
	void Update () 
	{
		if( attachment != null )
		{
			MoveWithAttachment();
		}
	}



	//
	// OnTriggerEnter
	//
	void OnTriggerEnter( Collider other )
	{
		if( other.tag == "PlayerBoundry" && other.transform.parent.gameObject.GetComponent<PlayerServerBehaviour>().hookHead == attachment )
		{
			DetachFromPlayer( attachment );
		}
	}



	//
	// OnTriggerStay
	//
	void OnTriggerStay( Collider other )
	{
		if( other.tag == "PlayerBoundry" && other.transform.parent.gameObject.GetComponent<PlayerServerBehaviour>().hookHead == attachment )
		{
			DetachFromPlayer( attachment );
		}
	}



	//
	// OnCollisionEnter
	//
	void OnCollisionEnter( Collision collision )
	{
		if( collision.gameObject.tag == "HookHead" && collision.gameObject != hookHead && collision.gameObject.GetComponent<HookHeadServerBehaviour>().IsFiring )
		{
			AttachToPlayer( collision.gameObject );
		}
	}


	
	private NavMeshAgent			agent	   			= null;


	
	//
	// MoveWithAttachment
	//
	private void MoveWithAttachment()
	{			
		// Move with the attached grapple.
		Vector3         normalToTarget = ( attachment.transform.position - transform.position ).normalized;
		transform.Translate( normalToTarget * hookHead.GetComponent<HookHeadServerBehaviour>().speed * Time.deltaTime, Space.World );
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
}
