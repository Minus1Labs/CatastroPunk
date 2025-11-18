using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DestructibleObject : MonoBehaviour
{
    private string objectName;
    private Animator objectAnimator;
    private int timesHit;

    private SpriteRenderer parentSprite;
    private Sprite setSprite;
    public enum SpriteSize
    {
        Size_1x1,
        Size_1x2,
        Size_1x3,
        Size_2x1,
        Size_3x1
    }

    public enum SpriteType
    {
        Normal,
        Electronics
    }

    [System.Serializable]
    public struct SpriteMapping
    {
        public SpriteSize size;
        public SpriteType type;
        public Sprite sprite;
    }

    public SpriteSize mySize; 
    public SpriteType myType; 

    public List<SpriteMapping> spriteDatabase;

    private static Dictionary<(SpriteSize, SpriteType), Sprite> spriteMap;

    void Awake()
    {
        if (spriteMap == null)
        {
            spriteMap = new Dictionary<(SpriteSize, SpriteType), Sprite>();

            foreach (var mapping in spriteDatabase)
            {
                var combinedKey = (mapping.size, mapping.type);

                if (!spriteMap.ContainsKey(combinedKey))
                {
                    spriteMap.Add(combinedKey, mapping.sprite);
                }
                else
                {
                    Debug.LogWarning("Duplicate database entry for: " + mapping.size + " / " + mapping.type);
                }
            }
        }
    }

    void Start()
    {
        objectName = transform.parent.name;
        objectAnimator = GetComponentInParent<Animator>();

        parentSprite = GetComponentInParent<SpriteRenderer>();

        objectAnimator = GetComponentInParent<Animator>();

        parentSprite = GetComponentInParent<SpriteRenderer>();
        if (parentSprite == null)//if not in parent
        {
            Transform spriteColliderObj = transform.parent.Find("SpriteCollider");
            if (spriteColliderObj != null)
            {
                parentSprite = spriteColliderObj.GetComponent<SpriteRenderer>();
            }
        }

        var lookupKey = (mySize, myType);

        if (spriteMap.ContainsKey(lookupKey))
        {
            setSprite = spriteMap[lookupKey];
            Debug.Log("Assigned sprite: " + setSprite.name + " based on size: " + mySize + " and type: " + myType);
        }
        else
        {
            Debug.LogError("Sprite for " + mySize + " / " + myType + " not found in database!", this.gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.tag == "Bullet" && timesHit < 4)
        {
            timesHit++;
            //play shader

            if (timesHit >= 3)
            {
                //disable shader
                //spawn animation ruble break //add next sprint
                parentSprite.sprite = setSprite;
            }
        }
    }

}


