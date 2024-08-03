using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grass : MonoBehaviour
{
    public ParticleSystem fxHit;
    public bool isCut;
    [SerializeField]
    private int timeGrassGrow = 15;


    public void GetHit(int amountDmg)
    {
        if(!isCut)
        {
            fxHit.Emit(10);
            transform.localScale = new Vector3(1f,1f,1f);
            isCut = true;
            StartCoroutine(LeafGrows());
        }

    }

    IEnumerator LeafGrows()
    {
        yield return new WaitForSeconds(timeGrassGrow);
        transform.localScale = new Vector3(3f, 3f, 3f);
        isCut = false;
    }
}
