using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class SplashVideoPlayer : MonoBehaviour {

    public RawImage rawImage;
    public VideoPlayer videoPlayer;

	// Use this for initialization
	void Start ()
    {
        StartCoroutine(PlayVideo());
	}

    IEnumerator PlayVideo()
    {
       videoPlayer.Prepare();
        while (!videoPlayer.isPrepared)
        {
			rawImage.color = Color.black;
			yield return null;
		}
		rawImage.color = Color.white;

		rawImage.texture = videoPlayer.texture;
        videoPlayer.Play();
    }
}
