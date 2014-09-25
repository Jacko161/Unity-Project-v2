using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TeamDeathMatch : GameTypeManager
{



	//
	// Initialise
	//
	public void Initialise( int neededKills, string teamOne, string teamTwo, int timeSeconds )
	{
		base.Initialise();

		targetKills 		= neededKills;
		teamOneName 		= teamOne;
		teamTwoName			= teamTwo;
		timeLimitSeconds 	= timeSeconds;
	}



	//
	// OnGUI
	// Draw team deathmatch specific GUI.
	//
	void OnGUI()
	{
		base.OnGUI();
	}





	private	int			targetKills 		= 10;
	private string		teamOneName			= "";
	private string		teamTwoName			= "";
	private int			timeLimitSeconds	= 0;
}

