using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageManager : MonoBehaviour
{
    // ゲーム終了時のステータス管理用（switchで使用するため）
    public enum EndStatusName
    {
        GameClear,  // ゲームクリア
        GameOver    // ゲームオーバー
    }

    public PlayerController playerBar;  // playerの動かすバー
    public GameObject ballPrefub;       // ボール
    public Vector3 defaultBallPos = new Vector3(0, -1, 0);  // ボールを生成する初期位置

    public readonly int defaultLife = 3;    // 残機の初期値

    public GameObject UI;   // UI管理のオブジェクト
    public TextMeshProUGUI scoreTxt;    // スコア表示用テキスト
    public GameObject ui_Menu;

    public GameObject spriteLife;    // 演出に使用するハートの画像
    public GameObject ui_LifeGage;  // 残機をUIに表示するための目印
    public GameObject ui_lifePrefub;    // UIに表示する残機の元画像

    public GameObject objGameClear;     // ゲームクリア時に表示する画像
    public GameObject objGameOver;     // ゲームオーバー時に表示する画像

    private GameObject[] ui_lifeObj;    // UIで残機を表示するための実態（配列）

    private int currentScore;   // ステージのスコア管理用
    public int Score    // スコアを外部から参照する用の変数
    {
        get { return currentScore; }
    }

    public string Name { get { return SceneManager.GetActiveScene().name; } }
    private EndStatusName endStatus;    // 終了ステータスを保持する用の変数
    public EndStatusName EndStatus { get { return endStatus; } }
    private int maxLife = 5;    // 残機の上限

    private int life = 3;   // 現在の残機
    // 残機取得用のプロパティ
    public int Life
    {
        get { return life; }
    }

    // スタート処理が終わっているか記録するためのフラグ
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
        Time.timeScale = 0; // スタート処理が終わるまで、ゲーム時間を止めておく
        inGame = false; // スタート処理が終わるまで、ゲーム中ではないとフラグを設定

        ui_Menu.SetActive(false);    // メニューを無効化する
        life = defaultLife; // 残機を初期値に設定する
        ui_lifeObj = new GameObject[life];  // 残機表示用の配列を生成
        float setPosX = 0;  // 残機表示の位置の値を初期化

        // 残機表示用のオブジェクトを生成、設定する
        for (int i = 0; i < ui_lifeObj.Length; i++)
        {
            // 残機表示のオブジェクトを生成
            ui_lifeObj[i] = Instantiate(
                ui_lifePrefub,
                Vector3.zero,
                Quaternion.identity,
                ui_LifeGage.transform
            );

            // 作成したオブジェクトの位置をUI用に調整
            ui_lifeObj[i].GetComponent<RectTransform>().anchoredPosition = new Vector3(setPosX, 0, 0);
            ui_lifeObj[i].SetActive(true);
            setPosX += 150;
        }

        GameObject ball = Instantiate(ballPrefub, defaultBallPos, Quaternion.identity);     // ボールを生成する
        ball.GetComponent<AttackBallController>().SetStageManager(GetComponent<StageManager>()); // ボールにStageManagerをセットする
        ball.GetComponent<AttackBallController>().SetPlayerBar(playerBar);  // ボールにバーの情報をセットする
    }

    // Update is called once per frame
    void Update()
    {
        if (InGame == false)
        {
            Time.timeScale = 0.0f;  // ゲーム中の時TimeScaleを0にする
        }

    }

    // ボールが落ちた際の処理
    public void BallDrop()
    {
        // ゲーム中なら処理を実行
        if (inGame)
        {
            SoundManager.Instance.PlaySE(SoundManager.Instance.seBallDrop);
            LifeDown();     // 残機を減らす

            GameObject ball = Instantiate(ballPrefub, defaultBallPos, Quaternion.identity);      // 新しいボールを生成する
            ball.GetComponent<AttackBallController>().SetStageManager(GetComponent<StageManager>());    // 生成したボールにStageManagerの情報をセットする
            ball.GetComponent<AttackBallController>().SetPlayerBar(playerBar);      // 生成したボールにバーの情報をセットする
        }
    }

    // 残機回復時に使用するメソッド
    public void LifeUp(int num = 1)
    {
        life += num;
        if (life > maxLife) life = maxLife;
        DisplayLife(life);
    }

    // 残機減少時に使用するメソッド
    public void LifeDown(int num = 1)
    {
        Debug.Log("LifeDown");
        life -= num;        // 残機を減らす
        DisplayLife(life);  // 残機の表示を更新する

        // 残機が０になっていたらゲームオーバーの処理を実施する
        if (life <= 0)
        {
            GameOver();
        }
        else
        {
            // 残機減少の演出（画面中央でハートが消えていく）
            Vector3 spritePos = new Vector3(0, 0, 0);
            GameObject heart = Instantiate(spriteLife, spritePos, Quaternion.identity).gameObject;
            heart.SetActive(true);
            Utility.Instance.ObjRotateOut(heart, 1f);
            Utility.Instance.ForTransparent(heart, 1f);
            Destroy(heart, 1.5f);
        }
    }

    // 現在の残機に応じて左から非アクティブにする
    public void DisplayLife(int display)
    {
        for (int i = ui_lifeObj.Length; i > display; i--)
        {
            ui_lifeObj[i - 1].SetActive(false);
        }
    }

    // スコアアップの処理をするメソッド
    public void UpScore(int val)
    {
        currentScore += val;    // スコアを加算する
        scoreTxt.text = currentScore.ToString("0,000,000"); // スコアの表示を更新する
    }

    // 外部からスタート処理完了を受け取るためのメソッド（ブロックの生成処理）
    public void GameStart()
    {
        inGame = true;  // ゲーム中だとフラグに設定する
        isStart = true; // スタート処理が終わっているとフラグを更新
        Time.timeScale = 1.0f;  // ゲーム時間を動かす
    }
    // ゲームクリア時に実行するメソッド
    public void GameClear()
    {
        if (inGame)
        {
            // Debug.Log("GameClear");
            endStatus = EndStatusName.GameClear;
            SoundManager.Instance.PlayBGM(SoundManager.Instance.bgmGameClear);
            StartCoroutine(GameEnd());
        }
    }

    // ゲームオーバー時に実行するメソッド
    public void GameOver()
    {
        if (inGame)
        {
            // Debug.Log("GameOver");
            endStatus = EndStatusName.GameOver;
            SoundManager.Instance.PlayBGM(SoundManager.Instance.bgmGameOver);
            StartCoroutine(GameEnd());
        }
    }

    // ゲーム終了時にコルーチンで実行する共通処理メソッド
    private IEnumerator GameEnd()
    {
        GameObject display = null;

        // ゲーム終了時のステータスを確認して表示する文字（display）を変更する
        switch (endStatus)
        {
            case EndStatusName.GameClear:
                display = Instantiate(objGameClear, Vector3.zero, Quaternion.identity);
                GameManager.Instance.GameSave(Name, true, Score);
                break;
            case EndStatusName.GameOver:
                display = Instantiate(objGameOver, Vector3.zero, Quaternion.identity);
                GameManager.Instance.GameSave(Name, false, Score);
                break;
        }

        // ゲーム中じゃないとフラグを変更
        inGame = false;

        // displayが設定されていることを確認して演出を実施
        if (display != null)
        {
            // オブジェクトやカラー設定を取得する
            Renderer renderer = display.GetComponent<Renderer>();
            Color c = renderer.material.color;
            Vector3 afterScale = display.transform.localScale;
            Vector3 beforeScale = display.transform.localScale * 5;     // 表示したい大きさの5倍を最初の大きさにする

            display.GetComponent<Renderer>().material.color = new Color(c.r, c.g, c.b, 0);  // 最初は透明の状態に設定
            display.transform.localScale = beforeScale;     // 最初の大きさをオブジェクトに反映
            display.SetActive(true);    // オブジェクトを有効化

            // 表示の更新を行う
            for (int s = 0; s <= 10; s += 1)
            {
                float f = s * 0.1f;     //floatでカウントすると誤差が出るため、整数値を10で割る
                renderer.material.color += new Color(0, 0, 0, 1.0f * 0.1f);
                renderer.transform.localScale = Vector3.Lerp(beforeScale, afterScale, f);
                yield return new WaitForSecondsRealtime(f);     // timeScaleが０になった後に動いてほしいので、WaitForSecondsRealtimeを使用する
            }
            yield return new WaitForSecondsRealtime(0.5f);
            ui_Menu.SetActive(true);    // メニューを有効化する
        }
    }
}
