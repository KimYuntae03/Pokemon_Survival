using UnityEngine;

public class RecoveryItem : MonoBehaviour
{
    public float healPercent = 0.3f; // 30% 회복

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) {
            Player_Controller player = collision.GetComponent<Player_Controller>();
            if (player != null) {
                // 최대 체력의 30% 계산 후 회복
                float healAmount = player.maxHp * healPercent;
                player.Heal(healAmount);

                gameObject.SetActive(false); // 먹으면 사라짐 
            }
        }
    }
}
