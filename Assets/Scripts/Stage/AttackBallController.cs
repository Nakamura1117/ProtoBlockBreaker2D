using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

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
    public float revideVecVal = 0.2f;   // ボールの縦移動の最低値
    public float accelLimit = 10f; // ボール加速の上限

    private PlayerController playerBar;
    private Rigidbody2D rbody;
    private Vector2 forceDirection;

    [SerializeField]
    private string[] defaultTargetTag = new string[3]{
        "Block",
        "PlayerBar",
        "Wall"
    };
    private List<string> targetTag = null;

    private float blinkTime = 3.0f;
    private bool isBlink = true;
    private bool isReturnH = false;
    private bool isReturnV = false;
    private float accel = 0.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transform.position = new Vector3(0, -1, 0);
        isReturnH = false;
        isReturnV = false;
        rbody = GetComponent<Rigidbody2D>();
        forceDirection = Vector2.down;
        targetTag = new List<string>(defaultTargetTag);
        accel = 0.0f;
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
    }

    void FixedUpdate()
    {
        if (isBlink) return;

        if (isReturnH)
        {
            forceDirection.x *= -1;
            isReturnH = false;
        }
        if (isReturnV)
        {
            forceDirection.y *= -1;
            isReturnV = false;
        }

        if (rbody.linearVelocity != forceDirection * speed)
        {
            // Debug.Log("forceDirection" + forceDirection);
            rbody.linearVelocity = forceDirection * speed + new Vector2(accel, accel);
        }

    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        string tag = collision.gameObject.tag;
        // Debug.Log("Collision " + collision.gameObject.tag + ">> " + targetTag.Contains(tag));

        Debug.Log("forceDirect : " + forceDirection);
        if (targetTag.Contains(tag))
        {
            accel += 0.1f;  // 何かに当たった時に加速する
            if (accel > accelLimit) accel = accelLimit;

            if (tag == "PlayerBar")
            {
                float moveVal = playerBar.MoveVal;

                if (moveVal > moveValLimit) moveVal = moveValLimit;
                if (moveVal < -(moveValLimit)) moveVal = -(moveValLimit);

                // バーが動いていたら、ボールにも横方向に力を加える
                if (Mathf.Abs(moveVal) > 0)
                {
                    //Debug.Log(moveNum);

                    // 縦の移動量が少ないとnomalizedで0になってしまうため、最低値を保証する
                    if (Mathf.Abs(forceDirection.y) < revideVecVal)
                    {
                        // 元の関数を書き換えたくないので一時的に関数を作る
                        float val = Mathf.Abs(revideVecVal);     // 移動方向が上の場合はプラスの補正値を設定
                        if (forceDirection.y < 0) val *= -1;   // 移動方向が下の場合はマイナスの補正値を設定
                        forceDirection.y = val;
                    }

                    // バーの移動量に合わせて、ボールの方向を再定義する。
                    forceDirection = new Vector2(forceDirection.x + (moveVal * 10), forceDirection.y).normalized;
                }
            }

            if (tag == "Block")
            {
                collision.gameObject.GetComponent<BlockController>().BreakBlock();
                accel = 0.0f;   // ブロックに当たった時、加速をリセットする
            }

            if (ballMode != BallModeName.Penetration)
            {
                float diffY = collision.GetContact(0).point.y - transform.position.y;
                float diffX = collision.GetContact(0).point.x - transform.position.x;

                Debug.Log("x:" + diffX + "、y:" + diffY);
                if (diffX > 0 && forceDirection.x > 0 || diffX < 0 && forceDirection.x < 0)
                {
                    isReturnH = true;
                }
                if (diffY > 0 && forceDirection.y > 0 || diffY < 0 && forceDirection.y < 0)
                {
                    isReturnV = true;
                }
            }
        }

        if (tag == "Dead")
        {
            stage.BallDrop();
            Destroy(gameObject);
        }
    }

    public void SetStageManager(StageManager sm)
    {
        stage = sm;
    }

    public void SetPlayerBar(PlayerController pc)
    {
        playerBar = pc;
    }
}
