/* Created by and for usage of FF Studios (2021). */

using UnityEngine;
using Sirenix.OdinInspector;

namespace FFStudio
{
	public class GameSettings : ScriptableObject
    {
#region Fields (Settings)
    // Info: Use Title() attribute ONCE for every game-specific group of settings.

  [ Title( "Player" ) ]
        public float player_movement_move_speed;
        public float player_movement_rotate_speed;

  [ Title( "Goon" ) ]
        public float goon_movement_move_speed;
        public float goon_movement_rotate_speed;
        public float goon_movement_lastPoint_distance;
        public float goon_movement_lastPoint_line_distance;
        public float goon_hit_delay;
        public float goon_damage_delay;
        public float goon_ui_fill_duration;
        public float goon_ui_disable_delay;
        public float goon_torso_height;
        public Color goon_line_color_start;
        public Color goon_line_color_end;
    
  [ Title( "Question UI" ) ]
        public float question_transparent_value;
        public Color hitpoint_color_default;
        public Color hitpoint_color_selected;

  [ Title( "Answer UI" ) ]
        public float answer_transparent_value;
        public float answer_submit_scale;
        public float answer_drag_smoothTime;
		public Color answer_popUp_color;
        [ LabelText( "Submit distance in percentage of the horizontal length" ) ] public float answer_submit_distance;

    // Info: 3 groups below (coming from template project) are foldout by design: They should remain hidden.
		[ FoldoutGroup( "Remote Config" ) ] public bool useRemoteConfig_GameSettings;
        [ FoldoutGroup( "Remote Config" ) ] public bool useRemoteConfig_Components;

        public int maxLevelCount;
        [ FoldoutGroup( "UI Settings" ), Tooltip( "Duration of the movement for ui element"          ) ] public float ui_Entity_Move_TweenDuration;
        [ FoldoutGroup( "UI Settings" ), Tooltip( "Duration of the fading for ui element"            ) ] public float ui_Entity_Fade_TweenDuration;
		[ FoldoutGroup( "UI Settings" ), Tooltip( "Duration of the scaling for ui element"           ) ] public float ui_Entity_Scale_TweenDuration;
		[ FoldoutGroup( "UI Settings" ), Tooltip( "Duration of the movement for floating ui element" ) ] public float ui_Entity_FloatingMove_TweenDuration;
		[ FoldoutGroup( "UI Settings" ), Tooltip( "Joy Stick"                                        ) ] public float ui_Entity_JoyStick_Gap;
		[ FoldoutGroup( "UI Settings" ), Tooltip( "Pop Up Text relative float height"                ) ] public float ui_PopUp_height;
		[ FoldoutGroup( "UI Settings" ), Tooltip( "Pop Up Text float duration"                       ) ] public float ui_PopUp_duration;
        [ FoldoutGroup( "UI Settings" ), Tooltip( "Percentage of the screen to register a swipe"     ) ] public int swipeThreshold;

        [ FoldoutGroup( "Debug" ) ] public float debug_ui_text_float_height;
        [ FoldoutGroup( "Debug" ) ] public float debug_ui_text_float_duration;
#endregion

#region Fields (Singleton Related)
        private static GameSettings instance;

        private delegate GameSettings ReturnGameSettings();
        private static ReturnGameSettings returnInstance = LoadInstance;

		public static GameSettings Instance => returnInstance();
#endregion

#region Implementation
        private static GameSettings LoadInstance()
		{
			if( instance == null )
				instance = Resources.Load< GameSettings >( "game_settings" );

			returnInstance = ReturnInstance;

			return instance;
		}

		private static GameSettings ReturnInstance()
        {
            return instance;
        }
#endregion

#region Editor Only
#if UNITY_EDITOR
        private void OnValidate()
        {
			hitpoint_color_default  = hitpoint_color_default.SetAlpha( 1 );
			hitpoint_color_selected = hitpoint_color_selected.SetAlpha( 1 );
			answer_popUp_color      = answer_popUp_color.SetAlpha( 1 );
			// goon_line_color_start   = goon_line_color_start.SetAlpha( 1 );
			// goon_line_color_end     = goon_line_color_end.SetAlpha( 1 );
		}
#endif
#endregion
    }
}
