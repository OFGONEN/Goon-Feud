/* Created by and for usage of FF Studios (2021). */

using System.Linq;
using UnityEngine;
using UnityEditor;

namespace FFEditor
{
	/*
	
	[ Example File Name ]		[ Has Model ] [ Rigged ? ] [ Has Animation ] [ Has Shape Key(s) ]
	envr_tree.fbx 						✔			✖				✖				✖
	prop_ball.fbx 						✔			✖				✖				✖
	prop_ball_skey.fbx 					✔			✖				✖				✔(1)			(This is just for information, do not use it.)
	gnrc_ball.fbx						✔			✔(2)			✖				✖
	gnrc_ball_skey.fbx					✔			❔(3)			✖				✔	
	gnrc_ball_skey_anim.fbx 			✔			✔				✔				✔	
	char_martha.fbx 					✔			✔			T-Pose				✖	
	char_martha_skey.fbx 				✔			✔			T-Pose				✔	
	char_walking.fbx 					✖			✔				✔				✖
	
	✔(1) (prop_ball_skey.fbx):
	Because prefix is not gnrc or char, shape keys can not be used, as the rig will be set to None, thus the model will be rendered by a Mesh Renderer.
	Shape keys can still be utilized though, if the mesh is placed on a Skinned Mesh Renderer manually.

	✔(2) (gnrc_ball.fbx):
	Has rig but no animation, this may be due to the fact that the animation will come from outside the model file,
	For example, the animation may be recorded in Unity.

	❔(3) (gnrc_ball_skey.fbx):
	May or may not have rig; We still NEED to set rig to Generic in order to utilize shape keys.

	*/

	public class FFModelImportUtility : AssetPostprocessor
	{
		public Material defaultMaterial;

		static readonly string prefix_prop = "prop";
		static readonly string prefix_envr = "envr";
		static readonly string prefix_gnrc = "gnrc";
		static readonly string prefix_char = "char";
		static readonly string suffix_anim = "anim";
		static readonly string suffix_skey = "skey";
		static readonly string infix_skey  = "_skey_";

		void OnPreprocessModel()
		{
			var  modelPrefix  = GetModelPrefix();
			var  modelSuffix  = GetModelSuffix();
			bool hasShapeKey  = HasInfix( infix_skey ) || modelSuffix == suffix_skey;

			if( ShouldProcess( modelPrefix ) == false )
				return;

			var modelImporter = assetImporter as ModelImporter;

			/* Model Tab. */
			modelImporter.importVisibility  = false;
			modelImporter.importCameras     = false;
			modelImporter.importLights      = false;
			modelImporter.importBlendShapes = hasShapeKey;
			
			if( hasShapeKey ) // Recalculation of the normals causes the lighting to be wrong.
				modelImporter.importBlendShapeNormals = ModelImporterNormals.None;

			/* Rig Tab. */
			if( modelPrefix == prefix_prop || modelPrefix == prefix_envr )
				modelImporter.animationType = ModelImporterAnimationType.None;
            else if( modelPrefix == prefix_char )
				modelImporter.animationType = ModelImporterAnimationType.Human;
            else if( modelPrefix == prefix_gnrc )
				modelImporter.animationType = ModelImporterAnimationType.Generic;

			/* Animation Tab. */
			modelImporter.importAnimation = modelPrefix == prefix_char || 
											( modelPrefix == prefix_gnrc && modelSuffix == suffix_anim );

			AssetDatabase.ImportAsset( assetPath );
		}

		void OnPostprocessModel( GameObject gameObject )
		{
			if( ShouldProcess( GetModelPrefix() ) )
			{
				FFStudio.FFLogger.Log( "FFModelImportUtility: Remapping default material for " + assetPath.Split( '/' ).Last() + ".",
								   	   gameObject );
				RemapDefaultMaterial( gameObject.transform );
				AssetDatabase.WriteImportSettingsIfDirty( assetPath );
			}
		}

		void RemapDefaultMaterial( Transform transform )
		{
			Renderer renderer = transform.gameObject.GetComponent< Renderer >();

			if( defaultMaterial == null )
				defaultMaterial = AssetDatabase.LoadAssetAtPath< Material >( "Assets/Material/mat_atlas.mat" );

			if( renderer != null )
			{
				Material[] materials = renderer.sharedMaterials;
                
				foreach( var material in materials )
					assetImporter.AddRemap( new AssetImporter.SourceAssetIdentifier( material ), defaultMaterial );
			}

			// Recurse.
			foreach( Transform child in transform )
				RemapDefaultMaterial( child );
		}

		bool ShouldProcess( string prefix )
		{
			var modelImporter = assetImporter as ModelImporter;

			// Info: Only pre-process models for the FIRST time.
			if( modelImporter.importSettingsMissing == false )
				return false;

			return prefix == prefix_prop || prefix == prefix_envr || prefix == prefix_gnrc || prefix == prefix_char;
		}
		
		string GetModelPrefix()
		{
			var modelNameOnly = assetPath.Split( '/' ).Last();
			var modelPrefix = modelNameOnly.Split( '_' ).First();
			return modelPrefix;
		}

		string GetModelSuffix()
		{
			var modelNameOnly = assetPath.Split( '/' ).Last();
			// Info: Skip the last (actual) suffix: It is the version number (for example; _v3).
			var modelSuffix = modelNameOnly.Split( '_' ).Reverse().Skip( 1 ).Take( 1 ).First();
			return modelSuffix;
		}
		
		bool HasInfix( string infix )
		{
			var modelNameOnly = assetPath.Split( '/' ).Last();
			return modelNameOnly.Contains( infix );
		}
	}
}