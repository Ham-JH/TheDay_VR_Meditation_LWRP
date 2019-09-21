using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroSceneManager : MonoBehaviour
{
    public GameManager GM;

    // Start is called before the first frame update
    void Start()
    {
        GM = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        GM.scenePos = GameManager.ScenePosition.Intro;
        GM.sceneStatus = GameManager.SceneStatus.Intro_0_DeviceCheck;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    

    
}
