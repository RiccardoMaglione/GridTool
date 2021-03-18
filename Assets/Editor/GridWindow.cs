﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class GridWindow : EditorWindow
{
    #region Variables
    public List<Object> Tile = new List<Object>();
    public int ID;
    public List<int> XPos = new List<int>();
    public List<int> YPos = new List<int>();
    public List<int> ZPos = new List<int>();

    public int TotalTile;

    public List<GameObject> TileInstantiate = new List<GameObject>();

    Vector2 ScrollPos;
    #endregion

    public bool IsFreeConstriction;
    public bool TestCube;
    public int Width;
    public int Height;
    public int TempTotalTile;

    public int J;
    public int K;
    public Object TestCubeObject;
    public bool OnlyOnce;
    //Forma della griglia
    //Quadrato = Lato
    //Rettangolo = lunghezza e larghezza -> se quadrato vuol dire che lunghezza = larghezza = lato
    //Cerchio
    //Libero

    [MenuItem("Grid/GridEditor")]
    public static void ShowWindow()
    {
        GetWindow<GridWindow>("Grid Editor");
    }

    private void OnGUI()
    {
        ScrollPos = EditorGUILayout.BeginScrollView(ScrollPos);




        #region GUIStyle - Bold and Middle Center
        GUIStyle CenterBoldStyle;
        CenterBoldStyle = EditorStyles.boldLabel;
        CenterBoldStyle.alignment = TextAnchor.MiddleCenter;
        #endregion

        #region Title Window
        GUILayout.Space(20);
        GUILayout.Label("This is a Grid Editor - Create a grid with specify coordinate and prefab", CenterBoldStyle);
        GUILayout.Space(20);
        #endregion

        /*Test Cube*/GUILayout.BeginHorizontal();
        /*Test Cube*/TestCube = EditorGUILayout.ToggleLeft("Instantiate automatically gameobject", TestCube);                                                            
        /*Test Cube*/if(TestCube == true)                                                                                                    
        /*Test Cube*/{                                                                                                                       
        /*Test Cube*/   TestCubeObject = EditorGUILayout.ObjectField(TestCubeObject, typeof(GameObject), true);
        /*Test Cube*/   OnlyOnce = true;
        /*Test Cube*/}
        /*Test Cube*/else
        /*Test Cube*/{
        /*Test Cube*/   if(OnlyOnce == true)
        /*Test Cube*/   {
        /*Test Cube*/       for (int i = 0; i < TotalTile; i++)
        /*Test Cube*/       {
        /*Test Cube*/          Tile[i] = EditorGUILayout.ObjectField(null, typeof(GameObject), true);
        /*Test Cube*/       }
        /*Test Cube*/       OnlyOnce = false;
        /*Test Cube*/   }
        /*Test Cube*/}
        /*Test Cube*/GUILayout.EndHorizontal();
        IsFreeConstriction = EditorGUILayout.ToggleLeft("Is free Constriction? True = Free; False = Widht and Height",IsFreeConstriction);
        if (IsFreeConstriction == true)
        {
            TotalTile = TempTotalTile;
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Number of total tile");
            TotalTile = EditorGUILayout.IntField(TotalTile);
            TempTotalTile = TotalTile;
            GUILayout.EndHorizontal();
        }
        else
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Width: ");
            Width = EditorGUILayout.IntField(Width);
            EditorGUILayout.LabelField("Height: ");
            Height = EditorGUILayout.IntField(Height);
            TotalTile = Width * Height;
            J = 0;
            K = 0;
            GUILayout.EndHorizontal();
        }

        #region Management of field in list
        for (int i = 0; i < TotalTile; i++)
        {
            if (Tile.Count < TotalTile)
            {
                Tile.Add(null);
                XPos.Add(0);
                YPos.Add(0);
                ZPos.Add(0);
                TileInstantiate.Add(null);
            }
            else if(Tile.Count > TotalTile)
            {
                Tile.RemoveAt(Tile.Count - 1);
                XPos.RemoveAt(XPos.Count - 1);
                YPos.RemoveAt(YPos.Count - 1);
                ZPos.RemoveAt(YPos.Count - 1);
                TileInstantiate.RemoveAt(TileInstantiate.Count - 1);
            }
        }
        #endregion

        #region GUI Title's Table
        GUILayout.Space(10);
        GUILayout.BeginHorizontal();

        #region GUIStyle - Label
        GUIStyle StyleLabel;
        StyleLabel = GUI.skin.label;
        #endregion

        #region GUI Title not enabled and with normal color
        GUI.enabled = false;
        GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, GUI.color.a * 2f);
        EditorGUILayout.TextField("Preview", StyleLabel, GUILayout.Width(64));
        EditorGUILayout.TextField("ID (Int)", StyleLabel);
        EditorGUILayout.TextField("Tile (GameObject)", StyleLabel);
        EditorGUILayout.TextField("X Position (Int)", StyleLabel);
        if(IsFreeConstriction == true)
        {
            EditorGUILayout.TextField("Y Position (Int)", StyleLabel);
        }
        EditorGUILayout.TextField("Z Position (Int)", StyleLabel);
        GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, GUI.color.a / 2f);
        GUI.enabled = true;
        #endregion

        GUILayout.EndHorizontal();
        GUILayout.Space(10);
        #endregion

        for (int i = 0; i < TotalTile; i++)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(AssetPreview.GetAssetPreview(Tile[i]), GUILayout.Width(64), GUILayout.Height(64));
            ID = EditorGUILayout.IntField(i, StyleLabel);
            /*Not Test Cube*///Tile[i] = EditorGUILayout.ObjectField(Tile[i], typeof(GameObject), true);
            /*Test Cube*/if (TestCube == false)                                                              
            /*Test Cube*/{                                                                                   
            /*Test Cube*/    Tile[i] = EditorGUILayout.ObjectField(Tile[i], typeof(GameObject), true);       
            /*Test Cube*/}                                                                                   
            /*Test Cube*/else                                                                                
            /*Test Cube*/{                                                                                   
            /*Test Cube*/    Tile[i] = EditorGUILayout.ObjectField(TestCubeObject, typeof(GameObject), true);
            /*Test Cube*/}                                                                                   
            if (IsFreeConstriction == true)
            {
                XPos[i] = EditorGUILayout.IntField(XPos[i]);
                YPos[i] = EditorGUILayout.IntField(YPos[i]);
                ZPos[i] = EditorGUILayout.IntField(ZPos[i]);
            }
            else
            {
                XPos[i] = EditorGUILayout.IntField(J, StyleLabel);
                ZPos[i] = EditorGUILayout.IntField(K, StyleLabel);
            }
            #region Layout Grid
            if (Width > Height)
            {
                if (J < Width)
                {
                    J++;
                    if(J >= Width)
                    {
                        J = 0;
                        if(K < Height)
                        {
                            K++;
                            if(K >= Height)
                            {
                                K = 0;
                            }
                        }
                    }
                }
            }
            else
            {
                if (K < Height)
                {
                    K++;
                    if (K >= Height)
                    {
                        K = 0;
                        if (J < Width)
                        {
                            J++;
                            if (J >= Width)
                            {
                                J = 0;
                            }
                        }
                    }
                }
            }
            #endregion
            GUILayout.EndHorizontal();
        }

        if (GUILayout.Button("Generate Grid"))
        {
            /*TestCube*/if(TestCubeObject != null)
            /*TestCube*/{
            /*TestCube*/    for (int i = 0; i < TileInstantiate.Count; i++)
            /*TestCube*/    {
            /*TestCube*/        DestroyImmediate(TileInstantiate[i]);
            /*TestCube*/    }
            /*TestCube*/}

            for (int i = 0; i < TotalTile; i++)
            {
                if (TileInstantiate[i] != null)
                {
                    DestroyImmediate(TileInstantiate[i]);
                }
            }
            for (int i = 0; i < TotalTile; i++)
            {
                if(Tile[i] != null)
                {
                    TileInstantiate[i] = (GameObject)Instantiate(Tile[i], new Vector3(XPos[i], YPos[i], ZPos[i]), Quaternion.identity);
                }
            }
        }
        if (GUILayout.Button("Reset Grid"))
        {
            /*TestCube*/if(TestCubeObject != null)
            /*TestCube*/{
            /*TestCube*/    for (int i = 0; i < TileInstantiate.Count; i++)
            /*TestCube*/    {
            /*TestCube*/        DestroyImmediate(TileInstantiate[i]);
            /*TestCube*/    }
            /*TestCube*/}
            for (int i = 0; i < TotalTile; i++)
            {
                if(TileInstantiate[i] != null)
                {
                    DestroyImmediate(TileInstantiate[i]);
                }                                     
            }
        }

        EditorGUILayout.EndScrollView();
    }
}
