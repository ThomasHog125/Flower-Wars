using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextTurn : MonoBehaviour
{

    public Turns player1Turns;
    // Start is called before the first frame update
    public void endTurn(){
        player1Turns.turnReset();
    }


}
