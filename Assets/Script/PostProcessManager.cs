/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using FFStudio;
using Sirenix.OdinInspector;


public class PostProcessManager : MonoBehaviour
{
#region Fields
    [ BoxGroup( "Setup" ), SerializeField ] Volume volume;

    // Post-Process Effects
    Vignette volume_vignette;
#endregion

#region Properties
#endregion

#region Unity API
    private void Awake()
    {
		volume.profile.TryGet<Vignette>( out volume_vignette );
	}
#endregion

#region API
    public void UpdateVignetteIntencity( float value )
    {
        volume_vignette.intensity.value = value;
    }
#endregion

#region Implementation
#endregion

#region Editor Only
#if UNITY_EDITOR
#endif
#endregion
}
