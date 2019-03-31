using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    [SerializeField] private CharacterController2D controller;
    [SerializeField] private float m_RunSpeed = 100f;

    private float m_HorizontalMovement = 0f;
    private bool m_Jump = false;
    private bool m_SwitchSprite = false;

    // Update is called once per frame
    void Update() {

        m_HorizontalMovement = Input.GetAxisRaw("Horizontal") * m_RunSpeed;

        m_Jump = Input.GetButtonDown("Jump");

        m_SwitchSprite = Input.GetButtonDown("Fire1");
    }

    void FixedUpdate()
    {
        controller.Move(m_HorizontalMovement * Time.fixedDeltaTime, false, m_Jump);
        m_Jump = false;

        if(m_SwitchSprite)
        {
            controller.SwitchSprite();
            m_SwitchSprite = false;
        }
    }
}
