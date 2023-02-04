using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using VGDevs;

public class InitScene : MonoBehaviour
{
    IEnumerator Start()
    {
        yield return new WaitUntil(() => GameResources.Runtime.IsReady);
        yield return new WaitUntil(() => GameResources.UIRuntime.IsReady);
        yield return new WaitUntil(() => GameResources.UserRuntime.IsReady);
        
        UIRuntime.TryShowWindow("UIWelcome", out UIWelcome welcome);

        welcome.OnShow += OnReadmeOnShow;
        welcome.AnimatedShow();
    }

    private void OnReadmeOnShow()
    {
        var currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadSceneAsync(currentScene.buildIndex + 1);
    }
}
