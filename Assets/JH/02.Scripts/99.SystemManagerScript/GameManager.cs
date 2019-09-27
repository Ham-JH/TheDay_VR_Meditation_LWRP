using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// GameManager Scene 변경시 삭제 재생성 or 유지, 변경 고려
/// </summary>
public class GameManager : MonoBehaviour
{
    /// <summary>
    /// 씬 위치 열거변수
    /// </summary>
    public enum ScenePosition
    {
        Intro,
        Main,
        Contents,
        Manual,
        Options,
        Credit
    }

    /// <summary>
    /// 씬 상태 열거변수
    /// </summary>
    public enum SceneStatus
    {
        Intro___SceneStart,
        Intro_0_DeviceCheck,
        Intro_1_UserCheck,
        Intro_2_Manual,
        Intro_3_Narration,
        Intro_4_FTUser,
        Intro_5_SkipBtn,
        Intro_6_IntroTimeOut,
        Intro_7_IntroEnd,

        Main_0_SceneChange,
        Main_1_MainMenuLayout,
        Main_2_Contents,
        Main_3_Manual,
        Main_4_Options,
        Main_5_Credit,
        Main_6_ProgramExit,

        Contents_0_SceneChange,

        Manual_0_SceneChange,
        Manual_1_ManualPlay,
        Manual_2_StopCheck,
        Manual_3_CreditEnd,
        Manual_4_ReturnMain,

        Options_0_SceneChange,
        Options_1_OptionsMenu,
        Options_s_Options01,
        Options_s_Options02,
        Options_s_Options03,
        Options_s_Options04,
        Options_s_Options05,
        Options_2_SelectOptionsQuit,
        Options_3_ReturnMain,

        Credit_0_SceneChange,
        Credit_1_CreditPlay,
        Credit_2_StopCheck,
        Credit_3_CreditEnd,
        Credit_4_ReturnMain,
    };

    /// <summary>
    /// 입력값 열거변수
    /// </summary>
    public enum InputStatus
    {
        RTrigDown,
        LTrigDown,
        RMenuDown,
        LMenuDown,
        RTracDown,
        LTracDown,
        RGripDown,
        LGripDown
    }

    //씬 위치 확인 열거변수
    public ScenePosition scenePos;
    //씬 상태 확인 열거변수
    public SceneStatus sceneStatus;

    //처음 사용자 확인 변수
    private bool firstTimeUser;
    //스킵 버튼 눌렀는지 확인
    private bool skipPressed;
    //타이머
    private float timer;

    // 씬 관리자 받는 변수들
    public IntroSceneManager introSceneManager;

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);

        Debug.Log("프로그램 시작");

        // 장비 상태코드 갱신은 IntroSceneManager에서 초기값을 잡음
        //scenePos = ScenePosition.Intro;
        //sceneStatus = SceneStatus.Intro___SceneStart;

        //장비 확인 전에 프로그램 시작단계 하나 추가해야함
        //Debug.Log("Intro_0 장비확인");

        firstTimeUser = false;
        skipPressed = false;
        timer = 0;

        // Start : 인트로 씬 장비 확인 Panel 출력
        InputCatch(SceneStatus.Intro___SceneStart);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //잠시 컨트롤러 순수입력 끔
    /// <summary>
    /// InputManager 입력값 전달받음
    /// </summary>
    /// <param name="value">전달받은 문자</param>
    public void InputCatch(InputStatus inputStatus)
    {
        //Debug.Log(value);
        //CheckAndSetStatus(inputStatus);
    }

    /// <summary>
    /// 현재 상태값 확인하고 상태 변환하기
    /// </summary>
    private void CheckAndSetStatus(InputStatus inputStatus)
    {
        //받은 입력값으로 조건처리함
        switch(scenePos)
        {
            case ScenePosition.Intro:
                switch(sceneStatus)
                {
                    case SceneStatus.Intro___SceneStart:
                        introSceneManager = GameObject.FindGameObjectWithTag("INTRO_1").GetComponent<IntroSceneManager>();
                        
                        //SceneStart 상태에 관련된 메서드 수행
                        introSceneManager.statusChange(sceneStatus);

                        Debug.Log("Intro__ 장비 확인");
                        break;

                    case SceneStatus.Intro_0_DeviceCheck:
                        sceneStatus = SceneStatus.Intro_1_UserCheck;
                        Debug.Log("Intro_1 처음 사용자 확인" +
                            "\n RTrig : 처음 사용자 RMenu : 처음 사용자 아님");
                        break;

                    case SceneStatus.Intro_1_UserCheck:
                        if (inputStatus == InputStatus.RTrigDown)        //처음 사용자임
                        {
                            firstTimeUser = true;

                            sceneStatus = SceneStatus.Intro_2_Manual;
                            Debug.Log("Intro_2 처음 사용자 조작법 안내");
                        }
                        else if (inputStatus == InputStatus.RMenuDown)   //처음 사용자 아님
                        {
                            firstTimeUser = false;

                            sceneStatus = SceneStatus.Intro_3_Narration;
                            Debug.Log("Intro_3 Intro 나레이션");
                        }
                        break;

                    case SceneStatus.Intro_2_Manual:
                        sceneStatus = SceneStatus.Intro_3_Narration;
                        Debug.Log("Intro_3 Intro 나레이션");
                        break;

                    case SceneStatus.Intro_3_Narration:
                        if (firstTimeUser == true)   //처음사용자인 경우
                        {
                            sceneStatus = SceneStatus.Intro_4_FTUser;
                            Debug.Log("Intro_4 처음 사용자고 스킵 못하는 경우");
                        }
                        else
                        {
                            sceneStatus = SceneStatus.Intro_5_SkipBtn;
                            Debug.Log("Intro_5 스킵 버튼 누르기 가능" +
                                "\nRTrig : 스킵 버튼 누름 RMenu : 스킵 버튼 안누름");
                        }
                        break;

                    case SceneStatus.Intro_4_FTUser:
                        sceneStatus = SceneStatus.Intro_6_IntroTimeOut;
                        Debug.Log("Intro_6 Intro 나레이션 타임아웃" +
                            "\nRTrig : 타임아웃으로 치기 RMenu : 타임아웃 안됨으로 치기");
                        break;

                    case SceneStatus.Intro_5_SkipBtn:
                        if (inputStatus == InputStatus.RTrigDown)
                        {
                            sceneStatus = SceneStatus.Intro_7_IntroEnd;
                            Debug.Log("Intro_7 Intro 종료");
                        }
                        else if (inputStatus == InputStatus.RMenuDown)
                        {
                            sceneStatus = SceneStatus.Intro_6_IntroTimeOut;
                            Debug.Log("Intro_6 Intro 나레이션 타임아웃" +
                                "\nRTrig : 타임아웃으로 치기 RMenu : 타임아웃 안됨으로 치기");
                        }
                        break;

                    case SceneStatus.Intro_6_IntroTimeOut:
                        if (inputStatus == InputStatus.RTrigDown)
                        {
                            sceneStatus = SceneStatus.Intro_7_IntroEnd;
                            Debug.Log("Intro_7 Intro 종료");
                        }
                        else if (inputStatus == InputStatus.RMenuDown)
                        {
                            sceneStatus = SceneStatus.Intro_3_Narration;
                        }
                        break;

                    case SceneStatus.Intro_7_IntroEnd:
                        Debug.Log("Intro_7 Intro 종료");
                        SceneManager.LoadScene("JH_02MainScene");
                        break;
                }
                break;

            case ScenePosition.Main:
                switch (sceneStatus)
                {
                    case SceneStatus.Main_0_SceneChange:
                        sceneStatus = SceneStatus.Main_1_MainMenuLayout;
                        Debug.Log("Main_1 Main Scene");
                        Debug.Log("RTrig : Contents LTrig : Manual" +
                            "\nLMenu : Options RMenu : Credit LGrip : Exit");
                        break;

                    case SceneStatus.Main_1_MainMenuLayout:
                        switch (inputStatus)
                        {
                            case InputStatus.RTrigDown:
                                sceneStatus = SceneStatus.Main_2_Contents;
                                Debug.Log("Main_2 Contents select");

                                break;

                            case InputStatus.LTrigDown:
                                sceneStatus = SceneStatus.Main_3_Manual;
                                Debug.Log("Main_3 Manual select");

                                break;

                            case InputStatus.LMenuDown:
                                sceneStatus = SceneStatus.Main_4_Options;
                                Debug.Log("Main_4 Options select");

                                break;

                            case InputStatus.RMenuDown:
                                sceneStatus = SceneStatus.Main_5_Credit;
                                Debug.Log("Main_5 Credit select");

                                break;

                            case InputStatus.LGripDown:
                                sceneStatus = SceneStatus.Main_6_ProgramExit;
                                Debug.Log("Main_6 Program exit select");
                                break;
                        }
                        break;

                    case SceneStatus.Main_2_Contents:
                        Debug.Log("Contents Scene load");
                        SceneManager.LoadScene("JH_03Contents");
                        break;

                    case SceneStatus.Main_3_Manual:
                        Debug.Log("Manual Scene load");
                        SceneManager.LoadScene("JH_04Manual");
                        break;

                    case SceneStatus.Main_4_Options:
                        Debug.Log("Options Scene load");
                        SceneManager.LoadScene("JH_05options");
                        break;

                    case SceneStatus.Main_5_Credit:
                        Debug.Log("Credit Scene load");
                        SceneManager.LoadScene("JH_06Credit");
                        break;

                    case SceneStatus.Main_6_ProgramExit:
                        Debug.Log("Program exit.");
                        break;
                }
                break;

            case ScenePosition.Contents:
                switch (sceneStatus)
                {
                    case SceneStatus.Contents_0_SceneChange:
                        Debug.Log("Contents_0 Contents Mode");
                        break;

                }
                break;

            case ScenePosition.Manual:
                switch (sceneStatus)
                {
                    case SceneStatus.Manual_0_SceneChange:
                        Debug.Log("Manual_0 Manual Mode");
                        sceneStatus = SceneStatus.Manual_1_ManualPlay;
                        break;

                    case SceneStatus.Manual_1_ManualPlay:
                        Debug.Log("Manual_1 Credit playing" +
                            "\nRTrig : stop RMenu : timeout RGrip : keep play");
                        sceneStatus = SceneStatus.Manual_2_StopCheck;
                        break;

                    case SceneStatus.Manual_2_StopCheck:
                        switch (inputStatus)
                        {
                            case InputStatus.RTrigDown:
                                Debug.Log("Manual_2 stop playing");
                                sceneStatus = SceneStatus.Manual_3_CreditEnd;
                                break;

                            case InputStatus.RMenuDown:
                                Debug.Log("Manual_2 manual timeout");
                                sceneStatus = SceneStatus.Manual_3_CreditEnd;
                                break;

                            case InputStatus.RGripDown:
                                Debug.Log("Credit_2 keep playing");
                                sceneStatus = SceneStatus.Manual_1_ManualPlay;
                                break;
                        }
                        break;

                    case SceneStatus.Manual_3_CreditEnd:
                        Debug.Log("credit playmode end");
                        sceneStatus = SceneStatus.Manual_4_ReturnMain;
                        break;

                    case SceneStatus.Manual_4_ReturnMain:
                        Debug.Log("return main");
                        SceneManager.LoadScene("JH_02MainScene");
                        break;

                }
                break;

            case ScenePosition.Options:
                switch (sceneStatus)
                {
                    case SceneStatus.Options_0_SceneChange:
                        Debug.Log("Options_0 Options Scene. select data to select options or quit" +
                            "\nRTrig : op01 LTrig : op02 RMenu : op03 LMenu : op04 RGrip : op05 LGrip : returnMain");
                        sceneStatus = SceneStatus.Options_1_OptionsMenu;
                        break;

                    case SceneStatus.Options_1_OptionsMenu:
                        switch (inputStatus)
                        {
                            case InputStatus.RTrigDown:
                                Debug.Log("01 select.");
                                sceneStatus = SceneStatus.Options_s_Options01;
                                break;

                            case InputStatus.LTrigDown:
                                Debug.Log("02 select.");
                                sceneStatus = SceneStatus.Options_s_Options02;
                                break;

                            case InputStatus.RMenuDown:
                                Debug.Log("03 select.");
                                sceneStatus = SceneStatus.Options_s_Options03;
                                break;

                            case InputStatus.LMenuDown:
                                Debug.Log("04 select.");
                                sceneStatus = SceneStatus.Options_s_Options04;
                                break;

                            case InputStatus.RGripDown:
                                Debug.Log("05 select.");
                                sceneStatus = SceneStatus.Options_s_Options05;
                                break;

                            case InputStatus.LGripDown:
                                Debug.Log("return main select.");
                                sceneStatus = SceneStatus.Options_3_ReturnMain;
                                break;
                        }
                        break;

                    case SceneStatus.Options_s_Options01:
                        Debug.Log("option check 01, return options select");
                        sceneStatus = SceneStatus.Options_0_SceneChange;
                        break;

                    case SceneStatus.Options_s_Options02:
                        Debug.Log("option check 02, return options select");
                        sceneStatus = SceneStatus.Options_0_SceneChange;
                        break;

                    case SceneStatus.Options_s_Options03:
                        Debug.Log("option check 03, return options select");
                        sceneStatus = SceneStatus.Options_0_SceneChange;
                        break;

                    case SceneStatus.Options_s_Options04:
                        Debug.Log("option check 04, return options select");
                        sceneStatus = SceneStatus.Options_0_SceneChange;
                        break;

                    case SceneStatus.Options_s_Options05:
                        Debug.Log("option check 05, return options select");
                        sceneStatus = SceneStatus.Options_0_SceneChange;
                        break;

                    case SceneStatus.Options_2_SelectOptionsQuit:
                        Debug.Log("return main scene");
                        sceneStatus = SceneStatus.Options_3_ReturnMain;
                        break;

                    case SceneStatus.Options_3_ReturnMain:
                        Debug.Log("return main");
                        SceneManager.LoadScene("JH_02MainScene");
                        break;


                }
                break;

            case ScenePosition.Credit:
                switch (sceneStatus)
                {
                    case SceneStatus.Credit_0_SceneChange:
                        Debug.Log("Credit_0 Credit Mode");
                        sceneStatus = SceneStatus.Credit_1_CreditPlay;
                        break;

                    case SceneStatus.Credit_1_CreditPlay:
                        Debug.Log("Credit_1 Credit playing" +
                            "\nRTrig : stop RMenu : timeout RGrip : keep play");
                        sceneStatus = SceneStatus.Credit_2_StopCheck;
                        break;

                    case SceneStatus.Credit_2_StopCheck:
                        switch (inputStatus)
                        {
                            case InputStatus.RTrigDown:
                                Debug.Log("Credit_2 stop playing");
                                sceneStatus = SceneStatus.Credit_3_CreditEnd;
                                break;

                            case InputStatus.RMenuDown:
                                Debug.Log("Credit_2 credit timeout");
                                sceneStatus = SceneStatus.Credit_3_CreditEnd;
                                break;

                            case InputStatus.RGripDown:
                                Debug.Log("Credit_2 keep playing");
                                sceneStatus = SceneStatus.Credit_1_CreditPlay;
                                break;
                        }
                        break;

                    case SceneStatus.Credit_3_CreditEnd:
                        Debug.Log("credit playmode end");
                        sceneStatus = SceneStatus.Credit_4_ReturnMain;
                        break;

                    case SceneStatus.Credit_4_ReturnMain:
                        Debug.Log("return main");
                        SceneManager.LoadScene("JH_02MainScene");
                        break;
                }
                break;
        }
    }

    public void InputCatch(SceneStatus sceneStatus)
    {
        CheckAndSetStatus(sceneStatus);
    }

    private void CheckAndSetStatus(SceneStatus sceneStatus)
    {
        //받은 입력값으로 조건처리함
        switch (scenePos)
        {
            case ScenePosition.Intro:
                switch (sceneStatus)
                {
                    case SceneStatus.Intro___SceneStart:
                        Debug.Log("Intro__ 장비 확인");

                        // introSceneManager 로드
                        introSceneManager = GameObject.FindGameObjectWithTag("INTRO_1").GetComponent<IntroSceneManager>();

                        // SceneStart 상태에 관련된 메서드 수행
                        introSceneManager.statusChange(sceneStatus);

                        break;

                    case SceneStatus.Intro_0_DeviceCheck:
                        Debug.Log("Intro_0 장비 확인 완료");

                        //씬 상태 변경
                        sceneStatus = SceneStatus.Intro_0_DeviceCheck;      

                        //장비 확인이 끝난 후 실행할 코드
                        introSceneManager.statusChange(sceneStatus);
                        
                        //장비가 확인된 경우와 확인이 되지 않은 경우를 파악할 코드를 어디에 넣을 것인가에 대한 문제
                        
                        break;

                    case SceneStatus.Intro_1_UserCheck:
                        
                        break;

                    case SceneStatus.Intro_2_Manual:
                        
                        break;

                    case SceneStatus.Intro_3_Narration:
                        
                        break;

                    case SceneStatus.Intro_4_FTUser:
                        
                        break;

                    case SceneStatus.Intro_5_SkipBtn:
                        
                        break;

                    case SceneStatus.Intro_6_IntroTimeOut:
                        
                        break;

                    case SceneStatus.Intro_7_IntroEnd:
                        
                        SceneManager.LoadScene("JH_02MainScene");
                        break;
                }
                break;

            case ScenePosition.Main:
                switch (sceneStatus)
                {
                    case SceneStatus.Main_0_SceneChange:
                        
                        break;

                    case SceneStatus.Main_1_MainMenuLayout:
                        
                        break;

                    case SceneStatus.Main_2_Contents:
                        
                        break;

                    case SceneStatus.Main_3_Manual:
                        
                        break;

                    case SceneStatus.Main_4_Options:
                        
                        break;

                    case SceneStatus.Main_5_Credit:
                        
                        break;

                    case SceneStatus.Main_6_ProgramExit:
                        
                        break;
                }
                break;

            case ScenePosition.Contents:
                switch (sceneStatus)
                {
                    case SceneStatus.Contents_0_SceneChange:
                        
                        break;

                }
                break;

            case ScenePosition.Manual:
                switch (sceneStatus)
                {
                    case SceneStatus.Manual_0_SceneChange:
                        
                        break;

                    case SceneStatus.Manual_1_ManualPlay:
                        
                        break;

                    case SceneStatus.Manual_2_StopCheck:
                        
                        break;

                    case SceneStatus.Manual_3_CreditEnd:
                        
                        break;

                    case SceneStatus.Manual_4_ReturnMain:
                        
                        break;

                }
                break;

            case ScenePosition.Options:
                switch (sceneStatus)
                {
                    case SceneStatus.Options_0_SceneChange:
                        
                        break;

                    case SceneStatus.Options_1_OptionsMenu:
                        
                        break;

                    case SceneStatus.Options_s_Options01:
                        
                        break;

                    case SceneStatus.Options_s_Options02:
                        
                        break;

                    case SceneStatus.Options_s_Options03:
                        
                        break;

                    case SceneStatus.Options_s_Options04:
                        
                        break;

                    case SceneStatus.Options_s_Options05:
                        
                        break;

                    case SceneStatus.Options_2_SelectOptionsQuit:
                        
                        break;

                    case SceneStatus.Options_3_ReturnMain:
                        
                        break;


                }
                break;

            case ScenePosition.Credit:
                switch (sceneStatus)
                {
                    case SceneStatus.Credit_0_SceneChange:
                        
                        break;

                    case SceneStatus.Credit_1_CreditPlay:
                        
                        break;

                    case SceneStatus.Credit_2_StopCheck:
                        
                        break;

                    case SceneStatus.Credit_3_CreditEnd:
                        
                        break;

                    case SceneStatus.Credit_4_ReturnMain:
                        
                        break;
                }
                break;
        }
    }
}
