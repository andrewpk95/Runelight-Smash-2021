using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.U2D.Animation;

[System.Serializable]
public class FlipableSpriteRecord
{
    public string category;
    public SpriteResolver resolver;
}

public class ModelComponent : MonoBehaviour
{
    [SerializeField]
    public List<FlipableSpriteRecord> flipableSprites;

    // Update is called once per frame
    void Update()
    {
        UpdateSpriteDirection();
    }

    void OnValidate()
    {
        UpdateSpriteDirection();
    }

    void UpdateSpriteDirection()
    {
        foreach (FlipableSpriteRecord record in flipableSprites)
        {
            SpriteResolver resolver = record.resolver;
            string label = resolver.transform.lossyScale.x > 0 ? "Right" : "Left";

            resolver.SetCategoryAndLabel(record.category, label);
            resolver.ResolveSpriteToSpriteRenderer();
        }
    }
}
