using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SplashScreenSwitcher : MonoBehaviour {

    public static int SceneNumber;

    // Use this for initialization
    void Start()
    {
        if (SceneNumber == 0)
        {
            StartCoroutine(ToMainMenu());
        }

    }

    IEnumerator ToMainMenu()
    {
        yield return new WaitForSeconds(10);
        SceneNumber = 1;
        SceneManager.LoadScene(1);
    }
}
