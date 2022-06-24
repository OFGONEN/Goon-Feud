/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FFStudio;
using Sirenix.OdinInspector;

public class Player : MonoBehaviour
{
#region Fields
    [ BoxGroup( "Setup" ), SerializeField ] SharedIntNotifier notif_player_stage_index; 
    [ BoxGroup( "Setup" ), SerializeField ] GameEvent event_stage_start; 

    [ BoxGroup( "Components" ), SerializeField ] PlayerMovement player_movement; 
    [ BoxGroup( "Components" ), SerializeField ] Animator player_animator; 

    int player_path_index = 0;
#endregion

#region Properties
#endregion

#region Unity API
#endregion

#region API
    // Info: Seriliazed Call for responding to event_level_start
    public void OnLevelStart()
    {
		player_animator.SetBool( "walking", true );
		player_movement.DoPath( player_path_index, OnLevelStartPathComplete );
	}

    // Info: Seriliazed Call for responding to event_stage_end
    public void OnStageEnd()
    {
		player_animator.SetBool( "walking", true );
		player_movement.DoPath( player_path_index, OnPathComplete );
	}
#endregion

#region Implementation
    void OnLevelStartPathComplete()
    {
		player_animator.SetBool( "walking", false );
		player_path_index++;
		event_stage_start.Raise();
	}

    void OnPathComplete()
    {
		player_animator.SetBool( "walking", false );

		player_path_index++;
		notif_player_stage_index.SharedValue++;

		// If player came to a new stage
		if( notif_player_stage_index.sharedValue < CurrentLevelData.Instance.levelData.stage_count )
		    event_stage_start.Raise();
        // else Player is on the finish line completed its final path
	}
#endregion

#region Editor Only
#if UNITY_EDITOR
#endif
#endregion
}
