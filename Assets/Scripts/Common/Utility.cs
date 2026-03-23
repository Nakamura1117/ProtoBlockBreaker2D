using UnityEngine;
using UnityEngine.SceneManagement;

public class Utility : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

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
}
