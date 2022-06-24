/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FFStudio;
using Sirenix.OdinInspector;
using UnityEditor;

public class PlayerPath : MovementPath
{
#region Fields
    [ BoxGroup( "Setup" ), SerializeField ] SetPath set_path;
    [ BoxGroup( "Setup" ), SerializeField ] int path_index;
#endregion

#region Properties
#endregion

#region Unity API
    private void Awake()
    {
		set_path.AddDictionary( path_index, path_points );
	}
#endregion

#region API
#endregion

#region Implementation
#endregion

#region Editor Only
#if UNITY_EDITOR
    // Info: Enable this on derived classes whenever it is necessary
    private void OnDrawGizmos()
    {
		if( path_points == null || path_points.Count <= 0 ) return;

		Gizmos.color  = Color.green;
		Handles.color = Color.green;

		// Draw Spheres on start and end of the path
		Gizmos.DrawWireSphere( path_points[ 0 ].position, 0.15f );
		Gizmos.DrawWireSphere( path_points[ path_points.Count - 1 ].position, 0.15f );

		// Label start and end of the path
		Handles.Label( path_points[ 0 ].position, $"PlayerPath Start: " + path_index );
		Handles.Label( path_points[ path_points.Count - 1 ].position, $"PlayerPath End: " + path_index );

		// Draw line between every point
		for( var i = 0; i < path_points.Count - 1; i++ )
        {
			Handles.DrawDottedLine( path_points[ i ].position, path_points[ i + 1 ].position, 10 );
		}
    }
#endif
#endregion
}
