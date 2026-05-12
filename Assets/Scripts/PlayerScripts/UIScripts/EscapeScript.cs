using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EscapeScript : MonoBehaviour
{
    [SerializeField] GameObject menuPanel;
    [SerializeField] FirstPersonLook cameraController;
    private bool isTrue = false;
    private void Update()
    {
        Escape();
    }

    void Escape()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isTrue = !isTrue;
            if (isTrue == true)
            {
                menuPanel.SetActive(true);
                Time.timeScale = 0f;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                cameraController.SetPaused(true);
            }
            else
            {
                isTrue = false;
                menuPanel.SetActive(false);
                Time.timeScale = 1f;
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                cameraController.SetPaused(false);
            }
        }
    }

    public void resumeButton()
    {
        isTrue = false;
        menuPanel.SetActive(false);
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        cameraController.SetPaused(false);
    }

    public void exitButton(string sceneName)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneName);
    }
}