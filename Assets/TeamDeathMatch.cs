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
	// ClientGameState
	//
	public enum ClientGameState
	{
		playing,
		dying,
		respawning,
		shopping,
		menu,
	}



	//
	// TDPlayerData
	//
	public class TDPlayerData : BasePlayerData
	{
		public float			maxHealth;
		public float			currentHealth;
		public float			gold;
		 
		public Team				team;
		 
		public int				kills;
		public int				deaths;
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
	}



	//
	// OnGUI
	// Draw team deathmatch specific GUI.
	//
	public void OnGUI()
	{
		if( gameState == ClientGameState.playing )
		{
			base.OnGUI();
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
		var		playerData	= ( TDPlayerData )FindDataFromPlayer( victim );

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
		playerData.player.GetComponent<PlayerServerBehaviour>().KillPlayer();

		// Switch the gamestate to dying.
		gameState = ClientGameState.dying;
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
			var		playerData	= ( TDPlayerData )FindDataFromPlayer( victim );
			playerData.currentHealth -= hookDamage;

			if( playerData.currentHealth <= 0.0f )
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
		TDPlayerData	playerData = new TDPlayerData();

		playerData.player 			= newPlayer;
		playerData.maxHealth		= initialHealth;
		playerData.currentHealth	= playerData.maxHealth;
		playerData.gold				= 0;
		playerData.kills			= 0;
		playerData.deaths			= 0;

		// Will need to add team selection.
		if( teamOne.count > teamTwo.count )
		{
			playerData.team = teamTwo;
		}
		else
		{
			playerData.team = teamOne;
		}

		players.Add( playerData );
	}





	private	int			targetKills 		= 10;
	private int			timeLimitSeconds	= 0;
	private Team		teamOne;
	private Team		teamTwo;


	// TD constants.
	private const float	initialHealth		= 100.0f;
	private const float	hookDamage			= 50.0f;
	private const float	meleeDamage			= 20.0f;

	// Client parameters.
	ClientGameState		gameState			= ClientGameState.playing;
}

