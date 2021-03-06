﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PacmanMove : MonoBehaviour
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
        if (dead)
        {
            Dead();
        }
        if (points.childCount == 0)
        {
            winText.SetActive(true);
            GameObject[] ghosts = GameObject.FindGameObjectsWithTag("ghost");
            foreach (var ghost in ghosts)
            {
                ghost.GetComponent<GhostMove>().StopAI();
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
        if (!Utility.WallHit(transform, (Vector2)dir) && !dead && points.childCount != 0)
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
        if (Input.GetButtonDown("Right") && canTurnRight)
        {
            if (dir == Vector3.left)
            {
                angle = Quaternion.Euler(0, 0, 180);
                dir = Vector3.right;
            }
            else
            {
                turnRight = true;
            }
        }
        if (Input.GetButtonDown("Left") && canTurnLeft)
        {
            if (dir == Vector3.right)
            {
                angle = Quaternion.Euler(0, 0, 0);
                dir = Vector3.left;
            }
            else
            {
                turnLeft = true;
            }
        }
        if (Input.GetButtonDown("Up") && canTurnUp)
        {
            if (dir == Vector3.down)
            {
                angle = Quaternion.Euler(0, 0, -90);
                dir = Vector3.up;
            }
            else
            {
                turnUp = true;
            }
        }
        if (Input.GetButtonDown("Down") && canTurnDown)
        {
            if (dir == Vector3.up)
            {
                angle = Quaternion.Euler(0, 0, 90);
                dir = Vector3.down;
            }
            else
            {
                turnDown = true;
            }
        }

    }

    private void Dead()
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
                        ghost.GetComponent<GhostMove>().Reset();
                    }
                    dead = false;
                    GetComponent<Animator>().SetBool("dead", dead);
                    pacman3d.GetComponent<Animator>().SetBool("dead", dead);
                }
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.name.CompareTo("small-point") == 0 ||
            other.gameObject.name.CompareTo("big-point") == 0)
        {
            score++;
            scoreUI.text = score.ToString();
            Destroy(other.gameObject);
        }
        else if (other.tag.CompareTo("ghost") == 0)
        {
            if (other.GetComponent<GhostMove>().scared == true)
            {
                other.GetComponent<GhostMove>().eaten = true;
                other.GetComponent<GhostMove>().scared = false;
                other.GetComponent<GhostMove>().scaredEnd = false;
                score += 200;
                scoreUI.text = score.ToString();
            }
            if (other.GetComponent<GhostMove>().eaten == false)
            {
                dead = true;
                GameObject[] ghosts = GameObject.FindGameObjectsWithTag("ghost");
                foreach (var ghost in ghosts)
                {
                    ghost.GetComponent<GhostMove>().StopAI();
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
                if (turnLeft && dis < 0.4)
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
                else if (turnRight && dis < 0.4)
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
                else if (turnUp && dis < 0.4)
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
                else if (turnDown && dis < 0.4)
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