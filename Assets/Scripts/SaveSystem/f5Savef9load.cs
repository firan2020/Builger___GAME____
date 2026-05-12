using UnityEngine;

public class f5Savef9load : MonoBehaviour
{
    void Start()
    {
        if (SaveSystem.Instance != null && SaveSystem.Instance.HasAnySave())
        {
            SaveSystem.Instance.LoadLatest(gameObject);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F5))
        {
            SaveSystem.Instance.Save(gameObject);
        }

        if (Input.GetKeyDown(KeyCode.F9))
        {
            SaveSystem.Instance.LoadLatest(gameObject);
        }
    }
}
