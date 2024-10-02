using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turns : MonoBehaviour
{

    public List<GameObject> units;

    public void turnReset(){
        for (int i =0;i<units.Count ;i++){
            GameObject unit = units[i];
            UnitMove movementscript =unit.GetComponent<UnitMove>();
            movementscript.resetMovement();

        }
    }

}
