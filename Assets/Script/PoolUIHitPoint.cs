/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FFStudio;
using Sirenix.OdinInspector;

[ CreateAssetMenu( fileName = "pool_ui_hitPoint", menuName = "FF/Data/Pool/UI Hit Point" ) ]
public class PoolUIHitPoint : ComponentPool< UIHitPoint >
{
    [ BoxGroup( "Setup" ), SerializeField ] SetGoon set_goon;

    public void AttachToGoons()
    {
        foreach( var goon in set_goon.itemDictionary.Values )
        {
			GetEntity().AttachToGoon( goon );
		}
    }
}
