using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManualSceneManager : MonoBehaviour
{
    public GameManager GM;

    // Start is called before the first frame update
    void Start()
    {
        GM = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        GM.scenePos = GameManager.ScenePosition.Manual;
        GM.sceneStatus = GameManager.SceneStatus.Manual_0_SceneChange;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
