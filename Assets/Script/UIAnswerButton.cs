/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FFStudio;
using TMPro;
using Sirenix.OdinInspector;

public class UIAnswerButton : MonoBehaviour
{
#region Fields
    [ BoxGroup( "Setup" ), SerializeField ] SetUIHitPoint set_ui_hitPoint;
    [ BoxGroup( "Setup" ), SerializeField ] int button_id;

    [ BoxGroup( "UI Elements" ), SerializeField ] Image ui_image_button;
    [ BoxGroup( "UI Elements" ), SerializeField ] Image ui_image_button_return;
    [ BoxGroup( "UI Elements" ), SerializeField ] TextMeshProUGUI ui_text_button; // child game-object of ui_image_button

    AnswerData answer_data;
    Vector3 ui_image_button_position;

    UnityMessage onFingerUp;
#endregion

#region Properties
#endregion

#region Unity API
    private void Awake()
    {
		onFingerUp = ExtensionMethods.EmptyMethod;
	}

    private void Start()
    {
		ui_image_button_position = ui_image_button.rectTransform.position;
	}
#endregion

#region API
    public void OnQuestionAppear()
    {
		answer_data = CurrentLevelData.Instance.CurrentQuestion.question_answer[ button_id ];

		ui_image_button.gameObject.SetActive( true );
		ui_image_button.rectTransform.localScale = Vector3.one;
		ui_image_button.rectTransform.position   = ui_image_button_position;

		ui_text_button.text = answer_data.answer;

		ui_image_button_return.gameObject.SetActive( false );
	}

    public void OnQuestionDisappear()
    {
		ui_image_button.gameObject.SetActive( false );
		ui_image_button_return.gameObject.SetActive( false );
	}

    public void AnswerTransparent()
    {
		ui_image_button.SetAlpha( GameSettings.Instance.answer_transparent_value );
		ui_image_button_return.SetAlpha( GameSettings.Instance.answer_transparent_value );
		ui_text_button.SetAlpha( GameSettings.Instance.answer_transparent_value );
	}

    public void AnswerOpaque()
    {
		ui_image_button.SetAlpha( 1 );
		ui_image_button_return.SetAlpha( 1 );
		ui_text_button.SetAlpha( 1 );
    }

    public void OnButtonClick()
    {
        FFLogger.Log( "On Click", gameObject );
		onFingerUp = FingerUp;
		AnswerOpaque();
	}

    public void OnFingerUp()
    {
		onFingerUp();
    }
#endregion

#region Implementation
    void FingerUp()
    {
		onFingerUp = ExtensionMethods.EmptyMethod;
		FFLogger.Log( "On Finger Up", gameObject );
    }
#endregion

#region Editor Only
#if UNITY_EDITOR
#endif
#endregion
}
