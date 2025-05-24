using UnityEngine;

public static class ScreenUtil
{
    public static Vector3 WorldToViewportPoint(Transform target)
    {
        // NDC座標
        Vector3 ndc = MatrixUtil.ToNDC(target);

        // ビューポート座標
        Vector3 viewport = new Vector3( // NDCを0〜1にマッピング
            ndc.x * 0.5f + 0.5f,
            ndc.y * 0.5f + 0.5f,
            ndc.z * 0.5f + 0.5f // ←ここはZが0〜1になるかは射影に依存（Orthographicなら真似可）
        );

        return viewport;
    }

    public static Vector2 WorldToScreenPoint(Transform target)
    {
        // ビューポート座標
        Vector3 viewport = WorldToViewportPoint(target);

        // スクリーン座標
        Vector2 screenPos = new Vector2(
            viewport.x * Screen.width,
            viewport.y * Screen.height
        );

        return screenPos;
    }

    public static Vector3 ScreenToWorldPoint(Vector3 screenPos)
    {
        float screenX = (screenPos.x / Screen.width) * 2f - 1f;
        float screenY = (screenPos.y / Screen.height) * 2f - 1f;

        Vector4 ndcPos = new Vector4(screenX, screenY, screenPos.z, 1f);

        // 2. NDC → ワールド空間への変換（inverse MVP）
        Matrix4x4 projection = MatrixUtil.ManualProjection();
        Matrix4x4 view = MatrixUtil.ManualView_LookAt();
        Matrix4x4 invVP = (projection * view).inverse;

        Vector4 worldPos = invVP * ndcPos;
        worldPos.z = Camera.main.transform.position.z; // Z軸はカメラの位置を使用
        worldPos /= worldPos.w;

        return worldPos;
    }

    public static Vector2 ScreenToViewportPoint(Vector3 screenPos)
    {
        // スクリーン座標をビューポート座標に変換
        Vector2 viewportPoint = new Vector2(
            screenPos.x / Screen.width,
            screenPos.y / Screen.height
        );

        return viewportPoint;
    }

    public static Vector3 ScreenToNDCPoint(Vector3 screenPos)
    {
        // スクリーン座標をビューポート座標に変換
        Vector2 viewportPoint = ScreenToViewportPoint(screenPos);

        // ビューポート座標をNDC座標に変換 // 0〜1 → -1〜1
        Vector3 ndcPoint = new Vector3(
            viewportPoint.x / 0.5f - 1.0f,
            viewportPoint.y / 0.5f - 1.0f,
            screenPos.z
        );

        return ndcPoint;
    }
}
