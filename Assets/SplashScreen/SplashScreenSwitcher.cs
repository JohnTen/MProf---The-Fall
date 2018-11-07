using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SplashScreenSwitcher : MonoBehaviour {
    
    // Use this for initialization
    void Start()
    {
        StartCoroutine(ToMainMenu());
    }

    IEnumerator ToMainMenu()
    {
        yield return new WaitForSeconds(10);
        SceneManager.LoadScene("TitleScreen");
    }
}
