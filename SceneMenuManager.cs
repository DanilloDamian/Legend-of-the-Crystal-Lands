using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneMenuManager : MonoBehaviour
{
    public GameObject[] images;

    public void PlayGame()
    {
        foreach (var image in images)
        {
            image.SetActive(false);
        }
        StartCoroutine(ChangeScene("SampleScene"));
    }

    public void LeaveGame()
    {
        StartCoroutine(Leave());
    }

    IEnumerator ChangeScene(string sceneName)
    {        
        yield return new WaitForSeconds(0.2f);
        SceneManager.LoadScene(sceneName);
    }

    IEnumerator Leave()
    {
        yield return new WaitForSecondsRealtime(0.2f);
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
