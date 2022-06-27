/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FFStudio;

[ CreateAssetMenu( fileName = "set_stage_goon", menuName = "FF/Data/Set/Stage Goons" ) ]
public class SetGoon : RuntimeSet< int, Goon >
{

    public void TakeDamage()
    {
        foreach( var goon in itemDictionary.Values )
        {
			goon.TakeDamge();
		}
    }
}
