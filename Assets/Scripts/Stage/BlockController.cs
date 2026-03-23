using UnityEngine;

public class BlockController : MonoBehaviour
{
    public StageManager stage;
    public BlockGenerator blockGenerator;
    public int scoreVaule = 0;

    public void SetStageManager(StageManager sm)
    {
        stage = sm;
    }

    public void SetBlockGen(BlockGenerator block)
    {
        blockGenerator = block;
    }

    public void BreakBlock()
    {
        //Debug.Log("BreakBlock");
        blockGenerator.BrokenBlock();
        Destroy(gameObject);
    }
}
