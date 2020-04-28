using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InputManager : MonoBehaviour
{
    Block lastBlock;

    bool isFlagging = false;
    public Image[] flagButtons;

    Vector2 touchStartPosition;
    bool clicking;
    bool tapping;
    bool dragging;

    public float dragDistance;

    void Update()
    {
        foreach(Image flagButton in flagButtons)
        {
            if (isFlagging)
            {
                flagButton.color += (new Color(1, 1, 1, 1) - flagButton.color) / 10f;
            }
            else
            {
                flagButton.color += (new Color(1, 1, 1, 0.6f) - flagButton.color) / 10f;
            }
        }

        if (Game.Instance.gameOver) return;

        if (Input.touchCount == 0)
        {
            clicking = false;
            dragging = false;
        }

        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);

            if (!EventSystem.current.IsPointerOverGameObject(0))
            {
                switch (touch.phase)
                {
                    case TouchPhase.Began:
                        touchStartPosition = touch.position;
                        clicking = true;
                        break;

                    case TouchPhase.Moved:
                        if (Vector2.Distance(Vector2.zero, touch.position - touchStartPosition) > dragDistance)
                        {
                            clicking = false;
                            dragging = true;
                        }
                        break;

                    case TouchPhase.Ended:
                        if (clicking)
                        {
                            clicking = false;
                            tapping = true;
                        }
                        break;
                }

                if (dragging)
                {
                    clicking = false;
                    tapping = false;
                }
            }
        }

        if (Input.touchCount == 2)
        {
            dragging = true;

            Camera camera = Camera.main;

            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

            float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

            CameraManager.Instance.Zoom(deltaMagnitudeDiff * 0.01f);
        }

        if (dragging)
        {
            Vector2 delta = Vector2.zero;

            foreach(Touch touch in Input.touches)
            {
                delta -= touch.deltaPosition;
            }

            delta /= Input.touchCount;
            Vector3 translation = Camera.main.ScreenToWorldPoint(delta);
            CameraManager.Instance.Translate(delta * 0.01f);
        }

        if (clicking || tapping)
        {
            Touch touch = Input.GetTouch(0);

            Vector3 origin = Camera.main.ScreenToWorldPoint(touch.position);
            RaycastHit2D hit = Physics2D.Raycast(origin, Vector3.forward, Mathf.Infinity);

            if (hit)
            {
                if (hit.transform.tag == "Block")
                {
                    Block block = hit.transform.GetComponent<Block>();

                    block.hovered = true;

                    if (block.opened)
                    {
                        block.hovered = true;
                        Game.Instance.HoverAround(block.x, block.y);
                    }

                    if (tapping)
                    {
                        if (block.opened)
                            Game.Instance.OpenAround(block.x, block.y);
                        else
                            if (isFlagging)
                            block.Flag();
                        else
                            Game.Instance.OpenBlock(block.x, block.y, true);

                        tapping = false;
                    }
                }
            }
        }
    }

    public void ToggleFlagging()
    {
        isFlagging = !isFlagging;
    }
}
