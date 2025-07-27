using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder;

public class TemporaryScanDot : Spawnable
{
    [SerializeField] float lifeTime;
    float deathTime;
    MaterialPropertyBlock mpb;
    Renderer r;

    protected override void Awake()
    {
        base.Awake();
        r = GetComponent<Renderer>();
        mpb = new MaterialPropertyBlock();
    }

    public override void Initialize(Vector3 position, Quaternion rotation, Spawnable reference)
    {
        base.Initialize(position, rotation, reference);
        deathTime = Time.time + lifeTime;
        mpb.SetFloat("_Fade", 1);
        r.SetPropertyBlock(mpb);
    }

    private void Update()
    {
        float timeRemaining = deathTime - Time.time;
        if (Time.time > deathTime)
        {
            spawnablesManager.RequeueSpawnable(this);
            return;
        }

        float fade = Mathf.Clamp01(timeRemaining / lifeTime);

        // Update material property block
        mpb.SetFloat("_Fade", fade);
        r.SetPropertyBlock(mpb);
    }
}
