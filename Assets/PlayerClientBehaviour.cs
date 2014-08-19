using UnityEngine;
using System.Collections;

public class PlayerClientBehaviour : MonoBehaviour 
{



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
		serverScript = GetComponent<PlayerServerBehaviour>();
		hookHead     = serverScript.hookHead;
	}
	
	
	
	//
	// Update
	// 
	void Update () 
	{
		// Specific to a single client. Invokes RPC methods.
		if( Network.player == clientPlayerId )
		{
			if( Input.GetMouseButtonDown( 1 ) )
			{
				Vector3 direction = Utility.DirectionToMousePosition( Camera.main, hookHead.transform.position );
				
				if( Network.isServer )
				{
					serverScript.FireHook( direction );
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
					serverScript.SetAgentDestination( ( Camera.main.ScreenToWorldPoint( new Vector3( Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane ) ) - Camera.main.transform.position ).normalized, Camera.main.transform.position );
				}
				else
				{
					networkView.RPC( "SetAgentDestination", RPCMode.Server, ( Camera.main.ScreenToWorldPoint( new Vector3( Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane ) ) - Camera.main.transform.position ).normalized, Camera.main.transform.position );	
                }
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
    
    
    
    private NetworkPlayer			clientPlayerId;
    private PlayerServerBehaviour	serverScript;
    private GameObject				hookHead;
    
    
    
    //
    // SetPlayerNetworkID
    //
    [RPC] private void SetNetworkPlayerID( NetworkPlayer id )
    {
        clientPlayerId = id;
    }

}
