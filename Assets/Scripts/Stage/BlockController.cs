using UnityEngine;

public class BlockController : MonoBehaviour
{
    public StageManager stage;  // ステージマネージャー
    public BlockGenerator blockGenerator;   // ブロックジェネレーター（一部処理で使用するため）
    public int scoreVaule = 0;  // このブロックが壊れた際に加算するスコア（インスペクターで設定）

    public bool isBloken = true;    // 壊れるブロックかどうか

    // 外部からステージマネージャーを設定する
    public void SetStageManager(StageManager sm)
    {
        stage = sm;
    }

    // 外部からブロックマネージャーを設定する
    public void SetBlockGen(BlockGenerator block)
    {
        blockGenerator = block;
    }

    // ブロックが壊れる際の処理
    public void BreakBlock()
    {
        // 壊れないブロックの場合は処理をしない
        if (isBloken)
        {
            //Debug.Log("BreakBlock");
            blockGenerator.BrokenBlock(); // 破壊された際の処理をブロックマネージャーで実施する
            stage.UpScore(scoreVaule);  // ステージマネージャーにスコア加算
            Destroy(gameObject);    // このオブジェクトを破棄
        }
    }
}
