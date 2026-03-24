using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Utility : MonoBehaviour
{


    public void MoveScene(string name)
    {
        // 引数で指定されたシーンへ移動
        SceneManager.LoadScene(name);
    }

    public void QuitGame()
    {
        // ゲーム終了
        Application.Quit();
    }

    public void ObjMove(GameObject obj, float time, Vector3 vec, float frame = 0.2f)
    {
        StartCoroutine(ObjMoveCol(obj, time, vec, frame));
    }

    public void ForTransparent(GameObject obj, float time, float frame = 0.2f)
    {
        StartCoroutine(ForTransparentCol(obj, time, frame));
    }

    // 指定したオブジェクトを指定した秒数、指定した方向に少しずつ移動する
    private IEnumerator ObjMoveCol(GameObject obj, float time, Vector3 vec, float frame = 0.2f)
    {
        float cnt = time;
        while (cnt >= 0)
        {
            obj.transform.position += vec.normalized * 2;
            yield return new WaitForSeconds(frame);

            cnt -= frame;
        }
        Debug.Log("Finish ObjMove");
    }

    private IEnumerator ForTransparentCol(GameObject obj, float time, float frame = 0.2f)
    {
        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
        float per = 1 / (time / frame);
        bool finFlg = true;


        while (finFlg)
        {
            finFlg = false;
            foreach (Renderer r in renderers)
            {
                if (r.material.color.a > 0)
                {
                    r.material.color -= new Color(0, 0, 0, per);
                    finFlg = true;

                    Debug.Log(r.material.color);
                }
            }
            yield return new WaitForSeconds(frame);
        }
        Debug.Log("Finish Transparent");
    }
}
