using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonScripts : MonoBehaviour
{
    [SerializeField] MenuManager menuManager;
    public void NewGame(string sceneName)
    {
        if (menuManager != null)
            menuManager.OnNewGameClick(sceneName);
    }

    public void ContinueGame(string sceneName)
    {
        if (menuManager != null)
            menuManager.OnContinueClick(sceneName);
    }

    public GameObject settingsPanel;
    public GameObject menuPanel;
    public void OpenSettings()
    {
        settingsPanel.SetActive(true);
        menuPanel.SetActive(false);
    }

    public void OpenMenu()
    {
        settingsPanel.SetActive(false);
        menuPanel.SetActive(true);
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }


}
