using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace KFrame.Editor
{
    /// <summary>
    /// 测试拖拽窗口
    /// </summary>
    public class TestDragWindow: EditorWindow
    {
        private Vector2 scrollPosition = Vector2.zero;
        private float zoomScale = 1f;

        /// <summary>
        /// �ڵ��б�
        /// </summary>
        private List<GUINode> nodes;
        /// <summary>
        /// ��ǰѡ����б�
        /// </summary>
        private GUINode curNode;
        /// <summary>
        /// �Ƿ�������ק�ڵ�
        /// </summary>
        private bool isDragging = false;
        /// <summary>
        /// ��������
        /// </summary>
        private int scrollDir = 1;
        /// <summary>
        /// �Ƿ��ڹ�������
        /// </summary>
        private bool isScrolling = false;
        /// <summary>
        /// ��ק��ʼλ��
        /// </summary>
        private Vector2 dragStartPosition;

        [MenuItem("测试/拖拽测试窗口")]
        public static void ShowWindow()
        {
            TestDragWindow window = GetWindow<TestDragWindow>();
            window.titleContent = new GUIContent("拖拽测试窗口");
        }
        private void Init()
        {
            nodes = new List<GUINode>();
            nodes.Add(new GUINode(new Rect(100, 20, 100, 50), new GUIContent("�ڵ�1"), GUI.skin.box));
            nodes.Add(new GUINode(new Rect(20, 100, 100, 50), new GUIContent("�ڵ�2"), GUI.skin.box));
            nodes.Add(new GUINode(new Rect(120, 100, 100, 50), new GUIContent("�ڵ�3"), GUI.skin.box));
            nodes.Add(new GUINode(new Rect(220, 20, 100, 50), new GUIContent("�ڵ�4"), GUI.skin.box));
            nodes[1].SetParent(nodes[0]);
            nodes[2].SetParent(nodes[0]);

        }


        private void OnGUI()
        {
            if(nodes == null)
            {
                Init();
            }

            //������
            DrawGrid(20, 0.2f, Color.gray, true);

            //���ڵ�
            DrawNodes();

            //�ռ������������¼�
            CollectInputEvents();
            HandleInputEvents();
        }

        /// <summary>
        /// ������
        /// </summary>
        /// <param name="gridSize">����ߴ�</param>
        /// <param name="gridOpacity">͸����</param>
        /// <param name="gridColor">��ɫ</param>
        /// <param name="withOuter">��û����Ȧ���</param>
        private void DrawGrid(float gridSize, float gridOpacity, Color gridColor, bool withOuter)
        {
            gridSize *= zoomScale;
            int widthDivs = Mathf.CeilToInt(position.width / gridSize);
            int heightDivs = Mathf.CeilToInt(position.height / gridSize);

            Handles.BeginGUI();
            Handles.color = new Color(gridColor.r, gridColor.g, gridColor.b, gridOpacity);

            scrollPosition = GUILayout.BeginScrollView(scrollPosition);

            for (int x = 0; x <= widthDivs; x++)
            {
                Handles.DrawLine(new Vector3(gridSize * x, 0, 0), new Vector3(gridSize * x, position.height, 0));
            }

            for (int y = 0; y <= heightDivs; y++)
            {
                Handles.DrawLine(new Vector3(0, gridSize * y, 0), new Vector3(position.width, gridSize * y, 0));
            }

            if (withOuter)
            {
                Handles.color = new Color(gridColor.r, gridColor.g, gridColor.b, gridOpacity * 2f);
                float gridSize2 = gridSize * 10f;
                int widthDivs2 = widthDivs / 10;
                int heightDivs2 = heightDivs / 10;

                for (int x = 0; x <= widthDivs2; x++)
                {
                    Handles.DrawLine(new Vector3(gridSize2 * x, 0, 0), new Vector3(gridSize2 * x, position.height, 0));
                }

                for (int y = 0; y <= heightDivs2; y++)
                {
                    Handles.DrawLine(new Vector3(0, gridSize2 * y, 0), new Vector3(position.width, gridSize2 * y, 0));
                }
            }


            Handles.color = Color.white;
            GUILayout.EndScrollView();
            Handles.EndGUI();
        }
        /// <summary>
        /// ���ƽڵ�
        /// </summary>
        private void DrawNodes()
        {
            //��������ÿ���ڵ�
            foreach (var node in nodes)
            {
                DrawNode(node);
            }
        }
        /// <summary>
        /// ���ƽڵ�
        /// </summary>
        private void DrawNode(GUINode node)
        {
            GUI.Box(node.NodeRect, node.GUIContent, node.GUIStyle);

            if(node.Children.Count >0)
            {
                Handles.BeginGUI();

                Handles.color = Color.white;

                foreach (var child in node.Children)
                {
                    Handles.DrawLine(node.RectBottom, child.RectTop);
                }

                Handles.EndGUI();
            }
        }
        /// <summary>
        /// �ռ��û������¼�������������ק��
        /// </summary>
        private void CollectInputEvents()
        {
            Event currentEvent = Event.current;

            if (curNode == null)
            {
                foreach(var node in nodes)
                {
                    if(node.NodeRect.Contains(currentEvent.mousePosition))
                    {
                        curNode = node;
                        node.GUIStyle.fontStyle = FontStyle.Bold;
                        Repaint();
                        break;
                    }
                }

            }
            else
            {
                if(curNode.NodeRect.Contains(currentEvent.mousePosition) == false)
                {
                    curNode.GUIStyle.fontStyle = FontStyle.Normal;
                    curNode = null;
                    Repaint();
                }
            }

            switch (currentEvent.type)
            {
                case EventType.MouseDown:
                    if (currentEvent.button == 0 && curNode != null)
                    {
                        isDragging = true;
                        dragStartPosition = currentEvent.mousePosition - curNode.NodeRect.position;
                        currentEvent.Use(); // �����¼�������Unity EditorĬ����Ϊ����
                    }
                    break;
                case EventType.MouseUp:
                    isDragging = false;
                    break;
                case EventType.ScrollWheel:
                    if (Event.current.delta.y < -1f)
                    {
                        isScrolling = true;
                        scrollDir = 1;
                    }
                    else if (Event.current.delta.y > 1f)
                    {
                        isScrolling = true;
                        scrollDir = -1;
                    }
                    else
                    {
                        isScrolling = false;
                    }
                    break;
                default:
                    isScrolling = false;
                    break;
            }
        }
        /// <summary>
        ///  �����û������¼�������������ק��
        /// </summary>
        private void HandleInputEvents()
        {
            bool repaint = false;

            if(isScrolling)
            {
                zoomScale += scrollDir * 0.02f * Time.unscaledDeltaTime;
                zoomScale = Mathf.Clamp(zoomScale, 0.1f, 10f);
                repaint = true;
            }

            // ���������ק�ڵ㣬���½ڵ�λ��
            if (isDragging)
            {
                curNode.NodeRect.position = Event.current.mousePosition - dragStartPosition;
                repaint = true;
            }

            //�����Ҫ���»��ƣ��Ǿ��ػ�
            if (repaint)
            {
                Repaint();
            }
        }
    }
}


