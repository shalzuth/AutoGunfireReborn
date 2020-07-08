using UnityEngine;
namespace GunfireRebornMods
{
    public class FreeCam : ModBase
    {
        Vector3 LastPos;
        Vector3 LastAngle;
        public override void Update()
        {
            var x = Input.GetAxis("Mouse X");
            var y = Input.GetAxis("Mouse Y");
            var lastMouse = new Vector3(x, y);
            if (Input.GetKey(KeyCode.LeftAlt))
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                return;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            var f = 0.0f;
            var p = GetBaseInput();
            p = p * mainSpeed;
            p = p * Time.deltaTime;
            var cameraTransform = CameraManager.MainCameraCom.transform;
            LastPos = cameraTransform.position;
            cameraTransform.position = LastPos;
            cameraTransform.Translate(p);
            LastPos = cameraTransform.position;

            LastAngle = cameraTransform.eulerAngles;
            lastMouse = new Vector3(-lastMouse.y * camSens, lastMouse.x * camSens, 0);
            lastMouse = new Vector3(LastAngle.x + lastMouse.x, LastAngle.y + lastMouse.y, 0);
            cameraTransform.eulerAngles = lastMouse;
            LastAngle = cameraTransform.eulerAngles;
        }
        float mainSpeed = 10f;
        float camSens = 5f;
        Vector3 GetBaseInput()
        {
            var dir = Vector3.zero;
            if (Input.GetKey(KeyCode.W))
                dir += new Vector3(0, 0, 1);
            if (Input.GetKey(KeyCode.S))
                dir += new Vector3(0, 0, -1);
            if (Input.GetKey(KeyCode.A))
                dir += new Vector3(-1, 0, 0);
            if (Input.GetKey(KeyCode.D))
                dir += new Vector3(1, 0, 0);
            if (Input.GetKey(KeyCode.Space))
                dir += new Vector3(0, 1, 0);
            if (Input.GetKey(KeyCode.C))
                dir += new Vector3(0, -1, 0);
            return dir;
        }
    }
}
