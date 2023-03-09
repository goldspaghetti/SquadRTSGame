using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class Unit : MonoBehaviour
{
    // Start is called before the first frame update
    public Queue<Command> commandQueue = new Queue<Command>();
    public ArrayList concurrentCommands = new ArrayList();
    public MoveCommand currMoveCommand;
    public Command currRotateCommand;

    public GameManager gameManager;
    public Command currCommand;
    public Vector2 unitPos;
    public float speed = 1;
    public int team = 0;
    public float unitSize = 1;

    //protected float currVisionDeg;
    public float visionRadius = 70f;
    public float currRotationDeg = 0;
    private float targetRotationDeg = 0;
    public float rotationSpeed = 360;    
    public float fireRate = 0.5f;
    public float fireRateTimer = 0.0f;
    protected float gunUpTime = 0.5f;
    public float gunAngle = 0;
    protected float accuracy = 3f;
    private int engageState = 0;
    public int hitpoints = 5;
    public bool dead = false;
    private int gunshotMaxDis = 50;
    protected Vector2 gunOffset = new Vector2(0.5f, 0.75f);
    public float gunRotateAngle = 40;
    public GameObject gunfirePrefab;
    public CircleCollider2D circleCollider2D;
    //public ArrayList enemiesSeen = new ArrayList();
    private bool[] enemiesSeen;
    //maybe have currTarget the index?
    public Unit currTarget;
    protected LayerMask enemyLayer;
    protected LayerMask wallLayer = 8;
    public bool currEngaging = false;

    public void init(int team, Vector2 unitPos, GameManager gameManager){
        this.team = team;
        if (this.team == 0){
            //CHANGE!
            Light2D unitVisionLight = gameObject.GetComponent<Light2D>();
            Debug.Log(unitVisionLight);
            unitVisionLight.pointLightInnerAngle = this.visionRadius;
            unitVisionLight.pointLightOuterAngle = this.visionRadius;
            enemyLayer=6;
        }
        else{
            //CHANGE!
            enemyLayer=7;
        }
        this.unitPos = unitPos;
        this.transform.position = new Vector3(this.unitPos.x, this.unitPos.y, 0);
        this.gameManager = gameManager;
        enemiesSeen = new bool[gameManager.enemyUnits.Length];
    }
    void Start(){
        this.circleCollider2D = gameObject.GetComponent<CircleCollider2D>();
        //Light2D unitVisionLight = gameObject.GetComponent<Light2D>();
        //Debug.Log(unitVisionLight);
    }
    void Update()
    {
        this.unitPos.x = this.transform.position.x;
        this.unitPos.y = this.transform.position.y;
        manageGunTimer();

        if (currCommand == null && commandQueue.Count > 0){
            currCommand = commandQueue.Dequeue();
            currCommand.updateCommand(this);
        }
        if (currCommand != null){
            if (currCommand.executeCommand(this)){
                Debug.Log("finished executing command");
                currCommand = null;
            }
        }
        if (currMoveCommand != null){
            if (currMoveCommand.executeCommand(this)){
                currMoveCommand = null;
            }
        }
        if (currRotateCommand != null){
            if (currRotateCommand.executeCommand(this)){
                currRotateCommand = null;
            }
        }

        foreach (Command command in concurrentCommands.ToArray()){
            if (command.executeCommand(this)){
                //commandsToRemove.Add(command);
                Debug.Log("removing com");
                concurrentCommands.Remove(command);
            }
        }
    }
    void checkSeenEnemies(){
        for (int i = 0; i < gameManager.enemyUnits.Length; i++){
            Unit enemyUnit = gameManager.enemyUnits[i];
            //enemiesSeen[i] = 
            bool currSeen = Physics2D.Linecast(this.unitPos, enemyUnit.unitPos, wallLayer);
            if (currSeen){
                //sees enemy
                if (enemiesSeen[i] != true && currEngaging == false){
                    currTarget = enemyUnit;
                    currEngaging = true;

                }
            }
            enemiesSeen[i] = currSeen;
        }
    }
    void manageGunTimer(){
        if (fireRateTimer != 0){
            fireRateTimer -= Time.deltaTime;
            if (fireRateTimer < 0){
                fireRateTimer = 0;
            }
        }
    }
    void rotateToTarget(){
        if (currRotationDeg != targetRotationDeg){
            float rotationAmount = rotationSpeed * Time.deltaTime;
            if (currRotationDeg < targetRotationDeg){
                if (currRotationDeg + rotationAmount < targetRotationDeg){
                    currRotationDeg += rotationAmount;
                }
                else{
                    currRotationDeg = targetRotationDeg;
                }
            }
            else if (currRotationDeg > targetRotationDeg){
                if (currRotationDeg - rotationAmount > targetRotationDeg){
                    currRotationDeg -= rotationAmount;
                }
                else{
                    currRotationDeg = targetRotationDeg;
                }
            }
        }
        //gameObject.transform.rotation. (currRotationDeg, 0, 0);
    }
    void handleAI(){
        checkSeenEnemies();
        //OH NO NO ENEMY DIED/OUT OF SIGHT
        if (currTarget != null){
            Debug.Log("oh shite");
            //ENGAGE!

            //check if there is currTarget

            //check if facing angle is enough, if not, face min enough towards enemy

            //move gun towards enemy

            //fire!
        }

        else{
            for (int i = 0; i < gameManager.enemyUnits.Length; i++){
                //maybe change to distance?
                if(enemiesSeen[i]){
                    currTarget = gameManager.enemyUnits[i];
                    break;
                }
            }
        }

        //ENGAGE CURRENT TARGET!

            //check if there is currTarget

            //check if facing angle is enough, if not, face min enough towards enemy

            //move gun towards enemy

            //fire!

    }
    void fireAtPosition(Vector2 position){
        //simulate gunshot
        
        //Physics2D.Raycast(this.unitPos, )
        
        //generate gameObject

    }
    void changeMoveState(int newMoveState){

    }

}


//MAYBE INIT AND ADD TO QUEUE/CONCURENT WITHOUT NEEDING TO AUTO DO THAT
public abstract class Command{
    
    public abstract bool commandFinished(Unit unit);
    public abstract bool executeCommand(Unit unit);
    public abstract void updateCommand(Unit unit);

}
//PROBABLY REDO LATER!
public class MoveToPosCommand : Command{
    private Vector2 newPos;
    private Vector2 unitStartPos;
    private float currProgress = 0;
    private float distance;
    public MoveToPosCommand(Vector2 newPos){
        this.newPos = newPos;
        //this.unitStartPos = unitStartPos;
    }
    public override void updateCommand(Unit unit)
    {
        this.unitStartPos = unit.unitPos;
        this.distance = Vector2.Distance(this.unitStartPos, newPos);
    }
    public override bool commandFinished(Unit unit){
        return unit.unitPos.Equals(this.newPos);
    }
    public override bool executeCommand(Unit unit)
    {
        //add pathing later, just moving through things for now
        this.currProgress += Time.deltaTime;

        unit.unitPos = Vector2.Lerp(this.unitStartPos, this.newPos, this.currProgress/(this.distance/unit.speed));
        unit.transform.position = unit.unitPos;
        //new Vector3(unit.unitPos.x, unit.unitPos.y, 0);
        return commandFinished(unit);
    }
}
public class RotateToTarget : Command{
    private float currProgress = 0;
    public float targetRotationDeg;
    public float positiveRotation;
    private bool rotateDegDecided;
    public RotateToTarget(float targetRotation){
        //BECAUSE UNITY UNIT ROTATION IS DUMB
        this.targetRotationDeg = targetRotation;
        this.positiveRotation = targetRotationDeg;
        
        if (targetRotationDeg < 0){
            positiveRotation = 360 + targetRotationDeg;
        }
        rotateDegDecided = true;
    }
    public RotateToTarget(){
        rotateDegDecided = false;
    }
    public void updateRotateDeg(float newTargetRotation){
        this.targetRotationDeg = newTargetRotation;
        rotateDegDecided = true;
        if (targetRotationDeg < 0){
            positiveRotation = 360 + targetRotationDeg;
        }
        rotateDegDecided = true;
    }
    public override void updateCommand(Unit unit)
    {
        
    }
    public override bool commandFinished(Unit unit)
    {
        return unit.currRotationDeg == targetRotationDeg;
    }
    public override bool executeCommand(Unit unit)
    {
        //Debug.Log("WTF!");
        if (unit.currRotationDeg != targetRotationDeg && rotateDegDecided){
            float rotationAmount = unit.rotationSpeed * Time.deltaTime;
            //Debug.Log(unit.rotationSpeed);
            //MAKE THE TURNING THE MOST EFFICIENT LATER!
            if (unit.currRotationDeg < targetRotationDeg){
                if ((unit.currRotationDeg + rotationAmount) < targetRotationDeg){
                    /*Debug.Log("movingR");
                    Debug.Log(targetRotationDeg);
                    Debug.Log(rotationAmount);
                    Debug.Log(unit.currRotationDeg);
                    Debug.Log(unit.currRotationDeg + rotationAmount);*/
                    unit.currRotationDeg += rotationAmount;

                }
                else{
                    unit.currRotationDeg = targetRotationDeg;
                    unit.gameObject.transform.eulerAngles = new Vector3(0, 0, unit.currRotationDeg);
                    /*Debug.Log("we're there!");
                    Debug.Log(targetRotationDeg);
                    Debug.Log(rotationAmount);
                    Debug.Log(unit.currRotationDeg);
                    Debug.Log(unit.currRotationDeg + rotationAmount);*/
                    return true;
                }
            }
            else if (unit.currRotationDeg > targetRotationDeg){
                if ((unit.currRotationDeg - rotationAmount) > targetRotationDeg){
                    unit.currRotationDeg -= rotationAmount;
                    /*Debug.Log("movingL");
                    Debug.Log(targetRotationDeg);
                    Debug.Log(rotationAmount);
                    Debug.Log(unit.currRotationDeg);
                    Debug.Log(unit.currRotationDeg + rotationAmount);*/
                }
                else{
                    unit.currRotationDeg = targetRotationDeg;
                    unit.gameObject.transform.eulerAngles = new Vector3(0, 0, unit.currRotationDeg);
                    /*Debug.Log("we're there");
                    Debug.Log(targetRotationDeg);
                    Debug.Log(rotationAmount);
                    Debug.Log(unit.currRotationDeg);
                    Debug.Log(unit.currRotationDeg + rotationAmount);*/
                    return true;
                }
            }
        }

        //Debug.Log(unit.currRotationDeg);
        //Debug.Log()
        //unit.currRotationDeg = targetRotationDeg;
        //unit.currRotationDeg = -70;
        //unit.currRotationDeg = targetRotationDeg;
        unit.gameObject.transform.eulerAngles = new Vector3(0, 0, unit.currRotationDeg);
        return commandFinished(unit);
    }
}
public class FocusOnPoint : Command{
    private Vector2 point;
    private RotateToTarget rotateToTarget;
    public FocusOnPoint(Vector2 point){
        this.point = point;
        this.rotateToTarget = new RotateToTarget();
    }
    public override void updateCommand(Unit unit)
    {
        
    }
    public override bool commandFinished(Unit unit)
    {
        return false;
    }
    public override bool executeCommand(Unit unit){
        float angleToTarget = Mathf.Rad2Deg * Mathf.Asin((unit.currTarget.unitPos.y - unit.unitPos.y)/Vector2.Distance(unit.currTarget.unitPos, unit.unitPos));
        angleToTarget -= 90;
        if ((unit.currTarget.unitPos.x - unit.unitPos.x) < 0){
            angleToTarget *= -1;
        }
        rotateToTarget.updateRotateDeg(angleToTarget);
        rotateToTarget.executeCommand(unit);
        return commandFinished(unit);
    }
    public void updatePoint(Vector2 newPoint){
        this.point = newPoint;
    }
    public void stopCommand(Unit unit){

    }
}
public class FocusOnAngle : Command{
    public override void updateCommand(Unit unit)
    {
        
    }
    public override bool commandFinished(Unit unit)
    {
        return false;
    }
    public override bool executeCommand(Unit unit){
        return commandFinished(unit);
    }
    public void stopCommand(Unit unit){

    }
}
public class MoveGunToTarget : Command{
    /*public MoveGunToTarget(float ){

    }*/
    public override void updateCommand(Unit unit)
    {
        
    }
    public override bool commandFinished(Unit unit)
    {
        return false;
    }
    public override bool executeCommand(Unit unit)
    {
        return false;
    }
}
public class FireAtPosition : Command{
    public override void updateCommand(Unit unit)
    {
        
    }
    public override bool commandFinished(Unit unit)
    {
        return false;
    }
    public override bool executeCommand(Unit unit)
    {
        return false;
    }
}
public class engageEnemy : Command{
    private RotateToTarget rotateToTargetCommand;
    private MoveGunToTarget moveGunToTargetCommand;
    private FireAtPosition fireAtPositionCommand;
    private bool gunOnTarget = false;
    public override void updateCommand(Unit unit)
    {
        
    }
    public override bool commandFinished(Unit unit)
    {
        //if enemy died?
        return false;
    }
    public override bool executeCommand(Unit unit)
    {
        if (Vector2.Angle(unit.unitPos, unit.currTarget.unitPos) > unit.gunRotateAngle){
            //add or execute command
            //check if command is still ok
            if (rotateToTargetCommand == null || Mathf.Abs(rotateToTargetCommand.targetRotationDeg - Vector2.SignedAngle(unit.unitPos, unit.currTarget.unitPos)) > 20f){
                //CREATE NEW ROTATETOATARGET Command
                this.rotateToTargetCommand = new RotateToTarget(Vector2.SignedAngle(unit.unitPos, unit.currTarget.unitPos));
            }
            if (rotateToTargetCommand.executeCommand(unit)){
                rotateToTargetCommand = null;
            }
        }
        //NO GUN ANGLE FOR NOW!
        if (! (unit.gunAngle > Mathf.Atan((unit.unitPos.y-unit.currTarget.unitPos.y)/(unit.unitPos.x-(unit.currTarget.unitSize/2))) && unit.gunAngle < Mathf.Atan((unit.unitPos.y-unit.currTarget.unitPos.y)/(unit.unitPos.x+(unit.currTarget.unitSize/2))))){
            //MOVE GUN?
            if (moveGunToTargetCommand == null){
                moveGunToTargetCommand = new MoveGunToTarget();
            }
        }
        else{
            //FiRE !
        }
        if (gunOnTarget){

        }
        return commandFinished(unit);
    }
}
public class TurnTowardsEnemy : Command{
   Unit currEnemyUnit;
    public TurnTowardsEnemy(){

    }
    public override void updateCommand(Unit unit)
    {
        
    }
    public override bool commandFinished(Unit unit)
    {
        return false;
    }
    public override bool executeCommand(Unit unit)
    {
        Debug.Log("----");
        //Debug.Log(Vector2.SignedAngle(new Vector2(0, 1), new Vector2(0, 1)));
        //float targetRotationDeg = -90 + -1 * Vector2.SignedAngle(unit.unitPos, unit.currTarget.unitPos);
        //float targetRotationdeg = Vector3.SignedAngle(unit.unitPos, unit.currTarget.unitPos);
        float targetRotationDeg = Mathf.Rad2Deg * Mathf.Acos((unit.unitPos.x-unit.currTarget.unitPos.x)/Vector2.Distance(unit.unitPos, unit.currTarget.unitPos));
        if (unit.unitPos.y < unit.currTarget.unitPos.y){
            targetRotationDeg *= -1;
        }
        //Vector2.SignedAngle(unit.unitPos, unit.currTarget.unitPos);
        unit.currRotationDeg = targetRotationDeg;
        unit.gameObject.transform.eulerAngles = new Vector3(0, 0, unit.currRotationDeg);
        Debug.Log(unit.unitPos);
        Debug.Log(unit.currTarget.unitPos);
        Debug.Log(unit.currRotationDeg);
        return true;
        /*
        float targetRotationDeg = Vector2.SignedAngle(unit.unitPos, unit.currTarget.unitPos);
        if (unit.currRotationDeg != targetRotationDeg){
            float rotationAmount = unit.rotationSpeed * Time.deltaTime;
            //Debug.Log(unit.rotationSpeed);
            if (unit.currRotationDeg < targetRotationDeg){
                if ((unit.currRotationDeg + rotationAmount) < targetRotationDeg){
                    Debug.Log("movingR");
                    Debug.Log(targetRotationDeg);
                    Debug.Log(rotationAmount);
                    Debug.Log(unit.currRotationDeg);
                    Debug.Log(unit.currRotationDeg + rotationAmount);
                    unit.currRotationDeg += rotationAmount;
                    unit.gameObject.transform.eulerAngles = new Vector3(0, 0, unit.currRotationDeg);
                    Debug.Log("movingR");
                    Debug.Log(targetRotationDeg);
                    Debug.Log(rotationAmount);
                    Debug.Log(unit.currRotationDeg);
                    Debug.Log(unit.currRotationDeg + rotationAmount);
                }
                else{
                    unit.currRotationDeg = targetRotationDeg;
                    unit.gameObject.transform.eulerAngles = new Vector3(0, 0, unit.currRotationDeg);
                    Debug.Log("we're there!");
                    Debug.Log(targetRotationDeg);
                    Debug.Log(rotationAmount);
                    Debug.Log(unit.currRotationDeg);
                    Debug.Log(unit.currRotationDeg + rotationAmount);
                    return true;
                }
            }
            else if (unit.currRotationDeg > targetRotationDeg){
                if ((unit.currRotationDeg - rotationAmount) > targetRotationDeg){
                    unit.currRotationDeg -= rotationAmount;
                    Debug.Log("movingL");
                    Debug.Log(targetRotationDeg);
                    Debug.Log(rotationAmount);
                    Debug.Log(unit.currRotationDeg);
                    Debug.Log(unit.currRotationDeg + rotationAmount);
                    unit.gameObject.transform.eulerAngles = new Vector3(0, 0, unit.currRotationDeg);
                }
                else{
                    unit.currRotationDeg = targetRotationDeg;
                    unit.gameObject.transform.eulerAngles = new Vector3(0, 0, unit.currRotationDeg);
                    Debug.Log("we're there");
                    Debug.Log(targetRotationDeg);
                    Debug.Log(rotationAmount);
                    Debug.Log(unit.currRotationDeg);
                    Debug.Log(unit.currRotationDeg + rotationAmount);
                    return true;
                }
            }
        }
        else{
            return true;
        }
        return false;
        */
    }
}
public class FireAtPositionNoGun : Command{
    Vector2 enemyPos;
    public FireAtPositionNoGun(Vector2 enemyPos){
        this.enemyPos = enemyPos;
    }
    public override void updateCommand(Unit unit)
    {
        
    }
    public override bool commandFinished(Unit unit)
    {
        return false;
    }
    public override bool executeCommand(Unit unit)
    {
        if (unit.fireRateTimer == 0){
            GameObject currGunFire = GameObject.Instantiate(unit.gunfirePrefab);
            LineRenderer lineRenderer = currGunFire.GetComponent<LineRenderer>();
            Vector3[] linePosition = new Vector3[2];

            //linePosition[0] = unit.unitPos;
            //linePosition[1] = enemyPos;
            //Debug.Log(unit.unitPos.y+(Mathf.Acos(Mathf.Deg2Rad * unit.currRotationDeg) * 10));
            linePosition[0] = unit.unitPos;
            /*float linePosx = unit.unitPos.x + 10;
            float linePosy = unit.unitPos.y + Mathf.Tan(Mathf.Deg2Rad * unit.currRotationDeg)*10;
            Debug.Log(Mathf.Tan(Mathf.Deg2Rad * unit.currRotationDeg));
            if (unit.unitPos.x > unit.currTarget.unitPos.x){
                linePosx*=-1;
                //linePosy-=180;
            }*/

            //linePosition[1] = new Vector2(linePosx, linePosy);
            //linePosition[1] = unit.unitPos + new Vector2(10, (Mathf.Tan(Mathf.Deg2Rad * unit.currRotationDeg) * 10));

            linePosition[1] = unit.currTarget.unitPos;

            lineRenderer.SetPositions(linePosition);
            GameObject.Destroy(currGunFire, 0.5f);
            Debug.Log("finished fireing");
            unit.fireRateTimer = unit.fireRate;
            return true;
        }
        return true;
    }
}
public class EngageEnemyNoGun : Command{

    private RotateToTarget rotateToTargetCommand;
    private MoveGunToTarget moveGunToTargetCommand;
    private FireAtPositionNoGun fireAtPositionCommand;
    private TurnTowardsEnemy turnTowardsEnemy = new TurnTowardsEnemy();
    private bool gunOnTarget = false;
    public Unit enemyUnit;
    /*public EngageEnemyNoGun(Unit enemyUnit){
        this.enemyUnit = enemyUnit;
    }*/
    public EngageEnemyNoGun(){
        this.rotateToTargetCommand = new RotateToTarget();
    }
    public override void updateCommand(Unit unit)
    {
        //turnTowardsEnemy = new TurnTowardsEnemy();
    }
    public override bool commandFinished(Unit unit)
    {
        //if enemy died?
        return false;
    }
    public override bool executeCommand(Unit unit)
    {
        //Debug.Log("execing");
        Vector2 rightMost = new Vector2(unit.currTarget.unitPos.x + unit.currTarget.unitSize/2, unit.currTarget.unitPos.y);
        Vector2 leftMost = new Vector2(unit.currTarget.unitPos.x - unit.currTarget.unitSize/2, unit.currTarget.unitPos.y);
        Vector2 upMost = new Vector2(unit.currTarget.unitPos.x, unit.currTarget.unitPos.y + unit.currTarget.unitSize/2);
        Vector2 downMost = new Vector2(unit.currTarget.unitPos.x, unit.currTarget.unitPos.y - unit.currTarget.unitSize/2);
        //duMB SHITE I TRIED
        //!(unit.currRotationDeg > Vector2.SignedAngle(unit.unitPos, rightMost) && unit.currRotationDeg < Vector2.SignedAngle(unit.unitPos, leftMost))
        //float angleToTarget = Vector2.SignedAngle(unit.unitPos, unit.currTarget.unitPos) + 90;
        Debug.Log("angle to target");
        float angleToTarget = Mathf.Rad2Deg * Mathf.Asin((unit.currTarget.unitPos.y - unit.unitPos.y)/Vector2.Distance(unit.currTarget.unitPos, unit.unitPos));
        Debug.Log(angleToTarget);
        angleToTarget -= 90;
        if ((unit.currTarget.unitPos.x - unit.unitPos.x) < 0){
            angleToTarget *= -1;
        }
        Debug.Log(angleToTarget);
        
        float changeDeg = Mathf.Rad2Deg * Mathf.Atan2(unit.currTarget.unitSize/2, Vector2.Distance(unit.unitPos, unit.currTarget.unitPos));
        //float angleToTarget = Vector2.SignedAngle(unit.unitPos, unit.currTarget.unitPos);
        rotateToTargetCommand.updateRotateDeg(angleToTarget);
        rotateToTargetCommand.executeCommand(unit);
        //if (unit.currRotationDeg > ){

        //}
        if (unit.currRotationDeg > angleToTarget + changeDeg || unit.currRotationDeg < angleToTarget - changeDeg){
            /*if (rotateToTargetCommand == null){
                //CREATE NEW ROTATETOATARGET Command
                Debug.Log("creating new rotate to target");
                this.rotateToTargetCommand = new RotateToTarget(angleToTarget);
            }
            if (rotateToTargetCommand.executeCommand(unit)){
                Debug.Log("rotateToTarget finished");
                rotateToTargetCommand = null;
            }*/
            //Debug.Log("'")
            /*else{
                Debug.Log("rotating");
                Debug.Log(rotateToTargetCommand.targetRotationDeg);
                rotateToTargetCommand.executeCommand(unit);
            }*/
            //rotateToTargetCommand.executeCommand(unit);
        }
        else{
            fireAtPositionCommand = new FireAtPositionNoGun(unit.currTarget.unitPos);
            fireAtPositionCommand.executeCommand(unit);
            fireAtPositionCommand = null;
        }

        /*if (turnTowardsEnemy.executeCommand(unit)){
            fireAtPositionCommand = new FireAtPositionNoGun(unit.currTarget.unitPos);
            fireAtPositionCommand.executeCommand(unit);
            fireAtPositionCommand = null;
        }*/
        return commandFinished(unit);
    }
}
public class MoveCommand : Command{
    public Queue<MoveNode> moveCoors;
    private float currentNodeProgress = 0;
    MoveNode currMoveCoor;
    private ArrayList rotateFixedArray = new ArrayList();
    public MoveCommandIcon moveCommandIcon;
    public MoveCommand(Queue<MoveNode> moveCoordinates){
        this.moveCoors = moveCoordinates;
        this.currMoveCoor = moveCoors.Dequeue();
    }
    public override void updateCommand(Unit unit)
    {
        
    }
    public override bool commandFinished(Unit unit)
    {
        return false;
    }
    public override bool executeCommand(Unit unit)
    {
        float currMoveAmount = unit.speed * Time.deltaTime;
        //Debug.Log(unit.speed);
        while (currMoveAmount > 0){
            //float distanceToMove = Vector2.Distance(currMoveCoor.startNode, unit.unitPos);
            float distanceToMove = Vector2.Distance(currMoveCoor.endNode, unit.unitPos);
            if (currMoveAmount > distanceToMove){
                //unit.unitPos = currMoveCoor.startNode;
                unit.unitPos = currMoveCoor.endNode;
                unit.transform.position = unit.unitPos;
                currMoveAmount -= distanceToMove;
                currMoveCoor.updateMoveProgress(distanceToMove, unit);
                if (moveCoors.Count == 0){
                    return true;
                }
                currMoveCoor = moveCoors.Dequeue();
            }
            else{
                //Vector2 newUnitPos = new Vector2(unit.unitPos.x + currMoveAmount/distanceToMove*(currMoveCoor.startNode.x - unit.unitPos.x), unit.unitPos.y + currMoveAmount/distanceToMove*(currMoveCoor.startNode.y - unit.unitPos.y));
                Vector2 newUnitPos = new Vector2(unit.unitPos.x + currMoveAmount/distanceToMove*(currMoveCoor.endNode.x - unit.unitPos.x), unit.unitPos.y + currMoveAmount/distanceToMove*(currMoveCoor.endNode.y - unit.unitPos.y));
                currMoveCoor.updateMoveProgress(currMoveAmount, unit);
                currMoveAmount = 0;
                unit.unitPos = newUnitPos;
                //unit.unitPos.updateMoveProgress()
                unit.transform.position = unit.unitPos;
            }
        }
        return false;
    }
    public void addRotateFixed(RotateFixed rotateFixed){
        //sort by startNode and percentageToNextNode
        //int currIndex = (int)rotateFixedArray.Count/2;
        int leftMost = 0;
        int rightMost = rotateFixedArray.Count - 1;
        int currIndex = (int)(rightMost + leftMost)/2;
        while (leftMost != rightMost){
            currIndex = (int)(rightMost + leftMost)/2;
            RotateFixed currRotateFixed = (RotateFixed)rotateFixedArray[currIndex];
            if (rotateFixed.startNode > currRotateFixed.startNode){
                leftMost = currIndex + 1;
            }
            else if (rotateFixed.startNode == currRotateFixed.startNode){
                break;
            }
            else{
                rightMost = currIndex - 1;
            }
        }
        RotateFixed finalRotateFixed = (RotateFixed)rotateFixedArray[currIndex];
        if (finalRotateFixed.startNode == rotateFixed.startNode){
            RotateFixed currRotateFixed = (RotateFixed)rotateFixedArray[currIndex];
            if (rotateFixed.percentageToNextNode > finalRotateFixed.percentageToNextNode){
                while (rotateFixed.startNode == currRotateFixed.startNode && rotateFixed.percentageToNextNode > currRotateFixed.percentageToNextNode){
                    currIndex++;
                    if (currIndex < 0){
                        break;
                    }
                    currRotateFixed = (RotateFixed)rotateFixedArray[currIndex];
                }
                rotateFixedArray.Insert(currIndex, rotateFixed);
            }
            else if (rotateFixed.percentageToNextNode == finalRotateFixed.percentageToNextNode){
                rotateFixedArray.RemoveAt(currIndex);
                rotateFixedArray.Insert(currIndex, rotateFixed);
            }
            else{
                while (rotateFixed.startNode == currRotateFixed.startNode && rotateFixed.percentageToNextNode < currRotateFixed.percentageToNextNode){
                    currIndex--;
                    if (currIndex < rotateFixedArray.Count){
                        break;
                    }
                    currRotateFixed = (RotateFixed)rotateFixedArray[currIndex];
                }
                rotateFixedArray.Insert(currIndex, rotateFixed);
            }
        }
    }
    public RotateFixed getMousePressLocation(Vector2 mousePress){
        foreach(MoveNode moveNode in moveCoors.ToArray()){
            RotateFixed currRF = moveNode.getMouseDown(mousePress);
            if (currRF != null){
                return currRF;
            }
        }
        return null;
    }
    
}
public class MoveNode{
    public Vector2 startNode;
    public Vector2 endNode;
    public GameObject lineGameObject;
    public ArrayList rotateCommands;
    
    //MAYBE
    public Vector2 normalizedVector;
    public float currProgress = 0;
    public float length;
    public float angle;
    public MoveNode(Vector2 startNode, Vector2 endNode, GameObject lineGameObject){
        this.startNode = startNode;
        this.endNode = endNode;
        //this.lineGameObject = GameObject.Instantiate(lineGameObject);
        //this.lineGameObject.transform.position = new Vector3();
        rotateCommands = new ArrayList();
        this.normalizedVector = (endNode - startNode).normalized;
        this.length = Vector2.Distance(startNode, endNode);

        this.lineGameObject = GameObject.Instantiate(lineGameObject);
        this.lineGameObject.transform.localScale = new Vector3(this.length, 0.1f, 0);
        float angleToTarget = Mathf.Rad2Deg * Mathf.Asin((endNode.y - startNode.y)/Vector2.Distance(endNode, startNode));
        if ((endNode.x - startNode.x) < 0){
            angleToTarget *= -1;
        }
        this.lineGameObject.transform.position = new Vector3(startNode.x + length/2*normalizedVector.x, startNode.y + length/2*normalizedVector.y, 0);
        this.lineGameObject.transform.eulerAngles = new Vector3(0, 0, angleToTarget);
    }
    public void addRotateFixed(RotateFixed rotateFixed){
        Debug.Log("adding rotateFixed");
        if (rotateCommands.Count == 0){
            rotateCommands.Insert(0, rotateFixed);
        }
        else{
            int leftMost = 0;
            int rightMost = rotateCommands.Count - 1;
            int currIndex = (int)(rightMost + leftMost)/2;
            while (leftMost != rightMost){
                currIndex = (int)(rightMost + leftMost)/2;
                RotateFixed currRotateFixed = (RotateFixed)rotateCommands[currIndex];
                if (rotateFixed.percentageToNextNode > currRotateFixed.percentageToNextNode){
                    leftMost = currIndex + 1;
                }
                else if (rotateFixed.percentageToNextNode == currRotateFixed.percentageToNextNode){
                    break;
                }
                else{
                    rightMost = currIndex - 1;
                }
            }
            rotateCommands.Insert(leftMost, rotateFixed);
        }
    }
    public void updateMoveProgress(float distanceMoved, Unit unit){
        //UPDATE lineGameObject
        //Try getting new rotateCommand
        this.currProgress += distanceMoved;
        float percentageProgress = currProgress / this.length;
        //get rotateCommands until less than percentageProgress
        //I MAY HAVE ACCIDENTLY STUMBLED UPON SOMETHING GREAT
        //this.lineGameObject.transform.position = new Vector3(startNode.x+((percentageProgress)*length)*normalizedVector.x +(1-percentageProgress)/2*normalizedVector.x, startNode.y+ ((percentageProgress)*length).y/2*normalizedVector.y, 0);
        this.lineGameObject.transform.position = new Vector3(startNode.x+((percentageProgress)*length)*normalizedVector.x +(1-percentageProgress)*length/2*normalizedVector.x, startNode.y+ ((percentageProgress)*length)*normalizedVector.y + ((1-percentageProgress)*length)*normalizedVector.y/2, 0);
        this.lineGameObject.transform.localScale = new Vector3((1-percentageProgress)*length, 0.1f, 0);
        foreach(RotateFixed rotateFixed in rotateCommands.ToArray()){
            if (rotateFixed.percentageToNextNode <= percentageProgress){
                Debug.Log("NEW ROTATE COMMAND!");
                unit.currRotateCommand = rotateFixed.rotateCommand;
                rotateCommands.Remove(rotateFixed);
            }
            else{
                break;
            }
        }
    }
    public RotateFixed getMouseDown(Vector2 mouseDownLocation){
        BoxCollider2D currBoxCollider = lineGameObject.GetComponent<BoxCollider2D>();
        if (currBoxCollider.OverlapPoint(mouseDownLocation)){
            float slope = (endNode.y-startNode.y)/(endNode.x-startNode.x);
            float newX = (mouseDownLocation.y - mouseDownLocation.x * -1f*(1f/slope) -startNode.y + startNode.x*slope)/(slope + 1f/slope);
            //(startNode.y - mouseDownLocation.y + startNode.x * ((endNode.y-startNode.y)/(endNode.x-startNode.x)))*(-1f*(endNode.y-startNode.y)/(endNode.x-startNode.x));
            float newY = (newX-startNode.x) * slope + startNode.y;
            //DO I NEED percentageToNextNode?
            
            Vector2 pointOnLine = new Vector2(newX, newY);
            /*Debug.Log("---");
            Debug.Log(mouseDownLocation);
            Debug.Log(slope);
            Debug.Log("New x: " + newX);
            Debug.Log("New Y" + newY);*/
            return new RotateFixed(this, pointOnLine, Vector2.Distance(startNode, pointOnLine));
        }
        return null;
    }

}
public class RotateFixed{
    public int startNode;
    public MoveNode moveNode;
    public Vector2 pointIntercepted;
    public float percentageToNextNode;
    public float distanceToStart;
    public Command rotateCommand;
    public RotateFixed(MoveNode moveNode, Vector2 pointIntercepted, float percentageToNextNode){
        this.moveNode = moveNode;
        this.pointIntercepted = pointIntercepted;
        this.percentageToNextNode = percentageToNextNode;
    }
    public RotateFixed(int startNode, float percentageToNextNode, Command rotateCommand){
        this.startNode = startNode;
        this.percentageToNextNode = percentageToNextNode;
        this.rotateCommand = rotateCommand;
    }
}
public class UIElements{
    
}