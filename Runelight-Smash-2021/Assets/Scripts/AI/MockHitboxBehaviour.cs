using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MockHitboxBehaviour : MonoBehaviour
{
    public int hitRateFrame = 60;
    public int hitboxDurationFrame = 5;
    public HitboxInfo hitboxInfo;

    private int hitRateLeft;
    private int hitboxDurationLeft;

    private HitboxComponent hitbox;

    void Start()
    {
        hitRateLeft = hitRateFrame;
        hitboxDurationLeft = hitboxDurationFrame;
    }

    void FixedUpdate()
    {
        hitRateLeft--;

        if (hitRateLeft == 0)
        {
            hitbox = HitboxFactory.Instance.GetObject();
            hitbox.gameObject.transform.SetParent(this.gameObject.transform);
            hitbox.gameObject.transform.localPosition = Vector3.zero;
            hitbox.hitboxInfo = hitboxInfo;
        }
        else if (hitRateLeft < 0)
        {
            hitboxDurationLeft--;

            if (hitboxDurationLeft <= 0)
            {
                HitboxFactory.Instance.ReturnObject(hitbox);
                hitbox = null;
                hitRateLeft += hitRateFrame;
                hitboxDurationLeft += hitboxDurationFrame;
            }
        }
    }
}
