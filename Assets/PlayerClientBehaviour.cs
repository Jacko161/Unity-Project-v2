using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerClientBehaviour : MonoBehaviour 
{



	//
	// State
	// If state == 0, then it is idle. That is, if no other flag is
	// set, the state is idle.
	//
	[System.Flags]
	public enum State
	{
		idle			= 0,
		walking			= 1,
		attached		= 2,
		firing			= 4,
		swinging		= 8,
		dying			= 16,
		dead			= 32,
	}



	//
	// OnSerializeNetworkView
	//
	void OnSerializeNetworkView( BitStream stream, NetworkMessageInfo info ) 
	{
		Vector3			syncPosition	= Vector3.zero;	
		Quaternion		syncRotation	= Quaternion.identity;
		NetworkPlayer	syncPlayer		= Network.player;

		if( stream.isWriting )
		{
			syncPosition = transform.position;
			syncRotation = transform.rotation;
			syncPlayer	 = clientPlayerId;
			stream.Serialize( ref syncPosition );
			stream.Serialize( ref syncRotation );
			stream.Serialize( ref syncPlayer );
		}
		else
		{
			stream.Serialize( ref syncPosition );
			stream.Serialize( ref syncRotation );
			stream.Serialize( ref syncPlayer );
			transform.position = syncPosition;
			transform.rotation = syncRotation;
			clientPlayerId	   = syncPlayer;
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
				Vector3 mousePosition = Utility.GetMousePosition( Camera.main );
				
				if( Network.isServer )
				{
					serverScript.FireHook( ( mousePosition - hookHead.transform.position ).normalized );
				}
				else
				{
					networkView.RPC( "FireHook", RPCMode.Server, ( mousePosition - hookHead.transform.position ).normalized );
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
		clientPlayerId = id;
    }



	//
	// GetPlayerID
	//
	public NetworkPlayer GetPlayerID()
	{
		return clientPlayerId;
	}



	//
	// GetGameData
	//
	public object GetGameData( string name )
	{
		return gameData[name];
	}



	//
	// SetGameData
	//
	public void SetGameData( string name, object newValue )
	{
		networkView.RPC( "SetGameDataOnAll", RPCMode.All, name, Utility.ObjectToByteArray( newValue ) );
	}



	//
	// SetState
	//
	[RPC] public void SetState( int newState )
	{
		state = ( State )newState;
	}



	//
	// GetState
	//
	public State GetState()
	{
		return state;
	}
    
    
    
    private NetworkPlayer					clientPlayerId;
    private PlayerServerBehaviour			serverScript;
    private GameObject						hookHead;

	// We'll hold game mode specific data in this. It will only be accessed through
	// the game controller, which will know which type it is. It will not be synced,
	// but will be changed when needed.
	private Dictionary<string, object>		gameData		= new Dictionary<string, object>();
	private State							state			= State.idle;
    


	//
	// SetGameDataOnAll
	//
	[RPC] private void SetGameDataOnAll( string name, byte[] data )
	{
		gameData[name] = Utility.ByteArrayToObject( data );
	}
}
