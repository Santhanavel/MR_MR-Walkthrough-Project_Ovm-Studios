using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManager : MonoBehaviour
{

    public bool isLoading = false;

    public void LoadScene(int index)
    {
        if (isLoading) return; // ignore extra clicks
        isLoading = true;
        StartCoroutine(LoadSceneWithSync(index));
    }
    // Coroutine for async scene loading with progress
    private IEnumerator LoadSceneWithSync(int sceneIndex)
    {
        AsyncOperation asyncLoad = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneIndex);
        asyncLoad.allowSceneActivation = false; // optional: wait before activating

        // Show loading progress if needed
        while (!asyncLoad.isDone)
        {
            float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f);
            Debug.Log("Loading Progress: " + (progress * 100f) + "%");

            // Once loading is nearly complete, activate scene
            if (asyncLoad.progress >= 0.9f)
            {
                // Here you can show "Press any key to continue" or auto-activate
                asyncLoad.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}
