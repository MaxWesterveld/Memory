using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class Card : MonoBehaviour
{
    [SerializeField] private CardStatus status;
    public float turnTargetTime;
    private float turnTimer;
    private Quaternion startRotation;
    private float targetRotation;
    [SerializeField] SpriteRenderer frontRenderer;
    [SerializeField] SpriteRenderer backRenderer;
    public Game game;

    private void Awake()
    {
        status = CardStatus.show_back;
        GetFrontAndBackSpriteRenderers();
        game = FindObjectOfType<Game>();
    }
    private void Start()
    {

    }
    private void FixedUpdate()
    {
        if (status == CardStatus.rotating_to_front || status == CardStatus.rotating_to_back)
        {
            turnTimer = turnTimer + Time.deltaTime;
            if (turnTargetTime == 0)
            {
                Debug.LogError("Card.turnTargetTime is null");
            }

            float l_percentage;
            l_percentage = (1f / turnTargetTime) * turnTimer;

            targetRotation = 0f;
            if (status == CardStatus.rotating_to_front)
            {
                targetRotation = 180f;
            }
            transform.rotation = Quaternion.Slerp(startRotation, Quaternion.Euler(0f, targetRotation, 0f), l_percentage);

            if (l_percentage >= 1)
            {
                if (status == CardStatus.rotating_to_back)
                {
                    status = CardStatus.show_back;
                }
                else if (status == CardStatus.rotating_to_front)
                {
                    status = CardStatus.show_front;

                    game.SelectCard(gameObject);
                }
            }
        }
    }
    private void OnMouseDown()
    {
        if (status == CardStatus.show_back)
        {
            TurnToFront();
        }
        else if (status == CardStatus.show_front)
        {
            TurnToBack();
        }
    }
    private void OnMouseUp()
    {
        if (game.AllowedToSelectCard(this) == true)
        {
            if (status == CardStatus.show_back)
            {
                TurnToFront();
                game.SelectCard(gameObject);
            }
            else if (status == CardStatus.show_front)
            {
                TurnToBack();
            }
        }
    }
    public void TurnToFront()
    {
        status = CardStatus.rotating_to_front;
        turnTimer = 0f;
        startRotation = transform.rotation;
    }
    public void TurnToBack()
    {
        status = CardStatus.rotating_to_back;
        turnTimer = 0f;
        startRotation = transform.rotation;
    }
    private void GetFrontAndBackSpriteRenderers()
    {
        foreach (Transform t in transform)
        {
            if ((t.name) == "Front")
            {
                frontRenderer = t.GetComponent<SpriteRenderer>();
            }
            else if ((t.name) == "Back")
            {
                backRenderer = t.GetComponent<SpriteRenderer>();
            }
        }
    }
    public void SetFront(Sprite s)
    {
        if (frontRenderer != null)
        {
            frontRenderer.sprite = s;
        }
    }
    public void SetBack(Sprite s)
    {
        if (backRenderer != null)
        {
            backRenderer.sprite = s;
        }
    }
    public Vector2 GetFrontSize() 
    {
        return frontRenderer.bounds.size;
    }
    public Vector2 GetBackSize()
    {
        return backRenderer.bounds.size;
    }
}