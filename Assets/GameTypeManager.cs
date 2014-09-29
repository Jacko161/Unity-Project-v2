using UnityEngine;
using System.Collections;
using System.Collections.Generic;


// Only the server runs this code. All effects of this code
// will be sync to the clients.
public class GameTypeManager : MonoBehaviour 
{



	//
	// ClientGameState
	//
	public enum GameState
	{
		playing		= 0,
		dying		= 1,
		respawning	= 2,
		shopping	= 3,
		menu		= 4,
	}



	//
	// Type
	//
	public enum Type
	{
		teamdeathMatch,
		freeForAll,
	}



	//
	// DeathType
	//
	public enum DeathType
	{
		torn,
		meleed,
		hooked,
	}




	//
	// Initialise
	//
	public void Initialise( int maxConnections, int maxPlayers, string roomName, bool isServerPlayer )
	{
		this.maxConnections = maxConnections;
		this.maxPlayers 	= maxPlayers;
		this.roomName		= roomName;
		this.isServerPlayer	= isServerPlayer;
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
	public virtual void OnPlayerKill( GameObject killer, GameObject victim, GameObject weapon, DeathType type )
    {

    }



    //
    // OnPlayerRespawn
    //
    public virtual void OnPlayerRespawn( GameObject player )
    {

    }



	//
	// OnPlayerDamage
	//
	public virtual void OnPlayerDamage( GameObject damager, GameObject victim, GameObject weapon )
	{

	}



	//
	// OnPlayerAttachToHook
	//
	public virtual void OnPlayerAttachToHook( GameObject attacher, GameObject attachee, GameObject hookHead )
	{

	}



	//
	// OnPlayerDetachFromHook
	//
	public virtual void OnPlayeDetachFromHook( GameObject attacher, GameObject attachee, GameObject hookHead )
	{

	}



    //
    // OnGameFinish
    //
    public virtual void OnGameFinish()
    {

    }



	//
	// AddNewPlayer
	//
	public virtual void AddNewPlayer( GameObject newPlayer )
	{

	}



	//
	// OnGUI
	//
	public void OnGUI()
	{

	}



	protected int							maxConnections		= 4;
	protected int							maxPlayers			= 4;
	protected string						roomName			= "";
	protected bool							isServerPlayer		= false;
}
