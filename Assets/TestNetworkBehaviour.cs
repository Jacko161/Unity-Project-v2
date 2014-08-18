using UnityEngine;
using System.Collections;

public class TestNetworkBehaviour : MonoBehaviour 
{


	public GameObject			playerPrefab;



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
	
	}



	//
	// OnServerInitialized
	//
	void OnServerInitialized()
	{
		CreateNewPlayerOnServer(  Network.player );
	}



	//
	// OnConnectedToServer
	//
	void OnConnectedToServer()
	{
		networkView.RPC( "CreateNewPlayerOnServer", RPCMode.Server, Network.player );
	}


	
	//
	// OnMasterServerEvent
	//
	void OnMasterServerEvent( MasterServerEvent msEvent )
	{
		if( msEvent == MasterServerEvent.HostListReceived )
		{
			hostList = MasterServer.PollHostList();
		}
	}



	//
	// OnGUI
	//
	void OnGUI()
	{
		if( !Network.isClient && !Network.isServer )
		{
			if( GUI.Button( new Rect( 100, 100, 250, 100 ), "Start Server" ) )
			{
				StartServer();
			}
			if( GUI.Button( new Rect( 100, 200, 250, 100 ), "Refresh Server List" ) )
			{
				RefreshHostList();
			}
			if( hostList != null )
			{
				for( int i = 0; i < hostList.Length; i++ )
				{
					if( GUI.Button( new Rect( 400, 100 + ( 110 * i ), 300, 100 ), hostList[i].gameName ) )
					{
						JoinServer( hostList[i] );
					}
				}
			}
		}
	}



	private const string 			typeName 	= "PWNetworkTest";
	private const string			gameName	= "RoomName";

	private int	  					connections	= 4;
	private int						listenPort	= 25000;
	private bool					useNat		= true;

	private HostData[]				hostList	= null;



	//
	// StartServer
	//
	private void StartServer()
	{
		Network.InitializeServer( connections, listenPort, useNat );
		MasterServer.RegisterHost( typeName, gameName );
	}



	//
	// RefreshHostList
	//
	private void RefreshHostList()
	{
		MasterServer.RequestHostList( typeName );
	}



	//
	// JoinServer
	//
	private void JoinServer( HostData hostData )
	{
		Network.Connect( hostData );
	}



	//
	// CreateNewPlayerOnServer
	//
	[RPC] private void CreateNewPlayerOnServer( NetworkPlayer id )
	{
		GameObject newPlayer = Network.Instantiate( playerPrefab, Vector3.zero, Quaternion.identity, 0 ) as GameObject;
		newPlayer.GetComponent<PlayerBehaviour>().SetPlayerID( id );
	}
	

}
