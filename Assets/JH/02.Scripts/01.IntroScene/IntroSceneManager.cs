using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IntroSceneManager : MonoBehaviour
{
    public GameManager GM;

    [Header("공유변수")]
    //private bool timerCheck;        //타이머가 돌아갈지 확인하는 코드
    private float timer;            //타이머 코드

    [Header ("장비확인패널")]
    public GameObject Intro__;       //장비확인패널
    public Text Intro__Text;        //장비확인패널 텍스트
    public Button Intro__Btn;       //장비확인패널 인트로 버튼

    // Start is called before the first frame update
    void Start()
    {
        GM = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        GM.scenePos = GameManager.ScenePosition.Intro;
        GM.sceneStatus = GameManager.SceneStatus.Intro___SceneStart;
    }

    // Update is called once per frame
    void Update()
    {
        //타이머가 0이 될 때까지 돌아간다.
        if(timer > 0)
        {
            timer -= Time.deltaTime;

            //첫 타이머가 0 아래가 되는 순간 이벤트 발생
            if(timer <= 0)
            {
                eventTrigger();     //현재 상태값에 따른 이벤트 실행
            }
        }
        
    }

    /// <summary>
    /// GM에서 씬 상태에 따라 이벤트 처리를 요청한다.
    /// </summary>
    /// <param name="status"></param>
    public void statusChange(GameManager.SceneStatus status)
    {
        switch(status)
        {
            case GameManager.SceneStatus.Intro___SceneStart:

                PanelSwitcher(status);      //패널스위처 Intro__ panel on
                ValueSetter(status);
                //버튼 텍스트 변경 고려

                break;

            case GameManager.SceneStatus.Intro_0_DeviceCheck:
                PanelSwitcher(status);
                ValueSetter(status);
                break;

            case GameManager.SceneStatus.Intro_1_UserCheck:

                break;

            case GameManager.SceneStatus.Intro_2_Manual:

                break;

            case GameManager.SceneStatus.Intro_3_Narration:

                break;

            case GameManager.SceneStatus.Intro_4_FTUser:

                break;

            case GameManager.SceneStatus.Intro_5_SkipBtn:

                break;

            case GameManager.SceneStatus.Intro_6_IntroTimeOut:

                break;

            case GameManager.SceneStatus.Intro_7_IntroEnd:

                break;
        }
    }

    private void PanelSwitcher(GameManager.SceneStatus status)
    {
        switch(status)
        {
            case GameManager.SceneStatus.Intro___SceneStart:
                Intro__.SetActive(true);
                
                break;

            case GameManager.SceneStatus.Intro_0_DeviceCheck:

                break;

            case GameManager.SceneStatus.Intro_1_UserCheck:

                break;

            case GameManager.SceneStatus.Intro_2_Manual:

                break;

            case GameManager.SceneStatus.Intro_3_Narration:

                break;

            case GameManager.SceneStatus.Intro_4_FTUser:

                break;

            case GameManager.SceneStatus.Intro_5_SkipBtn:

                break;

            case GameManager.SceneStatus.Intro_6_IntroTimeOut:

                break;

            case GameManager.SceneStatus.Intro_7_IntroEnd:

                break;

        }
    }

    private void ValueSetter(GameManager.SceneStatus status)
    {
        switch (status)
        {
            case GameManager.SceneStatus.Intro___SceneStart:
                Intro__Text.text = "장비 확인";

                break;

            case GameManager.SceneStatus.Intro_0_DeviceCheck:

                break;

            case GameManager.SceneStatus.Intro_1_UserCheck:

                break;

            case GameManager.SceneStatus.Intro_2_Manual:

                break;

            case GameManager.SceneStatus.Intro_3_Narration:

                break;

            case GameManager.SceneStatus.Intro_4_FTUser:

                break;

            case GameManager.SceneStatus.Intro_5_SkipBtn:

                break;

            case GameManager.SceneStatus.Intro_6_IntroTimeOut:

                break;

            case GameManager.SceneStatus.Intro_7_IntroEnd:

                break;

        }
    }

    /// <summary>
    /// 이벤트가 발생하는 상태에 한해서 eventTrigger 발생
    /// </summary>
    public void eventTrigger()
    {
        switch(GM.sceneStatus)
        {
            case GameManager.SceneStatus.Intro___SceneStart:
                //버튼 누르면 여기서 장비확인 코드 실행
                eventCheck();   //흐름 확인용(장비확인코드 실행한 상태)
                break;

            case GameManager.SceneStatus.Intro_0_DeviceCheck:

                break;

            case GameManager.SceneStatus.Intro_1_UserCheck:

                break;

            case GameManager.SceneStatus.Intro_2_Manual:

                break;

            case GameManager.SceneStatus.Intro_3_Narration:

                break;

            case GameManager.SceneStatus.Intro_4_FTUser:

                break;

            case GameManager.SceneStatus.Intro_5_SkipBtn:

                break;

            case GameManager.SceneStatus.Intro_6_IntroTimeOut:

                break;

            case GameManager.SceneStatus.Intro_7_IntroEnd:

                break;
        }
    }

    /// <summary>
    /// eventTrigger를 통해서 결과를 callback하거나 통상적인 과정에 의해 상태 종료가 발생한 모든 경우에 eventCheck를 발생시킨다.
    /// </summary>
    public void eventCheck()
    {
        switch(GM.sceneStatus)
        {
            case GameManager.SceneStatus.Intro___SceneStart:
                //장비확인 끝난 후에 스트림 상태 확인 (콜백)

                    //스트림 잘 붙었을 경우
                Debug.Log("장비가 잘 붙었습니다");
                GM.InputCatch(GameManager.SceneStatus.Intro_0_DeviceCheck);
                
                    //스트림 안 붙었을 경우

                break;

            case GameManager.SceneStatus.Intro_0_DeviceCheck:

                break;

            case GameManager.SceneStatus.Intro_1_UserCheck:

                break;

            case GameManager.SceneStatus.Intro_2_Manual:

                break;

            case GameManager.SceneStatus.Intro_3_Narration:

                break;

            case GameManager.SceneStatus.Intro_4_FTUser:

                break;

            case GameManager.SceneStatus.Intro_5_SkipBtn:

                break;

            case GameManager.SceneStatus.Intro_6_IntroTimeOut:

                break;

            case GameManager.SceneStatus.Intro_7_IntroEnd:

                break;
        }
    }

    
}
