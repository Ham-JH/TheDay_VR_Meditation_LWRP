using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCube : MonoBehaviour
{
    Renderer rend;
    Color originColor;

    // Start is called before the first frame update
    void Start()
    {
        rend = GetComponent<Renderer>();

        originColor = rend.material.GetColor("_Color1_F");
    }

    //데이터 EEG
    //Delta double
    //Theta double
    //Alpha double
    //Beta  double
    //Gamma double
    
    public void Change(double data)
    {
        float _data = (float)data / 100;
        rend.material.SetColor("_Color1_F", new Color(0, 1, _data, 1));
    }
}
