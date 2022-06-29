/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FFStudio;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEditor;

public class GoonMovement : MovementPath
{
#region Fields
    [ BoxGroup( "Setup" ), SerializeField ] Transform movement_transform;
    [ BoxGroup( "Setup" ), SerializeField ] SharedIntNotifier notif_goon_path_count;
    [ BoxGroup( "Setup" ), SerializeField ] SharedReferenceNotifier notif_player_transform;

    [ ShowInInspector, ReadOnly ] int path_index;

	Transform player_transform;

	// Delegates
	UnityMessage onPathComplete;
    RecycledSequence recycledSequence = new RecycledSequence();
#endregion

#region Properties
    public int PathIndex => path_index;
    public int PathCount => path_points.Count;
    public bool CanPath => path_index < path_points.Count;
	public Vector3 Position => movement_transform.position;
	#endregion

	#region Unity API
	private void OnDisable()
	{
		onPathComplete = ExtensionMethods.EmptyMethod;
		recycledSequence.Kill();
	}
#endregion

#region API
    // Info: Seriliazed Call for responding to event_stage_start
    public void OnStageStart()
    {
        // Add the player transform as a final path point
		player_transform = notif_player_transform.SharedValue as Transform;
		path_points.Add( player_transform );
	}

    public void DoPath( UnityMessage pathComplete )
    {
		onPathComplete = pathComplete; // Cache the method
		notif_goon_path_count.SharedValue++; // Increase the pathing goon count by 1

		DoMovementPath( OnPathComplete );
	}

	public void DoPathLastPoint( UnityMessage pathComplete )
	{
		onPathComplete = pathComplete; // Cache the method
		path_index     = path_points.Count - 1;

		DoMovementPath( OnPathComplete );
	}

	public Vector3 GetPathPoint( int index )
	{
		return path_points[ index ].position;
	}
#endregion

#region Implementation
	void DoMovementPath( TweenCallback onComplete )
	{
		var position        = movement_transform.position;
		var targetPosition  = path_points[ path_index ].position;
		var targetDirection = ( targetPosition - position ).normalized;
		var targetRotation  = Vector3.up * Quaternion.LookRotation( targetDirection ).eulerAngles.y;

		var movementDuration = GameSettings.Instance.goon_movement_move_speed.ReturnDuration( Vector3.Distance( position, targetPosition ) );
		var rotationDuration = GameSettings.Instance.goon_movement_rotate_speed.ReturnDuration( Vector3.Angle( movement_transform.forward, targetDirection ) );

		if( path_index == path_points.Count - 1 )
			targetPosition -= targetDirection * GameSettings.Instance.goon_movement_lastPoint_distance;

		var sequence = recycledSequence.Recycle();

		sequence.Append( movement_transform.DOMove( targetPosition,
			movementDuration )
			.SetEase( Ease.Linear ) );

		sequence.Join( movement_transform.DORotate( targetRotation,
			rotationDuration )
			.SetEase( Ease.Linear ) );

		sequence.OnComplete( onComplete );
	}

    void OnPathComplete()
    {
		path_index++; // Increase path index since path is complete

        // Reduce the pathing goon count only if goon can path again
        // If goon can't path anymore, it means that the goon is reached the Player
        if( CanPath )
		{
			// Look at player after completing path
			var targetDirection  = player_transform.position - movement_transform.position;
			var targetRotation   = Vector3.up * Quaternion.LookRotation( targetDirection ).eulerAngles.y;
			var rotationDuration = GameSettings.Instance.goon_movement_rotate_speed.ReturnDuration( Vector3.Angle( movement_transform.forward, targetDirection ) );

			var sequence = recycledSequence.Recycle();

			sequence.Append( movement_transform.DORotate( targetRotation,
				rotationDuration )
				.SetEase( Ease.Linear ) );

			notif_goon_path_count.SharedValue--;
		}

		// Invoke the cached path complete method.
		onPathComplete();
	}
#endregion

#region Editor Only
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
		Gizmos.color  = Color.red;
		Handles.color = Color.red;

		var firstPosition = movement_transform.position;

		// Draw Spheres on every path point
		Gizmos.DrawWireSphere( firstPosition, 0.15f );

		// Label Every path point
		Handles.Label( firstPosition, path_parent.parent.name + ": Start" );

		for( var i = 0; i < path_points.Count; i++ )
        {
			// Draw Spheres on every path point
			Gizmos.DrawWireSphere( path_points[ i ].position, 0.1f );

			// Label Every path point
			Handles.Label( path_points[ i ].position, path_parent.parent.name + ": " + i );
		}

		// Draw line between every point
		for( var i = -1; i < path_points.Count - 1; i++ )
        {
			Handles.DrawDottedLine( firstPosition, path_points[ i + 1 ].position, 10 );
			firstPosition = path_points[ i + 1 ].position;
		}
    }
#endif
#endregion
}
