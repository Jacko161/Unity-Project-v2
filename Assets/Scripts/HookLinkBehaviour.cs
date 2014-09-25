using UnityEngine;
using System.Collections;

public class HookLinkBehaviour : MonoBehaviour 
{
    public static float         gap             = 0.8f;



	// The link will follow it's target.
	public HookLinkBehaviour	target;
	public GameObject			frontPivot;
	public GameObject			backPivot;
	public float				speed			= 0.0f;



	//
	// Start
	//
	void Start () 
	{
		currentPivot = backPivot;
	}



	//
	// Update
	//
	void Update () 
	{
		// If the target is null, just move the link towards it's forward vector. This is intended
		// for the head link only. If it's not null, follow the target.
		if( target != null )
		{
			Transform				targetPivot = null;
			if( frontPivot == null && target.backPivot != null )
			{
				targetPivot = target.backPivot.transform;
			}
			else
			{
				targetPivot = target.CurrentPivot.transform;
			}

			// Get the direction of this link to it's target, and translate.
			Vector3			normalToTarget = ( targetPivot.position - CurrentPivot.transform.position ).normalized;

			Quaternion		look		   = Quaternion.FromToRotation( transform.forward, IsExtending ? normalToTarget : -normalToTarget );

			Vector3			axis		   = Vector3.zero;
			float			angle		   = 0.0f;
			look.ToAngleAxis( out angle, out axis );

			transform.RotateAround( CurrentPivot.transform.position, axis, angle );

			if( Vector3.Distance( targetPivot.position, CurrentPivot.transform.position ) > gap )
			{
				transform.Translate( normalToTarget * speed * Time.deltaTime, Space.World );
			}
		}
		else if( backPivot != null )
		{
			transform.Translate( Vector3.forward * speed * Time.deltaTime );
		}
	}



    //
    // IsExtending
    //
    public bool IsExtending
    {
        get
        {
            return isExtending;
        }
        set
        {
            isExtending = value;

			if( isExtending )
			{
				currentPivot = backPivot;
			}
			else if( frontPivot != null )
			{
				currentPivot = frontPivot;
			}
        }
    }



	//
	// CurrentPivot
	//
	public GameObject CurrentPivot
	{
		get
		{
			if( currentPivot == null )
			{
				return gameObject;
			}
			return currentPivot;
		}
	}



	private bool                 isExtending       = true;
	private	GameObject			 currentPivot	   = null;
}
