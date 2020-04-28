using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    public GameObject grass;
    public GameObject dirt;
    public GameObject obj;
    public GameObject hover;

    public Sprite[] grassSprites;
    public Sprite[] dirtSprites;
    public Sprite[] objectSprites;

    public Sprite mineSprite;
    public Sprite mineBackSprite;

    public GameObject flagPrefab;
    public GameObject activeFlagObject;

    public bool flagged = false;
    public bool opened = false;
    public bool hovered = false;

    bool clickingLeft = false;
    bool clickingRight = false;

    public bool burnt = false;

    public GameObject explosionEffect;

    public Type type;

    public int x, y;

    void Update()
    {
        if (hovered)
        {
            if (!opened || (type == Type.None && type == Type.Mine))
            {
                hover.SetActive(true);
                hover.GetComponent<SpriteRenderer>().sortingOrder = (opened) ? 1 : 4;
            }
            hovered = false;
        }
        else
        {
            hover.SetActive(false);
        }
    }

    public void Initialize(int dirtType, Type type)
    {
        this.type = type;

        grass.GetComponent<SpriteRenderer>().sprite = grassSprites[dirtType];

        if (type == Type.Mine)
        {
            dirt.GetComponent<SpriteRenderer>().sprite = mineBackSprite;
            obj.GetComponent<SpriteRenderer>().sprite = mineSprite;
        }
        else
        {
            dirt.GetComponent<SpriteRenderer>().sprite = dirtSprites[dirtType];
            obj.GetComponent<SpriteRenderer>().sprite = objectSprites[(int)type];
        }
    }

    public bool Open()
    {
        if (flagged || opened)
            return false;

        grass.GetComponent<Grass>().Remove();
        grass.GetComponent<SpriteRenderer>().sortingOrder = 5;
        opened = true;

        if (type == Type.None || type == Type.Mine)
            hovered = false;

        if(type == Type.Mine)
        {
            Camera.main.GetComponent<CameraShaker>().Shake(-0.5f, 0.5f, 0.5f);
            Instantiate(explosionEffect).transform.position = transform.position;
            SoundManager.Instance.PlayExplosionSound();
        }

        return true;
    }

    public void Flag()
    {
        if (opened) return;

        flagged = !flagged;

        if (flagged)
        {
            activeFlagObject = Instantiate(flagPrefab, transform);
            activeFlagObject.transform.position = transform.position;
            SoundManager.Instance.PlayFlagSound();
        }
        else
        {
            activeFlagObject.GetComponent<Flag>().Remove();
            activeFlagObject = null;
            SoundManager.Instance.PlayFlagRemoveSound();
        }
    }
}
