using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXManager : MonoBehaviour
{
    public static SFXManager instance;

    private void Awake()
    {
        instance = this;
    }

    public AudioSource bagSound, explodeSound, stoneSound, roundOverSound, buttonOver, buttonClick, bagSpawnSFX, bagNoMatchSFX;

    public void PlayBagBreak()
    {
        //bagSound.Stop();
        bagSound.pitch = Random.Range(.8f, 1.2f);
        bagSound.Play();
    }

    public void PlayExplode()
    {
        //explodeSound.Stop();
        explodeSound.pitch = Random.Range(.8f, 1.2f);
        explodeSound.Play();
    }

    public void PlayStoneBreak()
    {
        //stoneSound.Stop();
        stoneSound.pitch = Random.Range(.8f, 1.2f);
        stoneSound.Play();
    }

    public void PlayRoundOver()
    {
        roundOverSound.Play();
    }

    public void ButtonOver()
    {
        buttonOver.Play();
    }

    public void ButtonClick()
    {
        buttonClick.Play();
    }

    public void BagSpanwnSFX()
    {
        bagSpawnSFX.pitch = Random.Range(.4f, 2.8f);
        bagSpawnSFX.Play();
    }

    public void BagNoMatchSFX()
    {
        //bagNoMatchSFX.Stop();
        bagNoMatchSFX.pitch = Random.Range(.4f, 2.8f);
        bagNoMatchSFX.Play();
    }
}
