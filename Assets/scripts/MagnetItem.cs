using UnityEngine;

public class MagnetItem : MonoBehaviour
{   
    public float floatSpeed = 3f;
    public float floatAmplitude = 0.15f;
    Vector3 startPos;
    
    void Start()
    {
        startPos = transform.localPosition; 
    }

    void Update()
    {
        float newY = Mathf.Sin(Time.time * floatSpeed) * floatAmplitude;
        transform.localPosition = startPos + new Vector3(0, newY, 0);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {

            GameManager.instance.PlayMagnetSfx();
            
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
