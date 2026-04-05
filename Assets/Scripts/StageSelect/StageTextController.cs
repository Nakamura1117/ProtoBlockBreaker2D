using TMPro;
using UnityEngine;

public class StageTextController : MonoBehaviour
{
    public TextMeshProUGUI buttonText;
    public GameObject checkSprite;

    // ボタンのテキスト変更用のメソッド
    public void SetButtonText(string s)
    {
        buttonText.text = s;
    }

    // フラグがtrueの時に、ハートマークを表示するメソッド
    public void DisplayCheck(bool flg)
    {
        if (flg)
        {
            checkSprite.SetActive(true);
        }
        else
        {
            checkSprite.SetActive(false);
        }
    }
}
