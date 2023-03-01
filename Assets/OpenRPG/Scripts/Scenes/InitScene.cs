using System.Collections;
using PlayFab;
using PlayFab.ClientModels;
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
        
        PlayFabClientAPI.GetTitleNews(new GetTitleNewsRequest(), HandleOnNewsCallSuccess, null);
    }

    private void HandleOnNewsCallSuccess(GetTitleNewsResult result)
    {
        if (result.News.Count == 0)
            return;
        if (!UIRuntime.TryShowWindow("UIWelcome", out UIWelcome welcome))
            return;

        string message = "";
        foreach (var newsItem in result.News)
        {
            message += $"<h2>{newsItem.Title}</h2>\n<small>{newsItem.Timestamp}</small>\n{newsItem.Body}\n\n\n\n";
        }

        welcome.Build(message);
        welcome.OnShow += OnReadmeOnShow;
        welcome.AnimatedShow();
    }


    private void OnReadmeOnShow()
    {
        var currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadSceneAsync(currentScene.buildIndex + 1);
    }
}
