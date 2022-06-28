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
    [ BoxGroup( "Setup" ), SerializeField ] Material[] goon_material_killed;

    [ BoxGroup( "Shared Variables" ), SerializeField ] SharedIntNotifier notif_player_stage_index;
    [ BoxGroup( "Shared Variables" ), SerializeField ] SetGoon set_stage_goon;
    [ BoxGroup( "Shared Variables" ), SerializeField ] GameEvent event_player_killed;

    [ BoxGroup( "Component" ), SerializeField ] Fire_UnityEvent goon_event_responder;
    [ BoxGroup( "Component" ), SerializeField ] Animator goon_animator;
    [ BoxGroup( "Component" ), SerializeField ] GoonMovement goon_movement;
    [ BoxGroup( "Component" ), SerializeField ] GoonLine goon_line;
    [ BoxGroup( "Component" ), SerializeField ] SkinnedMeshRenderer goon_renderer;

	// Answer Cache
	bool answer_cached = false;
	int answer_value = 0;

	RecycledTween recycledTween = new RecycledTween();
	UnityMessage onDoPath;
#endregion

#region Properties
	public int GoonID => goon_id;
#endregion

#region Unity API
    private void OnDisable()
    {
		EmptyDelegates();
		recycledTween.Kill();

		goon_line.StopDraw();
	}

	private void Awake()
	{
		EmptyDelegates();
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

			goon_line.StartDraw();
		}
	}

    public void OnStageContinue()
    {
		onDoPath();
	}

	[ Button() ]
	public void CacheAnswer( int value )
	{
		answer_cached = true;
		answer_value  = value;
	}

	public void TakeDamge()
	{
		// Take damage if goon cached an answer
		if( answer_cached )
			TakeDamage( answer_value );

		answer_cached = false;
	}

	public void PathToPlayer()
	{
		goon_animator.SetBool( "walking", true );
		goon_movement.DoPathLastPoint( OnPathComplete );
	}
#endregion

#region Implementation
    void TakeDamage( int damage )
    {
		goon_health -= damage;

		if( goon_health <= 0 )
			Die();
		else
			goon_animator.SetTrigger( "hurt" );
	}

    void Die()
    {
		EmptyDelegates();

		goon_renderer.sharedMaterials = goon_material_killed;

		goon_event_responder.enabled = false;

		set_stage_goon.RemoveDictionary( goon_id );
		goon_animator.SetTrigger( "die" );
		goon_line.StopDraw();
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

    void KillPlayer()
    {
		goon_animator.SetTrigger( "attack" );
		recycledTween.Recycle( DOVirtual.DelayedCall( GameSettings.Instance.goon_hit_delay, event_player_killed.Raise ) );
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