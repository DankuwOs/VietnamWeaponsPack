using System;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using UnityEngine;
using VTNetworking;
using VTOLVR.Multiplayer;

public class FlechetteAirburstRocket : Rocket
{
    public ParticleSystem[] particleSystems;
    public Gun flechetteGun;
    public float distance = 100f;
    public float delay = 0.1f;
    public Transform rayTf;

    private void Start()
    {
        if (!flechetteGun.actor)
        {
            flechetteGun.actor = this.sourceActor;
        }
    }

    public override void Fire(Actor sourceActor)
    {
        base.Fire(sourceActor);
        if (VTOLMPUtils.IsMultiplayer())
        {
            VTNetEntity component = base.GetComponent<VTNetEntity>();
            if (component && !component.isMine)
            {
                return;
            }
        }
        
        StartCoroutine(FiredRoutine());
    }

    private IEnumerator FiredRoutine()
    {
        this.flechetteGun.actor = this.sourceActor;

        while (!Physics.Raycast(rayTf.position, rayTf.forward, out var raycastHit, distance))
        {
            yield return null;
        }
        
        for (int i = 0; i < particleSystems.Length; i++)
        {
            var ps = particleSystems[i];
            ps.gameObject.transform.SetParent(null);
            ps.transform.position = this.transform.position;
            ps.transform.rotation = this.transform.rotation;
            ps.Emit((int)ps.emission.GetBurst(0).count.constant);
        }

        yield return new WaitForSeconds(delay);
        
        this.flechetteGun.SetFire(true);
        while (flechetteGun.currentAmmo > 0)
        {
            yield return null;
        }

        this.Detonate();
    }
}