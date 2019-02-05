using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DamageHandler
{
    public static float DamageCalculation(Stats stats1, Stats stats2, Move move, float terrainBonus)
    {
        float res = 0;
        switch (move.moveElement)
        {
            case Element.fire:
                res = stats2.fire_res + stats2.fire_type_res;
                break;
            case Element.water:
                res = stats2.water_res + stats2.water_type_res;
                break;
            case Element.nature:
                res = stats2.nature_res + stats2.nature_type_res;
                break;
        }
        switch(move.moveType)
        {
            case MoveType.physical:
                return (move.damage * (stats1.atk / stats2.def)) * (2f - res) * terrainBonus * Random.Range(0.75f, 1.25f);
            case MoveType.special:
                return (move.damage * (stats1.sp_atk / stats2.sp_def)) * (2f - res) * terrainBonus * Random.Range(0.75f, 1.25f);
            default:
                return 0;
        }
       
    }

}
