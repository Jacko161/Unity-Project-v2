using UnityEngine;
using System.Collections;
using System.Collections.Generic;


// Only the server runs this code. All effects of this code
// will be sync to the clients.
public class GameTypeManager : MonoBehaviour 
{



	//
	// Initialise
	//
	public void Initialise()
	{
	}



	//
	// OnGUI
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
	
}
