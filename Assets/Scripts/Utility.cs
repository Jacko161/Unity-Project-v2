using UnityEngine;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class Utility 
{



	//
	// DirectionToMousePosition
	//
	static public Vector3 DirectionToMousePosition( Camera camera, Vector3 from )
	{
		Vector3			screenPoint = camera.ScreenToWorldPoint( new Vector3( Input.mousePosition.x, Input.mousePosition.y, camera.nearClipPlane ) );
		return ( screenPoint - from ).normalized;
	}



	//
	// GetMousePosition
	//
	static public Vector3 GetMousePosition( Camera camera )
	{
		RaycastHit 		hit;
		Ray 			cast = camera.ScreenPointToRay( Input.mousePosition );
		Physics.Raycast( cast, out hit );
		return hit.point;
	}



	//
	// ObjectToByteArray
	//
	static public byte[] ObjectToByteArray( object data )
	{
		if( data == null )
		{
			return null;
		}
		
		BinaryFormatter binaryFormatter = new BinaryFormatter();
		MemoryStream 	memoryStream	= new MemoryStream();
		
		binaryFormatter.Serialize( memoryStream, data );
		return memoryStream.ToArray();
	}



	//
	// ByteArrayToObject
	//
	static public object ByteArrayToObject( byte[] data )
	{
		MemoryStream		memoryStream 		= new MemoryStream();
		BinaryFormatter		binaryFormatter		= new BinaryFormatter();

		memoryStream.Write( data, 0, data.Length );
		memoryStream.Seek( 0, SeekOrigin.Begin );
		return ( object )binaryFormatter.Deserialize( memoryStream );
	}



	//
	// FindThisPlayer
	//
	static public GameObject FindThisPlayer()
	{
		var players = GameObject.FindGameObjectsWithTag( "Player" );

		foreach( GameObject player in players )
		{
			if( player.GetComponent<PlayerClientBehaviour>().GetPlayerID() == Network.player )
			{
				return player;
			}
		}

		return null;
	}
}
