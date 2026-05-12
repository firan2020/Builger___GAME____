using System.Collections;
using UnityEngine;
using System.IO;
using System.Linq;

public class SaveSystem : MonoBehaviour
{
    public static SaveSystem Instance;
    private string savesFolder;
    private string currentSavePath;

    [System.Serializable]
    class SaveData
    {
        public float px, py, pz;
        public float rx, ry, rz, rw;
        public bool started;
        public string timestamp;
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

        // Путь: Документы/My Games/НазваниеИгры/Saves/
        string documents = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
        savesFolder = Path.Combine(documents, "My Games", Application.productName, "Saves");

        if (!Directory.Exists(savesFolder))
            Directory.CreateDirectory(savesFolder);

        Debug.Log($"Папка сохранений: {savesFolder}");
    }

    void Start()
    {
        StartCoroutine(AutoSave());
    }

    // Сохранить игру (создаёт новый файл)
    public void Save(GameObject player)
    {
        SaveData data = new SaveData();
        data.px = player.transform.position.x;
        data.py = player.transform.position.y;
        data.pz = player.transform.position.z;
        data.rx = player.transform.rotation.x;
        data.ry = player.transform.rotation.y;
        data.rz = player.transform.rotation.z;
        data.rw = player.transform.rotation.w;
        data.started = true;
        data.timestamp = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        // Находим номер нового сохранения
        int saveNumber = GetNextSaveNumber();
        string savePath = Path.Combine(savesFolder, $"save_{saveNumber:D3}.json");

        File.WriteAllText(savePath, JsonUtility.ToJson(data));
        currentSavePath = savePath;

        Debug.Log($"Игра сохранена: {savePath}");
    }

    // Загрузить последнее сохранение
    public bool LoadLatest(GameObject player)
    {
        string latestSave = GetLatestSaveFile();

        if (latestSave == null)
        {
            Debug.Log("Нет сохранений!");
            return false;
        }

        string json = File.ReadAllText(latestSave);
        SaveData data = JsonUtility.FromJson<SaveData>(json);

        player.transform.position = new Vector3(data.px, data.py, data.pz);
        player.transform.rotation = new Quaternion(data.rx, data.ry, data.rz, data.rw);
        currentSavePath = latestSave;

        Debug.Log($"Загружено: {latestSave} ({data.timestamp})");
        return true;
    }

    // Проверка, есть ли сохранения (для кнопки Продолжить)
    public bool HasAnySave()
    {
        return Directory.GetFiles(savesFolder, "save_*.json").Length > 0;
    }

    // Получить путь к самому новому файлу
    private string GetLatestSaveFile()
    {
        var files = Directory.GetFiles(savesFolder, "save_*.json");
        if (files.Length == 0) return null;

        return files.OrderByDescending(f => File.GetLastWriteTime(f)).First();
    }

    // Получить номер следующего сохранения
    private int GetNextSaveNumber()
    {
        var files = Directory.GetFiles(savesFolder, "save_*.json");
        if (files.Length == 0) return 1;

        int maxNumber = 0;
        foreach (string file in files)
        {
            string name = Path.GetFileNameWithoutExtension(file);
            string numberStr = name.Replace("save_", "");
            if (int.TryParse(numberStr, out int num) && num > maxNumber)
                maxNumber = num;
        }
        return maxNumber + 1;
    }

    // Новая игра (создаёт пустое сохранение)
    public void NewGame(GameObject player)
    {
        player.transform.position = Vector3.zero;
        player.transform.rotation = Quaternion.identity;
        Save(player);
    }

    // Автосохранение каждые 2 минуты
    IEnumerator AutoSave()
    {
        while (true)
        {
            yield return new WaitForSeconds(120);
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                Save(player);
        }
    }
}