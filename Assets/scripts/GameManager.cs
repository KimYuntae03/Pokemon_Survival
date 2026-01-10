using UnityEngine;

public class GameManager : MonoBehaviour
{
    
    public static GameManager instance;

    public PoolManager pool;

    void Awake()
    {
        // 싱글톤 초기화
        instance = this;
    }
}
