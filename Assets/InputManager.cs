using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public GameManager gameManager;
    public Unit currUnit = null;
    public GameObject pathSquare;
    public GameObject lineGameObject;
    public InputCommand currInputCommand;
    public Vector2 mousePoint;
    void Awake(){
        //currPath = gameObject.GetComponent<LineRenderer>();
    }
    void Update(){
        //NOT TOO SURE HOW TO GET ALL KEYCODES DOWN AS AN ARR
        mousePoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (Input.GetKeyDown(KeyCode.Mouse0)){
            getCommand(KeyCode.Mouse0, true);
        }
        if (Input.GetKeyUp(KeyCode.Mouse0)){
            getCommand(KeyCode.Mouse0, false);
        }
        if (Input.GetKeyDown(KeyCode.Mouse1)){
            getCommand(KeyCode.Mouse1, true);
        }
        if (Input.GetKeyUp(KeyCode.Mouse1)){
            getCommand(KeyCode.Mouse1, false);
        }
        if (Input.GetKeyDown(KeyCode.A)){
            getCommand(KeyCode.A, true);
        }
        if (Input.GetKeyDown(KeyCode.F)){
            getCommand(KeyCode.F, true);
        }
        if (Input.GetKeyDown(KeyCode.E)){
            getCommand(KeyCode.E, true);
        }
        if (Input.GetKeyUp(KeyCode.E)){
            getCommand(KeyCode.E, false);
        }
        if (currInputCommand != null){
            if (currInputCommand.updateCommand()){
                currInputCommand.commandEnd(currUnit);
                currInputCommand = null;
            }
        }
    }
    void getCommand(KeyCode keyCode, bool keyDown){
        //NO CURRENT INPUT
        if (currInputCommand == null && keyDown == true){
            switch(keyCode){
                case KeyCode.Mouse0:
                    foreach(Unit playerUnit in this.gameManager.playerUnits){
                        if (playerUnit == null){
                            continue;
                        }
                        else{
                            Debug.Log("not null");
                        }
                        if (playerUnit.boxCollider.OverlapPoint(new Vector2(mousePoint.x, mousePoint.y))){
                            Debug.Log("found currUnit");
                            currUnit = playerUnit;
                            break;
                        }
                    }
                    break;
                case KeyCode.Mouse1:
                    currInputCommand = new TrackUnitMove(mousePoint, pathSquare, lineGameObject);
                    break;
                case KeyCode.A:
                    Debug.Log("AAA");
                    currUnit.currTarget = gameManager.enemyUnits[0];
                    currUnit.currEngaging = true;
                    currUnit.concurrentCommands.Add(new EngageEnemyNoGun());
                    break;
                case KeyCode.F:
                    Debug.Log("looking at new Pos?");
                    currInputCommand = new LookTowardsPos(this);
                    break;
                case KeyCode.E:
                    if (currUnit != null){

                    }
                    break;
            }
        }
        else if (currInputCommand != null){
            if (currInputCommand.handleInput(keyCode, keyDown)){
                currInputCommand.commandEnd(currUnit);
                currInputCommand = null;
            }
        }
    }
}

public abstract class InputCommand{
    public abstract bool handleInput(KeyCode keyCode, bool getKeyDown);
    public abstract bool updateCommand();
    public abstract void commandEnd(Unit currUnit);
}
public class LookTowardsPos : InputCommand{
    InputManager inputManager;
    float rotateDeg;
    public LookTowardsPos(InputManager inputManager){
        this.inputManager = inputManager;
    }
    public override bool handleInput(KeyCode keyCode, bool getKeyDown)
    {
        if (keyCode == KeyCode.Mouse0 && getKeyDown == false){
            if ((inputManager.currUnit.unitPos.y - inputManager.mousePoint.y) == 0){
                this.rotateDeg = 0;
            }
            else{
                this.rotateDeg = Mathf.Rad2Deg * Mathf.Asin((inputManager.mousePoint.y - inputManager.currUnit.unitPos.y)/Vector2.Distance(inputManager.mousePoint, inputManager.currUnit.unitPos));
            }
            //BECAUSE UNITY ROTATION IS DUMB
            this.rotateDeg -= 90;
            if ((inputManager.mousePoint.x - inputManager.currUnit.unitPos.x) < 0){
                this.rotateDeg *= -1;
            }
            Debug.Log((inputManager.currUnit.unitPos.y - inputManager.mousePoint.y));
            Debug.Log(Vector2.Distance(inputManager.mousePoint, inputManager.currUnit.unitPos));
            Debug.Log(this.rotateDeg);
            return true;
        }
        return false;
    }
    public override bool updateCommand()
    {
        return false;
    }
    public override void commandEnd(Unit currUnit)
    {
        Debug.Log("target rotation:");
        Debug.Log(this.rotateDeg);
        currUnit.concurrentCommands.Add(new RotateToTarget(this.rotateDeg));
    }
}
public class TrackUnitMove : InputCommand{
    static float moveDistances = 0.5f;
    public Queue<Vector2> moveCoors = new Queue<Vector2>();
    Vector2 pastMoveCoor;
    public GameObject lineGameObject;
    public MoveCommandIcon moveCommandIcon;
    //LineRenderer currPath;
    public ArrayList pathSquares = new ArrayList();
    //public ArrayList rotateFixedArray = new ArrayList();
    GameObject pathSquare;
    public TrackUnitMove(Vector2 mousePoint, GameObject pathSquare, GameObject lineGameObject){
        pastMoveCoor = mousePoint;
        this.pathSquare = pathSquare;
        this.lineGameObject = lineGameObject;
        this.moveCommandIcon = new MoveCommandIcon(this.lineGameObject);
    }
    public override void commandEnd(Unit currUnit)
    {
        currUnit.commandQueue.Enqueue(new MoveCommand(this.moveCoors));
        foreach(GameObject currPathSquare in pathSquares){
            GameObject.Destroy(currPathSquare);
        }
        this.pathSquares.Clear();
    }
    public override bool handleInput(KeyCode keyCode, bool getKeyDown)
    {
        if (keyCode == KeyCode.Mouse1 && getKeyDown == false){
            return true;
        }
        return false;
    }
    public override bool updateCommand()
    {
        Vector2 mousePoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        float distanceToPoint = Vector2.Distance(pastMoveCoor, mousePoint);
        int points = (int) Mathf.Floor(distanceToPoint/moveDistances);
        float xDis = moveDistances/distanceToPoint*(mousePoint.x - pastMoveCoor.x);
        float yDis = moveDistances/distanceToPoint*(mousePoint.y - pastMoveCoor.y);
        Vector2 lastPoint = pastMoveCoor;
        for (int i = 1; i <= points; i++){
            Vector2 currPoint = new Vector2(pastMoveCoor.x + xDis * i, pastMoveCoor.y + yDis * i);
            moveCommandIcon.addNode(lastPoint, currPoint);
            moveCoors.Enqueue(currPoint);
            GameObject newPath = GameObject.Instantiate(pathSquare);
            newPath.transform.position = currPoint;
            pathSquares.Add(newPath);
            if (i == points){
                pastMoveCoor = currPoint;
            }
            lastPoint = currPoint;
        }
        return false;
    }
}
public class TrackMoveAngleFocus : InputCommand{
    private int node;
    private float percentageToNextNode;
    private float rotateDeg;
    private Vector2 startingPoint;
    private InputManager inputManager;
    public TrackMoveAngleFocus(InputManager inputManager, Vector2 pointIntercepted, int node){
        this.inputManager = inputManager;
        this.node = node;


    }
    public override void commandEnd(Unit currUnit)
    {
        if (currUnit != null){
            //currUnit.currMoveCommand.addRotateFixed();
        }
    }
    public override bool handleInput(KeyCode keyCode, bool getKeyDown)
    {
        if (keyCode == KeyCode.Mouse0 && getKeyDown == false){
            if ((startingPoint.y - inputManager.mousePoint.y) == 0){
                this.rotateDeg = 0;
            }
            else{
                this.rotateDeg = Mathf.Rad2Deg * Mathf.Asin((inputManager.mousePoint.y - startingPoint.y)/Vector2.Distance(inputManager.mousePoint, startingPoint));
            }
            this.rotateDeg -= 90;
            if ((inputManager.mousePoint.x - inputManager.currUnit.unitPos.x) < 0){
                this.rotateDeg *= -1;
            }
            RotateFixed currRotateFixed = new RotateFixed(this.node, this.percentageToNextNode, new RotateToTarget(this.rotateDeg));
            
            return true;
        }
        return false;
    }
    public override bool updateCommand()
    {
        return false;
    }
}
public class MoveCommandIcon{
    ArrayList nodeRenderers = new ArrayList();
    GameObject lineGameObject;
    public int nodeCount = 0;
    public MoveCommandIcon(GameObject lineGameObject){
        this.lineGameObject = lineGameObject;
    }
    public void updateProgress(){

    }
    public void addNode(Vector2 lastNode, Vector2 newNode){
        GameObject currLineGameObject = GameObject.Instantiate(lineGameObject);
        currLineGameObject.transform.localScale += new Vector3(Vector2.Distance(lastNode, newNode), 0, 0);
        float angleToNextNode  = Mathf.Rad2Deg * Mathf.Asin((newNode.y - lastNode.y)/Vector2.Distance(newNode, lastNode));
        angleToNextNode -= 90;
        if ((newNode.x - lastNode.x) < 0){
            angleToNextNode *= -1;
        }
        currLineGameObject.transform.eulerAngles = new Vector3(0, 0, angleToNextNode);
        nodeRenderers.Add(currLineGameObject);
        nodeCount++;
    }
    public bool getMouseDownLocation(Vector2 mouseDownLocation){
        for (int i = 0; i < nodeCount-1; i++){
            GameObject currLine = (GameObject) nodeRenderers[i];
            BoxCollider2D currBoxCollider = currLine.GetComponent<BoxCollider2D>();
            if (currBoxCollider.OverlapPoint(mouseDownLocation)){
                int currNode = i;
                int nextNode = i+1;
                float percentageToNextNode;
                return true;
            }
        }
        return false;
    }
}