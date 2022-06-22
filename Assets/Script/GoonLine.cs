/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Shapes;

public class GoonLine : ImmediateModeShapeDrawer
{
#region Fields
#endregion

#region Properties
#endregion

#region Unity API
#endregion

#region API
#endregion

#region Implementation
	public override void DrawShapes( Camera cam )
    {
		using( Draw.Command( cam ) )
        {
            // Draw Lines
			Draw.Line( Vector3.zero, Vector3.right * 3.75f, 0.1f, LineEndCap.None, Color.white );
		}
	}
#endregion

#region Editor Only
#if UNITY_EDITOR
#endif
#endregion
}
