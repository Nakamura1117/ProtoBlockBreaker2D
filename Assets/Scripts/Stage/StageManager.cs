using System.Collections;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    enum EndStatusName
    {
        GameClear,
        GameOver
    }

    public GameObject ballPrefub;
    public readonly int defaultLife = 3;
    
    private EndStatusName endStatus;

    private int maxLife = 5;    // 残機の上限

    private int life = 3;   // 現在の残機

    private bool isStart = false;
    public bool IsStart
    {
        get { return isStart; }
    }

    private bool inGame = false;

    // 残機取得用のプロパティ
    public int Life
    {
        get { return life; }
    }

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
    }

    // Update is called once per frame
    void Update()
    {
        if (InGame) Time.timeScale = 1.0f;  // ゲーム中の時TimeScaleを動かす
    }

    public void BallDrop()
    {
        LifeDown();
        GameObject ball =  Instantiate(ballPrefub, new Vector3(0, -1, 0), Quaternion.identity);
        ball.GetComponent<AttackBallController>().SetStageManager(GetComponent<StageManager>());
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
        if(life <= 0)
        {
            GameOver();
        }
    }

    public void GameStart()
    {
        Debug.Log("GameStart");
        inGame = true;
    }

    // ゲームクリア時に実行するメソッド
    public void GameClear()
    {
        Debug.Log("GameClear");
        inGame = false;
        endStatus = EndStatusName.GameClear;
    }

    // ゲームオーバー時に実行するメソッド
    public void GameOver()
    {
        Debug.Log("GameOver");
        inGame = false;
        endStatus = EndStatusName.GameOver;
    }
}
