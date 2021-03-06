﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GrenadeAbility : Ability
{
    public GameObject GrenadePrefab, ExplosionPrefab;
    public float TimeToAsplode;
    public float AsplodingHurtDistance;
    public float NadeDamage;
    protected GameObject characterGameObject;
    protected GameObject dat_nade;
    protected Rigidbody GrenadeRB = null;

    public override void Execute(Target target)
    {
        base.Execute(target);
        Vector3 destination = target.GetLocationTarget();
        characterGameObject = this.owner.gameObject;

        dat_nade = Instantiate(GrenadePrefab, characterGameObject.transform.position + new Vector3(0, 2.5f, 0), Quaternion.identity);

        GrenadeRB = dat_nade.GetComponent<Rigidbody>();

        Vector3 ThrowVector = destination - characterGameObject.transform.position;

        ThrowVector = ThrowVector * 19.0f;

        GrenadeRB.AddForce(ThrowVector);

        StartCoroutine(NadeAsplode(TimeToAsplode));
    }

    private void Update()
    {
        if (GrenadeRB != null)
        {
            FindObjectOfType<CameraController>().FocusLocation(GrenadeRB.transform.position);
        }
    }

    public override void DoneWaiting()
    {
        base.DoneWaiting();
        GrenadeRB = null;
        Destroy(dat_nade);
        FindObjectOfType<CameraController>().FocusLocation(owner.transform.position);
    }

    IEnumerator NadeAsplode(float explosion_time)
    {
        yield return new WaitForSeconds(explosion_time - 0.1f);

        // create the explosion
        GameObject my_nade_asplode = Instantiate(ExplosionPrefab, dat_nade.transform.position, Quaternion.identity);

        // calculate damage to enemies & friendlies
        for (int i = 0; i < owner.owner.friendlies.Count; i++)
        {
            float distanceToNade = Vector3.Distance(owner.owner.friendlies[i].transform.position, dat_nade.transform.position);
            if (distanceToNade < AsplodingHurtDistance)
            {
                owner.owner.friendlies[i].TakeDamage((int)(0.6 * NadeDamage * (1 / distanceToNade)));
            }
        }
        for (int i = 0; i < owner.owner.enemy.friendlies.Count; i++)
        {
            float distanceToNade = Vector3.Distance(owner.owner.enemy.friendlies[i].transform.position, dat_nade.transform.position);
            if (distanceToNade < AsplodingHurtDistance)
            {
                owner.owner.enemy.friendlies[i].TakeDamage((int)(NadeDamage * (1/System.Math.Pow(distanceToNade, (1.0/3.0)))));
            }
        }
        // destroy nade and then the explosion object 5 seconds later
        dat_nade.SetActive(false);
        Destroy(my_nade_asplode, 5.0F);
        this.isDone = true;
    }
}

