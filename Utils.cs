using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace STRAFTAT_CC
{
    public class Utils
    {
        private static GUIStyle _style = null;
        private static Vector3[] _boxExtents = new Vector3[8];
        private static Vector3 _boxExtent = new Vector3();
        private static Material _drawMaterial = null;
        public static Material GetDrawMat()
        {
            if (_drawMaterial != null)
                return _drawMaterial;

            _drawMaterial = new Material(Shader.Find("Hidden/Internal-Colored"))
            {
                hideFlags = (HideFlags)61
            };
            _drawMaterial.SetInt("_SrcBlend", 5);
            _drawMaterial.SetInt("_DstBlend", 10);
            _drawMaterial.SetInt("_Cull", 0);
            _drawMaterial.SetInt("_ZWrite", 0);

            return _drawMaterial;
        }

        public static void DrawText(Vector2 position, string text, Color color, int fontSize, bool center = false)
        {
            if (_style == null)
            {
                _style = new GUIStyle();
                _style.fontSize = 14;
            }

            Vector2 drawPos = position;
            if (center)
            {
                Vector2 textSize = _style.CalcSize(new GUIContent(text));
                drawPos.x -= textSize.x / 2;
                drawPos.y -= textSize.y / 2;
            }

            _style.fontSize = fontSize;
            _style.normal.textColor = Color.black;
            
            // Outlined text effect
            GUI.Label(new Rect(drawPos.x - 1, drawPos.y - 1, 1000, 1000), text, _style);
            GUI.Label(new Rect(drawPos.x + 1, drawPos.y + 1, 1000, 1000), text, _style);
            GUI.Label(new Rect(drawPos.x - 1, drawPos.y + 1, 1000, 1000), text, _style);
            GUI.Label(new Rect(drawPos.x + 1, drawPos.y - 1, 1000, 1000), text, _style);
            
            _style.normal.textColor = color;
            GUI.Label(new Rect(drawPos.x, drawPos.y, 1000, 1000), text, _style);
            _style.normal.textColor = color;
            GUI.Label(new Rect(drawPos.x, drawPos.y, 1000, 1000), text, _style);
        }

        public static bool IsOnScreen(Vector2 screenPos)
        {
            return screenPos.x > 0 && screenPos.x < Screen.width && screenPos.y > 0 && screenPos.y < Screen.height;
        }

        public static void SetupExtentsBounds(Bounds b)
        {
            _boxExtent.x = b.center.x + b.size.x / 2f;
            _boxExtent.y = b.center.y + b.size.y / 2f;
            _boxExtent.z = b.center.z + b.size.z / 2f;
            _boxExtents[0] = (_boxExtent);

            _boxExtent.x = b.center.x + b.size.x / 2f;
            _boxExtent.y = b.center.y + b.size.y / 2f;
            _boxExtent.z = b.center.z - b.size.z / 2f;
            _boxExtents[1] = (_boxExtent);

            _boxExtent.x = b.center.x + b.size.x / 2f;
            _boxExtent.y = b.center.y - b.size.y / 2f;
            _boxExtent.z = b.center.z + b.size.z / 2f;
            _boxExtents[2] = (_boxExtent);

            _boxExtent.x = b.center.x + b.size.x / 2f;
            _boxExtent.y = b.center.y - b.size.y / 2f;
            _boxExtent.z = b.center.z - b.size.z / 2f;
            _boxExtents[3] = (_boxExtent);

            _boxExtent.x = b.center.x - b.size.x / 2f;
            _boxExtent.y = b.center.y + b.size.y / 2f;
            _boxExtent.z = b.center.z + b.size.z / 2f;
            _boxExtents[4] = (_boxExtent);

            _boxExtent.x = b.center.x - b.size.x / 2f;
            _boxExtent.y = b.center.y + b.size.y / 2f;
            _boxExtent.z = b.center.z - b.size.z / 2f;
            _boxExtents[5] = (_boxExtent);

            _boxExtent.x = b.center.x - b.size.x / 2f;
            _boxExtent.y = b.center.y - b.size.y / 2f;
            _boxExtent.z = b.center.z + b.size.z / 2f;
            _boxExtents[6] = (_boxExtent);

            _boxExtent.x = b.center.x - b.size.x / 2f;
            _boxExtent.y = b.center.y - b.size.y / 2f;
            _boxExtent.z = b.center.z - b.size.z / 2f;
            _boxExtents[7] = (_boxExtent);
        }

        public static void Draw3DBox(Camera camera, Color color)
        {
            GL.PushMatrix();
            GL.LoadProjectionMatrix(camera.projectionMatrix);
            GL.modelview = camera.worldToCameraMatrix;
            GL.Begin(1);
            GetDrawMat().SetPass(0);
            GL.Color(color);
            GL.Vertex(_boxExtents[0]);
            GL.Vertex(_boxExtents[1]);
            GL.Vertex(_boxExtents[1]);
            GL.Vertex(_boxExtents[5]);
            GL.Vertex(_boxExtents[5]);
            GL.Vertex(_boxExtents[4]);
            GL.Vertex(_boxExtents[4]);
            GL.Vertex(_boxExtents[0]);
            GL.Vertex(_boxExtents[2]);
            GL.Vertex(_boxExtents[3]);
            GL.Vertex(_boxExtents[3]);
            GL.Vertex(_boxExtents[7]);
            GL.Vertex(_boxExtents[7]);
            GL.Vertex(_boxExtents[6]);
            GL.Vertex(_boxExtents[6]);
            GL.Vertex(_boxExtents[2]);
            GL.Vertex(_boxExtents[2]);
            GL.Vertex(_boxExtents[0]);
            GL.Vertex(_boxExtents[3]);
            GL.Vertex(_boxExtents[1]);
            GL.Vertex(_boxExtents[7]);
            GL.Vertex(_boxExtents[5]);
            GL.Vertex(_boxExtents[6]);
            GL.Vertex(_boxExtents[4]);

            GL.End();
            GL.PopMatrix();
        }

        /*
        public static void Draw2DBox(Rect rect, Color color)
        {
            Material mat = GetDrawMat();
            mat.SetPass(0);

            GL.PushMatrix();
            GL.LoadPixelMatrix(0, Screen.width, Screen.height, 0);

            GL.Begin(GL.LINES);
            GL.Color(color);
            Vector3 bottomLeft = new Vector3(rect.xMin, rect.yMax, 0);
            Vector3 bottomRight = new Vector3(rect.xMax, rect.yMax, 0);
            Vector3 topRight = new Vector3(rect.xMax, rect.yMin, 0);
            Vector3 topLeft = new Vector3(rect.xMin, rect.yMin, 0);
            GL.Vertex(bottomLeft);
            GL.Vertex(bottomRight);

            GL.Vertex(bottomRight);
            GL.Vertex(topRight);

            GL.Vertex(topRight);
            GL.Vertex(topLeft);

            GL.Vertex(topLeft);
            GL.Vertex(bottomLeft);

            GL.End();
            GL.PopMatrix();
        }
        */



        public static Color DoubleColorLerp(float percent, Color full, Color middle, Color empty)
        {
            if (percent < 0.5f)
                return Color.Lerp(empty, middle, percent * 2f);
            return Color.Lerp(middle, full, (percent - 0.5f) * 2f);
        }
        public static Transform RecursiveFind(Transform parent, string name)
        {
            foreach (Transform child in parent)
            {
                if (child.name == name)
                    return child;

                Transform found = RecursiveFind(child, name);
                if (found != null)
                    return found;
            }

            return null;
        }
    }

    public static class Drawing
    {
        public static void DrawLine(Vector2 pointA, Vector2 pointB, float width, Color color)
        {
            if (Event.current.type != EventType.Repaint) return;

            GL.PushMatrix();
            Utils.GetDrawMat().SetPass(0);
            GL.LoadPixelMatrix();
            GL.Begin(GL.QUADS);
            GL.Color(color);

            Vector2 dir = (pointB - pointA).normalized;
            Vector2 normal = new Vector2(-dir.y, dir.x) * (width * 0.5f);

            GL.Vertex3(pointA.x - normal.x, pointA.y - normal.y, 0);
            GL.Vertex3(pointA.x + normal.x, pointA.y + normal.y, 0);
            GL.Vertex3(pointB.x + normal.x, pointB.y + normal.y, 0);
            GL.Vertex3(pointB.x - normal.x, pointB.y - normal.y, 0);

            GL.End();
            GL.PopMatrix();
        }

        public static void DrawBox(Rect rect, float thickness, Color color)
        {
            if (Event.current.type != EventType.Repaint) return;
            
            // Draw as 4 lines (reusing DrawLine is inefficient due to multiple GL calls, better to batch here)
            // But for simplicity and thickness support, we can just use Quads directly
            
            float t = thickness;
            
            GL.PushMatrix();
            Utils.GetDrawMat().SetPass(0);
            GL.LoadPixelMatrix();
            GL.Begin(GL.QUADS);
            GL.Color(color);

            // Top
            GL.Vertex3(rect.x, rect.y, 0);
            GL.Vertex3(rect.x + rect.width, rect.y, 0);
            GL.Vertex3(rect.x + rect.width, rect.y + t, 0);
            GL.Vertex3(rect.x, rect.y + t, 0);

            // Bottom
            GL.Vertex3(rect.x, rect.y + rect.height - t, 0);
            GL.Vertex3(rect.x + rect.width, rect.y + rect.height - t, 0);
            GL.Vertex3(rect.x + rect.width, rect.y + rect.height, 0);
            GL.Vertex3(rect.x, rect.y + rect.height, 0);

            // Left
            GL.Vertex3(rect.x, rect.y, 0);
            GL.Vertex3(rect.x + t, rect.y, 0);
            GL.Vertex3(rect.x + t, rect.y + rect.height, 0);
            GL.Vertex3(rect.x, rect.y + rect.height, 0);

            // Right
            GL.Vertex3(rect.x + rect.width - t, rect.y, 0);
            GL.Vertex3(rect.x + rect.width, rect.y, 0);
            GL.Vertex3(rect.x + rect.width, rect.y + rect.height, 0);
            GL.Vertex3(rect.x + rect.width - t, rect.y + rect.height, 0);

            GL.End();
            GL.PopMatrix();
        }

        public static void DrawCornerBox(Rect rect, float thickness, Color color)
        {
            if (Event.current.type != EventType.Repaint) return;

            float t = thickness;
            float cornerSize = rect.width / 4f;

            GL.PushMatrix();
            Utils.GetDrawMat().SetPass(0);
            GL.LoadPixelMatrix();
            GL.Begin(GL.QUADS);
            GL.Color(color);

            void DrawQuad(float x, float y, float w, float h)
            {
                GL.Vertex3(x, y, 0);
                GL.Vertex3(x + w, y, 0);
                GL.Vertex3(x + w, y + h, 0);
                GL.Vertex3(x, y + h, 0);
            }

            // Top Left
            DrawQuad(rect.x, rect.y, cornerSize, t);
            DrawQuad(rect.x, rect.y, t, cornerSize);

            // Top Right
            DrawQuad(rect.x + rect.width - cornerSize, rect.y, cornerSize, t);
            DrawQuad(rect.x + rect.width - t, rect.y, t, cornerSize);

            // Bottom Left
            DrawQuad(rect.x, rect.y + rect.height - t, cornerSize, t);
            DrawQuad(rect.x, rect.y + rect.height - cornerSize, t, cornerSize);

            // Bottom Right
            DrawQuad(rect.x + rect.width - cornerSize, rect.y + rect.height - t, cornerSize, t);
            DrawQuad(rect.x + rect.width - t, rect.y + rect.height - cornerSize, t, cornerSize);

            GL.End();
            GL.PopMatrix();
        }

        public static void DrawFilledBox(Rect rect, Color color)
        {
             if (Event.current.type != EventType.Repaint) return;

            GL.PushMatrix();
            Utils.GetDrawMat().SetPass(0);
            GL.LoadPixelMatrix();
            GL.Begin(GL.QUADS);
            GL.Color(color);

            GL.Vertex3(rect.x, rect.y, 0);
            GL.Vertex3(rect.x + rect.width, rect.y, 0);
            GL.Vertex3(rect.x + rect.width, rect.y + rect.height, 0);
            GL.Vertex3(rect.x, rect.y + rect.height, 0);

            GL.End();
            GL.PopMatrix();
        }
    }
}
