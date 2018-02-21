﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : Ability {

    public int Damage;

    void Start()//Initializes stats
    {
        var mod = gameObject;
        mod.SetActive(false);
    }

	public int Aim(Target target)
    {
        int accuracy = 0;
		if(!IsTargetInRange(owner, target))
		{
            float distance = Vector3.Distance(owner.transform.position, target.GetCharacterTarget().transform.position);
            if (distance < (2 * range))
			{
				accuracy = (int)(((distance % range) / range) * 100);
			}
		}
		else {
			accuracy = 100;
		}
		return accuracy;
	}

    IEnumerator EndFire()
    {
        yield return new WaitForSeconds(1);
        var mod = gameObject;
        mod.SetActive(false);
    }

    // Attack Function
    public override void Execute(Target target)
    {
        Vector3 targetPoint = new Vector3(target.GetCharacterTarget().transform.position.x, this.owner.transform.position.y, 
            target.GetCharacterTarget().transform.position.z) - this.owner.transform.position;
        this.owner.transform.rotation = Quaternion.LookRotation(targetPoint, Vector3.up);
        var mod = gameObject;
        mod.SetActive(true);
        int dam = 0;
        int accuracy = Aim(target);
		System.Random rand = new System.Random ();
		int num = rand.Next (0, 100);
		if(num < accuracy){
            dam = Damage;
        }
        if (target.GetTargetType().Equals(Target.TargetType.Enemy))
        {
            target.GetCharacterTarget().TakeDamage(dam);
        }
        var exp = GetComponent<ParticleSystem>();
        exp.Play();
        StartCoroutine(EndFire());
    }
}
