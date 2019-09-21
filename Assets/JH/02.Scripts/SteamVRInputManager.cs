using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR;

public class SteamVRInputManager : MonoBehaviour
{
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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(LTrigger.state);
        LText.text = "LTrigger  " + LTrigger.state
                   + "\nLGrip   " + LGrip.state
                   + "\nLTrackpad   " + LTrackpad.state
                   + "\nLMenu     " + LMenu.state
                   + "\nLTrackpadPos  " + LTrackpadPos.axis
                   + "\nLt_pose.localPosition " + LPose.localPosition
                   + "\nLt_pose.localRotation " + LPose.localRotation;

        RText.text = "RTrigger  " + RTrigger.state
                   + "\nRGrip   " + RGrip.state
                   + "\nRTrackpad   " + RTrackpad.state
                   + "\nRMenu     " + RMenu.state
                   + "\nRTrackpadPos  " + RTrackpadPos.axis
                   + "\nRt_pose.localPosition " + RPose.localPosition
                   + "\nRt_pose.localRotation " + RPose.localRotation;
    }
}
