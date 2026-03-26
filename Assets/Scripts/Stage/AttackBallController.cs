using System.Collections.Generic;
using UnityEngine;

public class AttackBallController : MonoBehaviour
{
    // ボールの状態によって貫通などの処理を与える
    public enum BallModeName
    {
        Nomel,  // 通常モード
        Penetration,    // 貫通モード
    }

    public BallModeName ballMode = BallModeName.Nomel;
    public StageManager stage;
    public float speed = 7.0f;
    public float moveValLimit = 0.5f;   // バーの移動量計算で使用する
    public float revideVecVal = 0.5f;   // ボールの縦移動の最低値
    public float accelLimit = 10f; // ボール加速の上限

    private PlayerController playerBar; // プレイヤーのバー情報
    private Rigidbody2D rbody;  // 自身のRigidbody情報
    private Vector2 forceVector; // ボールを動かす力の方向

    // 反射判定をするオブジェクトのタグの元情報（配列）
    [SerializeField]
    private string[] defaultTargetTag = new string[3]{
        "Block",
        "PlayerBar",
        "Wall"
    };
    private List<string> targetTag = null;  // 反射判定をするオブジェクトのタグ情報（処理の際に増減させる）

    private float blinkTime = 3.0f; // 生成後の点滅時間
    private bool isBlink = true;    // 点滅管理フラグ
    private bool isReturnH = false; // 横方向の反射フラグ
    private bool isReturnV = false; // 縦方向の反射フラグ
    private float accel = 0.0f; // 加速値

    private float cntTime;  // 経過時間カウント用変数

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // 初期値設定
        transform.position = new Vector3(0, -1, 0);
        isReturnH = false;
        isReturnV = false;
        rbody = GetComponent<Rigidbody2D>();
        forceVector = Vector2.down;
        targetTag = new List<string>(defaultTargetTag);
        accel = 0.0f;
        cntTime = 0;
        isBlink = true;
    }

    // Update is called once per frame
    void Update()
    {
        // ゲーム中でなければ、何もせずに終了する
        if (stage.InGame == false) return;

        // 開始後BlinkTimeの時間だけ点滅する。点滅している間は移動処理を実施しない。
        if (isBlink == true)
        {
            if (Mathf.Sin(Time.time * 10f) > 0)
            {
                foreach (SpriteRenderer s in GetComponentsInChildren<SpriteRenderer>())
                {
                    s.enabled = false;
                }
            }
            else
            {
                foreach (SpriteRenderer s in GetComponentsInChildren<SpriteRenderer>())
                {
                    s.enabled = true;
                }
            }

            blinkTime -= Time.deltaTime;

            if (blinkTime <= 0)
            {
                foreach (SpriteRenderer s in GetComponentsInChildren<SpriteRenderer>())
                {
                    s.enabled = true;
                }
                isBlink = false;
            }
            return;
        }
        // 経過時間をカウント
        cntTime += Time.deltaTime;
        if (cntTime >= 0.1f)
        {
            cntTime = 0;
        }
    }

    void FixedUpdate()
    {
        if (isBlink) return;    // 点滅しているときは処理を実施しない

        // 横方向の反射を実施
        if (isReturnH)
        {
            forceVector.x *= -1;
            isReturnH = false;
        }
        // 縦方向の反射を実施
        if (isReturnV)
        {
            forceVector.y *= -1;
            isReturnV = false;
        }

        // 力の方向＋スピード＋加速値　※加速値はブロック以外で反射した時に加算
        rbody.linearVelocity = forceVector * speed + new Vector2(accel, accel);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // 接触したオブジェクトのタグを変数に取得
        string tag = collision.gameObject.tag;

        // Debug.Log("Collision " + collision.gameObject.tag + ">> " + targetTag.Contains(tag));
        // Debug.Log("forceDirect : " + forceVector);

        // 衝突したタグが反射対象のタグか確認する
        if (targetTag.Contains(tag))
        {
            accel += 0.1f;  // 何かに当たった時に加速する
            if (accel > accelLimit) accel = accelLimit; // 加速上限

            // バーに当たった際に、バーの移動に応じて横方向に力を加える
            if (tag == "PlayerBar")
            {
                float moveVal = playerBar.MoveVal;

                // 移動値の上限・加減で設定
                if (moveVal > moveValLimit) moveVal = moveValLimit;
                if (moveVal < -(moveValLimit)) moveVal = -(moveValLimit);


                // バーが動いていたら、ボールにも横方向に力を加える
                if (Mathf.Abs(moveVal) > 0)
                {
                    //Debug.Log(moveNum);

                    // バーの移動量に合わせて、ボールの方向を再定義する。
                    forceVector = new Vector2(forceVector.x + (moveVal * 10), forceVector.y).normalized;
                }

                // 縦の移動量が少ないとnomalizedで0になってしまうため、最低値を保証する
                if (Mathf.Abs(forceVector.y) < revideVecVal)
                {
                    // 元の関数を書き換えたくないので一時的に関数を作る
                    float valY = Mathf.Abs(revideVecVal);     // 移動方向が上の場合はプラスの補正値を設定
                    float valX = 1 - valY;
                    if (forceVector.y < 0) valY *= -1;  // 移動方向が下の場合はマイナスの補正値を設定
                    if (forceVector.x < 0) valX *= -1;  // 移動方向が左の場合はマイナスの補正値を設定

                    forceVector = new Vector2(valX, valY).normalized;   // 補正した値で移動値を標準化
                }
            }

            // ブロックに当たった場合
            if (tag == "Block")
            {
                collision.gameObject.GetComponent<BlockController>().BreakBlock();
                accel = 0.0f;   // ブロックに当たった時、加速をリセットする
            }

            // 当たった位置を取得する（絶対座標を自身から見た座標にする）
            float diffY = collision.GetContact(0).point.y - transform.position.y;
            float diffX = collision.GetContact(0).point.x - transform.position.x;

            // 当たった方向と進行方向が同じ場合は反射フラグを立てる
            if ((diffX > 0 && forceVector.x > 0) || (diffX < 0 && forceVector.x < 0))
            {
                isReturnH = true;
            }
            if ((diffY > 0 && forceVector.y > 0) || (diffY < 0 && forceVector.y < 0))
            {
                isReturnV = true;
            }
        }

        // タグ「Dead」に触れたらボールが落ちた判定
        if (tag == "Dead")
        {
            stage.BallDrop();
            Destroy(gameObject);
        }
    }

    // 生成時にステージマネージャーを設定する用のメソッド
    public void SetStageManager(StageManager sm)
    {
        stage = sm;
    }

    // 生成時にバーを設定する用のメソッド
    public void SetPlayerBar(PlayerController pc)
    {
        playerBar = pc;
    }
}
