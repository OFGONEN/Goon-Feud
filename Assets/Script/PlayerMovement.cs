/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FFStudio;
using DG.Tweening;
using Sirenix.OdinInspector;

public class PlayerMovement : MonoBehaviour
{
#region Fields
    [ BoxGroup( "Setup" ), SerializeField ] SetPath set_path;
    [ BoxGroup( "Setup" ), SerializeField ] Transform movement_transform;
    [ BoxGroup( "Setup" ), SerializeField ] SharedFloatNotifier notif_level_progress;

	[ ShowInInspector, ReadOnly ] int path_point_index;

	List< Transform > path_points = new List< Transform >( 64 );
    RecycledSequence recycledSequence = new RecycledSequence();
    UnityMessage onPathComplete;

	int path_index;

    // We can achive steping like movement by placing points in a certain way
#endregion

#region Properties
#endregion

#region Unity API
	private void OnDisable()
	{
		onPathComplete = ExtensionMethods.EmptyMethod;
		recycledSequence.Kill();
	}
#endregion

#region API
    public void DoPath( int index, UnityMessage pathComplete )
    {
		recycledSequence.Kill();

		path_index       = index;
		path_point_index = 0;
		onPathComplete   = pathComplete;

		PopulatePathPoints( index );
		DoPath();
	}
#endregion

#region Implementation
    void PopulatePathPoints( int index )
    {
		path_points.Clear();
		set_path.itemDictionary.TryGetValue( index, out path_points );
	}

    void DoPath()
    {
		var position        = movement_transform.position;
		var targetPosition  = path_points[ path_point_index ].position;
		var targetDirection = ( targetPosition - position ).normalized;
		var targetRotation  = Vector3.up * Quaternion.LookRotation( targetDirection ).eulerAngles.y;  // Look only on +Y axis

		var movementDuration = GameSettings.Instance.player_movement_move_speed.ReturnDuration( Vector3.Distance( position, targetPosition ) );
		var rotationDuration = GameSettings.Instance.player_movement_rotate_speed.ReturnDuration( Vector3.Angle( movement_transform.forward, targetDirection ) );

		var sequence = recycledSequence.Recycle();

		sequence.Append( movement_transform.DOMove( targetPosition,
			movementDuration )
				.SetEase( Ease.Linear ) );

		sequence.Join( movement_transform.DORotate( targetRotation,
			rotationDuration )
				.SetEase( Ease.Linear ) );

		sequence.OnUpdate( OnPathSequenceUpdate );
		sequence.OnComplete( OnPathSequenceComplete );
	}

	void OnPathSequenceUpdate()
	{
		notif_level_progress.SharedValue = Mathf.Lerp( path_index, path_index + 1, recycledSequence.Sequence.ElapsedPercentage() ) / set_path.itemDictionary.Count;
	}

    void OnPathSequenceComplete()
    {
		path_point_index++;

        // If we are the last point of the path
        if( path_point_index >= path_points.Count )
        {
			var duration = GameSettings.Instance.player_movement_rotate_speed.ReturnDuration( Vector3.Angle( path_points[ path_points.Count - 1 ].forward, movement_transform.forward ) );

			// Look towards last points forward
			var sequence = recycledSequence.Recycle();
			sequence.Join( movement_transform.DORotate( path_points[ path_points.Count - 1 ].eulerAngles, duration)
					.SetEase( Ease.Linear ) )
                    .OnComplete( onPathComplete.Invoke );
		}
        else
			DoPath(); // Keep moving on the path
	}
#endregion

#region Editor Only
#if UNITY_EDITOR
#endif
#endregion
}