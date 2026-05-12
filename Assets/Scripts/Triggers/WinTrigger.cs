using System.Collections;
using UnityEngine;

public class WinTrigger : MonoBehaviour
{
    [SerializeField] GameObject winPanel;
    [SerializeField] float delayTime = 5f;
    private void OnTriggerEnter(Collider other)
    {
        winPanel.SetActive(true);
        StartCoroutine(HideMenuAfterDelay());
    }

    IEnumerator HideMenuAfterDelay()
    {
        yield return new WaitForSeconds(delayTime);

        winPanel.SetActive(false);
    }
}
