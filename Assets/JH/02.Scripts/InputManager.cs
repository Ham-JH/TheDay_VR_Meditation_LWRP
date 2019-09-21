using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public GameManager GM;

    /// <summary>
    /// 매 프레임마다 입력이 발생했는지 확인해주고 확인되면 입력값 GameManager로 전달
    /// </summary>
    void Update()
    {
        string str = Input.inputString;
        if(str != "")
        {
            str = str.Substring(0, 1);
            GM.InputCatch(str);     //Gamemanager로 입력 문자 전달
        }
    }
}
