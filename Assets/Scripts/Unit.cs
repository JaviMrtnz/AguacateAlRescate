using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Unit : MonoBehaviour
{
    public bool isSelected;
    public bool hasMoved;

    public int tileSpeed;
    public float moveSpeed;

    private GM gm;
    public static Grid grid;

    public int attackRadius;
    public bool hasAttacked;
    public List<Unit> enemiesInRange = new List<Unit>();

    public int playerNumber;

    public GameObject weaponIcon;

    // Attack Stats
    public int health;
    public int attackDamage;
    public int defenseDamage;
    public int armor;

    public DamageIcon damageIcon;

    public int cost;

	public GameObject deathEffect;

	private Animator camAnim;

    public bool isKing;

	private AudioSource source;

    public Text displayedText;

    public Node currentNode;
    List<Vector3> path;

    private void Start()
    {
		source = GetComponent<AudioSource>();
		camAnim = Camera.main.GetComponent<Animator>();
        gm = FindObjectOfType<GM>();
        grid = GetComponent<Grid>();
        UpdateHealthDisplay();
    }

    private void UpdateHealthDisplay ()
    {
        if (isKing)
        {
            //displayedText.text = health.ToString();
        }
    }

    public void OnMouseDown() // select character or deselect if already selected
    {

        if (gm.playerTurn == 1)
        {
            ResetWeaponIcon();
            
            if (isSelected == true)
            {
                isSelected = false;
                gm.selectedUnit = null;
                gm.ResetTiles();

            }
            else
            {
                if (playerNumber == gm.playerTurn)
                { // select unit only if it's his turn
                    if (gm.selectedUnit != null)
                    { // deselect the unit that is currently selected, so there's only one isSelected unit at a time
                        gm.selectedUnit.isSelected = false;
                    }
                    gm.ResetTiles();

                    gm.selectedUnit = this;

                    isSelected = true;
                    if (source != null)
                    {
                        source.Play();
                    }

                    GetWalkableTiles();
                    GetEnemies();
                }

            }

            Collider2D col = Physics2D.OverlapCircle(Camera.main.ScreenToWorldPoint(Input.mousePosition), 0.15f);
            if (col != null)
            {
                Unit unit = col.GetComponent<Unit>(); // double check that what we clicked on is a unit
                if (unit != null && gm.selectedUnit != null)
                {
                    if (gm.selectedUnit.enemiesInRange.Contains(unit) && !gm.selectedUnit.hasAttacked)
                    { // does the currently selected unit have in his list the enemy we just clicked on
                        gm.selectedUnit.Attack(unit);

                    }
                }
            }
        }
    }

    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(1))
        {
            gm.UpdateInfoPanel(this);
        }
    }

    void GetWalkableTiles()
    { // Looks for the tiles the unit can walk on
        if (hasMoved == true)
        {
            return;
        }

        Tile[] tiles = FindObjectsOfType<Tile>();
        foreach (Tile tile in tiles)
        {        
                if (Mathf.Abs(transform.position.x - tile.transform.position.x) + Mathf.Abs(transform.position.y - tile.transform.position.y) <= tileSpeed + 0.5f)
                { // how far he can move
                    if (tile.isClear() == true)
                    { // is the tile clear from any obstacles
                     tile.Highlight();
                  
                    }
                }          
        }
    }

    public List<Node> nGetWalkableTiles()
    { // Looks for the tiles the unit can walk on
        if (hasMoved == true)
        {
            return null;
        }

        Tile[] tiles = FindObjectsOfType<Tile>();
        List<Node> nodes = new List<Node>();
        foreach (Tile tile in tiles)
        {
            if (Mathf.Abs(transform.position.x - tile.transform.position.x) + Mathf.Abs(transform.position.y - tile.transform.position.y) <= tileSpeed + 0.5f)
            { // how far he can move
                if (tile.isClear() == true)
                { // is the tile clear from any obstacles
                    tile.Highlight();
                    nodes.Add(tile.node);
                }
            }
        }
        return nodes;
    }
    //Esta funcion hay que mirarla 
    

    void GetEnemies() {
    
        enemiesInRange.Clear();

        Unit[] enemies = FindObjectsOfType<Unit>();
        foreach (Unit enemy in enemies)
        {
            if (Mathf.Abs(transform.position.x - enemy.transform.position.x) + Mathf.Abs(transform.position.y - enemy.transform.position.y) <= attackRadius) // check is the enemy is near enough to attack
            {
                if (enemy.playerNumber != gm.playerTurn && !hasAttacked) { // make sure you don't attack your allies
                    enemiesInRange.Add(enemy);
                    enemy.weaponIcon.SetActive(true);
                }

            }
        }
    }

    public void Move(Node movePos)
    {
        gm.ResetTiles();
        StartCoroutine(StartMovement(movePos));
    }

    void Attack(Unit enemy) {
        hasAttacked = true;

        int enemyDamege = attackDamage - enemy.armor;
        int unitDamage = enemy.defenseDamage - armor;

        if (enemyDamege >= 1)
        {
            enemy.health -= enemyDamege;
            enemy.UpdateHealthDisplay();
            DamageIcon d = Instantiate(damageIcon, enemy.transform.position, Quaternion.identity);
            d.Setup(enemyDamege);
        }

        if (transform.tag == "Archer" && enemy.tag != "Archer")
        {
            if (Mathf.Abs(transform.position.x - enemy.transform.position.x) + Mathf.Abs(transform.position.y - enemy.transform.position.y) <= 1) // check is the enemy is near enough to attack
            {
                if (unitDamage >= 1)
                {
                    health -= unitDamage;
                    UpdateHealthDisplay();
                    DamageIcon d = Instantiate(damageIcon, transform.position, Quaternion.identity);
                    d.Setup(unitDamage);
                }
            }
        } else {
            if (unitDamage >= 1)
            {
                health -= unitDamage;
                UpdateHealthDisplay();
                DamageIcon d = Instantiate(damageIcon, transform.position, Quaternion.identity);
                d.Setup(unitDamage);
            }
        }

        if (enemy.health <= 0)
        {
         
            if (deathEffect != null){
				Instantiate(deathEffect, enemy.transform.position, Quaternion.identity);
				camAnim.SetTrigger("shake");
			}

            if (enemy.isKing)
            {
                gm.ShowVictoryPanel(enemy.playerNumber);
            }

            GetWalkableTiles(); // check for new walkable tiles (if enemy has died we can now walk on his tile)
            gm.RemoveInfoPanel(enemy);
            Destroy(enemy.gameObject);
        }

        if (health <= 0)
        {

            if (deathEffect != null)
			{
				Instantiate(deathEffect, enemy.transform.position, Quaternion.identity);
				camAnim.SetTrigger("shake");
			}

			if (isKing)
            {
                gm.ShowVictoryPanel(playerNumber);
            }

            gm.ResetTiles(); // reset tiles when we die
            gm.RemoveInfoPanel(this);
            Destroy(gameObject);
        }

        gm.UpdateInfoStats();
  

    }

    public void ResetWeaponIcon() {
        Unit[] enemies = FindObjectsOfType<Unit>();
        foreach (Unit enemy in enemies)
        {
            enemy.weaponIcon.SetActive(false);
        }
    }

    public IEnumerator StartMovement(Node moveTo)
    { // Moves the character to his new position.

        if (transform.tag == "Knight" || transform.tag == "Dragon")
            path = Pathfinding.FindPathKnightsDragons(transform.position, moveTo.worldPosition);
        else
            path = Pathfinding.FindPathArchersKing(transform.position, moveTo.worldPosition);


        Node lastNode = null;
        if (path.Count > 0)
        {
            int steps = 0;
            if (currentNode != null)
            {
                currentNode.walkable = true;
                currentNode.hasUnit = false;
            }

            while (path.Count > 0 && steps < tileSpeed)
            {
                transform.position = Vector3.MoveTowards(transform.position, path[0], moveSpeed * Time.deltaTime);
                gm.MoveInfoPanel(this);

                if (transform.position == path[0])
                {
                    steps++;
                    lastNode = Pathfinding.grid.NodeFromWorldPoint(path[0]);
                    path.RemoveAt(0);
                }

                yield return null;
            }
        }


        if (lastNode != null)
        {
            lastNode.walkable = false;
            lastNode.hasUnit = true;
        }

        currentNode = lastNode;
        hasMoved = true;
        ResetWeaponIcon();
        GetEnemies();
    }

}
