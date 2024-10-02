using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitMove : MonoBehaviour
{

    ///////////////////// lineRenderer////////////////////////////////
    public LineRenderer lineRenderer;
    [Range(6,60)]   //creates a slider - more than 60 is hard to notice
    public int lineCount;       //more lines = smoother ring
    public float movement;     // movment radius
    public float width;        // width of lines drawn
    public LineRenderer diagonal; 
    public LineRenderer MeleeCircle;
    public float meleerange;
    public Material moveStyle,line,melee,invalid;  // materials for lines 

    ///////////////////////////////////////////////////////////////////

    /////////////////// movement /////////////////////////////////////////
    Vector3 targetPos;
    public bool enableMovement = false;
    private float movementLeft;
    private Vector3 normalisedMovementVector;
    public CameraMovement controller;
    public GameObject unit ;
    public bool selected = false;
    private Vector3 oldPosition;
    public bool invalidTarget = false;
    ///////////////////////////////////////////////////////////////////////







    void Start()
    {
        
        lineRenderer.loop = true;   // makes line connect with eachother at ends
        MeleeCircle.loop = true;
        movementLeft = movement;
        
    }









    void Update () { 
        // creates vector that is the current position with its y axis set to 0 so normalising only effects x and z axis 
        Vector3 current = new Vector3(transform.position.x,0,transform.position.z);
        if(controller.getenableUnitMovement() == true && selected){
            lineRenderer.enabled = true;
            diagonal.enabled = true;
            MeleeCircle.enabled = true;
            Draw(lineRenderer,movementLeft,transform.position);

            // changes line material to the invalid material if mouse is over invalid target 
            if(invalidTarget ){
                lineRenderer.material = invalid;
                diagonal.material =invalid;
                MeleeCircle.material = invalid;
            }

            // changes line material if mouse position is allowed 
            if(!invalidTarget ){
                lineRenderer.material = moveStyle;
                diagonal.material =line;
                MeleeCircle.material = melee;

            }

            // if unit reaches target position disable movement
            // disallows movement if target is invalid 
            if((round2dp(current.x) == round2dp(targetPos.x))   ||(round2dp(current.z) == round2dp(targetPos.z)) || invalidTarget==true|| (targetPos == new Vector3(0,0,0))  ){
                
                // draws movement line when hovering
                Vector3 mousePoint =  controller.RaycastGround();
                DrawDiagonal(mousePoint);

                enableMovement = false;
            }


            // if unit can move and has movement left move it
            if(((movementLeft>=0.0f))&&(enableMovement == true)&&(invalidTarget ==false)){
                
                DrawDiagonal(targetPos);
                
                transform.position += normalisedMovementVector;
                // finds height of terrain where unit is
                float unitHeight = FindHeightLine(transform.position.x,transform.position.z);
                Vector3 unitPosition = transform.position;
                unitPosition.y = unitHeight+1;
                // changes unit height to be the height of the terrain
                transform.position= unitPosition;

                movementLeft+= -0.02f ;
                oldPosition = transform.position;


            }     

        }
        else{
            lineRenderer.enabled = false;
            diagonal.enabled = false;
            MeleeCircle.enabled = false;
        }
        

    }


    private float round2dp(float number){
        return Mathf.Round(number * 100f) / 100f;
    }











    // draws movement circle
    void Draw(LineRenderer renderer, float radius,Vector3 position) //Only need to draw when something changes
    {
        renderer.positionCount = lineCount;
        renderer.startWidth = width;
        float theta = (2f * Mathf.PI) / lineCount;  //find radians per segment
        float angle = 0;
        for (int i = 0; i < lineCount; i++)
        {
            float x = radius * Mathf.Cos(angle);
            float y = radius * Mathf.Sin(angle);

            float z = FindHeight(x,y,position)+0.1f;
            renderer.SetPosition(i, new Vector3(position.x+x, z, position.z+y));
            angle += theta;
        }
    }


    // draws digonal line towhere mouse is pointed 
    void DrawDiagonal( Vector3 target) 
    {
        int pointCount = 30;
        diagonal.startWidth = width;
        diagonal.positionCount = pointCount;
        Vector3 currentPos = transform.position;
        currentPos.y = 0;
        Vector3 targetPosition = target;
        targetPosition.y = 0;


        // checks if length between location and mouse position is greater than movement radius
        if((targetPosition-currentPos).magnitude >movementLeft){



            Vector3 finalTarget =  calculateFinalTarget(currentPos,targetPosition);
            //currentPos + clampedDirection;
            RaiseDiagonal(diagonal, currentPos, finalTarget, pointCount);
            // draws melee circle
            Draw(MeleeCircle, meleerange, finalTarget);
        }

        else{

            RaiseDiagonal(diagonal, currentPos,targetPosition,pointCount);
            Draw(MeleeCircle,meleerange,targetPosition);

        }
    }

    // clamps target vector to be movement radius left 
    private Vector3 calculateFinalTarget (Vector3 currentPos, Vector3 targetPosition){
        Vector3 direction = targetPosition - currentPos;
        Vector3 clampedDirection = Vector3.ClampMagnitude(direction, movementLeft);
        Vector3 finalTarget = currentPos + clampedDirection;

        return finalTarget;
    }

    void RaiseDiagonal(LineRenderer renderer ,Vector3 startLocation ,Vector3 targetPosition,int pointCount){
        // divides target vector by number of points on line 
        Vector3 lineSgment = (targetPosition-startLocation)/(pointCount-1);

        Vector3 previousPoint = startLocation;
        //////////// caclulates first point 
        Vector3 startPoint = startLocation;
        float y = FindHeightLine(startLocation.x,startLocation.z);
        startPoint.y = y+0.1f;
        renderer.SetPosition(0,startPoint);
        /////////////

        // for the number of points on the line it creates a point from last point to new point with height adusted for each point
        for(int point = 1; point < pointCount;point ++){
            Vector3 linePoint = previousPoint+lineSgment;
            // finds the height of terrain at that point
            float newY = FindHeightLine(linePoint.x,linePoint.z);
            linePoint.y = newY+0.1f;
            renderer.SetPosition(point,linePoint);
            previousPoint = linePoint;
        }
        
        CheckForObsitcal(previousPoint.x,previousPoint.z);
    }




    // calculates the direction the unit must move
    public void calculateNormalisedMovement(Vector3 targetPosition){
        // diallowed if target is invalid 
        if(invalidTarget==false){
            Vector3 currentPosition = transform.position;
            targetPos = calculateFinalTarget(currentPosition,targetPosition);

            Vector3 normalisedVector = (targetPosition - currentPosition).normalized;
            // divides the vector by 50 for walking effect
            normalisedMovementVector = new Vector3(normalisedVector.x/50,0,normalisedVector.z/50);
            enableMovement = true;
            //print("target " + targetPos);
            //print(normalisedMovementVector);
     

        
        }
        

        

    }

    // selects the unit when clicked
    void OnMouseDown()
    {
        controller.unitSelect(unit);
        lineRenderer.enabled = true;
        diagonal.enabled = true;
        MeleeCircle.enabled = true;
        Draw(lineRenderer,movementLeft,transform.position);



    }


    // finds all heights of terrain at points in the circle 
     private float FindHeight(float x, float z,Vector3 pos)
    {
        Vector3 startLocation = new Vector3(pos.x+x, 600f,pos.z+z);
        Ray ray = new Ray(startLocation, Vector3.down);
        // creates a ray that detects colllisions and returns co-oridnates of collision
        if (Physics.Raycast(ray, out RaycastHit hit, 600f,1<<3))
        {
            Vector3 position = hit.point;
            return position.y;
        }
        return 0;        
    }


    // finds height of terrain for single point not in circle 
    private float FindHeightLine(float x, float z)
    {
        Vector3 startLocation = new Vector3(x, 600f,z);
        Ray ray = new Ray(startLocation, Vector3.down);
        //Debug.DrawRay(ray.origin, ray.direction, Color.red, 40.0f);
        if (Physics.Raycast(ray, out RaycastHit hit, 600f,1<<3))
        {
            Vector3 position = hit.point;
            // Debug.Log("Hit: " + position.ToString() + " Normal: " + hit.normal.ToString() + " [Distance " + hit.distance.ToString() + " from " + hit.collider.gameObject.name + "]");
            //Debug.Log(position.y);
            
            return position.y;
        }
        return 0;        
    }


    // checks for collisions on layer with units and buildings.
    // excludes terrain
    public void CheckForObsitcal(float x, float z)
    {
        Vector3 startLocation = new Vector3(x, 600f,z);
        Ray ray = new Ray(startLocation, Vector3.down);
        // Debug.Log(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity,1<<0))
        {
            //print(true);
            invalidTarget = true;
        }
        else{
            invalidTarget = false;
        };
    }

    public void resetMovement(){
        movementLeft = movement;
    }
    
}
