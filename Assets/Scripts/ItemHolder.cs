using UnityEngine;
using System.Collections.Generic;

public class ItemHolder : MonoBehaviour
{
    [Header("Родитель для предметов в руке")]
    [SerializeField] public Transform handTransform;

    [Header("Список предметов (всегда слот 0 - кулаки)")]
    public List<GameObject> items = new List<GameObject>();
    public List<string> itemNames = new List<string>();

    private int currentIndex = 0;

    void Start()
    {
        if (items.Count == 0)
        {
            items.Add(null);
            itemNames.Add("Кулаки");
        }
        currentIndex = 0;
    }

    void Update()
    {
        // Переключение цифрами 1-5
        for (int i = 0; i < 5; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i) && i < items.Count)
                SwitchToItem(i);
        }

        // Колесо мыши
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll > 0) SwitchToItem((currentIndex + 1) % items.Count);
        if (scroll < 0) SwitchToItem((currentIndex - 1 + items.Count) % items.Count);
    }

    public void SwitchToItem(int index)  // 👈 ИСПРАВЛЕНО: private -> public
    {
        if (index >= items.Count) return;
        if (items[index] == null && index != 0) return;

        // Выключаем текущий
        if (currentIndex < items.Count && items[currentIndex] != null)
            items[currentIndex].SetActive(false);

        currentIndex = index;

        // Включаем новый
        if (items[currentIndex] != null)
            items[currentIndex].SetActive(true);
    }

    public bool AddItem(GameObject itemPrefab, string itemName)
    {
        if (items.Count >= 5) return false;

        GameObject newItem = Instantiate(itemPrefab, handTransform);
        newItem.transform.localPosition = Vector3.zero;
        newItem.transform.localRotation = Quaternion.identity;
        newItem.SetActive(false);

        items.Add(newItem);
        itemNames.Add(itemName);

        return true;
    }

    public void ClearItems()
    {
        for (int i = 1; i < items.Count; i++)
        {
            if (items[i] != null) Destroy(items[i]);
        }
        items.Clear();
        itemNames.Clear();
        items.Add(null);
        itemNames.Add("Кулаки");
        currentIndex = 0;
    }

    public List<string> GetItemNames()
    {
        return itemNames;
    }

    public int GetCurrentIndex() => currentIndex;
}