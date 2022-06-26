using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

public class MainCameraAxis : MonoBehaviour
{

    public Camera maincamera;
    public Transform cameraObject;
    public Transform CameraVector;
    public GameObject player;
    public Transform rayTarget;
    public Transform IfLayCastNothingTarget;

    public Transform CameraTransform;
    public Transform target;
    public Transform cameraAim;
    public Vector3 cameraposition;

    private PlayerInputAction inputSystem;

    [Space]
    [Tooltip("현재 카메라 회전각도")]
    public Quaternion CurrentTargetRotation;  // 최종적으로 축적된 Gap이 이 변수에 저장됨.
    [Tooltip("현재 카메라 거리")]
    public float PredictedDistance;             // 카메라와의 거리.
    private float CurrentDistance;
    public float padAutoZoomDistanceValue = 10f;

    [Space]
    [Header("Camera Rotation")]
    [Tooltip("카메라 회전 스피드")]
    //public float RotationSpeed;        // 회전 스피드.

    [Space]
    public float cameraSmoothSpeed = 0.125f;
    public float MoveSpeed;     // 플레이어를 따라오는 카메라 맨의 스피드.

    [Space]
    [Tooltip("카메라 줌인 줌아웃 스피드")]
    [Header("Camera Zoom")]
    public float ZoomSpeed;            // 줌 스피드.
    [Tooltip("카메라 최소 줌")]
    public float MinZoomDistance = 2;
    [Tooltip("카메라 최대 줌")]
    public float MaxZoomDistance = 20;
    [Tooltip("카메라와 플레이어 사이에 장애물이 있을시 부드럽게 보정되는정도 수치")]
    public float obstacleSmoothSpeed = .5f;
    public bool hitObstacle = false;

    [Space]
    [Header("Camera Obstacle Mask")]
    [Tooltip("장애물 검출 마스크")]
    public LayerMask obstacleMask;

    [Space]
    //public Transform aimOne, aimTwo, aimThree;

    private Vector3 desiredPosition;
    private Vector3 cameraDesiredPosition;
    private Vector3 smoothedPositon;
    private float hitDistance;
    private Vector3 Gap;               // 회전 축적 값.
    private Transform MainCamera;      // 카메라 컴포넌트.
    private PlayerController01 controller01;
    private RaycastHit Hit;
    private RaycastHit Hit2;
    private RaycastHit hitinfo;
    public int layermask;
    public GameObject TargetObject;

    public GameObject rayOrigin;
    public GameObject rayDirection;

    [Space]

    public float camera_fix = 3f;

    public CameraTrigger cameraTrigger;

    [Space]

    Vector2 padRotate;

    private void Awake()
    {
        inputSystem = new PlayerInputAction();

        inputSystem.Player.ZoomCamara.performed += context =>
        {
            if (ConsumeAbleItemQuickBar.instance)
            {
                if (!ConsumeAbleItemQuickBar.instance.First_itemQuickChangeOn && !ConsumeAbleItemQuickBar.instance.Second_itemQuickChangeOn)
                {

                    CurrentDistance += -context.ReadValue<Vector2>().y * ZoomSpeed;

                    CurrentDistance = Mathf.Clamp(CurrentDistance, MinZoomDistance, MaxZoomDistance);
                }
            }
        };

        inputSystem.Player.RotateCamera.performed += ctx =>
        {
            //if (PadCursor.instance.GetCurrentCursorScheme() == PadCursor.mouseScheme)
            //Debug.LogError(ctx.control.name);
                padRotate = ctx.ReadValue<Vector2>();
            //else
                //padRotate = Vector2.zero;

        };

        inputSystem.Player.RotateCamera.canceled += ctx => padRotate = Vector2.zero;

        inputSystem.Enable();
    }


    void Start()
    {
        MainCamera = cameraObject.transform;
        player = GameObject.FindGameObjectWithTag("Player").gameObject;
        controller01 = player.GetComponentInChildren<PlayerController01>();
        layermask = (1 << LayerMask.NameToLayer("obstacle")) + (1 << LayerMask.NameToLayer("NPC"));
        target = GameObject.FindGameObjectWithTag("Player").transform;

        this.enabled = false;
        this.enabled = true;
    }

    private void Update()
    {
        //Debug.LogError(padRotate);
        //Debug.LogError(ray.origin + " " + ray.direction);

        //Debug.DrawLine(ray.origin, ray.direction * CurrentDistance, Color.red, Time.deltaTime);
    }

    private void FixedUpdate()
    {


        //Vector3 cameraDesiredPosition = target.position + cameraposition;
        //aimOne.position = cameraDesiredPosition;

        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out Hit2, 1000f, obstacleMask))
        {
            cameraAim.position = Hit2.point;
        }

        //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Ray ray = new Ray(MainCamera.position, (Hit2.point - MainCamera.position).normalized  /*(IfLayCastNothingTarget.position - aimOne.position).normalized*/);
        Debug.DrawRay(ray.origin, ray.direction * 100, Color.white);
        //Physics.Raycast(player.transform.position + (transform.up * 1.5f), (MainCamera.forward * -Distance) + (transform.up * 1.25f), out hitinfo, Distance, obstacleMask)
        if (Physics.Raycast(ray.origin, ray.direction, out Hit, Mathf.Infinity, obstacleMask))
        {
            rayTarget.position = Hit.point;
        }
        else
        {
            rayTarget.position = IfLayCastNothingTarget.position;
        }
        /*Ray ray2 = Camera.main.ScreenPointToRay(camaraAimOffset);

        if (Physics.Raycast(ray2.origin, ray2.direction, out Hit2, Mathf.Infinity, obstacleMask))
        {
            cameraAim.position = Hit2.point;
        }*/
    }

    private void LateUpdate()
    {
        //Physics.Raycast(aimOne.position,(MainCamera.forward * -CurrentDistance)/*  + (transform.up * 1.25f)*/,
        //    out hitinfo, CurrentDistance, obstacleMask);

        //Physics.Raycast(aimOne.position, (AttackEffectFunctions.GetDirection(aimOne.position, aimTwo.position) * -CurrentDistance)/*  + (transform.up * 1.25f)*/,
        //    out hitinfo, CurrentDistance, obstacleMask);

        //Debug.DrawLine(aimOne.position, aimTwo.position, Color.red, Time.deltaTime);

        //if (hitinfo.point != Vector3.zero)//레이케스트 성공시
        //{
        //    hitObstacle = true;
        //hitDistance = Vector3.Distance(player.transform.position, hitinfo.point/* + transform.forward*/);
        //hitDistance = Mathf.Clamp(hitDistance, MinZoomDistance, MaxZoomDistance);

        //    Vector3 offset = (transform.forward * -hitDistance) + transform.up;

        //aimTwo.position = aimOne.position /*+ offset*/;

        //     desiredPosition = MainCamera.position /*+ offset*/;

        /*Vector3 SmoothedPositon = Vector3.Lerp(desiredPosition, hitinfo.point*//* + transform.forward*//*, obstacleSmoothSpeed);
        desiredPosition = SmoothedPositon;*/

        //    MainCamera.position = hitinfo.point;
        //}
        //else
        //{

        //}

        CameraRotation();

        cameraDesiredPosition = target.position + cameraposition;
        smoothedPositon = Vector3.Lerp(transform.position, cameraDesiredPosition, cameraSmoothSpeed * Time.deltaTime);

        CameraTransform.position = smoothedPositon;

        Vector3 offset2 = (transform.forward * -CurrentDistance) + transform.up;
        desiredPosition = transform.position + offset2;
        CurrentDistance = Mathf.Clamp(CurrentDistance, MinZoomDistance, MaxZoomDistance);

        var distToCamera = Vector3.Distance(target.position + cameraposition, desiredPosition);

        //Ray ray = new Ray(target.position + cameraposition, -((target.position + cameraposition) - desiredPosition).normalized);

        Ray ray = new Ray(target.position + cameraposition, -((target.position + cameraposition) - desiredPosition).normalized);

        if (Physics.Raycast(ray.origin, ray.direction, out hitinfo, distToCamera + .5f, obstacleMask))           //벽에 레이케스트 될시 카메라 이동
        {
            //transform.position = hitinfo.point;
            hitDistance = Vector3.Distance(target.position + cameraposition, hitinfo.point/* + transform.forward*/);
            hitDistance = Mathf.Clamp(hitDistance, 0, MaxZoomDistance);

            Vector3 offset = (transform.forward * -(hitDistance - 1)) + transform.up;
            desiredPosition = transform.position + offset;


            MainCamera.position = hitinfo.point + MainCamera.forward;

            TargetObject.transform.position = hitinfo.point;


        }
        else
        {
            Zoom();
        }

        Debug.DrawRay(ray.origin, ray.direction * distToCamera, Color.green);

        Debug.DrawRay(MainCamera.position, AttackEffectFunctions.GetDirection(MainCamera.position, hitinfo.point) * (distToCamera + .5f), Color.red);

        rayOrigin.transform.position = ray.origin;
        rayDirection.transform.position = ray.direction * distToCamera;

    }

    // 카메라 줌.
    void Zoom()
    {
        if (PadCursor.instance.GetCurrentCursorScheme() == PadCursor.gamepadScheme)
        {
            CurrentDistance = padAutoZoomDistanceValue;
        }

        hitObstacle = false;
        CurrentDistance = Mathf.Clamp(CurrentDistance, MinZoomDistance, MaxZoomDistance);
        hitDistance = CurrentDistance;

        Vector3 offset = (transform.forward * -CurrentDistance) + transform.up;
        desiredPosition = transform.position + offset;

        MainCamera.position = desiredPosition;

        //aimTwo.position = aimOne.position + offset;

        //MainCamera.position = Vector3.Lerp(MainCamera.position, aimTwo.position, 0.1f);
    }

    // 카메라 회전.
    void CameraRotation()
    {
        if (CameraTransform.rotation != CurrentTargetRotation)
        {
            //transform.rotation = Quaternion.Slerp(transform.rotation, CurrentTargetRotation, 20 * Time.deltaTime);
            CameraTransform.rotation = CurrentTargetRotation;

            //transform.rotation = CurrentTargetRotation;
        }



        if (!controller01.LockRotate())
        {// 값을 축적.
            //Gap.x += Input.GetAxis("Mouse Y") * Time.deltaTime * -150 * UIManager.instance.settingMenu.mouseSensitiveSlider.value;

            //Debug.LogError(Gap);

            Gap.x += padRotate.y * Time.deltaTime * -10 * UIManager.instance.settingMenu.mouseSensitiveSlider.value;
            //Gap.y += Input.GetAxis("Mouse X") * RotationSpeed;
            //Gap.y = controller01.Gap.y;
            Gap.y += padRotate.x * Time.deltaTime * 10 * UIManager.instance.settingMenu.mouseSensitiveSlider.value;
        }
        // 카메라 회전범위 제한.
        Gap.x = Mathf.Clamp(Gap.x, -70f, 85f);
        // 회전 값을 변수에 저장.
        CurrentTargetRotation = Quaternion.Euler(Gap);
        //CurrentTargetRotation = Quaternion.Euler(padRotate);

        // 카메라벡터 객체에 Axis객체의 x,z회전 값을 제외한 y값만을 넘긴다.
        Quaternion q = CurrentTargetRotation;
        q.x = q.z = 0;
        CameraVector.transform.rotation = q;
    }
}