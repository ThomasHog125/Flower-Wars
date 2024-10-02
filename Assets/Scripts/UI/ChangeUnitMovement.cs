using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeUnitMovement : MonoBehaviour
{
    public CameraMovement camController;
    public void changeMovement(){
        camController.changeEnableUnitMovement();
    }
}
