using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
public class Utility : MonoBehaviour
{
    // 複数のクラスから使用するツールをまとめたクラス。singletonで実装する
    public static Utility Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // 現在のシーンを再読み込み
    public void MoveScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // 指定されたシーンを読み込む（シーンマネージャーをできるだけほかのクラスで読み込まないように）
    public void MoveScene(string name)
    {
        // 引数で指定されたシーンへ移動
        SceneManager.LoadScene(name);
    }

    // ゲームを終了する（ほかのシーンからまとめて分かりやすくするように）
    public void QuitGame()
    {
        // ゲーム終了
        Application.Quit();
    }

    // コルーチン（ObjRotateOutCol）呼び出し用メソッド
    public void ObjRotateOut(GameObject obj, float s)
    {
        StartCoroutine(OjbRotateOutCol(obj, s));
    }

    // Ｙ軸の回転を使用して、引数objのオブジェクトを引数sの秒数で消えるようにする
    // 注意：２Ｄゲームでのみ有効。３Ｄだと消えない
    private IEnumerator OjbRotateOutCol(GameObject obj, float s)
    {
        // Debug.Log("ObjRotateOutCol");
        float maxAngle = 90;    // 回転する最大を設定
        float r = 0;    // 現在の回転量を保持する用の変数

        float perAngle = maxAngle * 0.05f;
        float frame = s * 0.05f;

        // 指定された秒数の1/20ずつ回転していく
        for (float remainingTime = s; remainingTime > 0; remainingTime -= frame)
        {
            // オブジェクトをＹ軸で回転する
            obj.transform.rotation = Quaternion.Euler(obj.transform.rotation.x, r, obj.transform.rotation.z);
            r += perAngle;   // 角度の変数を更新する
            yield return new WaitForSeconds(frame);  // 指定された秒数sの1/10だけ処理を止める
        }
    }

    // コルーチン（ObjMoveCol）呼び出し用のメソッド
    public void ObjMove(GameObject obj, float time, Vector3 vec, float frame = 0.2f)
    {
        StartCoroutine(ObjMoveCol(obj, time, vec, frame));
    }
    // 指定したオブジェクトを指定した秒数、指定した間隔で少しずつ移動する
    private IEnumerator ObjMoveCol(GameObject obj, float time, Vector3 vec, float frame = 0.2f)
    {
        // Debug.Log("ObjMoveCol");
        float cnt = time;
        // 毎frame、ベクター量２で移動を実施
        while (cnt >= 0)
        {
            obj.transform.position += vec.normalized * 2;
            yield return new WaitForSeconds(frame);

            cnt -= frame;
        }
        // Debug.Log("Finish ObjMove");
    }

    // コルーチン（ForTransparentCol）呼び出し用のメソッド
    public void ForTransparent(GameObject obj, float time, float frame = 0.2f)
    {
        StartCoroutine(ForTransparentCol(obj, time, frame));
    }
    // 指定されたオブジェクトobjを指定された時間time、指定された間隔frameで透明にする
    private IEnumerator ForTransparentCol(GameObject obj, float time, float frame = 0.2f)
    {
        // Debug.Log("ForTransparentCol");
        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
        float per = 1 / (time / frame);
        bool finFlg = true;
        // 透明になるまで徐々にα値を減らしていく
        while (finFlg)
        {
            finFlg = false;
            foreach (Renderer r in renderers)
            {
                // 対象のオブジェクトのいずれかのα値が０出ない場合、ループを続行
                if (r.material.color.a > 0)
                {
                    r.material.color -= new Color(0, 0, 0, per);
                    finFlg = true;
                }
            }
            yield return new WaitForSeconds(frame);
        }
        // Debug.Log("Finish Transparent");
    }
}
