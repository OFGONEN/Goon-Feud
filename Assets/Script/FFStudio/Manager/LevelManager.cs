/* Created by and for usage of FF Studios (2021). */

using UnityEngine;
using UnityEngine.SceneManagement;
using Sirenix.OdinInspector;
using DG.Tweening;

namespace FFStudio
{
    public class LevelManager : MonoBehaviour
    {
#region Fields
    [ Title( "Fired Events" ) ]
        [ SerializeField ] GameEvent event_level_completed;
        [ SerializeField ] GameEvent event_level_failed;
        [ SerializeField ] GameEvent event_stage_continue;
        [ SerializeField ] GameEvent event_stage_end;
		[ SerializeField ] GameEvent event_ui_question_appear;
        [ SerializeField ] GameEvent event_ui_question_disappear;

    [ Title( "Shared Variables" ) ]
        [ SerializeField ] SetGoon set_goon;
        [ SerializeField ] PoolUIHitPoint pool_ui_hitPoint;
        [ SerializeField ] SharedIntNotifier notif_ui_question_index;
        [ SerializeField ] SharedIntNotifier notif_player_stage_index; 
        [ SerializeField ] SharedFloatNotifier levelProgress;
        [ SerializeField ] SharedReferenceNotifier notif_player_transform;
    
        RecycledTween recycledTween = new RecycledTween();
#endregion

#region UnityAPI
        private void OnDisable()
        {
			recycledTween.Kill();
		}
#endregion

#region API
        //Info: Editor Call
        public void LevelLoadedResponse()
        {
			levelProgress.SetValue_NotifyAlways( 0 );

			var levelData = CurrentLevelData.Instance.levelData;
            // Set Active Scene.
			if( levelData.scene_overrideAsActiveScene )
				SceneManager.SetActiveScene( SceneManager.GetSceneAt( 1 ) );
            else
				SceneManager.SetActiveScene( SceneManager.GetSceneAt( 0 ) );
		}

        //Info: Editor Call
        public void LevelRevealedResponse()
        {

        }

        //Info: Editor Call
        public void LevelStartedResponse()
        {

        }

        // Info: Seriliazed Call of value change on the notif_goon_path_count
        public void OnGoonPathingCount( int value )
        {
			var questionCount        = CurrentLevelData.Instance.levelData.stage_data[ notif_player_stage_index.sharedValue ].stage_question.Length;
			var allQuestionsAnswered = questionCount <= notif_ui_question_index.sharedValue;

            // All goons are stopped pathing
            if( value == 0 ) 
            {
                // There is no other question to answer
                // So one goon needs to go and kill the Player
                if( allQuestionsAnswered )
					KillPlayerWithGoon();
				else
                {
				    event_ui_question_appear.Raise(); // There is question to answer
					pool_ui_hitPoint.AttachToGoons();
				}
            }
		}
        
        // Info: Seriliazed Call of value change on the notif_ui_answer_submit_count
        public void OnAnswerSubmit( int value )
        {
            // All Questions are answered or All alive goons are submited with an answer since there is less goon than answer count
            if( value == ExtensionMethods.ANSWER_COUNT || value == set_goon.itemDictionary.Count )
            {
				event_ui_question_disappear.Raise();

				set_goon.TakeDamage(); // Damage all goons
				recycledTween.Recycle( DOVirtual.DelayedCall( GameSettings.Instance.goon_damage_delay, ResolveStageState ) );

				notif_ui_question_index.SharedValue++;
			}
		}

        // Info: Seriliazed Call of value change on the notif_player_stage_index
        public void OnPlayerStageIndexChange( int value )
        {
            if( value == CurrentLevelData.Instance.levelData.stage_count )
				event_level_completed.Raise();
		}
#endregion

#region Implementation
        void KillPlayerWithGoon()
        {
			// Find closest goon 
			var playerPosition = ( notif_player_transform.SharedValue as Transform ).position;

			Goon closestGoon = null;
			float closestDistance = float.MaxValue;

            foreach( var goon in set_goon.itemDictionary.Values )
            {
				var goonDistance = Vector3.Distance( goon.transform.position, playerPosition );

                if( goonDistance < closestDistance )
                {
					closestDistance = goonDistance;
					closestGoon     = goon;
				}
			}

			closestGoon.PathToPlayer();
		}

        void ResolveStageState()
        {
            if( set_goon.itemDictionary.Count > 0 )
				event_stage_continue.Raise();
            else
				event_stage_end.Raise();
		}
#endregion

#region Editor Only
#if UNITY_EDITOR
        [ Button() ]
        void AnswerSubmit()
        {
			OnAnswerSubmit( 4 );
		}
#endif
#endregion
    }
}