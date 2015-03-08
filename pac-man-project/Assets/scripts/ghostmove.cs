using UnityEngine;
using System.Collections;

public class ghostmove : MonoBehaviour
{
    public Transform respawn;
    public Transform target;
    public float speed = 0.4f;
    public Vector3 dir = Vector3.left;
    Vector3 startPos;
    Vector3 startDir;
    bool stopMov = false;
    public bool turnLeft = false;
    public bool turnRight = false;
    public bool turnUp = false;
    public bool turnDown = false;
    public bool canTurnLeft = false;
    public bool canTurnRight = false;
    public bool canTurnUp = false;
    public bool canTurnDown = false;
    public string ghost;
    public bool scared = false;
    public bool scaredEnd = false;
    public bool eaten = false;
    public bool ai = true;
    public float timer = 0;
    public int bigPointsCount = 4;
    Quaternion angle;
    void Start()
    {
        bigPointsCount = GameObject.FindGameObjectsWithTag("big-point").Length;
        startPos = transform.position;
        startDir = dir;
        if (dir == Vector3.down)
        {
            transform.GetChild(0).transform.eulerAngles = new Vector3(0, 0, 0);
        }
        else if (dir == Vector3.up)
        {
            transform.GetChild(0).transform.eulerAngles = new Vector3(0, 0, 180);
        }
        else if (dir == Vector3.right)
        {
            transform.GetChild(0).transform.eulerAngles = new Vector3(0, 0, 90);
        }
        else if (dir == Vector3.left)
        {
            transform.GetChild(0).transform.eulerAngles = new Vector3(0, 0, -90);
        }
    }
    void Update()
    {
        transform.GetChild(0).transform.rotation = Quaternion.Lerp(transform.GetChild(0).rotation, angle, 0.1f);
        if (GameObject.FindGameObjectsWithTag("big-point").Length < bigPointsCount)
        {
            bigPointsCount--;
            if (eaten == false)
            {
                timer = Time.time;
                scared = true;
                scaredEnd = false;
                speed = 5;
            }
        }
        if (scared && Time.time - timer >= 18)
        {
            if (Time.time - timer >= 20)
            {
                timer = 0;
                scared = false;
                scaredEnd = false;
                speed = 10;
            }
            else
            {
                scaredEnd = true;
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
        if (!valid((Vector2)dir) && !stopMov)
        {
            transform.position += dir * speed * Time.deltaTime;
            GetComponent<Animator>().SetFloat("DirX", dir.x);
            GetComponent<Animator>().SetFloat("DirY", dir.y);
            GetComponent<Animator>().SetBool("scared", scared);
            GetComponent<Animator>().SetBool("scaredend", scaredEnd);
            GetComponent<Animator>().SetBool("eaten", eaten);
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
        if (other.transform.parent != null)
        {
            if (other.transform.parent.name.CompareTo("turns") == 0)
            {
                string[] turns = other.name.Split(',');
                if (turns[0].CompareTo("teleport") == 0)
                {
                    transform.position = new Vector3(float.Parse(turns[1]), float.Parse(turns[2]), 0);
                }
                if (turns[0].CompareTo("left") == 0 || turns[0].CompareTo("right") == 0 ||
                    turns[0].CompareTo("up") == 0 || turns[0].CompareTo("down") == 0 ||
                    turns[0].CompareTo("Left") == 0 || turns[0].CompareTo("Right") == 0 ||
                    turns[0].CompareTo("Up") == 0 || turns[0].CompareTo("Down") == 0)
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
                        else if (turns[i].CompareTo("Down") == 0 && eaten == true)
                        {
                            turnDown = true;
                            ai = false;
                        }
                        else if (turns[i].CompareTo("Up") == 0)
                        {
                            turnUp = true;
                            eaten = false;
                            ai = true;
                        }
                        else if (turns[i].CompareTo("Right") == 0 && eaten == false)
                        {
                            canTurnRight = true;
                        }
                        else if (turns[i].CompareTo("Left") == 0 && eaten == false)
                        {
                            canTurnLeft = true;
                        }
                    }
                }
                if (ai == true)
                {
                    if (!eaten && !scared)
                    {
                        switch (ghost)
                        {
                            case "red":
                                GhostAi(new Vector3(target.position.x + 7, target.position.y - 7, 0), false);
                                break;
                            case "blue":
                                GhostAi(new Vector3(target.position.x, target.position.y + 14, 0), false);
                                break;
                            case "pink":
                                GhostAi(new Vector3(target.position.x + 14, target.position.y + 7, 0), false);
                                break;
                            case "orange":
                                GhostAi(new Vector3(target.position.x, target.position.y, 0), false);
                                break;
                        }
                    }
                    else if (scared)
                    {
                        GameObject pacman = GameObject.FindGameObjectWithTag("pacman");
                        GhostAi(new Vector3(pacman.transform.position.x, pacman.transform.position.y, 0), true);
                    }
                    else if (eaten)
                    {
                        speed = 10;
                        GhostAi(new Vector3(respawn.position.x, respawn.position.y, 0), false);
                    }

                }
            }
        }
    }
    void GhostAi(Vector3 target, bool run)
    {
        while (canTurnDown || canTurnUp || canTurnRight || canTurnLeft)
        {
            if (!run && target.y > transform.position.y && System.Math.Abs(transform.position.y - target.y) > 4 && canTurnUp && dir != Vector3.down)
            {
                turnUp = true;
                break;
            }
            else if (!run && target.y < transform.position.y && System.Math.Abs(transform.position.y - target.y) > 4 && canTurnDown && dir != Vector3.up)
            {
                turnDown = true;
                break;
            }
            else if (!run && target.x < transform.position.x && canTurnLeft && dir != Vector3.right)
            {
                turnLeft = true;
                break;
            }
            else if (!run && target.x > transform.position.x && canTurnRight && dir != Vector3.left)
            {
                turnRight = true;
                break;
            }
            else if (run && target.y < transform.position.y && System.Math.Abs(transform.position.y - target.y) > 4 && canTurnUp && dir != Vector3.down)
            {
                turnUp = true;
                break;
            }
            else if (run && target.y > transform.position.y && System.Math.Abs(transform.position.y - target.y) > 4 && canTurnDown && dir != Vector3.up)
            {
                turnDown = true;
                break;
            }
            else if (run && target.x > transform.position.x && canTurnLeft && dir != Vector3.right)
            {
                turnLeft = true;
                break;
            }
            else if (run && target.x < transform.position.x && canTurnRight && dir != Vector3.left)
            {
                turnRight = true;
                break;
            }
            else
            {
                if (canTurnUp && dir != Vector3.down)
                {
                    turnUp = true;
                    break;
                }
                else if (canTurnDown && dir != Vector3.up)
                {
                    turnDown = true;
                    break;
                }
                else if (canTurnLeft && dir != Vector3.right)
                {
                    turnLeft = true;
                    break;
                }
                else if (canTurnRight && dir != Vector3.left)
                {
                    turnRight = true;
                    break;
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
                    angle = Quaternion.Euler(0, 0, -90);
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
                    angle = Quaternion.Euler(0, 0, 90);
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
                    angle = Quaternion.Euler(0, 0, 180);
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
                    angle = Quaternion.Euler(0, 0, 0);
                    dir = Vector3.down;
                }
            }
        }
    }

    public void StopAI()
    {
        ai = false;
        stopMov = true;
        MoveToLayer(transform, 10);
    }

    public void Reset()
    {
        ai = false;
        if (ghost.CompareTo("red") == 0)
        {
            ai = true;
        }
        stopMov = false;
        transform.position = startPos;
        dir = startDir;
        timer = 0;
        scared = false;
        scaredEnd = false;
        speed = 10;
        MoveToLayer(transform, 9);
        MoveToLayer(transform.GetChild(0), 8);
    }

    void MoveToLayer(Transform root, int layer)
    {
        root.gameObject.layer = layer;
        foreach (Transform child in root)
        {
            MoveToLayer(child, layer);
        }
    }
}
