using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;

public class SceneToLoadManager : SingletonMono<SceneToLoadManager>
{
    public AssetReference currentScene = null;
    // Start is called before the first frame update  
    void Start()
    {
        EventCenter.Instance.AddEventListener<string, UnityAction>("ChangeScene", ChangeScene);
    }
    private void OnDestroy()
    {
        EventCenter.Instance.RemoveEventListener<string,UnityAction>("ChangeScene", ChangeScene);
    }
    public async void ChangeScene(string targetSceneName,UnityAction action=null)
    {
        await UnloadSceneTask();
        await LoadSceneTask(targetSceneName);
    }
    private async Awaitable UnloadSceneTask()
    {
        await SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
    }
    private async Awaitable LoadSceneTask(string targetSceneName,UnityAction action = null)
    {
        var s = Addressables.LoadSceneAsync(targetSceneName, LoadSceneMode.Additive);
        await s.Task;
        if (s.Status == AsyncOperationStatus.Succeeded)
        {
            SceneManager.SetActiveScene(s.Result.Scene);
            currentScene = new AssetReference(s.Result.Scene.name);
            if(action != null)
            {
                action.Invoke();
            }
        }
        else
        {
            Debug.LogError("Failed to load scene: " + s.OperationException);
        }
    }
}
