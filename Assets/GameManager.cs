using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    public Unit[] enemyUnits = new Unit[1];
    public Unit[] playerUnits = new Unit[1];
    public GameObject playerUnitPrefab;
    public GameObject enemyUnitPrefab;
    public bool puased = false;
    void Awake()
    {
        initEnemyUnits();
        initPlayerUnits();
    }
    void initEnemyUnits(){
        GameObject enemyUnitGameObject = GameObject.Instantiate(enemyUnitPrefab);
        Unit enemyUnit = enemyUnitGameObject.GetComponent<Unit>();
        enemyUnits[0] = enemyUnit;
        enemyUnit.init(1, new Vector2(0, 3), this);
    }
    void initPlayerUnits(){
        GameObject playerUnitObject = GameObject.Instantiate(playerUnitPrefab);
        Unit playerUnit = playerUnitObject.GetComponent<Unit>();
        playerUnits[0] = playerUnit;
        playerUnit.init(0, new Vector2(0, 0), this);
    }
    void initEnemyUnit(Vector2 pos){
        //enemyUnits.
    }
    void initPlayerUnit(Vector2 pos){

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public static float getAngle(Vector2 point1, Vector2 point2){
        return 0;
    }
}
