using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace GunfireRebornMods
{
    public class SceneDebugger : ModBase
    {
        Rect HierarchyWindow;
        Vector2 HierarchyScrollPos;
        String SearchText = "";
        Vector2 PropertiesScrollPos;
        Transform SelectedGameObject;
        List<String> ExpandedObjs = new List<String>();

        Rect ProjectWindow;
        Vector2 ProjectScrollPos;
        ConcurrentDictionary<object, Boolean> ExpandedObjects = new ConcurrentDictionary<object, Boolean>();

        public override void OnEnable()
        {
            ModName = "Scene Debugger";
            HasConfig = false;

            RootObjects = new List<GameObject>();
        }
        public override void OnGUI()
        {
            var area = new Rect(525, 25, 250, 800);
            GUI.Box(area, "scene debugger");
            GUILayout.BeginArea(area);
            GUILayout.Space(12);
            HierarchyWindowMethod(0);
            GUILayout.EndArea();
           // HierarchyWindow = GUILayout.Window(HierarchyWindowId, HierarchyWindow, (GUI.WindowFunction)HierarchyWindowMethod, "Hierarchy", new GUILayoutOption[0]);
            //ProjectWindow = GUILayout.Window(ProjectWindowId, ProjectWindow, (GUI.WindowFunction)ProjectWindowMethod, "Project", new GUILayoutOption[0]);
        }
        #region Hierarchy GUI
        void DisplayGameObject(GameObject gameObj, Int32 level)
        {
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            {
                GUILayout.Space(level * 20);
                var color = GUI.color;
                if (SelectedGameObject == gameObj.transform)
                    GUI.color = Color.green;
                if (!gameObj.activeSelf && gameObj.transform.childCount == 0)
                    GUI.color = Color.magenta;
                else if (gameObj.transform.childCount == 0)
                    GUI.color = Color.yellow;
                else if (!gameObj.activeSelf)
                    GUI.color = Color.red;
                if (GUILayout.Toggle(ExpandedObjs.Contains(gameObj.name), gameObj.name, new GUILayoutOption[1] { GUILayout.ExpandWidth(false) }))
                {
                    if (!ExpandedObjs.Contains(gameObj.name))
                    {
                        ExpandedObjs.Add(gameObj.name);
                        SelectedGameObject = gameObj.transform;
                    }
                }
                else
                {
                    if (ExpandedObjs.Contains(gameObj.name))
                    {
                        ExpandedObjs.Remove(gameObj.name);
                        SelectedGameObject = gameObj.transform;
                    }
                }
                GUI.color = color;
            }
            GUILayout.EndHorizontal();
            if (ExpandedObjs.Contains(gameObj.name))
                for (var i = 0; i < gameObj.transform.childCount; ++i)
                    DisplayGameObject(gameObj.transform.GetChild(i).gameObject, level + 1);
        }
        List<GameObject> RootObjects = new List<GameObject>();
        void HierarchyWindowMethod(Int32 id)
        {
            GUILayout.BeginVertical(GUIContent.none, GUI.skin.box, new GUILayoutOption[0]);// { GUI.skin.box });
            {
                GUILayout.BeginHorizontal(new GUILayoutOption[0]);
                {
                    SearchText = GUILayout.TextField(SearchText, 100, new GUILayoutOption[1] { GUILayout.ExpandWidth(true) });
                    if (GUILayout.Button("Search", new GUILayoutOption[1] { GUILayout.ExpandWidth(false) }))
                    { }
                }
                GUILayout.EndHorizontal();
                if (RootObjects.Count == 0)
                {
                    foreach (Transform xform in GameObject.FindObjectsOfType<Transform>())
                        if (xform.parent == null && !xform.name.Contains("(Clone)"))
                            RootObjects.Add(xform.gameObject);
                }
                //var rootObjects = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
                if (SelectedGameObject == null)
                    SelectedGameObject = RootObjects.First().transform;
                HierarchyScrollPos = GUILayout.BeginScrollView(HierarchyScrollPos, new GUILayoutOption[2] { GUILayout.Height(HierarchyWindow.height / 3), GUILayout.ExpandWidth(true) });
                {
                    foreach (var rootObject in RootObjects)
                        DisplayGameObject(rootObject, 0);
                }
                GUILayout.EndScrollView();
            }
            GUILayout.EndVertical();
            GUILayout.BeginVertical(GUIContent.none, GUI.skin.box, new GUILayoutOption[0]);//GUI.skin.box);
            {
                PropertiesScrollPos = GUILayout.BeginScrollView(PropertiesScrollPos, new GUILayoutOption[0]);// GUI.skin.box);
                {
                    var fullName = SelectedGameObject.name;
                    var parentTransform = SelectedGameObject.parent;
                    while (parentTransform != null)
                    {
                        fullName = parentTransform.name + "/" + fullName;
                        parentTransform = parentTransform.parent;
                    }
                    GUILayout.Label(fullName, new GUILayoutOption[0]);
                    GUILayout.BeginHorizontal(new GUILayoutOption[0]);
                    {
                        GUILayout.Label(SelectedGameObject.gameObject.layer + " : " + LayerMask.LayerToName(SelectedGameObject.gameObject.layer), new GUILayoutOption[0]);
                        GUILayout.FlexibleSpace();
                        SelectedGameObject.gameObject.SetActive(GUILayout.Toggle(SelectedGameObject.gameObject.activeSelf, "Active", new GUILayoutOption[1] { GUILayout.ExpandWidth(false) }));
                        if (GUILayout.Button("?", new GUILayoutOption[0]))
                            Console.WriteLine("?");
                        if (GUILayout.Button("X", new GUILayoutOption[0]))
                            GameObject.Destroy(SelectedGameObject.gameObject);
                    }
                    GUILayout.EndHorizontal();
                    foreach (var component in SelectedGameObject.GetComponents<Component>())
                    {
                        GUILayout.BeginHorizontal(GUIContent.none, GUI.skin.box, new GUILayoutOption[0]);// GUI.skin.box);
                        {

                            if (component is Behaviour)
                                (component as Behaviour).enabled = GUILayout.Toggle((component as Behaviour).enabled, "", new GUILayoutOption[1] { GUILayout.ExpandWidth(false) });

                            GUILayout.Label(component.GetIl2CppType().Name + " : " + component.GetIl2CppType().Namespace, new GUILayoutOption[0]);
                            GUILayout.FlexibleSpace();
                            if (GUILayout.Button("?", new GUILayoutOption[0]))
                                Console.WriteLine("?");
                            if (!(component is Transform))
                                if (GUILayout.Button("X", new GUILayoutOption[0]))
                                    GameObject.Destroy(component);
                        }
                        GUILayout.EndHorizontal();
                    }
                }
                GUILayout.EndScrollView();
            }
            GUILayout.EndVertical();
            //GUI.DragWindow();
        }
        #endregion
        #region Project GUI
        void ProjectWindowMethod(Int32 id)
        {
            GUILayout.BeginVertical(GUIContent.none, GUI.skin.box, new GUILayoutOption[0]);// GUI.skin.box);
            {
                ProjectScrollPos = GUILayout.BeginScrollView(ProjectScrollPos, new GUILayoutOption[2] { GUILayout.Height(ProjectWindow.height / 3), GUILayout.ExpandWidth(true) });
                {
                    var assemblies = AppDomain.CurrentDomain.GetAssemblies();
                    foreach (var assembly in assemblies)
                    {
                        ExpandedObjects[assembly] = GUILayout.Toggle(ExpandedObjects.ContainsKey(assembly) ? ExpandedObjects[assembly] : false, assembly.GetName().Name, new GUILayoutOption[1] { GUILayout.ExpandWidth(false) });
                        if (ExpandedObjects[assembly])
                        {
                            var types = assembly.GetTypes().Where(t => t.IsClass && !t.IsAbstract && !t.ContainsGenericParameters).ToList();
                            foreach (var type in types)
                            {
                                var staticfields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.FlattenHierarchy).Count(f => f.Name != "OffsetOfInstanceIDInCPlusPlusObject");
                                if (staticfields == 0)
                                    continue;
                                GUILayout.BeginHorizontal(new GUILayoutOption[0]);
                                {
                                    var color = GUI.color;
                                    GUILayout.Space(20);
                                    ExpandedObjects[type] = GUILayout.Toggle(ExpandedObjects.ContainsKey(type) ? ExpandedObjects[type] : false, type.Name, new GUILayoutOption[1] { GUILayout.ExpandWidth(false) });
                                    GUI.color = color;
                                }
                                GUILayout.EndHorizontal();
                                if (ExpandedObjects[type])
                                {
                                    var fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.FlattenHierarchy);
                                    foreach (var field in fields)
                                    {
                                        if (field.Name == "OffsetOfInstanceIDInCPlusPlusObject") continue;
                                        //var val = field.GetValue(null);
                                        GUILayout.BeginHorizontal(new GUILayoutOption[0]);
                                        {
                                            GUILayout.Space(40);
                                            ExpandedObjects[field] = GUILayout.Toggle(ExpandedObjects.ContainsKey(field) ? ExpandedObjects[field] : false, field.Name + " : " + field.FieldType, new GUILayoutOption[1] { GUILayout.ExpandWidth(false) });
                                        }
                                        GUILayout.EndHorizontal();
                                    }
                                }
                            }
                        }
                    }
                }
                GUILayout.EndScrollView();
            }
            GUILayout.EndVertical();
            //GUI.DragWindow();
        }
        #endregion
    }
}