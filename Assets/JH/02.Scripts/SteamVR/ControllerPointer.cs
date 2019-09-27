using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Valve.VR;

public class ControllerPointer : MonoBehaviour
{
    private SteamVR_Behaviour_Pose pose;
    private SteamVR_Input_Sources rightHand;
    public LineRenderer line;
    //public Transform tr;

    public SteamVR_Action_Boolean RTrigger;
    public SteamVR_Action_Boolean LTrigger;

    private float maxDistance = 20.0f;
    private float distance = 20.0f;
    public Color defaultColor = Color.white;

    private RaycastHit hit;
    private Ray ray;

    //이벤트 처리 델리게이트(레이저 들어감)
    public delegate void LaserEnterHandler(GameObject btn);
    public static event LaserEnterHandler OnLaserEnter;

    //이벤트 처리 델리게이트(레이저 나감)
    public delegate void LaserExitHandler();
    public static event LaserExitHandler OnLaserExit;

    //line 그릴지 안그릴지 판단하는 변수
    public bool lineEnableHandler;

    // Start is called before the first frame update
    void Start()
    {
        pose = GetComponent<SteamVR_Behaviour_Pose>();
        rightHand = pose.inputSource;
        //tr = transform.parent.transform;

        RTrigger = SteamVR_Actions.default_RTrigger;
        LTrigger = SteamVR_Actions.default_LTrigger;

        lineEnableHandler = true;

        CreateLine();
    }

    /// <summary>
    /// 선 그리기
    /// </summary>
    void CreateLine()
    {
        line = this.gameObject.AddComponent<LineRenderer>();
        line.useWorldSpace = false;
        line.receiveShadows = false;

        line.positionCount = 2;
        line.SetPosition(0, Vector3.zero);
        line.SetPosition(1, new Vector3(0, 0, distance));
        line.startWidth = 0.05f;
        line.endWidth = 0.005f;
        line.material = new Material(Shader.Find("Unlit/Color"));
        line.material.color = defaultColor;
    }

    private GameObject prevButton = null;

    // Update is called once per frame
    void Update()
    {

        //line.SetPosition(1, new Vector3(0, 0, Random.Range(0, 10)));
        if (lineEnableHandler)
        {
            line.enabled = true;

            ray = new Ray(transform.position, transform.forward);
            if (Physics.Raycast(ray, out hit, maxDistance))
            {
                //Debug.Log(hit.collider.gameObject);
                distance = hit.distance;
                line.SetPosition(1, new Vector3(0, 0, distance));


                prevButton = hit.collider.gameObject;
                //Debug.Log(hit.collider.gameObject);

                OnLaserEnter(hit.collider.gameObject);

                //오른쪽 컨트롤러에서 버튼 보면서 클릭
                if (RTrigger.stateDown && hit.collider.gameObject.tag == "BUTTON")
                {
                    ExecuteEvents.Execute(hit.collider.gameObject
                                        , new PointerEventData(EventSystem.current)
                                        , ExecuteEvents.pointerClickHandler);
                }
                //else if(LTrigger.stateDown)
                //{
                //    ExecuteEvents.Execute(hit.collider.gameObject
                //                        , new PointerEventData(EventSystem.current)
                //                        , ExecuteEvents.pointerClickHandler);
                //}
            }
            else
            {
                distance = 20f;
                line.SetPosition(1, new Vector3(0, 0, distance));

                if (prevButton != null)
                {
                    OnLaserExit();
                    prevButton = null;
                }
            }

        }
        else
        {
            line.enabled = false;
        }
        //raycast를 쐈음

    }
}
