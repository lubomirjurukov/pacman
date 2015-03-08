using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class pacmanmove : MonoBehaviour
{
    public Text scoreUI;
    public Text livesUI;
    public GameObject winText;
    public GameObject gameoverText;
    public GameObject pacman3d;
    public Transform points;
    public float speed = 0.4f;
    Vector3 dir = Vector3.left;
    Vector3 startPos;
    Vector3 startDir;
    Quaternion startAngle;
    bool turnLeft = false;
    bool turnRight = false;
    bool turnUp = false;
    bool turnDown = false;
    bool canTurnLeft = false;
    bool canTurnRight = false;
    bool canTurnUp = false;
    bool canTurnDown = false;
    bool dead = false;
    public int score = 0;
    public int lives = 3;
    Quaternion angle;
    void Start()
    {
        startPos = transform.position;
        startDir = dir;
        if (dir == Vector3.down)
        {
            startAngle = Quaternion.Euler(0, 0, 90);
        }
        else if (dir == Vector3.up)
        {
            startAngle = Quaternion.Euler(0, 0, -90);
        }
        else if (dir == Vector3.right)
        {
            startAngle = Quaternion.Euler(0, 0, 180);
        }
        else if (dir == Vector3.left)
        {
            startAngle = Quaternion.Euler(0, 0, 0);
        }
    }
    void Update()
    {
        transform.GetChild(0).transform.rotation = Quaternion.Lerp(transform.GetChild(0).rotation, angle, 0.1f);
        if (dead == true)
        {
            if (GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("death-left") ||
                GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("death-right") ||
                GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("death-up") ||
                GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("death-down"))
            {
                if (GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
                {
                    if (lives == 0)
                    {
                        gameoverText.SetActive(true);
                    }
                    else
                    {
                        lives--;
                        livesUI.text = lives.ToString();
                        transform.position = startPos;
                        dir = startDir;
                        angle = startAngle;
                        GameObject[] ghosts = GameObject.FindGameObjectsWithTag("ghost");
                        foreach (var ghost in ghosts)
                        {
                            ghost.GetComponent<ghostmove>().Reset();
                        }
                        dead = false;
                        GetComponent<Animator>().SetBool("dead", dead);
                        pacman3d.GetComponent<Animator>().SetBool("dead", dead);
                    }
                }
            }
        }
        if (points.childCount == 0)
        {
            winText.SetActive(true);
            GameObject[] ghosts = GameObject.FindGameObjectsWithTag("ghost");
            foreach (var ghost in ghosts)
            {
                ghost.GetComponent<ghostmove>().StopAI();
            }
        }
        if (dir == Vector3.left || dir == Vector3.right)
        {
            canTurnLeft = true;
            canTurnRight = true;
        }
        if (dir == Vector3.up || dir == Vector3.down)
        {
            canTurnUp = true;
            canTurnDown = true;
        }
        if (!valid((Vector2)dir) && !dead && points.childCount != 0)
        {
            transform.position += dir * speed * Time.deltaTime;
            GetComponent<Animator>().SetFloat("DirX", dir.x);
            GetComponent<Animator>().SetFloat("DirY", dir.y);
        }
        if (Input.GetKey("escape"))
        {
            Application.Quit();
        }
        if (Input.GetKey("r") && (points.childCount == 0 || dead))
        {
            Application.LoadLevel(0);
        }
        if (Input.GetButton("Right"))
        {
            if (dir == Vector3.left && canTurnUp)
            {
                turnUp = true;
            }
            if (dir == Vector3.right && canTurnDown)
            {
                turnDown = true;
            }
            if (dir == Vector3.up && canTurnRight)
            {
                turnRight = true;
            }
            if (dir == Vector3.down && canTurnLeft)
            {
                turnLeft = true;
            }
        }
        if (Input.GetButtonDown("Down"))
        {
            if (dir == Vector3.left && canTurnRight)
            {
                angle = Quaternion.Euler(0, 0, 180);
                dir = Vector3.right;
            }
            else if (dir == Vector3.right && canTurnLeft)
            {
                angle = Quaternion.Euler(0, 0, 0);
                dir = Vector3.left;
            }
            else if (dir == Vector3.up && canTurnDown)
            {
                angle = Quaternion.Euler(0, 0, 90);
                dir = Vector3.down;
            }
            else if (dir == Vector3.down && canTurnUp)
            {
                angle = Quaternion.Euler(0, 0, -90);
                dir = Vector3.up;
            }
        }
        if (Input.GetButton("Left"))
        {
            if (dir == Vector3.left && canTurnDown)
            {
                turnDown = true;
            }
            if (dir == Vector3.right && canTurnUp)
            {
                turnUp = true;
            }
            if (dir == Vector3.up && canTurnLeft)
            {
                turnLeft = true;
            }
            if (dir == Vector3.down && canTurnRight)
            {
                turnRight = true;
            }
        }
    }

    bool valid(Vector2 dir)
    {
        Vector2 pos = transform.position;
        RaycastHit2D[] hit = Physics2D.LinecastAll(pos + dir + dir + (dir * 0.1f), pos);
        Debug.DrawLine(pos, pos + dir + dir + (dir * 0.1f), Color.red);
        bool wallHit = false;
        for (int i = 0; i < hit.Length; i++)
        {
            if (hit[i].collider.gameObject.name.CompareTo("Wall") == 0)
            {
                wallHit = true;
                break;
            }
        }
        return wallHit;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.name.CompareTo("small-point") == 0)
        {
            score++;
            scoreUI.text = score.ToString();
            Destroy(other.gameObject);
        }
        else if (other.gameObject.name.CompareTo("big-point") == 0)
        {
            score++;
            scoreUI.text = score.ToString();
            Destroy(other.gameObject);
        }
        else if (other.tag.CompareTo("ghost") == 0)
        {
            if (other.GetComponent<ghostmove>().scared == true)
            {
                other.GetComponent<ghostmove>().eaten = true;
                other.GetComponent<ghostmove>().scared = false;
                other.GetComponent<ghostmove>().scaredEnd = false;
                score += 200;
                scoreUI.text = score.ToString();
            }
            if (other.GetComponent<ghostmove>().eaten == false)
            {
                dead = true;
                GameObject[] ghosts = GameObject.FindGameObjectsWithTag("ghost");
                foreach (var ghost in ghosts)
                {
                    ghost.GetComponent<ghostmove>().StopAI();
                }
                GetComponent<Animator>().SetBool("dead", dead);
                pacman3d.GetComponent<Animator>().SetBool("dead", dead);
            }
        }
        if (other.transform.parent != null)
        {
            string[] turns = other.name.Split(',');
            if (turns[0].CompareTo("teleport") == 0)
            {
                transform.position = new Vector3(float.Parse(turns[1]), float.Parse(turns[2]), 0);
            }
            if (other.transform.parent.name.CompareTo("turns") == 0)
            {
                canTurnUp = false;
                canTurnDown = false;
                canTurnLeft = false;
                canTurnRight = false;
                turnUp = false;
                turnDown = false;
                turnLeft = false;
                turnRight = false;
                for (int i = 0; i < turns.Length; i++)
                {
                    if (turns[i].CompareTo("left") == 0)
                    {
                        canTurnLeft = true;
                    }
                    else if (turns[i].CompareTo("right") == 0)
                    {
                        canTurnRight = true;
                    }
                    else if (turns[i].CompareTo("up") == 0)
                    {
                        canTurnUp = true;
                    }
                    else if (turns[i].CompareTo("down") == 0)
                    {
                        canTurnDown = true;
                    }
                }
            }
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.transform.parent != null)
        {
            if (other.transform.parent.name.CompareTo("turns") == 0)
            {
                float dis = Vector3.Distance(transform.position, other.transform.position);
                if (turnLeft && dis < 0.3)
                {
                    turnLeft = false;
                    canTurnUp = false;
                    canTurnDown = false;
                    canTurnLeft = true;
                    canTurnRight = false;
                    transform.position = other.transform.position;
                    angle = Quaternion.Euler(0, 0, 0);
                    dir = Vector3.left;
                }
                else if (turnRight && dis < 0.3)
                {
                    turnRight = false;
                    canTurnUp = false;
                    canTurnDown = false;
                    canTurnLeft = false;
                    canTurnRight = true;
                    transform.position = other.transform.position;
                    angle = Quaternion.Euler(0, 0, 180);
                    dir = Vector3.right;
                }
                else if (turnUp && dis < 0.3)
                {
                    turnUp = false;
                    canTurnUp = true;
                    canTurnDown = false;
                    canTurnLeft = false;
                    canTurnRight = false;
                    transform.position = other.transform.position;
                    angle = Quaternion.Euler(0, 0, -90);
                    dir = Vector3.up;
                }
                else if (turnDown && dis < 0.3)
                {
                    turnDown = false;
                    canTurnUp = false;
                    canTurnDown = true;
                    canTurnLeft = false;
                    canTurnRight = false;
                    transform.position = other.transform.position;
                    angle = Quaternion.Euler(0, 0, 90);
                    dir = Vector3.down;
                }
            }
        }
    }
}