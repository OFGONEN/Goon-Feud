/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FFStudio;
using Sirenix.OdinInspector;

[ CreateAssetMenu( fileName = "set_stage_goon", menuName = "FF/Data/Set/Stage Goons" ) ]
public class SetGoon : RuntimeSet< int, Goon >
{
    List< Goon > goon_cache = new List< Goon >( 16 );
    [ Button() ]
    public void TakeDamage()
    {
		goon_cache.Clear();

		foreach( var goon in itemDictionary.Values )
        {
			goon_cache.Add( goon );
		}

        foreach( var goon in goon_cache )
        {
			goon.TakeDamge();
		}
    }
}
