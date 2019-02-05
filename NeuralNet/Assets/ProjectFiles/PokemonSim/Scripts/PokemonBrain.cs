using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PokemonBrain : MonoBehaviour
{
    const int secondaryGeneSize = 14; 
    bool hasBeenInitialised = false;
    List<GenePair> genes;
    NeuralNet net;
    public Stats pokemonStats;
    public LayerMask interactable;


    public int[] test = { 20, 30, 65, 56, 80, 90 };

    float[] NormalizeArray(float[] array, float maxSize, float currentSize)
    {
        float ratio = maxSize / currentSize;
        for(int i = 0; i < array.Length; i++)
        {
            array[i] = array[i] * ratio;
        }
        return array;
    }
    
    public void Init(int inputSize, int[] hiddenSizes, int outputSize)
    {
        net = new NeuralNet(inputSize, hiddenSizes[0], hiddenSizes[1], outputSize);

        hasBeenInitialised = true;
    }
    public void Init(NeuralNet net, List<GenePair> otherGeneData, Stats stats)
    {

    }
    private void Update()
    {
        if (hasBeenInitialised)
        {
            //run network
        }
    }

}
[System.Serializable]
public class Stats
{
    public float currentHp;
    public float hp;
    public float atk;
    public float def;
    public float sp_atk;
    public float sp_def;
    public float spd;   
    public float fire_res;
    public float water_res;
    public float nature_res;
    public float fire_type_res;
    public float water_type_res;
    public float nature_type_res;

    /* types:
     *      0. fire
     *      1. water
     *      2. nature
     */

    public Stats(float _hp, float _atk, float _def, float _sp_atk, float _sp_def, float _spd, float _fires, float _watres, float _natres, int type)
    {
        hp = _hp;
        atk = _atk;
        def = _def;
        sp_atk = _sp_atk;
        sp_def = _sp_def;
        spd = _spd;
        currentHp = hp;
        fire_res = _fires;
        water_res = _watres;
        nature_res = _natres;
        SetTypeResistances(type);
    }
    public Stats(float[] statsArray, int type)
    {
        hp = statsArray[0];
        atk = statsArray[1];
        def = statsArray[2];
        sp_atk = statsArray[3];
        sp_def = statsArray[4];
        spd = statsArray[5];
        fire_res = statsArray[6];
        water_res = statsArray[7];
        nature_res = statsArray[8];
        currentHp = hp;
        SetTypeResistances(type);
    }
    public Stats(float hp_min, float hp_max, float stat_min, float stat_max, float res_min, float res_max, int type)
    {
        hp = Random.Range(hp_min, hp_max);
        atk = Random.Range(stat_min, stat_max);
        def = Random.Range(stat_min, stat_max);
        sp_atk = Random.Range(stat_min, stat_max);
        sp_def = Random.Range(stat_min, stat_max);
        spd = Random.Range(stat_min, stat_max);
        fire_res = Random.Range(res_min, res_max);
        water_res = Random.Range(res_min, res_max);
        nature_res = Random.Range(res_min, res_max);
        currentHp = hp;
        SetTypeResistances(type);
    }
    public void SetTypeResistances(int type)
    {
        switch (type)
        {
            case 0: fire_type_res = 1.5f; water_type_res = 0.5f; nature_res = 1f;
                break;
            case 1: fire_type_res = 1f; water_type_res = 1.5f; nature_res = 0.5f;
                break;
            case 2: fire_type_res = 0.5f; water_type_res = 1.0f; nature_res = 1.5f;
                break;
        }
    }
    public void HandleDamage(int damage)
    {
        hp -= damage;
    }
}
