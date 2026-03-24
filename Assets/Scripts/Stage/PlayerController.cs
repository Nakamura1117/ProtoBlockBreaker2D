using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class PlayerController : MonoBehaviour
{
    public StageManager stage;
    public GameObject body;
    public string[] contactTag = {
         "AttackBar",
         "Block",
        "Wall"
         };
    private Vector3 beforePosition;
    public float MoveVal
    {
        get { return beforePosition.x - transform.position.x; }
    }

    float cntTime = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // input = GetComponent<PlayerInput>();
        transform.position = new Vector2(0, -4); // バーを初期値に移動（真ん中）
    }

    // Update is called once per frame
    void Update()
    {
        cntTime += Time.deltaTime;
        if (stage.InGame == false) return;

        TouchControl touch = Touchscreen.current.touches[0];
        if (touch.press.isPressed)
        {
            Vector3 convertPos = Camera.main.ScreenToWorldPoint(new Vector3(touch.position.ReadValue().x, touch.position.ReadValue().y, 10));
            transform.position = new Vector2(convertPos.x, transform.position.y);
        }

        if (cntTime >= 0.1f)
        {
            beforePosition = transform.position;
            cntTime = 0;
        }
    }
}
