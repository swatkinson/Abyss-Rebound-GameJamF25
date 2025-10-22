using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField] public float speed = 2f;
    public Transform[] points;

    private int i;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transform.position = points[0].position;
    }

    // Update is called once per frame
    void Update()
    {
        if(Vector2.Distance(transform.position, points[i].position) < 0.1f)
        {
            i++;
            if (i >= points.Length)
            {
                i = 0;
            }
        }

        transform.position = Vector2.MoveTowards(transform.position, points[i].position, speed * Time.deltaTime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.transform.SetParent(transform);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.transform.SetParent(null);
        }
    }
}
