using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionsSceneManager : MonoBehaviour
{
    public GameManager GM;

    // Start is called before the first frame update
    void Start()
    {
        GM = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        GM.scenePos = GameManager.ScenePosition.Options;
        GM.sceneStatus = GameManager.SceneStatus.Options_0_SceneChange;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
