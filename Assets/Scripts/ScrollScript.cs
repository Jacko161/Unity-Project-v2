using UnityEngine;
using System.Collections;

public class ScrollScript : MonoBehaviour 
{
	// Multiplyer for scrolling speed.
	public float 				ScrollSpeed = 1.0f;

	// Multiplyer for zoom speed.
	public float				zoomSpeed 	= 1.0f;

	// Fraction of the screen size area from the edge of the screen that will cause scrolling.
	public float 				ScrollFactor = 0.2f;




	//
	// Start
	// 
	void Start () 
	{

	}



	//
	// Update
	//
	void Update () 
	{
		if ( Input.mousePosition.x > ( Screen.width - ( Screen.width * ScrollFactor ) ) ) 
		{
			transform.Translate( Vector3.right * ScrollSpeed * Time.deltaTime, Space.World );
		}
		else if( Input.mousePosition.x < ( Screen.width * ScrollFactor ) )
		{
			transform.Translate( Vector3.left * ScrollSpeed * Time.deltaTime, Space.World );
		}

		if ( Input.mousePosition.y > ( Screen.height - ( Screen.height * ScrollFactor ) ) ) 
		{
			transform.Translate( Vector3.forward * ScrollSpeed * Time.deltaTime, Space.World );
		}
		else if( Input.mousePosition.y < ( Screen.height * ScrollFactor ) )
		{
			transform.Translate( Vector3.back * ScrollSpeed * Time.deltaTime, Space.World );
		}

		
		// Translate the transform towards the mouse.
		transform.Translate( Utility.DirectionToMousePosition( camera, transform.position ) * Input.GetAxis( "Mouse ScrollWheel" ) * zoomSpeed, Space.World );
	}
}
