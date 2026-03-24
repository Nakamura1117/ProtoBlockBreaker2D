using TMPro;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    enum EndStatusName
    {
        GameClear,
        GameOver
    }

    public PlayerController playerBar;
    public GameObject ballPrefub;
    public readonly int defaultLife = 3;

    public TextMeshProUGUI scoreTxt;

    public GameObject lifeImage;

    private EndStatusName endStatus;

    private int maxLife = 5;    // 残機の上限

    private int life = 3;   // 現在の残機
    // 残機取得用のプロパティ
    public int Life
    {
        get { return life; }
    }

    private bool isStart = false;
    public bool IsStart
    {
        get { return isStart; }
    }

    private bool inGame = false;
    // ゲーム中フラグの取得用
    public bool InGame
    {
        get { return inGame; }
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        life = defaultLife;
        inGame = false;
        Time.timeScale = 0;

        GameObject ball = Instantiate(ballPrefub, new Vector3(0, -1, 0), Quaternion.identity);
        ball.GetComponent<AttackBallController>().SetStageManager(GetComponent<StageManager>());
        ball.GetComponent<AttackBallController>().SetPlayerBar(playerBar);
    }

    // Update is called once per frame
    void Update()
    {
        if (InGame == false) Time.timeScale = 0.0f;  // ゲーム中の時TimeScaleを0にする
    }

    public void BallDrop()
    {
        LifeDown();
        if (inGame)
        {
            GameObject ball = Instantiate(ballPrefub, new Vector3(0, -1, 0), Quaternion.identity);
            ball.GetComponent<AttackBallController>().SetStageManager(GetComponent<StageManager>());
            ball.GetComponent<AttackBallController>().SetPlayerBar(playerBar);
        }
    }

    // 残機回復時に使用するメソッド
    public void LifeUp(int num = 1)
    {
        life += num;
    }

    // 残機減少時に使用するメソッド
    public void LifeDown(int num = 1)
    {
        life -= num;
        if (life <= 0)
        {
            Debug.Log("life0");
            GameOver();
        }
    }

    public void GameStart()
    {
        Debug.Log("GameStart");
        inGame = true;
        isStart = true;
        Time.timeScale = 1.0f;
    }

    // ゲームクリア時に実行するメソッド
    public void GameClear()
    {
        if (inGame)
        {
            // Debug.Log("GameClear");
            endStatus = EndStatusName.GameClear;
            GameEnd();
        }
    }

    // ゲームオーバー時に実行するメソッド
    public void GameOver()
    {
        if (inGame)
        {
            // Debug.Log("GameOver");
            endStatus = EndStatusName.GameOver;
            GameEnd();
        }
    }

    private void GameEnd()
    {
        inGame = false;
    }
}
