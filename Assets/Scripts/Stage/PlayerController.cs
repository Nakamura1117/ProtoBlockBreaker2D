using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class PlayerController : MonoBehaviour
{
    public StageManager stage;  // ステージマネージャー
    public GameObject body; //
    // ボールが衝突するオブジェクトのタグ
    public string[] contactTag = {
         "AttackBar",
         "Block",
        "Wall"
         };
    private Vector3 beforePosition; // 移動方向確認用の変数
    // 移動方向を返すプロパティ
    public float MoveVal
    {
        get { return beforePosition.x - transform.position.x; }
    }

    float cntTime = 0;  // 時間カウント用変数

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // input = GetComponent<PlayerInput>();
        transform.position = new Vector2(0, -4); // バーを初期値に移動（真ん中）
    }

    // Update is called once per frame
    void Update()
    {
        cntTime += Time.deltaTime;  // 経過時間をカウントする
        if (stage.InGame == false) return;  // ゲーム中でなければ処理をしない

        TouchControl touch = Touchscreen.current.touches[0];    // 現在のタッチ位置を取得する
        // タッチされていたら、バーの移動処理を実施する
        if (touch.press.isPressed)
        {
            // タッチされたスクリーン座標をゲームのワールド座標に変換する
            Vector3 convertPos = Camera.main.ScreenToWorldPoint(new Vector3(touch.position.ReadValue().x, touch.position.ReadValue().y, 10));
            transform.position = new Vector2(convertPos.x, transform.position.y);   // 変換した座標をバーに設定する
        }

        // 0.1秒ごとに前の位置を取得する
        if (cntTime >= 0.1f)
        {
            beforePosition = transform.position;
            cntTime = 0;
        }
    }
}
