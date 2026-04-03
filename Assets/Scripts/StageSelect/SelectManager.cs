using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectManager : MonoBehaviour
{
    // ステージの一覧を配列で定義
    private Dictionary<string, bool> stageList;

    public GameObject canvas;   // UI表示のためキャンバスを定義
    public RectTransform stagePos;  // ステージのリスト生成時の起点
    public GameObject stageTxt; // ステージのリストの元オブジェクト 
    private Vector3 movePos = new Vector3(0, -200);     // 移動する方向量を定義

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        stageList = GameManager.Instance.StageList;     // ステージのクリア状況を取得する
        RectTransform setPos = stagePos;        // 生成位置を取得

        bool isOpen = true; // ステージ１を有効にするためにフラグを設定

        foreach (KeyValuePair<string, bool> s in stageList)
        {
            // ステージ移動用のボタンを生成する
            GameObject text = Instantiate(stageTxt, Vector3.zero, Quaternion.identity, canvas.transform);
            text.GetComponent<RectTransform>().position = stagePos.position;    // 生成したボタンの位置を移動する
            text.GetComponent<StageTextController>().SetButtonText(s.Key);      // ボタンのテキストを設定する
            text.GetComponent<Button>().onClick.AddListener(() => Utility.Instance.MoveScene(s.Key));   // ボタンにシーン移動メソッドを追加
            text.GetComponent<Button>().onClick.AddListener(() => SoundManager.Instance.PlayBGM(SoundManager.Instance.bgmStagePlay));   // ボタンにBGM再生用のメソッドを追加
            text.GetComponent<Button>().onClick.AddListener(() => SoundManager.Instance.PlaySE(SoundManager.Instance.seButtonClick));   // ボタンにクリック音のSEを鳴らすメソッドを追加
            text.GetComponent<StageTextController>().DisplayCheck(s.Value); // クリア済みの場合にハートマークを表示する
            text.GetComponent<Button>().interactable = isOpen;  // 前のステージがクリアされていたらボタンの機能を有効化（クリアされていなかったら反応しない）
            isOpen = s.Value;   // 次のループに備えて現在のステージのクリア状況を保存する
            setPos.position += movePos;     // 次に生成するボタンの位置を設定する
        }
    }
}
