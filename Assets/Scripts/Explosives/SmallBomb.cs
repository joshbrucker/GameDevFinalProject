﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallBomb : MonoBehaviour, Explosive
{
    public int time = 3;
    public int x, y;
    public Sprite primedSprite;
    public GameObject warningPrefab;
    public GameObject explosionPrefab;

    public bool primed { get; set; }

    Node[,] grid;
    List<GameObject> warnings;

    void Start()
    {
        grid = Grid.grid;
        warnings = new List<GameObject>();
        primed = false;
        Invoke("Warn", time - 0.5f);
        Invoke("Explode", time);
    }

    void Update()
    {

    }

    // Triggers explosions across closs horizontal/vertical
    public void Explode()
    {
        ClearWarnings();

        SetExplosion(0, 0);

        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (Mathf.Abs(i) != Mathf.Abs(j) && (x + i < grid.GetLength(0) && x + i >= 0) && (y + j < grid.GetLength(1) && y + j >= 0))
                {
                    SetExplosion(i, j);
                }
            }
        }

        Destroy(gameObject);
    }

    // Sets an explosion on an individual node
    public void SetExplosion(int i, int j)
    {
        Node node = grid[x + i, y + j];
        node.harmful = true;
        if (node.IsOccupied() && (i != 0 || j != 0))
        {
            if (node.currentObj.GetComponent<Explosive>() != null && !node.currentObj.GetComponent<Explosive>().primed)
            {
                node.currentObj.GetComponent<Explosive>().ChainExplode();
            }
        }

        GameObject explosion = Instantiate(explosionPrefab, transform.position + new Vector3(j, -i, 0), transform.rotation);
        explosion.GetComponent<Explosion>().x = x + i;
        explosion.GetComponent<Explosion>().y = y + j;
        node.currentObj = explosion;
    }

    // Triggers a warning across close horizontal/vertical
    public void Warn()
    {
        primed = true;

        SetWarning(0, 0);

        gameObject.GetComponent<SpriteRenderer>().sprite = primedSprite;
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (Mathf.Abs(i) != Mathf.Abs(j) && (x + i < grid.GetLength(0) && x + i >= 0) && (y + j < grid.GetLength(1) && y + j >= 0))
                {
                    SetWarning(i, j);
                }
            }
        }
    }

    // Sets warnings on an individual node
    public void SetWarning(int i, int j)
    {
        Node node = grid[x + i, y + j];
        GameObject warning = Instantiate(warningPrefab, transform.position + new Vector3(j, -i, 0), transform.rotation);
        warnings.Add(warning);
    }

    public void ClearWarnings()
    {
        foreach (Object warning in warnings)
        {
            if (warning is GameObject)
            {
                Destroy((GameObject)warning);
            }
        }

        warnings.Clear();
    }

    public void Kill()
    {
        CancelInvoke();
        ClearWarnings();
        Destroy(gameObject);
    }

    // Immediately sets explosive into primed mode
    public void ChainExplode()
    {
        CancelInvoke();
        Warn();
        Invoke("Explode", 0.5f);
    }
}
