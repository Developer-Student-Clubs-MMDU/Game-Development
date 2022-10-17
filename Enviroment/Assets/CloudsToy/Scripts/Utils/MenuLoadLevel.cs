using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuLoadLevel : MonoBehaviour {

    public void LoadLevel(int sceneToLoad)
    {
        SceneManager.LoadScene(sceneToLoad);
    }
}
