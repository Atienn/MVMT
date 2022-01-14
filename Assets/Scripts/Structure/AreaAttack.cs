using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Misc;

[RequireComponent (typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class AreaAttack : Attack
{
    static Color noneColor = new Color(0f, 0f, 0f, 0f);
    static Color attkColor = new Color(1f, 0f, 0f, 0.6666666f);
    static Color warnColor = new Color(1f, 0.5f, 0f, 0.6666666f);

    float attkFadeAmt = 0.025f;
    float warnFadeAmt = 0.01f;

    public Collider hitBox;
    MeshRenderer mesh;

    float alpha;

    VoidStrategy fadeStrat;


    void Start()
    {
        hitBox = GetComponent<Collider>();
        mesh = GetComponent<MeshRenderer>();
        fadeStrat = Methods.None;
        mesh.material.color = noneColor;
    }

    void FixedUpdate() { fadeStrat(); }

    public void SetFadeAmts(float startup, float recovery)
    {
        warnFadeAmt = startup * Time.fixedDeltaTime;
        attkFadeAmt = recovery * Time.fixedDeltaTime;
    }

    public void Warn()
    {
        mesh.material.color = warnColor;
        alpha = warnColor.a;
        fadeStrat = WarnFade;
    }

    public void Attk()
    {
        mesh.material.color = attkColor;
        alpha = attkColor.a;
        fadeStrat = AttkFade;
    }

    void WarnFade()
    {
        alpha -= warnFadeAmt;
        if (alpha < 0) { fadeStrat = Methods.None; }
        else { mesh.material.color = new Color(warnColor.r, warnColor.g, warnColor.b, alpha); }
    }

    void AttkFade()
    {
        alpha -= attkFadeAmt;
        if (alpha < 0) 
        { 
            fadeStrat = Methods.None;
            alpha = 0;
        }
        mesh.material.color = new Color(attkColor.r, attkColor.g, attkColor.b, alpha);
    }

    public AreaAttack(Vector3 direction, float damage, Collider hitBox) : base(direction, damage)
    {
        this.hitBox = hitBox;
    }
}