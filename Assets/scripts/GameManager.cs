using UnityEngine;

public class GameManager : MonoBehaviour
{
    
    public static GameManager instance;

    public PoolManager pool;
    public Player_Controller player;

    public float gameTime;
    public float maxGameTime = 2 * 10f;

    void Awake()
    {
        // 싱글톤 초기화
        instance = this;
    }
        void Update()
    {
        // 타이머 작동
        gameTime += Time.deltaTime;

        if (gameTime > maxGameTime) {
            gameTime = maxGameTime;
        }
    }
}
