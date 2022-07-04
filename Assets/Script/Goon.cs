/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
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
    [ BoxGroup( "Shared Variables" ), SerializeField ] Pool_UIPopUpText pool_ui_popUpText;
    [ BoxGroup( "Shared Variables" ), SerializeField ] GameEvent event_player_killed;
    [ BoxGroup( "Shared Variables" ), SerializeField ] GameEvent event_question_disappear;

    [ BoxGroup( "UI Elements" ), SerializeField ] RectTransform goon_ui_healthBar_parent;
    [ BoxGroup( "UI Elements" ), SerializeField ] Image goon_ui_healthBar_fill;

    [ BoxGroup( "Component" ), SerializeField ] Fire_UnityEvent goon_event_responder;
    [ BoxGroup( "Component" ), SerializeField ] Animator goon_animator;
    [ BoxGroup( "Component" ), SerializeField ] GoonMovement goon_movement;
    [ BoxGroup( "Component" ), SerializeField ] GoonLine goon_line;
    [ BoxGroup( "Component" ), SerializeField ] SkinnedMeshRenderer goon_renderer;

    [ BoxGroup( "Particle" ), SerializeField ] ParticleSystem pfx_goon_damage;
    [ BoxGroup( "Particle" ), SerializeField ] ParticleSystem pfx_goon_death;

	// Answer Cache
	bool answer_cached = false;
	int answer_value = 0;
	bool answer_best = false;
	int goon_health_start;

	RecycledTween recycledTween = new RecycledTween();
	RecycledTween recycledTween_ui = new RecycledTween();
	UnityMessage onDoPath;
#endregion

#region Properties
	public int GoonID => goon_id;
	public float GoonHealthRatio => ( float )goon_health / goon_health_start;
	public Vector3 GoonPosition => goon_movement.Position + Vector3.up * GameSettings.Instance.goon_torso_height;
	#endregion

	#region Unity API
	private void OnDisable()
    {
		EmptyDelegates();
		recycledTween.Kill();
		recycledTween_ui.Kill();

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
			goon_movement.OnStageStart();
			onDoPath = DoPath;
			set_stage_goon.AddDictionary( goon_id, this );
			onDoPath();

			goon_line.StartDraw();

			goon_health_start = goon_health;
			goon_ui_healthBar_fill.fillAmount = 1;
		}
	}

    public void OnStageContinue()
    {
		onDoPath();
	}

	[ Button() ]
	public void CacheAnswer( AnswerData value )
	{
		answer_cached = true;
		answer_value  = value.answer_value;
		answer_best   = value.answer_best;
	}

	[ Button() ]
	public void ClearAnswer()
	{
		answer_cached = false;
		answer_value  = 0;
		answer_best = false;
	}

	public void TakeDamge()
	{
		// Take damage if goon cached an answer
		if( answer_cached )
			TakeDamage( answer_value, answer_best );

		answer_cached = false;
	}

	public void PathToPlayer()
	{
		goon_animator.SetBool( "walking", true );
		goon_movement.DoPathLastPoint( OnPathComplete );
	}

	public void OnQuestionAppear()
	{
		recycledTween_ui.Kill();

		goon_ui_healthBar_fill.fillAmount = GoonHealthRatio;
		goon_ui_healthBar_parent.gameObject.SetActive( true );
	}
#endregion

#region Implementation
    void TakeDamage( int damage, bool bestAnswer )
    {
		goon_health -= damage;

		recycledTween_ui.Recycle( goon_ui_healthBar_fill.DOFillAmount( GoonHealthRatio, GameSettings.Instance.ui_Entity_Scale_TweenDuration ), OnHealthBarFillComplete );

		if( goon_health <= 0 )
			Die();
		else
		{
			goon_animator.SetTrigger( "hurt" );
			pfx_goon_damage.Play();
		}

		if( answer_best )
			pool_ui_popUpText.GetEntity().Spawn( GoonPosition + goon_movement.Forward, "-" + damage + "\nBest Answer", 1, GameSettings.Instance.answer_popUp_color );
		else
			pool_ui_popUpText.GetEntity().Spawn( GoonPosition + goon_movement.Forward, "-" + damage, 1, GameSettings.Instance.answer_popUp_color );
	}

    void Die()
    {
		EmptyDelegates();

		goon_renderer.sharedMaterials = goon_material_killed;

		goon_event_responder.enabled = false;

		set_stage_goon.RemoveDictionary( goon_id );
		goon_animator.SetTrigger( "die" );
		goon_line.StopDraw();

		pfx_goon_death.Play();
	}

    void OnPathComplete()
    {
		goon_animator.SetBool( "walking", false );

		if( !goon_movement.CanPath )
			KillPlayer();
	}

	void OnHealthBarFillComplete()
	{
		recycledTween_ui.Recycle( DOVirtual.DelayedCall( GameSettings.Instance.goon_ui_disable_delay, DisableGoonUI ) );
	}

	void DisableGoonUI()
	{
		goon_ui_healthBar_parent.gameObject.SetActive( false );
	}

    void DoPath()
    {
		goon_animator.SetBool( "walking", true );
		goon_movement.DoPath( OnPathComplete );
	}

    void KillPlayer()
    {
		goon_animator.SetTrigger( "attack" );
		recycledTween.Recycle( DOVirtual.DelayedCall( GameSettings.Instance.goon_hit_delay, OnPlayerKilled ) );
	}

	void OnPlayerKilled()
	{
		event_question_disappear.Raise();
		event_player_killed.Raise();
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