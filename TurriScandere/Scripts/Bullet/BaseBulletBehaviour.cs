using UnityEngine;

public class BaseBulletBehaviour : MonoBehaviour
{
    [SerializeField] private float disappearanceTime = 20.0f; // 弾が消えるまでの時間

    void Start()
    {
        Destroy(this.gameObject, disappearanceTime);
    }

    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject obj = collision.gameObject;

        if (obj.CompareTag("Player"))
        {
            Destroy(this.gameObject);
        }
    }

}
