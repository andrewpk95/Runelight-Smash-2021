using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.U2D.Animation;

public class ModelComponent : MonoBehaviour
{
    private bool isFacingRight = true;

    [SerializeField]
    private List<SpriteResolver> flipableSprites;

    public void UpdateSpriteDirection(bool isFacingRight)
    {
        if (this.isFacingRight == isFacingRight)
        {
            return;
        }
        Debug.Log(isFacingRight);
        this.isFacingRight = isFacingRight;
        transform.localScale = new Vector3(isFacingRight ? 1 : -1, 1, 1);

        foreach (SpriteResolver resolver in flipableSprites)
        {
            if (resolver == null)
            {
                continue;
            }
            string label = isFacingRight ? "Right" : "Left";

            resolver.SetCategoryAndLabel(resolver.GetCategory(), label);
            resolver.ResolveSpriteToSpriteRenderer();
        }
    }
}
