using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameControllerBahviour : MonoBehaviour 
{



	//
	// StartTeamDeathMatch
	//
	public void StartTeamDeathMatch( int neededKills, string teamOne, string teamTwo, int timeSeconds, int maxConnections, int maxPlayers, string roomName, bool isServerPlayer )
	{
		manager = gameObject.AddComponent<TeamDeathMatch>();
		( ( TeamDeathMatch )manager ).Initialise( neededKills, teamOne, teamTwo, timeSeconds, maxConnections, maxPlayers, roomName, isServerPlayer );
	}



	//
	// JoinTeamDeathMatch
	//
	public void JoinTeamDeathMatch()
	{
		gameObject.AddComponent<TeamDeathMatch>();
		manager = null;
	}



	//
	// Manager
	//
	public GameTypeManager Manager
	{
		get
		{
			return manager;
		}
	}


	private GameTypeManager		manager;
}
