using UnityEngine;

public class GameManager : MonoBehaviour
{

    public static int totalScore;
    public int stageScore;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ScoreUp(int score)
    {
        stageScore += score;
        Debug.Log("スコアが加算された：" + score);
    }
}
