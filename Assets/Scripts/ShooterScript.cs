using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShooterScript : MonoBehaviour
{
    public Vector2 direction;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if(transform.position.y <= -5)
        {
            Destroy(gameObject);
        }
    }
    public void SetY(float y_value)
    {
        direction = new Vector2(20,y_value);
    }
    public void Shoot(float power)
    {
        GameObject ball = this.gameObject;
        ball.GetComponent<Rigidbody2D>().AddForce(direction * power, ForceMode2D.Force);
    }
}
