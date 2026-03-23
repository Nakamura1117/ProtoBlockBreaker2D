using System.ComponentModel;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class AttackBallController : MonoBehaviour
{
    public StageManager stage;
    Rigidbody2D rbody;
    Vector2 forceDirection;
    Vector3 beforePosition;

    float speed = 3.0f;
    bool isReturnH = false;
    bool isReturnV = false;

    [SerializeField]
    string[] targetTag = new string[3]{
        "PlayerBar",
        "Wall",
        "Block"
    };

    private float blinkTime = 3.0f;
    private bool isBlink = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transform.position = new Vector3(0, -1, 0);
        isReturnH = false;
        isReturnV = false;
        rbody = GetComponent<Rigidbody2D>();
        forceDirection = Vector2.down;
        beforePosition = transform.position;

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

        beforePosition = transform.position;
    }

    void FixedUpdate()
    {
        if (isBlink == false)
        {
            if (rbody.linearVelocity != forceDirection * speed)
            {
                // Debug.Log("forceDirection" + forceDirection);
                rbody.linearVelocity = forceDirection * speed;
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        string tag = collision.gameObject.tag;

        float moveNum = beforePosition.x - transform.position.x;

        // Debug.Log("Collision " + collision.gameObject.tag + ">> " + targetTag.Contains(tag));

        if (moveNum > 0)
        {
            //Debug.Log(moveNum);
            forceDirection = new Vector2(moveNum * 10, forceDirection.y).normalized;
        }

        if (targetTag.Contains(tag))
        {
            if (tag == "Block")
            {
                collision.gameObject.GetComponent<BlockController>().BreakBlock();
            }
            float diffY = collision.GetContact(0).point.y - transform.position.y;
            float diffX = collision.GetContact(0).point.x - transform.position.x;
            if ((diffY > 0 && forceDirection.y > 0) || (diffY < 0 && forceDirection.y < 0))
            {
                isReturnV = true;
            }
            if ((diffX > 0 && forceDirection.x > 0) || (diffX < 0 && forceDirection.x < 0))
            {
                isReturnH = true;
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
}
