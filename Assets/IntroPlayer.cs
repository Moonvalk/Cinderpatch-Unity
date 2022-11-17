using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class IntroPlayer : MonoBehaviour
{
    private VideoPlayer _videoPlayer;

    private void Awake()
    {
        // Screen.fullScreenMode = FullScreenMode.MaximizedWindow;
        // Screen.fullScreen = !Screen.fullScreen;
    }

    // Start is called before the first frame update
    private void Start()
    {
        this._videoPlayer = this.GetComponent<VideoPlayer>();
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        if ((this._videoPlayer.frame) > 0 && (this._videoPlayer.isPlaying == false))
        {
            SceneManager.LoadScene("Menu", LoadSceneMode.Single);
        }
    }
}
