using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    public void StartGame()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene("GameScene");
    }

    public void QuitGame()
    {
        Debug.Log("ÉQÅ[ÉÄÇèIóπÇµÇ‹Ç∑");

        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}