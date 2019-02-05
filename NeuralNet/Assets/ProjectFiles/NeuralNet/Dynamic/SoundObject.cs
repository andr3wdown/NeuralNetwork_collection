using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundObject : MonoBehaviour
{
    public const float speedOfSound = 80f;
    public const float soundDispersal = 40f;
    public float loudness = 10f;
    float maxLoudness;
    bool initialised = false;
    SpriteRenderer srd;
    float scale = 1f;
    public void InitSound(Color c, float _loudness)
    {
        loudness = _loudness;
        srd = GetComponent<SpriteRenderer>();
        srd.color = c;
        maxLoudness = loudness;
        initialised = true;
    }
    public void Update()
    {
        if (initialised)
        {
            loudness -= Time.deltaTime * soundDispersal;
            if (loudness <= 0)
            {
                Destroy(gameObject);
            }
            float ratio = loudness / maxLoudness;
            Color c = srd.color;
            c.a = ratio;
            srd.color = c;
            scale += Time.deltaTime * speedOfSound;
            transform.localScale = new Vector3(scale, scale, scale);
        }      
    }
    public Color GetSoundData()
    {
        return srd.color;
    }

}
