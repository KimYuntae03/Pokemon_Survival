using UnityEngine;
using System.Collections;

public class RecoveryItem : MonoBehaviour
{
    public float healPercent = 0.3f; // 30% 회복

    public float floatSpeed = 3f;   // 떠다니는 속도
    public float floatAmplitude = 0.15f;
    Vector3 startLocalPos;

    void Start()
    {
        startLocalPos = transform.localPosition; 
    }

    void Update()
    {
        float newY = Mathf.Sin(Time.time * floatSpeed) * floatAmplitude;
        transform.localPosition = startLocalPos + new Vector3(0, newY, 0);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) {
            Player_Controller player = collision.GetComponent<Player_Controller>();
            if (player != null) {
                // 최대 체력의 30% 계산 후 회복
                float healAmount = player.maxHp * healPercent;
                player.Heal(healAmount);
                player.OnHeal();

                GameManager.instance.StartCoroutine(GameManager.instance.PlayBerrySfxSequence());

                transform.SetParent(GameManager.instance.pool.transform);
                gameObject.SetActive(false); // 먹으면 사라짐 
            }
        }
    }
}
