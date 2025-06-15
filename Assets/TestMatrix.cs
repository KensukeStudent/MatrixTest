using UnityEngine;

public class TestMatrix : MonoBehaviour
{
    [SerializeField]
    private int xT, yT;

    [SerializeField, Range(0, 360)]
    private float angleX, angleY, angleZ;

    [SerializeField]
    private Vector3 initScale = new Vector3(1, 1, 1);

    private void OnValidate()
    {
        // TRS
        Matrix4x4 translation = new Matrix4x4()
        {
            // x     y        z        w
            m00 = 1, m01 = 0, m02 = 0, m03 = xT, // x
            m10 = 0, m11 = 1, m12 = 0, m13 = yT, // y
            m20 = 0, m21 = 0, m22 = 1, m23 = 0, // z
            m30 = 0, m31 = 0, m32 = 0, m33 = 1  // w
        };

        Matrix4x4 rotateX = MatrixUtil.RotationX(angleX);
        Matrix4x4 rotateY = MatrixUtil.RotationY(angleY);
        Matrix4x4 rotateZ = MatrixUtil.RotationZ(angleZ);

        Matrix4x4 rotate = rotateZ * rotateY * rotateX; // XYZ順で回転

        Matrix4x4 scale = new Matrix4x4()
        {
            // x     y        z        w
            m00 = initScale.x, m01 = 0,           m02 = 0,           m03 = 0, // x
            m10 = 0,           m11 = initScale.y, m12 = 0,           m13 = 0, // y
            m20 = 0,           m21 = 0,           m22 = initScale.z, m23 = 0, // z
            m30 = 0,           m31 = 0,           m32 = 0,           m33 = 1  // w
        };

        Matrix4x4 trs = translation * rotate * scale;
        transform.position = new Vector3(trs.m03, trs.m13, trs.m23);
        transform.rotation = trs.rotation;
        transform.localScale = trs.lossyScale;
    }
}
