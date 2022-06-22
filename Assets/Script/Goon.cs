/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FFStudio;
using DG.Tweening;
using Sirenix.OdinInspector;

public class Goon : MonoBehaviour
{
#region Fields
    [ LabelText( "ID of the Stage Goon belongs to"), BoxGroup( "Setup" ), SerializeField ] int goon_stage_id;
    [ LabelText( "ID of the Goon in current Stage" ), BoxGroup( "Setup" ), SerializeField ] int goon_id;
    [ LabelText( "Health Count" ), BoxGroup( "Setup" ), SerializeField ] int goon_health;

    [ BoxGroup( "Shared Variables" ), SerializeField ] SharedIntNotifier notif_player_stage_index;
    [ BoxGroup( "Shared Variables" ), SerializeField ] SetGoon set_stage_goon;
    [ BoxGroup( "Shared Variables" ), SerializeField ] GameEvent event_player_killed;

    [ BoxGroup( "Component" ), SerializeField ] Animator goon_animator;
    [ BoxGroup( "Component" ), SerializeField ] GoonMovement goon_movement;

    RecycledTween recycledTween = new RecycledTween();

	UnityMessage onDoPath;
#endregion

#region Properties
#endregion

#region Unity API
    private void OnDisable()
    {
		EmptyDelegates();
		recycledTween.Kill();
	}

	private void Awake()
	{
		onDoPath = ExtensionMethods.EmptyMethod;
	}
#endregion

#region API
    public void OnStageStart()
    {
		if( notif_player_stage_index.SharedValue == goon_stage_id )
		{
			onDoPath = DoPath;
			set_stage_goon.AddDictionary( goon_id, this );
			onDoPath();
		}
	}

    public void OnStageContinue()
    {
		onDoPath();
	}

    public void TakeDamage( int damage )
    {
		goon_health -= damage;

		goon_animator.SetTrigger( "hurt" );

		if( goon_health <= 0 )
			Die();
	}

    public void KillPlayer()
    {
		goon_animator.SetTrigger( "hit" );
		recycledTween.Recycle( DOVirtual.DelayedCall( GameSettings.Instance.goon_hit_delay, event_player_killed.Raise ) );
	}
#endregion

#region Implementation
    void Die()
    {
		EmptyDelegates();
		set_stage_goon.RemoveDictionary( goon_id );
		goon_animator.SetTrigger( "die" );
    }

    void OnPathComplete()
    {
		goon_animator.SetBool( "walking", false );

		if( !goon_movement.CanPath )
			KillPlayer();
	}

    void DoPath()
    {
		goon_animator.SetBool( "walking", true );
		goon_movement.DoPath( OnPathComplete );
	}

	void EmptyDelegates()
	{
		onDoPath = ExtensionMethods.EmptyMethod;
	}
#endregion

#region Editor Only
#if UNITY_EDITOR
#endif
#endregion
}