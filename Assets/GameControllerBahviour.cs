using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameControllerBahviour : MonoBehaviour 
{



	//
	// StartTeamDeathMatch
	//
	public void StartTeamDeathMatch( int neededKills, string teamOne, string teamTwo, int timeSeconds  )
	{
		manager = gameObject.AddComponent<TeamDeathMatch>();
		( ( TeamDeathMatch )manager ).Initialise( neededKills, teamOne, teamTwo, timeSeconds );
	}



	private GameTypeManager		manager;
}
