using UnityEngine;

public class MagnetItem : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            GameObject[] gems = GameObject.FindGameObjectsWithTag("Gem");

            foreach (GameObject gem in gems)
            {
                Item itemScript = gem.GetComponent<Item>();
                if (itemScript != null && gem.activeSelf)
                {
                    itemScript.StartMagnet(); //모든 보석 끌어당김
                }
            }
            Destroy(gameObject); 
        }
    }
}
