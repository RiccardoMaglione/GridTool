using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering;

namespace RiccardoMaglioneEditor
{

    public class GridWindow : EditorWindow
    {
        #region Variables
        #region Variables Tile
        public List<Object> Tile = new List<Object>();                      //Lista di oggetti che contiene le tile
        public int ID;                                                      //Id della tile
        public List<int> XPos = new List<int>();                            //Posizione X della tile
        public List<int> YPos = new List<int>();                            //Posizione Y della tile
        public List<int> ZPos = new List<int>();                            //Posizione Z della tile

        public int TotalTile;                                               //Numero totale delle tile

        public List<GameObject> TileInstantiate = new List<GameObject>();   //Lista delle tile istanziate

        Vector2 ScrollPos;

        public bool IsFreeConstriction;     //Booleano per attivare la modalità Free Constriction
        public bool TestCube;               //Booleano per l'esecuzione automatica
        public int Width;                   //Larghezza della griglia
        public int Height;                  //Altezza della griglia
        public int TempTotalTile;           //Totale temporaneo delle tile

        public int J;                       //Variabile di conteggio
        public int K;                       //Variabile di conteggio
        public Object TestCubeObject;       //Oggetto per il prefab della tile
        public bool OnlyOnce;               //Booleano per esecuzione unica
        #endregion

        #region Variables Combine Mesh
        public GameObject GridParent;       //Variabile per il parent per la combine mesh
        #endregion

        #region Variables Perlin Noise
        public bool PerlinNoiseBool;        //Booleano per attivare la perlin noise
        public float PerlinNoise = 0f;      //Valore di perlin noise
        public float Refinement = 0f;       //Valore di raffinatezza
        public float Multiplier = 0f;       //Valore moltiplicativo
        #endregion
        #endregion
    
        [MenuItem("Grid/GridEditor")]
        public static void ShowWindow()
        {
            GetWindow<GridWindow>("Grid Editor");
        }
    
        private void OnGUI()
        {
            ScrollPos = EditorGUILayout.BeginScrollView(ScrollPos);
    
            #region GUIStyle - Bold and Middle Center   //Creazione di uno stile
            GUIStyle CenterBoldStyle;
            CenterBoldStyle = EditorStyles.boldLabel;
            CenterBoldStyle.alignment = TextAnchor.MiddleCenter;
            #endregion
    
            #region Title Window                        //Region che contiene il titolo con una label
            GUILayout.Space(20);
            GUILayout.Label("This is a Grid Editor - Create a grid with specify coordinate and prefab", CenterBoldStyle);
            GUILayout.Space(20);
            #endregion
    
            #region Grid Parent                         //Setto il parent per istanziare le tile come figli
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Grid parent for instantiate cube");
            GridParent = (GameObject)EditorGUILayout.ObjectField(GridParent, typeof(GameObject), true);
            GUILayout.EndHorizontal();
            #endregion
    
            #region Instantiate Automatically Grid      //Region in cui istanzio con coordinate automatiche le tile
            GUILayout.BeginHorizontal();
            TestCube = EditorGUILayout.ToggleLeft("Instantiate automatically gameobject", TestCube);                                                            
            if(TestCube == true)                                                                                                    
            {
                IsFreeConstriction = false;
                TestCubeObject = EditorGUILayout.ObjectField(TestCubeObject, typeof(GameObject), true);
                OnlyOnce = true;
            }
            else
            {
                if(OnlyOnce == true)
                {
                    for (int i = 0; i < TotalTile; i++)
                    {
                       Tile[i] = EditorGUILayout.ObjectField(null, typeof(GameObject), true);
                    }
                    OnlyOnce = false;
                }
            }
            GUILayout.EndHorizontal();

            #region Perlin Noise                        //Setto la perlin noise con cui andrò a modificare le coordinate delle tile
            GUILayout.BeginHorizontal();
            if (TestCube == true)
            {
                PerlinNoiseBool = EditorGUILayout.ToggleLeft("Add perlin noise to grid", PerlinNoiseBool);
                if (PerlinNoiseBool == true && TestCube == true)
                {
                    GUILayout.BeginVertical();
                    GUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Perlin Noise Value");
                    PerlinNoise = EditorGUILayout.FloatField(PerlinNoise);
                    GUILayout.EndHorizontal();
                    
                    GUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Refinement");
                    Refinement = EditorGUILayout.FloatField(Refinement);
                    GUILayout.EndHorizontal();
                    
                    GUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Multiplier");
                    Multiplier = EditorGUILayout.FloatField(Multiplier);
                    GUILayout.EndHorizontal();
                    GUILayout.EndVertical();
                }
            }
            else
            {
                PerlinNoiseBool = false;
            }
            GUILayout.EndHorizontal();
            #endregion
            #endregion
    
            #region Free Constriction                   //Region in cui istanzio le tile con coordinate settate automaticamente
            IsFreeConstriction = EditorGUILayout.ToggleLeft("Is free Constriction? True = Free; False = Widht and Height",IsFreeConstriction);
            if (IsFreeConstriction == true)
            {
                TestCube = false;
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
            #endregion
    
            #region Management of field in list         //Aggiunta e rimozione delle tile e delle coordinate dalla lista
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
    
            #region Constuction Inspector               //Region in cui costruisco l'inspector in base alla scelta dei toggle iniziali
            #region GUI Title's Table           //Crezione di uno stile e settaggio delle label per la costruzione dell'inspector
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
                if (TestCube == false)                                                              
                {                                                                                   
                    Tile[i] = EditorGUILayout.ObjectField(Tile[i], typeof(GameObject), true);       
                }                                                                                   
                else                                                                                
                {                                                                                   
                    Tile[i] = EditorGUILayout.ObjectField(TestCubeObject, typeof(GameObject), true);
                }                                                                                   
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
            #endregion
    
            #region Generate Grid                       //Region in cui genero la griglia
            if (GUILayout.Button("Generate Grid"))
            {
                if(TestCubeObject != null)
                {
                    for (int i = 0; i < TileInstantiate.Count; i++)
                    {
                        DestroyImmediate(TileInstantiate[i]);
                    }
                }

                for (int i = 0; i < TotalTile; i++)
                {
                    if (TileInstantiate[i] != null)
                    {
                        DestroyImmediate(TileInstantiate[i]);
                    }
                }
                for (int i = 0; i < TotalTile; i++)
                {
                    if(Tile[i] != null && PerlinNoiseBool == false)         //Genero la griglia senza il perlin noise
                    {
                        TileInstantiate[i] = (GameObject)Instantiate(Tile[i], new Vector3(XPos[i], YPos[i], ZPos[i]), Quaternion.identity);
                        TileInstantiate[i].transform.parent = GridParent.transform;
                    }
                    if (Tile[i] != null && PerlinNoiseBool == true)         //Genero la griglia con il perlin noise
                    {
                        PerlinNoise = Mathf.PerlinNoise(XPos[i] * Refinement, ZPos[i] * Refinement);
                        TileInstantiate[i] = (GameObject)Instantiate(Tile[i], new Vector3(XPos[i], YPos[i], ZPos[i]), Quaternion.identity);
                        TileInstantiate[i].transform.position = new Vector3(XPos[i], PerlinNoise * Multiplier, ZPos[i]);
                        TileInstantiate[i].transform.parent = GridParent.transform;
                    }
                }

                #region Combine Mesh                    //Combino la mesh per ottimizzare
                MeshFilter[] meshFilters = GridParent.GetComponentsInChildren<MeshFilter>();
                CombineInstance[] combine = new CombineInstance[meshFilters.Length];

                int k = 0;
                while (k < meshFilters.Length)
                {
                    combine[k].mesh = meshFilters[k].sharedMesh;
                    combine[k].transform = meshFilters[k].transform.localToWorldMatrix;
                    meshFilters[k].gameObject.SetActive(false);

                    k++;
                }
                GridParent.transform.GetComponent<MeshFilter>().mesh = new Mesh();
                GridParent.transform.GetComponent<MeshFilter>().sharedMesh.indexFormat = IndexFormat.UInt32;
                GridParent.transform.GetComponent<MeshFilter>().sharedMesh.CombineMeshes(combine);
                GridParent.transform.gameObject.SetActive(true);
                #endregion
            }
            #endregion
    
            #region Reset Grid                          //Resetto la griglia cancellandola, per non lasciare possibili tile in scena
            if (GUILayout.Button("Reset Grid"))
            {
                if(TestCubeObject != null)
                {
                    for (int i = 0; i < TileInstantiate.Count; i++)
                    {
                        DestroyImmediate(TileInstantiate[i]);
                    }
                }
                for (int i = 0; i < TotalTile; i++)
                {
                    if(TileInstantiate[i] != null)
                    {
                        DestroyImmediate(TileInstantiate[i]);
                    }                                     
                }
                GridParent.transform.GetComponent<MeshFilter>().mesh = new Mesh();

                foreach (GameObject Child in GridParent.transform)
                {
                    Destroy(Child);
                }
            }
            #endregion
            
            EditorGUILayout.EndScrollView();
        }
    }
}
