using UnityEngine;
using System.Collections;

public class PlayerServerBehaviour : MonoBehaviour 
{
    

    
    public GameObject				hookHead;
	public string					gameControllerTag		= "GameController";

	// The gameObject of any hookhead that may be attached to this player. If null, there is
	// no attachment. If not null and another hook trys to attach itself, the player dies.
	public GameObject               attachment;



	//
	// FireHook
	//
	[RPC] public void FireHook( Vector3 direction )
	{
        if( !hookScript.IsFiring )
        {
            hookHead.transform.forward = new Vector3( direction.x, hookHead.transform.forward.y, direction.z );
			hookScript.FireHook();

			networkView.RPC( "SetState", RPCMode.All, ( int )( clientScript.GetState() | PlayerClientBehaviour.State.firing ) );
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
		hookScript			= hookHead.GetComponent<HookHeadServerBehaviour>();
		clientScript		= GetComponent<PlayerClientBehaviour>();

		// Try to find the game manager script. If we can't find one, null will flag that there are no game rules
		// to follow.
		gameTypeScript 		= GameObject.FindGameObjectWithTag( gameControllerTag ).GetComponent<GameTypeManager>();
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
		if( Input.GetKeyDown( KeyCode.U ) )
		{
			hookScript.StartFade();
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
	private PlayerClientBehaviour	clientScript;
	private HookHeadServerBehaviour	hookScript;
	private	GameTypeManager			gameTypeScript	= null;

	
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

			networkView.RPC( "SetState", RPCMode.All, ( int )( clientScript.GetState() | PlayerClientBehaviour.State.attached ) );
			return true;
		}
		else
		{
			KillPlayer();

			// Play specific death animation.
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

			// Remove the attached flag.
			networkView.RPC( "SetState", RPCMode.All, ( int )( clientScript.GetState() & ~PlayerClientBehaviour.State.attached ) );
		}
	}



	//
	// KillPlayer
	//
	public void KillPlayer()
	{
		// Reset state to JUST dying.
		networkView.RPC( "SetState", RPCMode.All, ( int )PlayerClientBehaviour.State.dying );
		
		if( hookScript.IsFiring )
		{
			// We've died while shooting the hook. Make it stop and
			// fade out.
			hookScript.StartFade();
        }
    }
}
