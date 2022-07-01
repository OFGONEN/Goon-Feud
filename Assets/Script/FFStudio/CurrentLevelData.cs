/* Created by and for usage of FF Studios (2021). */

using UnityEngine;
using Sirenix.OdinInspector;

namespace FFStudio
{
    public class CurrentLevelData : ScriptableObject
    {
#region Fields
		public int currentLevel_Real;
		public int currentLevel_Shown;
		public LevelData levelData;

    [ Title( "Question" ) ]
        public SharedIntNotifier notif_player_stage_index;
        public SharedIntNotifier notif_ui_question_index;

        private static CurrentLevelData instance;

        private delegate CurrentLevelData ReturnCurrentLevel();
        private static ReturnCurrentLevel returnInstance = LoadInstance;

        public static CurrentLevelData Instance => returnInstance();
#endregion

#region API
		public void LoadCurrentLevelData()
		{
			if( currentLevel_Real > GameSettings.Instance.maxLevelCount )
				currentLevel_Real = Random.Range( 1, GameSettings.Instance.maxLevelCount );

			levelData = Resources.Load< LevelData >( "level_data_" + currentLevel_Real );
		}

		public Question CurrentQuestion => levelData.stage_data[ notif_player_stage_index.sharedValue ].stage_question[ notif_ui_question_index.sharedValue ];
#endregion

#region Implementation
		static CurrentLevelData LoadInstance()
		{
			if( instance == null )
				instance = Resources.Load< CurrentLevelData >( "level_current" );

			returnInstance = ReturnInstance;

            return instance;
        }

        static CurrentLevelData ReturnInstance()
        {
            return instance;
        }
#endregion
    }
}