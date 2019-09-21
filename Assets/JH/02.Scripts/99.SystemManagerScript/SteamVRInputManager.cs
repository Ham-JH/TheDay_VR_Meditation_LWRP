using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR;
using System.Threading;
using Unity.Mathematics;

public class SteamVRInputManager : MonoBehaviour
{
    [Header("GameManager")]
    public GameManager GM;

    [Header("출력 텍스트")]
    public Text LText;
    public Text RText;

    [Header("컨트롤러 종류")]
    public SteamVR_Input_Sources LeftController = SteamVR_Input_Sources.LeftHand;
    public SteamVR_Input_Sources RightController = SteamVR_Input_Sources.RightHand;

    [Header("왼쪽 컨트롤러 변수")]
    public SteamVR_Action_Boolean LMenu;
    public SteamVR_Action_Boolean LGrip;
    public SteamVR_Action_Boolean LTrigger;
    public SteamVR_Action_Boolean LTrackpad;
    public SteamVR_Action_Vector2 LTrackpadPos;
    public SteamVR_Action_Pose LPose;

    [Header("오른쪽 컨트롤러 변수")]
    public SteamVR_Action_Boolean RMenu;
    public SteamVR_Action_Boolean RGrip;
    public SteamVR_Action_Boolean RTrigger;
    public SteamVR_Action_Boolean RTrackpad;
    public SteamVR_Action_Vector2 RTrackpadPos;
    public SteamVR_Action_Pose RPose;

    [Header("왼쪽 컨트롤러 변수 상태")]
    public bool LMenuStateDown;
    public bool LGripStateDown;
    public bool LTriggerStateDown;
    public bool LTrackpadStateDown;
    public float2 LTrackpadPosPosition;
    public float4 LPosePosition;

    [Header("오른쪽 컨트롤러 변수 상태")]
    public bool RMenuStateDown;
    public bool RGripStateDown;
    public bool RTriggerStateDown;
    public bool RTrackpadStateDown;
    public float2 RTrackpadPosPosition;
    public float4 RPosePosition;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //빈도순으로 한 조건식으로 엮음
        if(RTrigger.stateDown)
        {
            GM.InputCatch(GameManager.InputStatus.RTrigDown);
        }
        else if(LTrigger.stateDown)
        {
            GM.InputCatch(GameManager.InputStatus.LTrigDown);
        }
        else if(RMenu.stateDown)
        {
            GM.InputCatch(GameManager.InputStatus.RMenuDown);
        }
        else if(LMenu.stateDown)
        {
            GM.InputCatch(GameManager.InputStatus.LMenuDown);
        }
        else if(RTrackpad.stateDown)
        {
            GM.InputCatch(GameManager.InputStatus.RTracDown);
        }
        else if(LTrackpad.stateDown)
        {
            GM.InputCatch(GameManager.InputStatus.LTracDown);
        }
        else if(RGrip.stateDown)
        {
            GM.InputCatch(GameManager.InputStatus.RGripDown);
        }
        else if(LGrip.stateDown)
        {
            GM.InputCatch(GameManager.InputStatus.LGripDown);
        }

        //TrackPadPos, 컨트롤러 위치 할당은 논외
        
        //Debug.Log(LTriggerStateDown);
        //LText.text = "LTrigger  " + LTrigger.state
        //           + "\nLGrip   " + LGrip.state
        //           + "\nLTrackpad   " + LTrackpad.state
        //           + "\nLMenu     " + LMenu.state
        //           + "\nLTrackpadPos  " + LTrackpadPos.axis
        //           + "\nLt_pose.localPosition " + LPose.localPosition
        //           + "\nLt_pose.localRotation " + LPose.localRotation;

        //RText.text = "RTrigger  " + RTrigger.state
        //           + "\nRGrip   " + RGrip.state
        //           + "\nRTrackpad   " + RTrackpad.state
        //           + "\nRMenu     " + RMenu.state
        //           + "\nRTrackpadPos  " + RTrackpadPos.axis
        //           + "\nRt_pose.localPosition " + RPose.localPosition
        //           + "\nRt_pose.localRotation " + RPose.localRotation;

    }
}
