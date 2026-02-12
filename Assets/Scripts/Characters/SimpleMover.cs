using UnityEngine;

// Rigidbodyがついていないとエラーになるようにするお守り
[RequireComponent(typeof(Rigidbody))]
public class SimpleMover : MonoBehaviour
{
    public float speed = 5.0f;
    public float junmpForce = 5.0f;
    private Rigidbody rb;
    private bool isGrounded;

    void Start()
    {
        // 自分のRigidbodyを取得する
        rb = GetComponent<Rigidbody>();
    }

    void Update() // 入力の受付はUpdateでやる
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 moveDir = new Vector3(moveX, 0, moveZ).normalized; // 斜め移動が速くならないように正規化

        // ★ここが変更点：物理エンジンに「速度」をセットする
        // （y軸の速度はそのままにしておかないと、重力で落ちなくなるので注意）
        Vector3 newVelocity = moveDir * speed;
        newVelocity.y = rb.linearVelocity.y; // 重力落下を維持
        
        rb.linearVelocity = newVelocity;
        
        // ※ Unity 6 の場合は rb.linearVelocity = newVelocity; になります
    }
}