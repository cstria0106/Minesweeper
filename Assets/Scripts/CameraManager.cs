using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance
    {
        get
        {
            return FindObjectOfType<CameraManager>();
        }
    }

    public Vector3 position = new Vector3(0, 0, -10);
    public float size;

    public float minSize;
    public float maxSize;

    public void InitializeSize(int width, int height)
    {
        width++; height++;

        minSize = 10 * 0.32f;

        maxSize = (width > height) ? width : height;
        if (maxSize == width) maxSize *= (float)Screen.height / Screen.width;
        maxSize = maxSize * 0.32f;

        size = maxSize;
        Camera.main.orthographicSize = size;
    }

    void Update()
    {
        transform.position += (position - transform.position) / 5f;
        Camera.main.orthographicSize += (size - Camera.main.orthographicSize) / 5f;
    }

    public void Translate(Vector3 delta)
    {
        this.position += (0.4f + size / maxSize * 0.6f) * delta;
    }

    public void Zoom(float delta)
    {
        this.size += (0.4f + size / maxSize * 0.6f) * delta;

        size = Mathf.Clamp(size, minSize, maxSize);
    }
}
