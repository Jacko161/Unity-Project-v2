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
		base.OnGUI();
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
    public override void OnPlayerKill( GameObject killer, GameObject victim )
    {

    }



	//
	// OnPlayerDamage
	//
	public virtual void OnPlayerDamage( GameObject damager, GameObject victim )
	{

	}



	//
	// AddNewPlayer
	//
	public virtual void AddNewPlayer( GameObject newPlayer )
	{
		TDPlayerData	playerData = new TDPlayerData();

		playerData.player 			= newPlayer;
		playerData.maxHealth		= 100;
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
}

