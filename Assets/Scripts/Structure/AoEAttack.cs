using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AoEAttack : MonoBehaviour
{
    //Caster can trigger attack if target is within area.
    [SerializeField] Collider[] triggerArea;
    //Actual hitboxes of attack.
    [SerializeField] AreaAttack[] attacks;

    [SerializeField] CasterEntity caster;
    [SerializeField] bool harmsCaster;
    [SerializeField] float startup;
    [SerializeField] float recovery;


    void Start()
    {
        foreach(AreaAttack attk in attacks)
            attk.SetFadeAmts(startup, recovery);
    }

    public IEnumerator<WaitForSeconds> Trigger()
    {
        caster.AttackStart();

        foreach(AreaAttack attk in attacks) { attk.Warn(); } 

        yield return new WaitForSeconds(startup);

        if (harmsCaster)
        {
            foreach (Entity entity in GameState.entities) { TryHit(entity); }
        }
        else
        {
            foreach (Entity entity in GameState.entities)
            {
                if (entity != caster) { TryHit(entity); }
            }
        }

        foreach(AreaAttack attk in attacks) { attk.Attk(); }

        yield return new WaitForSeconds(recovery);
        
        caster.AttackEnd();
    }

    public void TryHit(Entity entity)
    {
        foreach (AreaAttack attk in attacks)
        {
            if (attk.hitBox.bounds.Contains(entity.transform.position))
            {
                entity.OnHit(attk);
                return;
            }
        }
    }

    AoEAttack(AreaAttack[] attacks,
          Collider[] area,
          CasterEntity caster,
          float startup,
          float recovery,
          bool harmsCaster = false)
    {
        this.attacks = attacks;
        this.triggerArea = area;
        this.caster = caster;
        this.startup = startup;
        this.recovery = recovery;
        this.harmsCaster = harmsCaster;
    }
}
