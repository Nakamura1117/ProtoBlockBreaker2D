using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // singletonで実装
    public static GameManager Instance { get; private set; }

    // ゲームモード設定用
    public enum GameMode
    {
        Title,
        StageSelect,
        InStage,
        GameClear,
        GameOver
    }

    //　ステージのリスト管理用
    [SerializeField]
    private string[] stageList ={
        "Stage1",
        "Stage2",
        "Stage3",
        "Stage4",
        "Stage5"
    };

    // ステージのクリア状況を取得する用のプロパティ
    public Dictionary<string, bool> StageList { get { return saveData.ListData(); } }

    // 現在のゲームモード（状況）を保存する用
    private GameMode currentMode;
    // ゲームモードを外部から参照する用
    public GameMode Mode { get { return currentMode; } }

    // スコア記録用
    public int stageScore;

    // セーブデータ用
    private SaveDataManager saveData;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // セーブデータを読み込む
        saveData = new SaveDataManager();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // スタート時にゲームモードをタイトルにする
        ChangeMode(GameMode.Title);
        // タイトルのBGMを流す
        SoundManager.Instance.PlayBGM(SoundManager.Instance.bgmTitle);
    }

    // 次のステージの名前を返すメソッド
    public string GetNextStage()
    {
        return GetNextStage(SceneManager.GetActiveScene().name);
    }
    public string GetNextStage(string s = null)
    {
        // 指定されたステージ名のインデックスを調べる
        int nextIdx = Array.IndexOf(stageList, s);

        // 指定されたステージが存在しない場合「-1」なので、それ以外の場合は次のステージの名前を返す
        if (nextIdx != -1)
        {
            return stageList[nextIdx + 1];
        }
        else
        {
            // 指定されたステージ名が存在しない場合はnullを返す
            return null;
        }
    }

    // ゲームモードを変更する用のメソッド
    public void ChangeMode(GameMode mode)
    {
        currentMode = mode;
    }

    // ステージのクリア状況を保存する用のメソッド
    public void GameSave(string name, bool flg, int score)
    {
        saveData.Save(name, flg, score);    // ゲームマネージャのデータを更新
        saveData.SaveFile();    // データをファイルに保存
    }
}


// セーブデータ管理用のクラス
public class SaveDataManager
{
    // セーブデータ用のクラスをリストで作成
    private List<SaveData> DataValue = new List<SaveData>();
    // セーブデータのファイルパスを設定
    string filePath = Path.Combine(Application.streamingAssetsPath, "savedata");

    public SaveDataManager()
    {
        // 指定のパスからファイルを読み込む
        string[] line = File.ReadAllLines(filePath);

        // 1行ずつデータを読み込んでセーブデータを設定
        foreach (string l in line)
        {
            string[] piece = l.Split(",");  // CSVなのでカンマで分割
            // セーブデータを読み込む
            SaveData data = new SaveData(
                piece[0],
                Convert.ToBoolean(piece[1]),
                int.Parse(piece[2]));
            DataValue.Add(data);    // データを登録する
        }
    }

    // プレイ中のデータを更新する用のメソッド
    public void Save(string name, bool flg, int score)
    {
        // セーブが正しく実施されたか確認用のフラグ
        bool isSaving = false;

        // 読み込んだデータから該当ステージのデータがあるか１行ずつ確認する
        foreach (SaveData data in DataValue)
        {
            // 該当ステージのデータがあった場合は、登録を実施
            if (data.StageName == name)
            {
                data.SetValue(flg, score);
                isSaving = true;
                break;
            }
        }
        // セーブされなかった場合は、新しくステージを登録する
        if (!(isSaving))
        {
            DataValue.Add(new SaveData(name, flg, score));
        }
    }

    // 現在のデータをファイルへ保存する用のメソッド
    public void SaveFile()
    {
        // セーブデータのファイルが存在するか確認
        if (!(File.Exists(filePath)))
        {
            // 存在しない場合、作成する
            Debug.Log("ファイルが存在しないため、作成します");
            File.Create(filePath);
        }
        // ファイルを書き込む
        File.WriteAllLines(filePath, this.StringData(), System.Text.Encoding.UTF8);
    }

    // 指定されたステージのクリア状況をboolで返すメソッド
    public bool GetClearInfo(string name)
    {
        bool returnValue = false;
        foreach (SaveData data in DataValue)
        {
            if (data.StageName == name)
            {
                returnValue = data.IsClear;
            }
        }
        return returnValue;
    }

    // 指定されたステージのスコアを返すメソッド
    public int GetClearScore(string name)
    {
        int returnValue = 0;    // 返り値用の変数を定義
        bool isGet = false;     // スコア取得の確認用変数

        // 現在のデータから該当ステージのデータがないか総当たりで確認
        foreach (SaveData data in DataValue)
        {
            if (data.StageName == name)
            {
                isGet = true;
                returnValue = data.Score;   // 返り値にスコアを設定する
                break;
            }
        }
        // スコアを取得できなかった場合は、明確に０を返す
        if (isGet)
        {
            return returnValue;
        }
        else
        {
            return 0;
        }
    }

    // データ保存時にテキスト化する用のメソッド
    public string[] StringData()
    {
        string[] line = new string[DataValue.Count];
        for (int i = 0; i < DataValue.Count; i++)
        {
            string[] data = {
                DataValue[i].StageName,
                DataValue[i].IsClear.ToString(),
                DataValue[i].Score.ToString()
                 };

            line[i] = String.Join(",", data);
        }
        return line;
    }

    // ステージの名前とクリア状況を辞書型で返すメソッド
    public Dictionary<string, bool> ListData()
    {
        Dictionary<string, bool> r = new Dictionary<string, bool>();
        foreach (SaveData s in DataValue)
        {
            r.Add(s.StageName, s.IsClear);
        }
        return r;
    }
}

// 個別セーブデータのクラス
public class SaveData
{
    //ステージ名
    string stageName;
    public string StageName { get { return stageName; } }

    // クリア状況
    bool isClear;
    public bool IsClear { get { return isClear; } }

    // スコア
    int score;
    public int Score { get { return score; } }

    // 引数なしの場合は何もしないコンストラクタ
    public SaveData() { }

    // 作成時に値を設定するコンストラクタ
    public SaveData(string name, bool flg, int value)
    {
        SetSavedata(name, flg, value);
    }

    // 値をまとめて設定する用のメソッド
    public void SetSavedata(string stagename, bool isclear, int setscore)
    {
        stageName = stagename;
        SetValue(isclear, setscore);
    }

    // 値を更新する用のメソッド
    public void SetValue(bool isclear, int setscore)
    {
        isClear = isclear;
        score = setscore;
    }
}