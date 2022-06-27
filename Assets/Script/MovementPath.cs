/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FFStudio;
using Sirenix.OdinInspector;
using UnityEditor;

public abstract class MovementPath : MonoBehaviour
{
#region Fields
    [ SerializeField ] protected List< Transform > path_points;
    [ SerializeField ] protected Transform path_parent;
#endregion

#region Properties
#endregion

#region Unity API
#endregion

#region API
#endregion

#region Implementation
#endregion

#region Editor Only
#if UNITY_EDITOR
    // Cache every childs transform
    // We can use it while Drawing Gizmoz without trying to get all reference every time
    // We can start the game without the overhead of trying to cache the child's transform references on Start
    [ Button() ]  
    private void BakePathPoints()
    {
		path_points = new List< Transform >( path_parent.childCount );

		for( var i = 0; i < path_parent.childCount; i++ )
        {
			path_points.Add( path_parent.GetChild( i ) );
		}
    }

    //Info: Enable this on derived classes whenever it is necessary
    // private void OnDrawGizmos()
    // {
    //     for( var i = 0; i < path_points.Length; i++ )
    //     {
	// 		// Draw Spheres on every path point
	// 		Gizmos.DrawWireSphere( path_points[ i ].position, 0.15f );

	// 		// Label Every path point
	// 		Handles.Label( path_points[ i ].position, path_parent.name + ": " + i );
	// 	}

    //     // Draw line between every point
    //     for( var i = 0; i < path_points.Length - 1; i++ )
    //     {
	// 		Handles.DrawDottedLine( path_points[ i ].position, path_points[ i + 1 ].position, 10 );
	// 	}
    // }
#endif
#endregion
}
