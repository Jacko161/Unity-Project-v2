using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TeamDeathMatch : GameTypeManager
{



	//
	// Team
	//
	public struct Team
	{
		public 		int 		score;
		public 		int			count;
		public		string 		Name;
	}



	//
	// Initialise
	//
	public void Initialise( int neededKills, string teamOneName, string teamTwoName, int timeSeconds, int maxConnections, int maxPlayers, string roomName, bool isServerPlayer )
	{
		base.Initialise( maxConnections, maxPlayers, roomName, isServerPlayer );

		targetKills 		= neededKills;
		teamOne.Name 		= teamOneName;
		teamTwo.Name		= teamTwoName;
		timeLimitSeconds 	= timeSeconds;

		// Sync all the score attributes to the clients. This will be buffered so future clients will still run
		// this upon joining.
		networkView.RPC( "SyncScores", RPCMode.AllBuffered, teamOne.Name, teamOne.score, teamOne.count, teamTwo.Name, teamTwo.score, teamTwo.count );
	}



	//
	// OnGUI
	// Draw team deathmatch specific GUI.
	//
	public void OnGUI()
	{
		base.OnGUI();

		// Find player.
		GameObject 			player = Utility.FindThisPlayer();

		if( player != null )
		{
			var 		playerScript = player.GetComponent<PlayerClientBehaviour>();

			// If the player is not dead or dying.
			if( ( int )( playerScript.GetState() & ( PlayerClientBehaviour.State.dying | PlayerClientBehaviour.State.dead ) ) == 0 )
			{
				GUI.Box(new Rect(100, Screen.height - 100, 50, 50), "$");
				GUI.Box(new Rect(Screen.width / 2, Screen.height - 150, 50, 50), "HP: " + playerScript.GetGameData( currentHealth ) );
				GUI.Box(new Rect(Screen.width / 4, Screen.height - 100, 50, 50), "CD");
				GUI.Box(new Rect(Screen.width / 2, Screen.height - 75, 50, 50), "Mana");
				
				GUI.Box(new Rect(Screen.width - 250, Screen.height - 250, 250, 250), "Map");
				
				GUI.Box(new Rect(Screen.width / 2 - 50, Screen.height - (Screen.height * 0.99f), 250, 50), "Scores:\n" + teamOne.Name + ": " + teamOne.score + "\n" + teamTwo.Name + ": " + teamTwo.score );
				GUI.Box(new Rect(Screen.width / 2 - 50, Screen.height - (Screen.height * 0.99f) + 50, 50, 50), "Timer");
				
				GUI.Button(new Rect(50, Screen.height - 200, 100, 50), "Shop");
			}
			// If the player is dying.
			else if( ( playerScript.GetState() & PlayerClientBehaviour.State.dying ) == PlayerClientBehaviour.State.dying )
			{
				GUI.Box( new Rect( 300, 300, 200, 200 ), "Dying" );
			}
		}
	}



    //
    // Update
    //
    public void Update()
    {

    }



	//
	// OnPlayerKill
	//
	public override void OnPlayerKill( GameObject killer, GameObject victim, GameObject weapon, DeathType type )
	{
		// Do specific things based on how the player died.
		if( type == DeathType.torn )
		{
			// Torn animation.
		}
		else if( type == DeathType.meleed )
		{
			// Meleed animation.
		}
		else if( type == DeathType.hooked )
		{
			// Hooked animation.
		}

		// Kill the player.
		victim.GetComponent<PlayerServerBehaviour>().KillPlayer();

		// Increase the killer's team's score.
		networkView.RPC( "UpdateScore", RPCMode.All, killer.GetComponent<PlayerClientBehaviour>().GetGameData( team ), 1 );
	}
	
	
	
	//
	// OnPlayerRespawn
	//
	public override void OnPlayerRespawn( GameObject player )
	{
		
	}

	
	
	
	//
	// OnPlayerDamage
	//
	public override void OnPlayerDamage( GameObject damager, GameObject victim, GameObject weapon )
	{
		if( weapon.tag == "HookHead" )
		{
			var			playerScript = victim.GetComponent<PlayerClientBehaviour>();

			playerScript.SetGameData( currentHealth, ( float )( playerScript.GetGameData( currentHealth ) ) - hookDamage );

			if( ( float )( playerScript.GetGameData( currentHealth ) ) <= 0.0f )
			{
				OnPlayerKill( damager, victim, weapon, DeathType.hooked );
			}
		}
	}
	
	
	
	//
	// OnPlayerAttachToHook
	//
	public override void OnPlayerAttachToHook( GameObject attacher, GameObject attachee, GameObject hookHead )
	{
		
	}
	
	
	
	//
	// OnPlayerDetachFromHook
	//
	public override void OnPlayeDetachFromHook( GameObject attacher, GameObject attachee, GameObject hookHead )
	{
		
	}
	
	
	
	//
	// OnGameFinish
	//
	public override void OnGameFinish()
	{
		
	}



	//
	// AddNewPlayer
	//
	public override void AddNewPlayer( GameObject newPlayer )
	{
		PlayerClientBehaviour		playerScript = newPlayer.GetComponent<PlayerClientBehaviour>();

		playerScript.SetGameData( maxHealth, initialHealth );
		playerScript.SetGameData( currentHealth, initialHealth );
		playerScript.SetGameData( gold, 0 );
		playerScript.SetGameData( kills, 0 );
		playerScript.SetGameData( deaths, 0 );
		
		// Will need to add team selection.
		if( teamOne.count <= teamTwo.count )
		{
			// Set the player team and update team count on all clients.
			playerScript.SetGameData( team, teamOne.Name );
			networkView.RPC( "UpdateCount", RPCMode.All, teamOne.Name, 1 );
		}
		else
		{
			playerScript.SetGameData( team, teamTwo.Name );
			networkView.RPC( "UpdateCount", RPCMode.All, teamTwo.Name, 1 );
		}
	}




	private	int			targetKills 		= 10;
	private int			timeLimitSeconds	= 0;
	private Team		teamOne;
	private Team		teamTwo;


	// TD constants.
	private const float	initialHealth		= 100.0f;
	private const float	hookDamage			= 50.0f;
	private const float	meleeDamage			= 20.0f;

	private const string maxHealth			= "maxHealth";
	private const string currentHealth		= "currentHealth";
	private const string gold				= "gold";
	private const string kills				= "kills";
	private const string deaths				= "deaths";
	private const string team				= "team";



	//
	// UpdateScore
	// Update the score across all clients.
	//
	[RPC] private void UpdateScore( string teamName, int delta )
	{
		if( teamName == teamOne.Name )
		{
			teamOne.score += delta;
		}
		else if( teamName == teamTwo.Name )
		{
			teamTwo.score += delta;
		}
	}


	//
	// UpdateCount
	//
	[RPC] private void UpdateCount( string teamName, int delta )
	{
		if( teamName == teamOne.Name )
		{
			teamOne.count += delta;
		}
		else if( teamName == teamTwo.Name )
		{
			teamTwo.count += delta;
		}
	}



	//
	// SyncScores
	// Completely syncs scores across all clients.
	//
	[RPC] private void SyncScores( string teamOneName, int teamOneScore, int teamOneCount, string teamTwoName, int teamTwoScore, int teamTwoCount )
	{
		teamOne.Name 			= teamOneName;
		teamOne.score 			= teamOneScore;
		teamOne.count 			= teamOneCount;

		teamTwo.Name			= teamTwoName;
		teamTwo.score			= teamTwoScore;
		teamTwo.count			= teamTwoCount;
	}



}

