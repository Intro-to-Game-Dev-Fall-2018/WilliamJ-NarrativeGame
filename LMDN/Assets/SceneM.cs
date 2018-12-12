using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneM : MonoBehaviour {

    public Button start;

	// Use this for initialization
	void Start () {
        start.onClick.AddListener(delegate
        {
            startGame();
        });
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void startGame()
    {
        SceneManager.LoadScene("LMDN-Minimal");
    }

}
