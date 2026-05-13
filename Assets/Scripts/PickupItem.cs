using UnityEngine;

public class PickupItem : MonoBehaviour
{
    [Header("Что даёт этот предмет")]
    [SerializeField] public GameObject handPrefab;  // Префаб для рук (Pistol_Hand)
    [SerializeField] public string itemName = "Предмет";

    [Header("Радиус подбора")]
    [SerializeField] public float pickupRange = 2f;

    private Transform player;
    private ItemHolder itemHolder;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);
        if (distance <= pickupRange && Input.GetKeyDown(KeyCode.E))
        {
            TryPickup();
        }
    }

    void TryPickup()
    {
        if (player == null) return;

        itemHolder = player.GetComponent<ItemHolder>();
        if (itemHolder == null)
        {
            Debug.LogError("У игрока нет компонента ItemHolder!");
            return;
        }

        if (itemHolder.AddItem(handPrefab, itemName))
        {
            Debug.Log($"Подобрано: {itemName}");
            Destroy(gameObject);
        }
        else
        {
            Debug.Log("Инвентарь полон!");
        }
    }

    // Визуализация радиуса в редакторе
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, pickupRange);
    }
}