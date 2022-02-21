using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public GameManager gameManager;
    public Unit currUnit = null;
    void Update(){
        if (Input.GetKeyDown(KeyCode.Mouse0)){
            Vector3 mousePoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            /*if (currUnit == null){
                foreach(Unit playerUnit in this.gameManager.playerUnits){
                    if (playerUnit.boxCollider.OverlapPoint(new Vector2(mousePoint.x, mousePoint.y))){
                        currUnit = playerUnit;
                        break;
                    }
                }
            }*/
            Debug.Log("m0 down");
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
            //currUnit.commandQueue.Enqueue(new MoveToPosCommand(Camera.main.ScreenToWorldPoint(Input.mousePosition)));
        }
        if (Input.GetKeyDown(KeyCode.Mouse1)){
            /*Debug.Log("m1 down");
            Vector3 mousePoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (!Input.GetKey(KeyCode.LeftShift)){
                currUnit.currCommand = null;
                currUnit.commandQueue.Clear();
            }
            if (currUnit != null){
                currUnit.commandQueue.Enqueue(new MoveToPosCommand(mousePoint));
            }
            else{
                Debug.Log("currUnit is null");
            }*/

        }
        if (Input.GetKeyUp(KeyCode.Mouse1)){
            //FINISH LOGGING MOVE
        }
        if (Input.GetKeyDown(KeyCode.A)){
            Debug.Log("AAA");
            currUnit.currTarget = gameManager.enemyUnits[0];
            currUnit.currEngaging = true;
            currUnit.concurrentCommands.Add(new EngageEnemyNoGun());
        }
    }
}