using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonEvent : MonoBehaviour
{
    private void OnEnable()
    {
        ControllerPointer.OnLaserEnter += LaserEnter;
        ControllerPointer.OnLaserExit += LaserExit;
    }

    private void OnDisable()
    {
        ControllerPointer.OnLaserEnter -= LaserEnter;
        ControllerPointer.OnLaserExit -= LaserExit;
    }

    void LaserEnter(GameObject btn)
    {
        if(this.gameObject == btn)
        {
            ExecuteEvents.Execute(this.gameObject
                                , new PointerEventData(EventSystem.current)
                                , ExecuteEvents.pointerEnterHandler);
        }
    }

    void LaserExit()
    {
        ExecuteEvents.Execute(this.gameObject
                                , new PointerEventData(EventSystem.current)
                                , ExecuteEvents.pointerExitHandler);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
