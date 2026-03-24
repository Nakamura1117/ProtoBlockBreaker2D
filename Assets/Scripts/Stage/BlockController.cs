using UnityEngine;

public class BlockController : MonoBehaviour
{
    public StageManager stage;
    public BlockGenerator blockGenerator;
    public int scoreVaule = 0;

    public bool isBloken = true;

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
        if (isBloken)
        {
            //Debug.Log("BreakBlock");
            blockGenerator.BrokenBlock(transform.position);

            Destroy(gameObject);
        }
    }
}
