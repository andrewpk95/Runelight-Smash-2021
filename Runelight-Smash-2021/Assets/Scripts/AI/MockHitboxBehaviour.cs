using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MockHitboxBehaviour : MonoBehaviour
{
    public int hitRateFrame = 60;
    public int hitboxDurationFrame = 5;
    public int rotationRate = 2500;
    public List<HitboxInfo> hitboxInfos = new List<HitboxInfo>();

    private int hitRateLeft;
    private int hitboxDurationLeft;

    private List<HitboxComponent> hitboxes = new List<HitboxComponent>();

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
            foreach (HitboxInfo hitboxInfo in hitboxInfos)
            {
                HitboxComponent hitbox = HitboxFactory.Instance.GetObject();

                hitbox.gameObject.transform.SetParent(this.gameObject.transform);
                hitbox.gameObject.transform.localPosition = Vector3.zero;
                hitbox.SetHitboxInfo(hitboxInfo);

                hitboxes.Add(hitbox);
            }

        }
        else if (hitRateLeft < 0)
        {
            hitboxDurationLeft--;

            if (hitboxDurationLeft <= 0)
            {
                HitboxResolverComponent.Instance.ResetVictimList(transform.root.gameObject);

                foreach (HitboxComponent hitbox in hitboxes)
                {
                    HitboxFactory.Instance.ReturnObject(hitbox);
                }
                hitboxes.Clear();

                hitRateLeft += hitRateFrame;
                hitboxDurationLeft += hitboxDurationFrame;
            }
        }

        transform.Rotate(Vector3.forward * rotationRate * Time.fixedDeltaTime);
    }
}
