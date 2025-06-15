using UnityEngine;

/// <summary>
/// 行列計算のユーティリティクラス
/// </summary>
public static class MatrixUtil
{
    public static Matrix4x4 ManualProjection()
    {
        Camera cam = Camera.main;

        float size = cam.orthographicSize;
        float aspect = cam.aspect;

        float top = size;
        float bottom = -size;
        float right = size * aspect;
        float left = -right;

        float near = cam.nearClipPlane;
        float far = cam.farClipPlane;

        Matrix4x4 manualProjection = Ortho(left, right, bottom, top, near, far);
        return manualProjection;
    }

    /// <summary>
    /// Orthographic射影行列
    /// </summary>
    private static Matrix4x4 Ortho(float left, float right, float bottom, float top, float near, float far)
    {
        /*
        例えば、
            x方向を [-8.884, 8.884] → [-1, 1] にスケーリング。

            y方向を [-5, 5] → [-1, 1] にスケーリング。

            z方向を [0.3, 1000] → [-1, 1] にスケーリングしつつ反転（-1 に近い方が遠い）。

            中心点が (0,0,0) になるように平行移動。
        */
        Matrix4x4 m = new Matrix4x4()
        {
            m00 = 2f / (right - left),
            m01 = 0,
            m02 = 0,
            m03 = -(right + left) / (right - left),
            m10 = 0,
            m11 = 2f / (top - bottom),
            m12 = 0,
            m13 = -(top + bottom) / (top - bottom),
            m20 = 0,
            m21 = 0,
            m22 = -2f / (far - near),
            m23 = -(far + near) / (far - near),
            m30 = 0,
            m31 = 0,
            m32 = 0,
            m33 = 1,
        };

        return m;
    }

    public static Matrix4x4 ManualView_LookAt()
    {
        Camera cam = Camera.main;

        Vector3 eye = cam.transform.position;
        Vector3 target = eye + cam.transform.forward;
        Vector3 up = cam.transform.up;

        // 左手座標系のビュー行列
        return LookAtLH(eye, target, up);
    }

    private static Matrix4x4 LookAtLH(Vector3 eye, Vector3 target, Vector3 up)
    {
        Vector3 e = (target - eye).normalized;           // Forward
        Vector3 v = Vector3.Normalize(Vector3.Cross(up, e));  // Right
        Vector3 u = Vector3.Cross(e, v);         // Up

        Matrix4x4 view = new Matrix4x4();
        view.SetColumn(0, new Vector4(v.x, u.x, e.x, 0));
        view.SetColumn(1, new Vector4(v.y, u.y, e.y, 0));
        view.SetColumn(2, new Vector4(v.z, u.z, -e.z, 0)); // -e.z
        view.SetColumn(3, new Vector4(Vector3.Dot(-eye, v), Vector3.Dot(-eye, u), Vector3.Dot(eye, e), 1));

        return view;
    }

    private static Matrix4x4 ToMVP(Transform target)
    {
        // モデル行列
        Matrix4x4 model = target.localToWorldMatrix;

        // ビュー行列
        Matrix4x4 view = ManualView_LookAt();

        // 射影行列
        Matrix4x4 proj = ManualProjection();

        // MVP行列
        Matrix4x4 MVP = proj * view * model;
        return MVP;
    }

    private static Vector4 ToClipSpace(Transform target)
    {
        // MVP行列
        Matrix4x4 MVP = ToMVP(target);

        // クリップ空間座標
        Vector3 world = Vector3.zero; // モデル空間の原点（Spriteの中心）
        Vector4 clip = MVP * new Vector4(world.x, world.y, -world.z, 1);
        return clip;
    }

    public static Vector3 ToNDC(Transform target)
    {
        // クリップ空間座標
        Vector4 clip = ToClipSpace(target);

        // NDC座標
        Vector3 ndc = new Vector3(clip.x, clip.y, clip.z) / clip.w; // -1〜1の範囲に正規化
        return ndc;
    }

    public static Matrix4x4 RotationX(float angle)
    {
        float radian = angle * Mathf.Deg2Rad;
        float cos = Mathf.Cos(radian);
        float sin = Mathf.Sin(radian);

        Matrix4x4 rotationX = new Matrix4x4()
        {
            m00 = 1, m01 = 0,   m02 = 0,    m03 = 0,
            m10 = 0, m11 = cos, m12 = -sin, m13 = 0,
            m20 = 0, m21 = sin, m22 = cos,  m23 = 0,
            m30 = 0, m31 = 0,   m32 = 0,    m33 = 1
        };

        return rotationX;
    }

    public static Matrix4x4 RotationY(float angle)
    {
        float radian = angle * Mathf.Deg2Rad;
        float cos = Mathf.Cos(radian);
        float sin = Mathf.Sin(radian);

        Matrix4x4 rotationY = new Matrix4x4()
        {
            m00 = cos,  m01 = 0, m02 = sin,  m03 = 0,
            m10 = 0,    m11 = 1, m12 = 0,    m13 = 0,
            m20 = -sin, m21 = 0, m22 = cos,  m23 = 0,
            m30 = 0,    m31 = 0, m32 = 0,    m33 = 1
        };

        return rotationY;
    }

    public static Matrix4x4 RotationZ(float angle)
    {
        float radian = angle * Mathf.Deg2Rad;
        float cos = Mathf.Cos(radian);
        float sin = Mathf.Sin(radian);

        Matrix4x4 rotationZ = new Matrix4x4()
        {
            m00 = cos,  m01 = -sin, m02 = 0, m03 = 0,
            m10 = sin,  m11 = cos,  m12 = 0, m13 = 0,
            m20 = 0,    m21 = 0,    m22 = 1, m23 = 0,
            m30 = 0,    m31 = 0,    m32 = 0, m33 = 1
        };

        return rotationZ;
    }
}
