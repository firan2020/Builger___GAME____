using System.Collections;
using UnityEngine;
using System.IO;
using System.Linq;
using System.Collections.Generic;

public class SaveSystem : MonoBehaviour
{
    public static SaveSystem Instance;
    private string savesFolder;

    [Header("WORLD префабы (лежат на земле, с PickupItem)")]
    [SerializeField] public GameObject[] worldPrefabs;

    [Header("HAND префабы (в руках, с Attack)")]
    [SerializeField] public GameObject[] handPrefabs;

    [System.Serializable]
    class SaveData
    {
        public float px, py, pz;
        public float rx, ry, rz, rw;
        public bool started;
        public string timestamp;
        public string[] handItemNames;
        public int currentItemSlot;
        public SerializablePickup[] worldPickups;
    }

    [System.Serializable]
    class SerializablePickup
    {
        public float x, y, z;
        public string prefabName;
    }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        string documents = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
        savesFolder = Path.Combine(documents, "My Games", Application.productName, "Saves");
        if (!Directory.Exists(savesFolder)) Directory.CreateDirectory(savesFolder);
        Debug.Log($"[SaveSystem] Папка сохранений: {savesFolder}");
    }

    void Start() => StartCoroutine(AutoSave());

    public void Save(GameObject player)
    {
        Debug.Log("========== [SAVE] НАЧАЛО СОХРАНЕНИЯ ==========");
        SaveData data = new SaveData();

        // Игрок
        data.px = player.transform.position.x;
        data.py = player.transform.position.y;
        data.pz = player.transform.position.z;
        data.rx = player.transform.rotation.x;
        data.ry = player.transform.rotation.y;
        data.rz = player.transform.rotation.z;
        data.rw = player.transform.rotation.w;
        data.started = true;
        data.timestamp = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        // Инвентарь
        ItemHolder holder = player.GetComponent<ItemHolder>();
        if (holder != null)
        {
            data.handItemNames = holder.GetItemNames().ToArray();
            data.currentItemSlot = holder.GetCurrentIndex();
        }

        // Предметы на земле (очищаем имена от (Clone) и (цифра))
        GameObject[] pickups = GameObject.FindGameObjectsWithTag("Pickup");
        data.worldPickups = new SerializablePickup[pickups.Length];
        for (int i = 0; i < pickups.Length; i++)
        {
            string cleanName = CleanGameObjectName(pickups[i].name);
            data.worldPickups[i] = new SerializablePickup();
            data.worldPickups[i].x = pickups[i].transform.position.x;
            data.worldPickups[i].y = pickups[i].transform.position.y;
            data.worldPickups[i].z = pickups[i].transform.position.z;
            data.worldPickups[i].prefabName = cleanName;
            Debug.Log($"[SAVE] Предмет: {cleanName} на ({data.worldPickups[i].x:F2}, {data.worldPickups[i].y:F2}, {data.worldPickups[i].z:F2})");
        }

        int saveNumber = GetNextSaveNumber();
        string savePath = Path.Combine(savesFolder, $"save_{saveNumber:D3}.json");
        File.WriteAllText(savePath, JsonUtility.ToJson(data));
        Debug.Log($"[SAVE] Сохранено: {savePath}");
        Debug.Log("========== [SAVE] СОХРАНЕНИЕ ЗАВЕРШЕНО ==========");
    }

    // Вспомогательный метод очистки имени от (Clone) и (цифра)
    private string CleanGameObjectName(string rawName)
    {
        string cleaned = rawName.Replace("(Clone)", "");
        int lastSpace = cleaned.LastIndexOf(' ');
        if (lastSpace > 0 && cleaned[lastSpace + 1] == '(' && cleaned[cleaned.Length - 1] == ')')
        {
            cleaned = cleaned.Substring(0, lastSpace);
        }
        return cleaned;
    }

    public bool LoadLatest(GameObject player)
    {
        Debug.Log("========== [LOAD] НАЧАЛО ЗАГРУЗКИ ==========");
        string latestSave = GetLatestSaveFile();
        if (latestSave == null) return false;

        string json = File.ReadAllText(latestSave);
        SaveData data = JsonUtility.FromJson<SaveData>(json);

        // Загружаем игрока
        player.transform.position = new Vector3(data.px, data.py, data.pz);
        player.transform.rotation = new Quaternion(data.rx, data.ry, data.rz, data.rw);

        // Загружаем инвентарь
        ItemHolder holder = player.GetComponent<ItemHolder>();
        if (holder != null && data.handItemNames != null)
        {
            holder.ClearItems();
            for (int i = 0; i < data.handItemNames.Length; i++)
            {
                string itemName = data.handItemNames[i];
                if (!string.IsNullOrEmpty(itemName) && itemName != "Кулаки")
                {
                    GameObject handPrefab = FindHandPrefabByName(itemName);
                    if (handPrefab != null)
                        holder.AddItem(handPrefab, itemName);
                }
            }
            holder.SwitchToItem(data.currentItemSlot);
        }

        // Удаляем старые предметы с земли
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Pickup"))
            Destroy(obj);

        // Спавним предметы с земли
        if (data.worldPickups != null)
        {
            foreach (var pickup in data.worldPickups)
            {
                GameObject worldPrefab = FindWorldPrefabByName(pickup.prefabName);
                if (worldPrefab != null)
                {
                    Vector3 pos = new Vector3(pickup.x, pickup.y, pickup.z);
                    Instantiate(worldPrefab, pos, Quaternion.identity);
                }
            }
        }

        Debug.Log($"[LOAD] Загрузка завершена: {latestSave}");
        Debug.Log("========== [LOAD] ЗАГРУЗКА ЗАВЕРШЕНА ==========");
        return true;
    }

    private GameObject FindHandPrefabByName(string name)
    {
        foreach (var prefab in handPrefabs)
            if (prefab != null && prefab.name == name) return prefab;
        Debug.LogError($"HAND-префаб '{name}' не найден!");
        return null;
    }

    private GameObject FindWorldPrefabByName(string name)
    {
        foreach (var prefab in worldPrefabs)
            if (prefab != null && prefab.name == name) return prefab;
        Debug.LogError($"WORLD-префаб '{name}' не найден!");
        return null;
    }

    public bool HasAnySave() => Directory.GetFiles(savesFolder, "save_*.json").Length > 0;

    private string GetLatestSaveFile()
    {
        var files = Directory.GetFiles(savesFolder, "save_*.json");
        return files.Length == 0 ? null : files.OrderByDescending(f => File.GetLastWriteTime(f)).First();
    }

    private int GetNextSaveNumber()
    {
        var files = Directory.GetFiles(savesFolder, "save_*.json");
        if (files.Length == 0) return 1;
        int maxNumber = 0;
        foreach (string file in files)
        {
            string numStr = Path.GetFileNameWithoutExtension(file).Replace("save_", "");
            if (int.TryParse(numStr, out int num) && num > maxNumber) maxNumber = num;
        }
        return maxNumber + 1;
    }

    public void NewGame(GameObject player)
    {
        player.transform.position = Vector3.zero;
        player.transform.rotation = Quaternion.identity;
        ItemHolder holder = player.GetComponent<ItemHolder>();
        if (holder != null) holder.ClearItems();
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Pickup")) Destroy(obj);
        Save(player);
    }

    IEnumerator AutoSave()
    {
        while (true)
        {
            yield return new WaitForSeconds(120);
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) Save(player);
        }
    }
}