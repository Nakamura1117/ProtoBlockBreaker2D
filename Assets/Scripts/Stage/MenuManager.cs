using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public StageManager stage;  // StageManage
    public Button buttonStageSelect;    // ステージセレクトボタン
    public Button buttonNextStage;      // 次のステージへ進むボタン
    public Button buttonTitle;      // タイトルへ戻るボタン

    public TextMeshProUGUI stageName;   // ステージ名をメニューに表示するテキスト
    public TextMeshProUGUI stageScore;  // ステージのスコアをメニューに表示するテキスト

    void Start()
    {
        // ステージセレクトボタンが押された際に実行するメソッドを追加
        buttonStageSelect.onClick.AddListener(() => Utility.Instance.MoveScene("SelectStage")); // シーンを指定して移動するメソッドを追加
        buttonStageSelect.onClick.AddListener(() => SoundManager.Instance.PlayBGM(SoundManager.Instance.bgmStageSelect));   // 押された際にBGMを再生するメソッドを追加

        // 次のステージへ進むボタンが押された際に実行するメソッドを追加
        buttonNextStage.onClick.AddListener(() => SoundManager.Instance.PlayBGM(SoundManager.Instance.bgmStagePlay));   // 押された際にBGMを再生するメソッドを追加

        // タイトルへ戻るボタンが押された際に実行するメソッドを追加
        buttonTitle.onClick.AddListener(() => Utility.Instance.MoveScene("Title"));     // シーンを指定して移動するメソッドを追加
        buttonTitle.onClick.AddListener(() => SoundManager.Instance.PlayBGM(SoundManager.Instance.bgmTitle));       // 押された際にBGMを再生するメソッドを追加
    }

    // メニューが有効化されたときに、だんだんと表示される
    void OnEnable()
    {
        // 完全に表示されるまでボタンの反応を無効にする
        foreach (Button b in GetComponentsInChildren<Button>())
        {
            b.interactable = false;
        }

        stageName.text = SceneManager.GetActiveScene().name;    // ステージ名を取得してオブジェクトに反映する
        stageScore.text = stage.Score.ToString("0,000,000");    // ステージのスコアを取得してオブジェクトに反映する
        StartCoroutine(DisplayMenu());  // メニューの表示処理を呼び出す
    }

    // だんだん表示されるようにするためコルーチン
    private IEnumerator DisplayMenu()
    {
        // 自身と自身の子オブジェクトを透明にする
        GetComponent<CanvasGroup>().alpha = 0;

        // ゲームオーバーだった場合、次のステージへ進むボタンを「リトライボタン」へ変更
        if (stage.EndStatus == StageManager.EndStatusName.GameOver)
        {
            buttonNextStage.GetComponentInChildren<TextMeshProUGUI>().text = "Retry Stage"; // 次のステージへ進むボタンのテキストを変更する
            buttonNextStage.onClick.AddListener(() => Utility.Instance.MoveScene());    // シーンを指定して移動するメソッドを追加
        }
        else
        {
            // ゲームオーバーではなかった場合は、次のステージへ進むメソッドをボタンに追加する
            buttonNextStage.onClick.AddListener(() => Utility.Instance.MoveScene(GameManager.Instance.GetNextStage(SceneManager.GetActiveScene().name)));
        }

        // 繰り返しでα値を加算して、だんだんと表示する
        for (int i = 0; i <= 10; i++)
        {
            GetComponent<CanvasGroup>().alpha += 0.1f;
            yield return new WaitForSecondsRealtime(0.1f);
        }

        // 完全に表示されたらボタンの反応を有効にする
        foreach (Button b in GetComponentsInChildren<Button>())
        {
            b.onClick.AddListener(() => SoundManager.Instance.PlaySE(SoundManager.Instance.seButtonClick));
            b.interactable = true;
        }
    }
}
