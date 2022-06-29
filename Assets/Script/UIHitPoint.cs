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
    [ BoxGroup( "Setup" ), SerializeField ] Image ui_image;
    [ BoxGroup( "Setup" ), SerializeField ] SharedReferenceNotifier notif_camera_transform;

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
		gameObject.SetActive( true );
		ui_rectTransform = ui_image.rectTransform;

		ui_image.color   = GameSettings.Instance.hitpoint_color_default;
		ui_image.enabled = true;
		goon_current     = goon;

		camera_main = ( notif_camera_transform.SharedValue as Transform ).GetComponent< Camera >();
		transform.position = camera_main.WorldToScreenPoint( goon_current.GoonPosition );
	}
#endregion

#region Implementation
#endregion

#region Editor Only
#if UNITY_EDITOR
#endif
#endregion
}
