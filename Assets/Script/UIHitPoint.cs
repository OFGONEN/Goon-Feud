/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FFStudio;
using Sirenix.OdinInspector;

public class UIHitPoint : MonoBehaviour
{
#region Fields
    [ BoxGroup( "Shared Variables" ), SerializeField ] SharedReferenceNotifier notif_camera_transform;
    [ BoxGroup( "Shared Variables" ), SerializeField ] SharedIntNotifier notif_answer_submit_count;

    [ BoxGroup( "Setup" ), SerializeField ] Image ui_image;
	[ BoxGroup( "Setup" ), SerializeField ] PoolUIHitPoint pool_ui_hitPoint;
	[ BoxGroup( "Setup" ), SerializeField ] SetUIHitPoint set_ui_hitPoint;

    Goon goon_current;
    Camera camera_main;
	RectTransform ui_rectTransform;
#endregion

#region Properties
#endregion

#region Unity API
#endregion

#region API
    public void AttachToGoon( Goon goon )
    {
		set_ui_hitPoint.AddList( this );

		gameObject.SetActive( true );
		ui_rectTransform = ui_image.rectTransform;

		ui_image.color   = GameSettings.Instance.hitpoint_color_default;
		ui_image.enabled = true;
		goon_current     = goon;

		camera_main = ( notif_camera_transform.SharedValue as Transform ).GetComponent< Camera >();
		transform.position = camera_main.WorldToScreenPoint( goon_current.GoonPosition );
	}

	public void OnSelected()
	{
		ui_image.color = GameSettings.Instance.hitpoint_color_selected;
	}

	public void OnDefault()
	{
		ui_image.color = GameSettings.Instance.hitpoint_color_default;
	}

	public void OnAnswerCache( AnswerData value )
	{
		set_ui_hitPoint.RemoveList( this );

		ui_image.enabled = false;
		goon_current.CacheAnswer( value );

		notif_answer_submit_count.SharedValue++;
	}

	public void OnAnswerClear()
	{
		set_ui_hitPoint.AddList( this );

		goon_current.ClearAnswer();

		ui_image.color = GameSettings.Instance.hitpoint_color_default;
		ui_image.enabled = true;

		notif_answer_submit_count.SharedValue--;
	}

	public void OnQuestionDisappear()
	{
		set_ui_hitPoint.RemoveList( this );
		pool_ui_hitPoint.ReturnEntity( this );
	}
#endregion

#region Implementation
#endregion

#region Editor Only
#if UNITY_EDITOR
#endif
#endregion
}