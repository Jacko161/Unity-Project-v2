using UnityEngine;
using System.Collections;

public class Utility 
{



	//
	// DirectionToMousePosition
	//
	static public Vector3 DirectionToMousePosition( Camera camera, Vector3 from )
	{
		RaycastHit 		hit;
		Ray 			cast = camera.ScreenPointToRay( Input.mousePosition );
		Physics.Raycast( cast, out hit );
		return ( hit.point - from ).normalized;
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
}
