/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FFStudio;
using Shapes;
using Sirenix.OdinInspector;

public class GoonLine : ImmediateModeShapeDrawer
{
#region Fields
	[ BoxGroup( "Setup" ), SerializeField ] GoonMovement goon_movement;
	[ BoxGroup( "Setup" ), SerializeField ] Transform goon_transform;

    DrawShape onDrawShape;
#endregion

#region Properties
#endregion

#region Unity API
	private void Awake()
	{
		onDrawShape = ExtensionMethods.EmptyMethod;
	}
#endregion

#region API
	[ Button() ]
	public void StartDraw()
	{
		onDrawShape = DrawLine;
	}

	[ Button() ]
	public void StopDraw()
	{
		onDrawShape = ExtensionMethods.EmptyMethod;
	}
#endregion

#region Implementation
	public override void DrawShapes( Camera cam )
    {
		onDrawShape( cam );
	}

	void DrawLine( Camera cam )
	{
		using( Draw.Command( cam ) )
        {
			Vector3 startPosition = goon_transform.position;

			// Draw Lines
			for( var i = goon_movement.PathIndex; i < goon_movement.PathCount - 1; i++ )
			{
				var targetPosition = goon_movement.GetPathPoint( i );

				Draw.Line( startPosition, targetPosition, 0.125f, LineEndCap.None, Color.white );

				startPosition = targetPosition;
			}

			var lastPosition = goon_movement.GetPathPoint( goon_movement.PathCount - 1 );
			var direction    = ( lastPosition - startPosition ).normalized;
			lastPosition -= direction * GameSettings.Instance.goon_movement_lastPoint_line_distance;

			Draw.Line( startPosition, lastPosition, 0.125f, LineEndCap.None, Color.white );
		}
	}
#endregion

#region Editor Only
#if UNITY_EDITOR
#endif
#endregion
}