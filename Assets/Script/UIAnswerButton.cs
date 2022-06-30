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
    Vector3 ui_image_button_velocity;

    UnityMessage onFingerUp;
    UnityMessage onUpdate;
#endregion

#region Properties
#endregion

#region Unity API
    private void Awake()
    {
		onFingerUp = ExtensionMethods.EmptyMethod;
		onUpdate   = ExtensionMethods.EmptyMethod;
	}

    private void Start()
    {
		ui_image_button_position = ui_image_button.rectTransform.position;
	}

    private void Update()
    {
		onUpdate();
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
		ui_image_button_velocity = Vector3.zero;

		onFingerUp = FingerUp;
		onUpdate   = OnUpdate;

        // Since event_quesiton_transparent event is called on a answer button click
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
		onUpdate   = ExtensionMethods.EmptyMethod;
		onFingerUp = ExtensionMethods.EmptyMethod;

		ui_image_button.rectTransform.position   = ui_image_button_position;
    }
    
    void OnUpdate()
    {
		var position = ui_image_button.transform.position;
		var targetPosition = Input.mousePosition;

		ui_image_button.transform.position = Vector3.SmoothDamp( position, targetPosition, ref ui_image_button_velocity, GameSettings.Instance.answer_drag_smoothTime );
	}
#endregion

#region Editor Only
#if UNITY_EDITOR
#endif
#endregion
}
