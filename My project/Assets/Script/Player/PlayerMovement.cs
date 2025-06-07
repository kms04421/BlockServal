using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
 
    public float moveSpeed = 5f;
    public float gravity = -10f;
    public float jumpHeight = 2f;

    public Transform cameraHolder; // 카메라
    public float mouseSensitivity = 100f;

    float xRotation = 0f;
    Vector3 velocity; // 속도
    bool isGrounded; // 땅 위에 있는지 여부

    public Transform groundCheck; // 땅 체크 위치
    public float groundDistance = 0.4f; // 땅 체크 거리
    public LayerMask groundMask; // 땅 레이어

    CharacterController controller; // 캐릭터 컨트롤러
    public GameObject equippedObject; // 장착된 무기

    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked; // 마우스 커서 고정
    }

    void Update()
    {
        // 마우스 커서 이동
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 160f); // 카메라 회전 제한

        cameraHolder.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX); // 플레이어 회전

        // 땅 체크
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        
        if (isGrounded && velocity.y < 0)
            velocity.y = -2f;

        // 이동
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * moveSpeed * Time.deltaTime);

        // 점프
        if (Input.GetButtonDown("Jump") && isGrounded)
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        // 마우스 왼쪽 버튼 클릭
        if(Input.GetMouseButtonDown(0))
        {
            IUsable usable = equippedObject?.GetComponent<IUsable>();
            usable?.Attack();
        }
        // 중력 적용
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        
    }
}
