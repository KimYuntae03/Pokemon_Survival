using UnityEngine;

public class DamageText : MonoBehaviour 
{
    public float moveSpeed = 2.0f; // Text올라가는 속도
    public float destroyTime = 0.5f; // 사라지는 시간

    void Start() {
        Destroy(gameObject, destroyTime); // 생성 후 지정된 시간 뒤 파괴
    }

    void Update() {
        transform.Translate(Vector3.up * moveSpeed * Time.deltaTime); 
        //DamageText 위로 이동
    }
    void Awake()
    {
        MeshRenderer meshRenderer = GetComponentInChildren<MeshRenderer>();

        if (meshRenderer != null)
        {
            meshRenderer.sortingOrder = 10; //레이어를 높여서 맨앞으로 오도록 함
        }
    }
}
