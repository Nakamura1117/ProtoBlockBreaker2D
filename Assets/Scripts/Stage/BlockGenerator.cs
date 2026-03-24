using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;


public class BlockGenerator : MonoBehaviour
{
    public StageManager stage;

    // １マス　0.5、0.5（横長）　最大　80個（8×10）
    // BlockのScale　X：1.67、Y：3.35
    // Blockを置くエリア　X：-2～2（8マス）、Y：0.0～4.5（10マス）

    public GameObject blockPrefub_Red;
    public GameObject blockPrefub_Blue;
    public GameObject blockPrefub_Green;
    public GameObject blockPrefub_White;
    public GameObject blockPrefub_UnBreak;

    private static int blockNum;        // ステージに配置されたブロックの数
    private static int currentBlockNum; // 現在ステージに残っているブロックの数

    // ブロック配置の始点（左上）
    float genTop = 4.5f;
    float genLeft = -2.0f;

    public const int sizeX = 8, sizeY = 10;     // ステージのサイズ
    float blockWidth = 0.5f, blockHeight = 0.5f;    // ブロックのサイズ


    private void Awake()
    {
        transform.position = new Vector2(genLeft, genTop);
        // ステージ名はシーン名と合わせる
        string stageName = SceneManager.GetActiveScene().name;
        int[,] stageData = LoadStage(stageName);

        Debug.Log("縦：" + stageData.GetLength(0) + " × 横：" + stageData.GetLength(1));

        CreateStage(stageData);

        currentBlockNum = blockNum;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (stage.IsStart == false) stage.GameStart();
    }

    int[,] LoadStage(string stageName, int x = sizeX, int y = sizeY)
    {
        // ステージの２次元配列データを読み込んでリターンする（デフォルトは8×10）
        string filePath = Path.Combine(Application.streamingAssetsPath, "Stage", stageName);
        // Debug.Log("filepath: " + filePath);
        string[] line = File.ReadAllLines(filePath);
        int[,] stage = new int[y, x];
        for (int i = 0; i < line.Length; i++)
        {
            int[] piece = line[i].Split(",").Select(int.Parse).ToArray();
            for (int j = 0; j < piece.Length; j++)
            {
                stage[i, j] = piece[j];
            }
        }
        return stage;
    }

    void CreateStage(int[,] stageDeta)
    {
        // 以下の色でブロックを生成する
        // 1：赤、2：青、3：緑、4：白
        // 9：壊せないブロック（黒）

        GameObject instantPrefub = null;
        for (int i = 0; i < stageDeta.GetLength(0); i++)
        {
            for (int j = 0; j < stageDeta.GetLength(1); j++)
            {
                switch (stageDeta[i, j])
                {
                    case 1:
                        // 赤
                        instantPrefub = blockPrefub_Red;
                        blockNum++;
                        break;
                    case 2:
                        // 青
                        instantPrefub = blockPrefub_Blue;
                        blockNum++;
                        break;
                    case 3:
                        // 緑
                        instantPrefub = blockPrefub_Green;
                        blockNum++;
                        break;
                    case 4:
                        // 白
                        instantPrefub = blockPrefub_White;
                        blockNum++;
                        break;
                    case 9:
                        // 壊せないブロック
                        instantPrefub = blockPrefub_UnBreak;
                        break;
                    case 0:
                    default:
                        continue;
                }
                GameObject go = Instantiate(instantPrefub, transform);
                go.transform.position = new Vector3(
                    (blockWidth * j) + genLeft,     // 始点Xを加算することで、生成を左から開始する
                    -(blockHeight * i) + genTop,    // 始点Yを加算することで上から生成を開始する。上から順番に生成する都合、マイナスにしないと上下反転してしまう。
                    0);
                go.GetComponent<BlockController>().SetStageManager(stage);
                go.GetComponent<BlockController>().SetBlockGen(GetComponent<BlockGenerator>());
            }
        }
    }

    public void BrokenBlock(Vector3 pos)
    {
        currentBlockNum--;
        if (currentBlockNum == 0)
        {
            stage.GameClear();
        }
    }
}

