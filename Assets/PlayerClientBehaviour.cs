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
    //Player Hud
    //
    public void OnGUI()
    {
        GUI.Box(new Rect(100, Screen.height - 100, 50, 50), "$");
        GUI.Box(new Rect(Screen.width / 2, Screen.height - 150, 50, 50), "HP");
        GUI.Box(new Rect(Screen.width / 4, Screen.height - 100, 50, 50), "CD");
        GUI.Box(new Rect(Screen.width / 2, Screen.height - 75, 50, 50), "Mana");

        GUI.Box(new Rect(Screen.width - 250, Screen.height - 250, 250, 250), "Map");

        GUI.Box(new Rect(Screen.width / 2 - 50, Screen.height - (Screen.height * 0.99f), 50, 50), "Scores");
        GUI.Box(new Rect(Screen.width / 2 - 50, Screen.height - (Screen.height * 0.99f) + 50, 50, 50), "Timer");

        GUI.Button(new Rect(50, Screen.height - 200, 100, 50), "Shop");
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
