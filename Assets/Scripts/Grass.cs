using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grass : MonoBehaviour
{
    bool freeze = true;

    float time;
    public float lifeTime;

    public Vector3 gravity;
    public Vector3 accelMin;
    public Vector3 accelMax;

    Vector3 velocity;

    void Update()
    {
        if (freeze) return;

        time += Time.deltaTime;
        if (time > lifeTime) Destroy(gameObject);

        velocity += gravity * Time.deltaTime;
        transform.position += velocity * Time.deltaTime;
        transform.eulerAngles += new Vector3(0, 0, -velocity.x);
        transform.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, time / lifeTime);
    }

    public void Remove()
    {
        freeze = false;
        velocity = new Vector3(Random.Range(accelMin.x, accelMax.x), Random.Range(accelMin.y, accelMax.y));
    }
}
