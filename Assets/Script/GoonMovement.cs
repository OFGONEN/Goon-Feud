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

    // Delegates
    UnityMessage onPathComplete;
    RecycledSequence recycledSequence = new RecycledSequence();
#endregion

#region Properties
    public int PathIndex => path_index;
    public bool CanPath => path_index < path_points.Count;
#endregion

#region Unity API
#endregion

#region API
    // Info: Seriliazed Call for responding to event_stage_start
    public void OnStageStart()
    {
        // Add the player transform as a final path point
		var playerTransform = notif_player_transform.SharedValue as Transform;
		path_points.Add( playerTransform );
	}

    public void DoPath( UnityMessage pathComplete )
    {
		onPathComplete = pathComplete; // Cache the method

		notif_goon_path_count.SharedValue++; // Increase the pathing goon count by 1

		var targetTransform = path_points[ path_index ];

		var sequence = recycledSequence.Recycle();

		sequence.Append( transform.DORotate( targetTransform.rotation.eulerAngles,
			GameSettings.Instance.goon_movement_rotate_speed )
			.SetEase( Ease.Linear )
			.SetSpeedBased() );

		sequence.Join( transform.DOMove( targetTransform.position,
			GameSettings.Instance.goon_movement_move_speed )
			.SetEase( Ease.Linear )
			.SetSpeedBased() );

		sequence.OnComplete( OnPathComplete );
	}
#endregion

#region Implementation
    void OnPathComplete()
    {
		path_index++; // Increase path index since path is complete

        // Reduce the pathing goon count only if goon can path again
        // If goon can't path anymore, it means that the goon is reached the Player
        if( CanPath )
			notif_goon_path_count.SharedValue--;

		// Invoke the cached path complete method.
		onPathComplete();
	}
#endregion

#region Editor Only
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        for( var i = 0; i < path_points.Count; i++ )
        {
			// Draw Spheres on every path point
			Gizmos.DrawWireSphere( path_points[ i ].position, 0.15f );

			// Label Every path point
			Handles.Label( path_points[ i ].position, path_parent.name + ": " + i );
		}

        // Draw line between every point
        for( var i = 0; i < path_points.Count - 1; i++ )
        {
			Handles.DrawDottedLine( path_points[ i ].position, path_points[ i + 1 ].position, 10 );
		}
    }
#endif
#endregion
}
