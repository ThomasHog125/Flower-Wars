using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraMovement : MonoBehaviour
{
    
    public bool enableMovement = false;
    public GameObject playerCamera;
    private Transform player;
    public float mouseSensitivity;
    public float playerSpeed;
    private float inputX = 0f;
    private float inputY = 0f;

    public bool unitSelected = true;
    public GameObject unit;


    public Camera mainCamera;


    private bool enableUnitMovement = false;

    public TextMeshProUGUI HealthText;
    public TextMeshProUGUI DefenceText;
    public TextMeshProUGUI AttackText;
    public TextMeshProUGUI UnitNameText;
    // Start is called before the first frame update
    void Start()
    {
        player = playerCamera.transform;


    }

    // Update is called once per frame
    void Update()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            // Prevent game interactions
            return;
        }





        if (enableMovement == true)
        { 
            // changes cameras psoition
            float xMovement = Input.GetAxis("Horizontal");
            float yMovement = Input.GetAxis("Vertical");
 
            Vector3 move = new Vector3(xMovement, 0, yMovement);
            move = Camera.main.transform.TransformDirection(move);
            player.position += move * playerSpeed * Time.deltaTime;


            // changes cameras rotation when right click is pressed 
            if (Input.GetMouseButton(1))
            {
                changeCamera();

            }
        }

        // finds location user clicked on and gives that info to unit for movement
        if ((Input.GetMouseButton(0)) && (enableUnitMovement == true))
        {
            Vector3 targetPos = RaycastGround();
            UnitMove unitScript= unit.GetComponent<UnitMove>();
            unitScript.calculateNormalisedMovement(targetPos);
        }


    }

    // function for finding click location 
    public Vector3 RaycastGround()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity,1<<3))
        {
            Vector3 position = hit.point;
            return position;
        }
        return new Vector3(0,0,0);
    }

        
    // changes cameras rotation 
    void changeCamera()
    {
        inputX = inputX - Input.GetAxis("Mouse X") * mouseSensitivity;
        inputY = Mathf.Clamp(inputY - Input.GetAxis("Mouse Y") * mouseSensitivity, -90f, 90f);
        player.rotation = Quaternion.Euler(inputY, -1 * (inputX), 0);
    }



    // changes unit controller effetcs 
    public void unitSelect(GameObject newUnit){
        UnitMove unitScript = unit.GetComponent<UnitMove>();
        unitScript.selected = false;
        unitScript.enableMovement = false;
        unit = newUnit;
        unitScript = unit.GetComponent<UnitMove>();
        unitScript.selected = true;
        unitController unitcontroller = unit.GetComponent<unitController>();
        HealthText.text = "Health : "+unitcontroller.currentHealth+"/"+unitcontroller.startHealth;
        DefenceText.text = "Defence : "+unitcontroller.defence;
        AttackText.text = "Attack : "+ unitcontroller.attackStrength;
        UnitNameText.text = unitcontroller.type;
        enableUnitMovement = false;
        
    }


    public void changeEnableUnitMovement(){

        enableUnitMovement = !enableUnitMovement;

    }

    public bool getenableUnitMovement(){
        return enableUnitMovement;
    }


    

}

