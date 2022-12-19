using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GM : MonoBehaviour
{
    public Unit selectedUnit;

    public int playerTurn = 1;
    public int villageNumber = 3;
   

    public Transform selectedUnitSquare;
    public enemyController enemyController;
    public CharacterCreation characterCreator;

    public GameObject createknight;
    public GameObject createArcher;
    public GameObject createDragon;
    public GameObject createVillage;
    private GameObject unitParent;
    Node buyCurrentNode;


    private Animator camAnim;
    public Image playerIcon; 
    public Sprite playerOneIcon;
    public Sprite playerTwoIcon;

    public GameObject unitInfoPanel;
    public Vector2 unitInfoPanelShift;
    Unit currentInfoUnit;
    public Text heathInfo;
    public Text attackDamageInfo;
    public Text armorInfo;
    public Text defenseDamageInfo;

    public int player1Gold;
    public int player2Gold;

    public Text player1GoldText;
    public Text player2GoldText;

    public Unit createdUnit;
    public Village createdVillage;

    public GameObject blueVictory;
    public GameObject darkVictory;


    public RandomCreation creacion;

	private AudioSource source;



    private void Start()
    {
		source = GetComponent<AudioSource>();
        camAnim = Camera.main.GetComponent<Animator>();
        characterCreator = GetComponent<CharacterCreation>();
        enemyController = FindObjectOfType<enemyController>();
        GetGoldIncome(1);

    }

    private void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.Space) && playerTurn ==1) {
            EndTurn();
        }

        if (selectedUnit != null) // moves the white square to the selected unit!
        {
            selectedUnitSquare.gameObject.SetActive(true);
            selectedUnitSquare.position = selectedUnit.transform.position;
        }
        else
        {
            selectedUnitSquare.gameObject.SetActive(false);
        }
       
    }

    

    // Sets panel active/inactive and moves it to the correct place
    public void UpdateInfoPanel(Unit unit) {

        if (unit.Equals(currentInfoUnit) == false)
        {
            unitInfoPanel.transform.position = (Vector2)unit.transform.position + unitInfoPanelShift;
            unitInfoPanel.SetActive(true);

            currentInfoUnit = unit;

            UpdateInfoStats();

        } else {
            unitInfoPanel.SetActive(false);
            currentInfoUnit = null;
        }

    }

    // Updates the stats of the infoPanel
    public void UpdateInfoStats() {
        if (currentInfoUnit != null)
        {
            attackDamageInfo.text = currentInfoUnit.attackDamage.ToString();
            defenseDamageInfo.text = currentInfoUnit.defenseDamage.ToString();
            armorInfo.text = currentInfoUnit.armor.ToString();
            heathInfo.text = currentInfoUnit.health.ToString();
        }
    }

    // Moves the udpate panel (if the panel is actived on a unit and that unit moves)
    public void MoveInfoPanel(Unit unit) {
        if (unit.Equals(currentInfoUnit))
        {
            unitInfoPanel.transform.position = (Vector2)unit.transform.position + unitInfoPanelShift;
        }
    }

    // Deactivate info panel (when a unit dies)
    public void RemoveInfoPanel(Unit unit) {
        if (unit.Equals(currentInfoUnit))
        {
            unitInfoPanel.SetActive(false);
			currentInfoUnit = null;
        }
    }

    public void ResetTiles() {
        Tile[] tiles = FindObjectsOfType<Tile>();
        foreach (Tile tile in tiles)
        {
            tile.Reset();
        }
    }

    public void EndTurn() {
		source.Play();
        camAnim.SetTrigger("shake");

        // deselects the selected unit when the turn ends
        if (selectedUnit != null) {
            selectedUnit.ResetWeaponIcon();
            selectedUnit.isSelected = false;
            selectedUnit = null;
        }

        ResetTiles();

        Unit[] units = FindObjectsOfType<Unit>();
        foreach (Unit unit in units) {
            unit.hasAttacked = false;
            unit.hasMoved = false;
            unit.ResetWeaponIcon();
        }

        if (playerTurn == 1) {
            playerIcon.sprite = playerTwoIcon;
            playerTurn = 2;
        } else if (playerTurn == 2) {
            playerIcon.sprite = playerOneIcon;
            playerTurn = 1;
        }

        GetGoldIncome(playerTurn);
        characterCreator.CloseCharacterCreationMenus();
        createdUnit = null;
        if (playerTurn == 2)
        {
            StartCoroutine(enemyController.enemyTurn());
        }
    }

    public List<Unit> checkEnemies(Node node)
    {
        List<Unit> enemiesInRange = new List<Unit>();
        //Debug.Log("se ralla aqui");
        Unit[] enemies = FindObjectsOfType<Unit>();
        foreach (Unit enemy in enemies)
        {
            if (Mathf.Abs(node.worldPosition.x - enemy.transform.position.x) + Mathf.Abs(node.worldPosition.y - enemy.transform.position.y) <= selectedUnit.attackRadius) 
            {
                if (enemy.playerNumber != playerTurn && !selectedUnit.hasAttacked)
                {
                    enemiesInRange.Add(enemy);
                }
            }
        }
        return enemiesInRange;
    }

    public List<Unit> checkAllies(Node node)
    {
        List<Unit> enemiesInRange = new List<Unit>();
        //Debug.Log("se ralla aqui");
        Unit[] enemies = FindObjectsOfType<Unit>();
        foreach (Unit enemy in enemies)
        {
            if (Mathf.Abs(node.worldPosition.x - enemy.transform.position.x) + Mathf.Abs(node.worldPosition.y - enemy.transform.position.y) <= selectedUnit.attackRadius)
            {
                if (enemy.playerNumber == playerTurn && !selectedUnit.hasAttacked)
                {
                    enemiesInRange.Add(enemy);
                }
            }
        }
        return enemiesInRange;
    }

    void GetGoldIncome(int playerTurn) {
        foreach (Village village in FindObjectsOfType<Village>())
        {
            if (village.playerNumber == playerTurn)
            {
                if (playerTurn == 1)
                {
                    player1Gold += village.goldPerTurn;
                }
                else
                {
                    player2Gold += village.goldPerTurn;
                }
            }
        }
        UpdateGoldText();
    }

    public void UpdateGoldText()
    {
        player1GoldText.text = player1Gold.ToString();
        player2GoldText.text = player2Gold.ToString();
    }

    public void BuyUnits()
    {
        if (player2Gold >=100 && villageNumber <= 3)
        {
            CreateVillage();
        }
        if (player2Gold > 119 && villageNumber > 3)
        {
            CreateUnit();
        }
    }

    public void CreateVillage()
    {
        unitParent = GameObject.FindGameObjectWithTag("unitParent").gameObject;

        characterCreator.BuyVillage(createdVillage);
        int rand = Random.Range(0, creacion.spawnpoints.Count);
        Instantiate(createdVillage, creacion.spawnpoints[rand].transform.position, Quaternion.identity, unitParent.transform);
        buyCurrentNode = Pathfinding.grid.NodeFromWorldPoint(creacion.spawnpoints[rand].transform.position);
        buyCurrentNode.walkable = false;
        creacion.spawnpoints.RemoveAt(rand);
        villageNumber++;
    }

    public void CreateUnit()
    {
        unitParent = GameObject.FindGameObjectWithTag("unitParent").gameObject;
        GameObject[] units = new GameObject[3];
        units[0] = createArcher;
        units[1] = createDragon;
        units[2] = createknight;

        int unit = Random.Range(0, 3);



        characterCreator.BuyVillage(createdVillage);
        int rand = Random.Range(0, creacion.spawnpoints.Count);
        if (creacion.spawnpoints.Count != 0 )
        {
            Instantiate(units[unit], creacion.spawnpoints[rand].transform.position, Quaternion.identity, unitParent.transform);
            buyCurrentNode = Pathfinding.grid.NodeFromWorldPoint(creacion.spawnpoints[rand].transform.position);
            buyCurrentNode.walkable = false;
            buyCurrentNode.hasUnit = true;
            creacion.spawnpoints.RemoveAt(rand);
        }
       
    }
    // Victory UI

    public void ShowVictoryPanel(int playerNumber) {

        if (playerNumber == 1)
        {
            blueVictory.SetActive(true);
        } else if (playerNumber == 2) {
            darkVictory.SetActive(true);
        }
    }

    public void RestartGame() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }


}

// El problema esta en que selectedUnit no tiene nunca una unidad seleccionada, por eso da el error, hay que mirarlo 

