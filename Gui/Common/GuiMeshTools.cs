
// -------------------------------------------------------
// LeopotamGroupLibrary for unity3d
// Copyright (c) 2012-2017 Leopotam <leopotam@gmail.com>
// -------------------------------------------------------

using System.Collections.Generic;
using UnityEngine;

namespace LeopotamGroup.Gui.Common {
    /// <summary>
    /// Mesh tools helper.
    /// </summary>
    public static class GuiMeshTools {
        static readonly Vector2[] RoundFilledV = {
            new Vector2 (-1f, 0f),
            new Vector2 (-1f, -1f),
            new Vector2 (0f, -1f),
            new Vector2 (0f, 0f)
        };

        static readonly Vector2[] RoundFilledUV = {
            new Vector2 (0f, 1f),
            new Vector2 (0f, 0f),
            new Vector2 (1f, 0f),
            new Vector2 (1f, 1f)
        };

        static readonly List<Vector3> _cacheV = new List<Vector3> (4096);

        static readonly List<Vector2> _cacheUV = new List<Vector2> (4096);

        static readonly List<Color> _cacheC = new List<Color> (4096);

        static readonly List<int> _cacheT = new List<int> (4096);

        static Rect _vRect;

        static Rect _vRect2;

        static Rect _uvRect;

        static Rect _uvRect2;

        static GuiFontEffect _effect;

        static Vector2 _effectValue;

        static Color _effectColor;

        static void FillBufferT () {
            var vOffset = _cacheV.Count;
            _cacheT.Add (vOffset);
            _cacheT.Add (vOffset + 1);
            _cacheT.Add (vOffset + 2);
            _cacheT.Add (vOffset + 2);
            _cacheT.Add (vOffset + 3);
            _cacheT.Add (vOffset);
        }

        /// <summary>
        /// Get new instance of mesh.
        /// </summary>
        public static Mesh GetNewMesh () {
            var mesh = new Mesh ();
            mesh.MarkDynamic ();
            mesh.hideFlags = HideFlags.DontSave;
            return mesh;
        }

        /// <summary>
        /// Prepare mesh buffer for filling.
        /// </summary>
        /// <param name="effect">Effect.</param>
        /// <param name="effectValue">Effect value.</param>
        /// <param name="effectColor">Effect color.</param>
        public static void PrepareBuffer (
            GuiFontEffect effect = GuiFontEffect.None, Vector2? effectValue = null, Color? effectColor = null) {
            _cacheV.Clear ();
            _cacheUV.Clear ();
            _cacheC.Clear ();
            _cacheT.Clear ();
            _effect = effect;
            _effectValue = effectValue.HasValue ? effectValue.Value : Vector2.zero;
            _effectColor = effectColor.HasValue ? effectColor.Value : Color.black;
        }

        /// <summary>
        /// Fill buffer.
        /// </summary>
        /// <param name="v">Sprite size rect.</param>
        /// <param name="uv">Sprite UV rect.</param>
        /// <param name="color">Color.</param>
        public static void FillBuffer (ref Rect v, ref Rect uv, ref Color color) {
            var uv0 = new Vector2 (uv.xMin, uv.yMin);
            var uv1 = new Vector2 (uv.xMin, uv.yMax);
            var uv2 = new Vector2 (uv.xMax, uv.yMax);
            var uv3 = new Vector2 (uv.xMax, uv.yMin);
            FillBuffer (ref v, ref uv0, ref uv1, ref uv2, ref uv3, ref color);
        }

        /// <summary>
        /// Fill buffer.
        /// </summary>
        /// <param name="v">Sprite size rect.</param>
        /// <param name="uv0">Sprite UV of 1 corner.</param>
        /// <param name="uv1">Sprite UV of 2 corner.</param>
        /// <param name="uv2">Sprite UV of 3 corner.</param>
        /// <param name="uv3">Sprite UV of 4 corner.</param>
        /// <param name="color">Color.</param>
        public static void FillBuffer (
            ref Rect v, ref Vector2 uv0, ref Vector2 uv1, ref Vector2 uv2, ref Vector2 uv3, ref Color color) {
            var v0 = new Vector3 (v.xMin, v.yMin, 0f);
            var v1 = new Vector3 (v.xMin, v.yMax, 0f);
            var v2 = new Vector3 (v.xMax, v.yMax, 0f);
            var v3 = new Vector3 (v.xMax, v.yMin, 0f);
            FillBuffer (ref v0, ref v1, ref v2, ref v3, ref uv0, ref uv1, ref uv2, ref uv3, ref color);
        }

        /// <summary>
        /// Fill buffer.
        /// </summary>
        /// <param name="v0">Position of 1 corner.</param>
        /// <param name="v1">Position of 2 corner.</param>
        /// <param name="v2">Position of 3 corner.</param>
        /// <param name="v3">Position of 4 corner.</param>
        /// <param name="uv0">Sprite UV of 1 corner.</param>
        /// <param name="uv1">Sprite UV of 2 corner.</param>
        /// <param name="uv2">Sprite UV of 3 corner.</param>
        /// <param name="uv3">Sprite UV of 4 corner.</param>
        /// <param name="color">Color.</param>
        public static void FillBuffer (
            ref Vector3 v0, ref Vector3 v1, ref Vector3 v2, ref Vector3 v3,
            ref Vector2 uv0, ref Vector2 uv1, ref Vector2 uv2, ref Vector2 uv3, ref Color color) {
            if (_effect != GuiFontEffect.None) {
                FillBufferT ();

                _cacheC.Add (_effectColor);
                _cacheC.Add (_effectColor);
                _cacheC.Add (_effectColor);
                _cacheC.Add (_effectColor);

                _cacheUV.Add (uv0);
                _cacheUV.Add (uv1);
                _cacheUV.Add (uv2);
                _cacheUV.Add (uv3);
                switch (_effect) {
                case GuiFontEffect.Shadow:
                    _cacheV.Add (new Vector3 (v0.x + _effectValue.x, v0.y - _effectValue.y, v0.z + 0.01f));
                    _cacheV.Add (new Vector3 (v1.x + _effectValue.x, v1.y - _effectValue.y, v1.z + 0.01f));
                    _cacheV.Add (new Vector3 (v2.x + _effectValue.x, v2.y - _effectValue.y, v2.z + 0.01f));
                    _cacheV.Add (new Vector3 (v3.x + _effectValue.x, v3.y - _effectValue.y, v3.z + 0.01f));
                    break;
                case GuiFontEffect.Outline:
                    for (int i = 0; i < 3; i++) {
                        _cacheC.Add (_effectColor);
                        _cacheC.Add (_effectColor);
                        _cacheC.Add (_effectColor);
                        _cacheC.Add (_effectColor);
                        _cacheUV.Add (uv0);
                        _cacheUV.Add (uv1);
                        _cacheUV.Add (uv2);
                        _cacheUV.Add (uv3);
                    }

                    _cacheV.Add (new Vector3 (v0.x + _effectValue.x, v0.y, v0.z + 0.01f));
                    _cacheV.Add (new Vector3 (v1.x + _effectValue.x, v1.y, v1.z + 0.01f));
                    _cacheV.Add (new Vector3 (v2.x + _effectValue.x, v2.y, v2.z + 0.01f));
                    _cacheV.Add (new Vector3 (v3.x + _effectValue.x, v3.y, v3.z + 0.01f));

                    FillBufferT ();
                    _cacheV.Add (new Vector3 (v0.x - _effectValue.x, v0.y, v0.z + 0.01f));
                    _cacheV.Add (new Vector3 (v1.x - _effectValue.x, v1.y, v1.z + 0.01f));
                    _cacheV.Add (new Vector3 (v2.x - _effectValue.x, v2.y, v2.z + 0.01f));
                    _cacheV.Add (new Vector3 (v3.x - _effectValue.x, v3.y, v3.z + 0.01f));

                    FillBufferT ();
                    _cacheV.Add (new Vector3 (v0.x, v0.y + _effectValue.y, v0.z + 0.01f));
                    _cacheV.Add (new Vector3 (v1.x, v1.y + _effectValue.y, v1.z + 0.01f));
                    _cacheV.Add (new Vector3 (v2.x, v2.y + _effectValue.y, v2.z + 0.01f));
                    _cacheV.Add (new Vector3 (v3.x, v3.y + _effectValue.y, v3.z + 0.01f));

                    FillBufferT ();
                    _cacheV.Add (new Vector3 (v0.x, v0.y - _effectValue.y, v0.z + 0.01f));
                    _cacheV.Add (new Vector3 (v1.x, v1.y - _effectValue.y, v1.z + 0.01f));
                    _cacheV.Add (new Vector3 (v2.x, v2.y - _effectValue.y, v2.z + 0.01f));
                    _cacheV.Add (new Vector3 (v3.x, v3.y - _effectValue.y, v3.z + 0.01f));
                    break;
                }
            }

            FillBufferT ();

            _cacheC.Add (color);
            _cacheC.Add (color);
            _cacheC.Add (color);
            _cacheC.Add (color);

            _cacheV.Add (v0);
            _cacheV.Add (v1);
            _cacheV.Add (v2);
            _cacheV.Add (v3);

            _cacheUV.Add (uv0);
            _cacheUV.Add (uv1);
            _cacheUV.Add (uv2);
            _cacheUV.Add (uv3);
        }

        /// <summary>
        /// Put mesh buffer data to mesh.
        /// </summary>
        /// <param name="mesh">Mesh.</param>
        /// <param name="recalculateBounds">Are mesh bounds should be recalculated.</param>
        public static void GetBuffers (Mesh mesh, bool recalculateBounds = true) {
            if (mesh != null) {
                mesh.Clear (true);
                if (_cacheV.Count <= 65535) {
                    mesh.SetVertices (_cacheV);
                    mesh.SetUVs (0, _cacheUV);
                    mesh.SetColors (_cacheC);
                    mesh.SetTriangles (_cacheT, 0);
                } else {
                    Debug.LogWarning ("Too many vertices", mesh);
                }
                if (recalculateBounds) {
                    mesh.RecalculateBounds ();
                }
            }
        }

        /// <summary>
        /// Fill simple sprite.
        /// </summary>
        /// <param name="mesh">Mesh.</param>
        /// <param name="width">Sprite width.</param>
        /// <param name="height">Sprite height.</param>
        /// <param name="color">Sprite color.</param>
        /// <param name="spriteData">Sprite data.</param>
        /// <param name="effect">Effect.</param>
        /// <param name="effectValue">Effect value.</param>
        /// <param name="effectColor">Effect color.</param>
        public static void FillSimpleSprite (Mesh mesh, int width, int height, Color color, GuiSpriteData spriteData,
                                             GuiFontEffect effect = GuiFontEffect.None, Vector2? effectValue = null,
                                             Color? effectColor = null) {
            if (mesh == null) {
                return;
            }
            PrepareBuffer (effect, effectValue, effectColor);
            if (spriteData != null) {
                var halfW = 0.5f * width;
                var halfH = 0.5f * height;
                _vRect.Set (-halfW, -halfH, width, height);
                _uvRect.Set (spriteData.CornerX, spriteData.CornerY, spriteData.CornerW, spriteData.CornerH);
                FillBuffer (ref _vRect, ref _uvRect, ref color);
            }
            GetBuffers (mesh);
        }

        /// <summary>
        /// Fill sliced / tiled sprite.
        /// </summary>
        /// <param name="mesh">Mesh.</param>
        /// <param name="width">Sprite width.</param>
        /// <param name="height">Sprite height.</param>
        /// <param name="color">Sprite color.</param>
        /// <param name="sd">Sprite data.</param>
        /// <param name="texSize">Tex size.</param>
        /// <param name="isHorTiled">Is sprite tiled horizontally.</param>
        /// <param name="isVerTiled">Is sprite tiled vertically.</param>
        /// <param name="fillCenter">Is sprite center should be filled.</param>
        public static void FillSlicedTiledSprite (
            Mesh mesh, int width, int height, Color color, GuiSpriteData sd,
            Vector2 texSize, bool isHorTiled, bool isVerTiled, bool fillCenter) {
            if (mesh == null) {
                return;
            }
            PrepareBuffer ();
            if (sd != null && width != 0 && height != 0) {
                var leftBorderV = (int) (sd.BorderL * texSize.x);
                var rightBorderV = (int) (sd.BorderR * texSize.x);
                var topBorderV = (int) (sd.BorderT * texSize.y);
                var bottomBorderV = (int) (sd.BorderB * texSize.y);

                if ((leftBorderV + rightBorderV) <= width && (bottomBorderV + topBorderV) <= height) {
                    var cW = sd.CenterWidth;
                    var cH = sd.CenterHeight;
                    var bR = sd.CornerX + sd.CornerW - sd.BorderR;
                    var bT = sd.CornerY + sd.CornerH - sd.BorderT;

                    var halfW = 0.5f * width;
                    var halfH = 0.5f * height;

                    if (width < 0) {
                        leftBorderV = -leftBorderV;
                        rightBorderV = -rightBorderV;
                    }

                    if (height < 0) {
                        topBorderV = -topBorderV;
                        bottomBorderV = -bottomBorderV;
                    }

                    int centerWidthV;
                    int horTileCount;

                    int centerHeightV;
                    int verTileCount;

                    if (isHorTiled) {
                        centerWidthV = (int) (cW * texSize.x);
                        if (width < 0) {
                            centerWidthV = -centerWidthV;
                        }
                        horTileCount = Mathf.Max (0, Mathf.FloorToInt ((width - leftBorderV - rightBorderV) / (float) centerWidthV));
                    } else {
                        centerWidthV = width - rightBorderV - leftBorderV;
                        horTileCount = 1;
                    }

                    if (isVerTiled) {
                        centerHeightV = (int) (cH * texSize.y);
                        if (height < 0) {
                            centerHeightV = -centerHeightV;
                        }
                        verTileCount = Mathf.Max (0, Mathf.FloorToInt ((height - bottomBorderV - topBorderV) / (float) centerHeightV));
                    } else {
                        centerHeightV = height - topBorderV - bottomBorderV;
                        verTileCount = 1;
                    }

                    // top-bottom sides
                    if (sd.BorderT > 0 || sd.BorderB > 0) {
                        _uvRect.Set (sd.CornerX + sd.BorderL, bT, cW, sd.BorderT);
                        _uvRect2.Set (sd.CornerX + sd.BorderL, sd.CornerY, cW, sd.BorderB);
                        _vRect.Set (-halfW + leftBorderV, halfH - topBorderV, centerWidthV, topBorderV);
                        _vRect2.Set (-halfW + leftBorderV, -halfH, centerWidthV, bottomBorderV);
                        for (var i = 0; i < horTileCount; i++) {
                            FillBuffer (ref _vRect, ref _uvRect, ref color);
                            FillBuffer (ref _vRect2, ref _uvRect2, ref color);
                            _vRect.x += centerWidthV;
                            _vRect2.x += centerWidthV;
                        }
                    }

                    // left-right sides
                    if (sd.BorderL > 0 || sd.BorderR > 0) {
                        _uvRect.Set (sd.CornerX, sd.CornerY + sd.BorderB, sd.BorderL, cH);
                        _uvRect2.Set (bR, sd.CornerY + sd.BorderB, sd.BorderR, cH);
                        _vRect.Set (-halfW, -halfH + bottomBorderV, leftBorderV, centerHeightV);
                        _vRect2.Set (halfW - rightBorderV, -halfH + bottomBorderV, rightBorderV, centerHeightV);
                        for (var i = 0; i < verTileCount; i++) {
                            FillBuffer (ref _vRect, ref _uvRect, ref color);
                            FillBuffer (ref _vRect2, ref _uvRect2, ref color);
                            _vRect.y += centerHeightV;
                            _vRect2.y += centerHeightV;
                        }
                    }

                    // center
                    if (fillCenter) {
                        _uvRect.Set (sd.CornerX + sd.BorderL, sd.CornerY + sd.BorderB, cW, cH);
                        _vRect.Set (0, -halfH + bottomBorderV, centerWidthV, centerHeightV);
                        for (var y = 0; y < verTileCount; y++) {
                            _vRect.x = -halfW + leftBorderV;
                            for (var x = 0; x < horTileCount; x++) {
                                FillBuffer (ref _vRect, ref _uvRect, ref color);
                                _vRect.x += centerWidthV;
                            }
                            _vRect.y += centerHeightV;
                        }
                    }

                    // left-top corner
                    if (sd.BorderL > 0 && sd.BorderT > 0) {
                        _vRect.Set (-halfW, halfH - topBorderV, leftBorderV, topBorderV);
                        _uvRect.Set (sd.CornerX, bT, sd.BorderL, sd.BorderT);
                        FillBuffer (ref _vRect, ref _uvRect, ref color);
                    }

                    // right-top corner
                    if (sd.BorderR > 0 && sd.BorderT > 0) {
                        _vRect.Set (halfW - rightBorderV, halfH - topBorderV, rightBorderV, topBorderV);
                        _uvRect.Set (bR, bT, sd.BorderR, sd.BorderT);
                        FillBuffer (ref _vRect, ref _uvRect, ref color);
                    }

                    // right-bottom corner
                    if (sd.BorderR > 0 && sd.BorderB > 0) {
                        _vRect.Set (halfW - rightBorderV, -halfH, rightBorderV, bottomBorderV);
                        _uvRect.Set (bR, sd.CornerY, sd.BorderR, sd.BorderB);
                        FillBuffer (ref _vRect, ref _uvRect, ref color);
                    }

                    // left-bottom corner
                    if (sd.BorderL > 0 && sd.BorderB > 0) {
                        _vRect.Set (-halfW, -halfH, leftBorderV, bottomBorderV);
                        _uvRect.Set (sd.CornerX, sd.CornerY, sd.BorderL, sd.BorderB);
                        FillBuffer (ref _vRect, ref _uvRect, ref color);
                    }
                }
            }
            GetBuffers (mesh);
        }

        /// <summary>
        /// Fill round-filled sprite.
        /// </summary>
        /// <param name="mesh">Mesh.</param>
        /// <param name="width">Sprite width.</param>
        /// <param name="height">Sprite height.</param>
        /// <param name="fillValue">Normalized progress of round-filling.</param>
        /// <param name="color">Sprite color.</param>
        /// <param name="spriteData">Sprite data.</param>
        /// <param name="effect">Effect.</param>
        /// <param name="effectValue">Effect value.</param>
        /// <param name="effectColor">Effect color.</param>
        public static void FillRoundFilledSprite (
            Mesh mesh, int width, int height, float fillValue, Color color, GuiSpriteData spriteData,
            GuiFontEffect effect = GuiFontEffect.None, Vector2? effectValue = null,
            Color? effectColor = null) {
            if (mesh == null) {
                return;
            }
            PrepareBuffer (effect, effectValue, effectColor);
            if (spriteData != null && fillValue > 0f) {
                var halfW = 0.5f * width;
                var halfH = 0.5f * height;
                _vRect.Set (-halfW, -halfH, width, height);
                _uvRect.Set (spriteData.CornerX, spriteData.CornerY, spriteData.CornerW, spriteData.CornerH);

                var uvHalfW = spriteData.CornerW * 0.5f;
                var uvHalfH = spriteData.CornerH * 0.5f;

                int i;
                for (i = 0; i < 4; i++) {
                    _uvRect.Set (
                        RoundFilledUV[i].x * uvHalfW + spriteData.CornerX,
                        RoundFilledUV[i].y * uvHalfH + spriteData.CornerY,
                        uvHalfW, uvHalfH);
                    _vRect.Set (RoundFilledV[i].x * halfW, RoundFilledV[i].y * halfH, halfW, halfH);
                    if (fillValue < (i + 1) * 0.25f) {
                        break;
                    }
                    FillBuffer (ref _vRect, ref _uvRect, ref color);
                }

                if (i < 4) {
                    fillValue = (fillValue - i * 0.25f) * 8f;
                    var needFlip = (i & 1) == 0;
                    var uv0 = new Vector2 (_uvRect.xMin, _uvRect.yMin);
                    var uv1 = new Vector2 (_uvRect.xMin, _uvRect.yMax);
                    var uv2 = new Vector2 (_uvRect.xMax, _uvRect.yMax);
                    var uv3 = new Vector2 (_uvRect.xMax, _uvRect.yMin);
                    var v0 = new Vector3 (_vRect.xMin, _vRect.yMin, 0f);
                    var v1 = new Vector3 (_vRect.xMin, _vRect.yMax, 0f);
                    var v2 = new Vector3 (_vRect.xMax, _vRect.yMax, 0f);
                    var v3 = new Vector3 (_vRect.xMax, _vRect.yMin, 0f);
                    if (needFlip) {
                        var t = uv0.y;
                        uv0.y = uv1.y;
                        uv1.y = t;
                        t = uv2.y;
                        uv2.y = uv3.y;
                        uv3.y = t;

                        t = v0.y;
                        v0.y = v1.y;
                        v1.y = t;
                        t = v2.y;
                        v2.y = v3.y;
                        v3.y = t;
                    }

                    var fvW = _vRect.width * fillValue;
                    var fvH = _vRect.height * fillValue;
                    var fuvW = _uvRect.width * fillValue;
                    var fuvH = _uvRect.height * fillValue;

                    if (fillValue >= 1f) {
                        // more than 45 degrees.
                        fvW -= _vRect.width;
                        fvH -= _vRect.height;
                        fuvW -= _uvRect.width;
                        fuvH -= _uvRect.height;
                        switch (i) {
                        case 0:
                            v1.y = _vRect.yMax - fvH;
                            uv1.y = _uvRect.yMax - fuvH;
                            break;
                        case 1:
                            v3.x = _vRect.xMin + fvW;
                            uv3.x = _uvRect.xMin + fuvW;
                            break;
                        case 2:
                            v3.y = _vRect.yMin + fvH;
                            uv3.y = _uvRect.yMin + fuvH;
                            break;
                        case 3:
                            v1.x = _vRect.xMax - fvW;
                            uv1.x = _uvRect.xMax - fuvW;
                            break;
                        }
                    } else {
                        // less than 45 degrees
                        switch (i) {
                        case 0:
                            v0.x = _vRect.xMax - fvW;
                            uv0.x = _uvRect.xMax - fuvW;
                            uv1 = uv0;
                            v1 = v0;
                            break;
                        case 1:
                            v0.y = _vRect.yMax - fvH;
                            uv0.y = _uvRect.yMax - fuvH;
                            uv3 = uv0;
                            v3 = v0;
                            break;
                        case 2:
                            v2.x = _vRect.xMin + fvW;
                            uv2.x = _uvRect.xMin + fuvW;
                            uv3 = uv2;
                            v3 = v2;
                            break;
                        case 3:
                            v2.y = _vRect.yMin + fvH;
                            uv2.y = _uvRect.yMin + fuvH;
                            uv1 = uv2;
                            v1 = v2;
                            break;
                        }
                    }

                    FillBuffer (ref v0, ref v1, ref v2, ref v3, ref uv0, ref uv1, ref uv2, ref uv3, ref color);
                }
            }
            GetBuffers (mesh);
        }
    }
}