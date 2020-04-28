using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShaker : MonoBehaviour
{
    public void Shake(float min, float max, float time)
    {
        StartCoroutine(ShakeRoutine(min, max, time));
        CameraManager.Instance.size = CameraManager.Instance.maxSize;
    }

    IEnumerator ShakeRoutine(float min, float max, float time)
    {
        float t = Time.time;

        Vector3 origin = new Vector3(0, 0, -10);

        while(Time.time - t  < time)
        {
            Vector3 delta = new Vector3();

            delta.x = Random.Range(min, max) * (1 - (Time.time - t) / time);
            delta.y = Random.Range(min, max) * (1 - (Time.time - t) / time);

            delta.x = Mathf.Floor(delta.x / 0.01f) * 0.01f;
            delta.y = Mathf.Floor(delta.y / 0.01f) * 0.01f;

            CameraManager.Instance.position = origin + delta;

            yield return new WaitForEndOfFrame();
        }

        transform.position = origin;
    }
}
