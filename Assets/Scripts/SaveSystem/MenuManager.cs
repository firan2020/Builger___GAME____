using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField] Button newGameButton;
    [SerializeField] Button continueButton;

    void Awake()
    {
        Invoke("CheckSave", 0.1f);
    }

    void CheckSave()
    {
        bool hasSave = SaveSystem.Instance != null && SaveSystem.Instance.HasAnySave();

        continueButton.gameObject.SetActive(hasSave);
        newGameButton.gameObject.SetActive(!hasSave);

        Debug.Log($"≈сть сохранени€: {hasSave}");
    }

    public void OnNewGameClick(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
        // —охранение создастс€ при первом автосохранении или вручную
    }

    public void OnContinueClick(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}