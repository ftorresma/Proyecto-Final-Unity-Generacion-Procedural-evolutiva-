using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using Random = UnityEngine.Random;
using UnityEngine.UI;

public class GeneracionMapa : MonoBehaviour
{
    [Range(0,100)]

    public int PorcLlenado;             // Porcentaje del mapa que se llenada de salas
    public GameObject Objeto;           // Prefab usado para las paredes (mapa)
    public GameObject Llegada;          // Prefab de bloques de llegada (mapa)
    public GameObject Coleccionable;    // Prefab de coleccionable (mapa)
    public GameObject Plane;            // Prefab de bloques del piso (mapa)
    public GameObject Player;           // Objeto Jugador
    public int width;                   // ancho del mapa
    public int height;                  // alto del mapa
    public string seed;                 // palabra semilla - opcional
    public bool UsarSeed;               // si se usara la seed o no
    public int MinRoom;                 // minimo tamaño de las salas
    public int MinReg;                  // minimo de regiones entre salas
    public int PassageRadius;           // radio de pasajes entre salas
    public Text completado;             // texto que mostrara que parte del mapa se ha explorado
    public Slider Opinion;              // Slider del cuestionario final
    public Slider Difficult;            // Slider del cuestionario final
    public GameObject FinalPanel;       // Panel del cuestionario final

    public GameObject SBoss;            // Objeto Jugador
    public GameObject SEvent;           // Objeto Jugador
    public GameObject SFight;           // Objeto Jugador
    public GameObject SFinal;           // Objeto Jugador
    public GameObject SKey;             // Objeto Jugador
    public GameObject SLock;            // Objeto Jugador
    public GameObject SStart;           // Objeto Jugador
    public GameObject STreasure;        // Objeto Jugador
    //GameObject container;

    string obj = "container";           // nombre de Contenedor de tdos los bloques cuando se genera el mapa
    string pre_obj;                     // guarda el nombre anterior para poder borrar los mapas cada vez que se cree uno nuevo
    int version = 1;                    // guarda la ersion del mapa creado

    int Dificultad = 1;                 // determina la dificultad del juego
    int PromedioSalas = 0;                 // determina la dificultad del juego
    int evol=2;                         // ?

    bool activaEsce = false;            // ?

    bool termino = false;               // si termino el mapa
    bool empezar = false;               // ?

    float TimeStato;                    // ?

    int exploration = 0;                // bloques explorados

    int posInix;                        //Posicion x inicial del player
    int posIniy;                        //Posicion inicial y del player


    int posFinx;                        //Posicion x final del player
    int posFiny;                        //Posicion final y del player

    int RealFinalx = 1000;              // ?
    int RealFinaly = 1000;              // ?

    Grupo_matrices[] MapsPull;          // Pull de matrices
    int Act_matrix = 0;                 //actual matriz en Gizmos
    int Max_matrix = 0;                 //Numero de matrices en la lista

    //matrix
    int[,] mapa;                        // mapa
    int[,] regiones;                    // regiones actuales
    int[] regionSize;                   // tamaños de regiones actuales
    int[] regionCategorie;              // categorias de regiones actuales
    List<Room> rooms;

    //Variables  de usuario
    int ActualRooms;                    // numero de salas actales
    Caracteristicas Caract;             // ?
    float UserTimeComplete=0;           // Tiempo en el que completo el juego
    int UserExploration = 0;            // Porcentaje del mapa explorado
    int UserScore;                      // Puntaje del personaje
    int UserOpinion;                    // Opinion del usuario
    bool UserClear=false;               // El usuario temino todo el jueg
    int racha = 0;                      // racha de mapas ganados

    GameObject borra;                   // ?
    GameObject container;               // ?
    MeshGenerator meshGen;              // ?



    void Start()

    {
        int maxIT = 0;
        bool cumplido = false;
        int repeticiones = 100;

        while (cumplido == false)
        {
            Grupo_matrices GP;
            GP = newMap(mapa, regiones, regionSize, regionCategorie);
            mapa = GP.mapa;
            regiones = GP.regiones;
            regionSize = GP.regionSize;
            regionCategorie = GP.regionCategorie;
            MapsPull = new Grupo_matrices[200];
            MapsPull[Max_matrix] = GP;
            Max_matrix++;
            MapsPull[0].rooms = detectRegions(MapsPull[0].mapa, MapsPull[0].regionSize, MapsPull[0].regiones, MapsPull[0].regionCategorie, height, width);
            GP.salas = ProcessMap(regiones,mapa, true);
            rooms = GP.salas;

            /*
            Max_matrix--;
            maxIT++;
            cumplido = false;
            if (maxIT == repeticiones)
            {
                Max_matrix++;
                cumplido = true;
            }
            Debug.Log(GP.salas.Count.ToString());
            PromedioSalas = PromedioSalas + GP.salas.Count;
            continue;
            */
            //PONER SALAS
            try
            {
                int NumInicios      = 1;
                int NumPeleas       = 2;
                int NumTesoros      = 1;
                int NumBosses       = 0;
                int NumPuertas      = 1;
                int NumPuertasEx    = 0;
                int NumPuertasBoss  = 0;

                int salasTotales = NumInicios + NumPeleas + NumTesoros + NumBosses + NumPuertas + NumPuertasEx + NumPuertasBoss;

                if (GP.salas.Count < salasTotales)
                {
                    Max_matrix--;
                    maxIT++;
                    cumplido = false;
                    repeticiones--;
                    continue;
                }
                if (SetRoomTYPES(NumInicios, NumPeleas, NumTesoros, NumBosses, NumPuertas, NumPuertasEx, NumPuertasBoss, GP.salas))
                {
                    Debug.Log("Creacion exitosa");
                    Debug.Log(Max_matrix.ToString());
                    cumplido = true;
                }
                else
                {
                    Max_matrix--;
                    maxIT++;
                    cumplido = false;
                    if (maxIT == repeticiones)
                    {
                        Max_matrix++;
                        cumplido = true;
                    }
                    Debug.Log("========El mapa "+maxIT+" no cumple con los parametros, Creando nuevo mapa======== ");
                    continue;
                }
            }
            catch(Exception e)
            {
                Max_matrix--;
                maxIT++;
                cumplido = false;
                if (maxIT == repeticiones)
                {
                    Max_matrix++;
                    cumplido = true;
                }
                Debug.Log("========Error interno en creacion, Creando nuevo mapa======== ");
                continue;
            }
            //PONER SIMBOLOS
            SetSymbols(GP.salas);

            SpawCubes(GP.salas);
            Detecct_start(GP.salas);
            start_timer();
            //printRegiones(mapa);
            printSalas(GP.salas);
        }
        Debug.Log("Promedio : " + (PromedioSalas/ 100).ToString());
    }
    void Update()
    {
        countingTime();
        if (Input.GetKeyUp("d"))
        {
            string tmp1 = "RegionSize:\n";
            
            for (int i=0;i< regionSize.Length;i++)
            {
                tmp1 = tmp1 + "pos " + i.ToString() + " -> " + regionSize[i].ToString() + "\n";
            }
            Debug.Log(tmp1);
            tmp1 = "regionCategorie:\n";
            for (int i = 0; i < regionCategorie.Length; i++)
            {
                tmp1 = tmp1 + "pos " + i.ToString() + " -> " + regionCategorie[i].ToString() + "\n";
            }
            Debug.Log(tmp1);

            EvaluarSalas(regionCategorie, regionSize);
        }
        if (Input.GetKeyUp("space"))
        {
            width = width+20;
            height = height+20;
            Grupo_matrices[] individuos = GenerarIndividuos(width, height ,0,5,MapsPull[0]);
            
            for(int i = 1; i < 5+1; i++)
            {
                
                MapsPull[i] = individuos[i - 1];
                Debug.Log(MapsPull[i].TYPE);

            }
            if (Max_matrix < 6)
            {
                Max_matrix = 6;
            }

        }
        if (Input.GetKeyUp("k"))
        {
            
            if (Act_matrix < Max_matrix-1)
            {
                Act_matrix++; 
            }
            Debug.Log(Act_matrix.ToString());
        }
        if (Input.GetKeyUp("j"))
        {
            
            if (Act_matrix > 0)
            {
                Act_matrix--;
            }
            Debug.Log(Act_matrix.ToString());
        }
        if (Input.GetKeyUp("q"))
        {
            MapsPull[0].swap();
        }
        if (Input.GetKeyUp("w"))
        {
            MapsPull[0].swap2();
        }
        if (Input.GetKeyUp("n"))
        {
            ShowSymbols(rooms);
        }
        if (Input.GetKeyUp("m"))
        {
            DelSymbols(rooms);
        }
    }
    void countingTime()
    {
        if (empezar)
        {
            UserTimeComplete = UserTimeComplete + Time.deltaTime;
        }
    }
    void start_timer()
    {
        empezar = true;
    }
    void End_time()
    {
        //float diffInSeconds = (Time.deltaTime - TimeStato);
        Debug.Log("Time -> "+UserTimeComplete.ToString());
        empezar = false;

        FinalPanel.SetActive(true);
        activaEsce = true;

    }
    public void ContinuarJugando()
    {
        UserOpinion = (int)Opinion.value;
        UserScore = (int)Difficult.value;

        FinalPanel.SetActive(false);
        activaEsce = false;

        int Accion = Evolucionar();
        //0 avanza
        //1 retrocede
        //2 mantiene
        if (Accion == 2 && UserClear)
        {
            racha++;
        }
        else if (Accion == 2 && !UserClear)
        {
            racha = 0;
        }
        else if (Accion == 0)
        {
            if(width!=140 && height != 140)
            {
                width = width + 20;
                height = height + 20;
            }
            else
            {
                Accion = 2;
            }
        }
        else if (Accion == 1)
        {
            if (width != 60 && height != 60)
            {
                width = width - 20;
                height = height - 20;
            }
            else
            {
                Accion = 2;
            }
        }
        Reiniciar(Accion);
    }
    void Reiniciar(int Accion)
    {
        int maxIT = 0;
        bool cumplido = false;
        int repeticiones = 100;
        Debug.Log("Entro al reinicio");
        while (cumplido == false)
        {
            Debug.Log("Bucle :" + maxIT.ToString());
            //REINICAR VARIABLES
            UserTimeComplete = 0;       // Tiempo en el que completo el juego
            UserExploration = 0;        // Porcentaje del mapa explorado
            UserClear = false;             // El usuario temino todo el jueg
                                           //GENERAR NUEVA PULL
            int pullN = 100;
            Grupo_matrices[] individuos = GenerarIndividuos(width, height, Accion, pullN, MapsPull[0]);
            for (int i = 1; i < pullN + 1; i++)
            {
                MapsPull[i] = individuos[i - 1];
                //Debug.Log(MapsPull[i].TYPE);
            }
            if (Max_matrix < pullN + 1)
            {
                Debug.Log("Maximo de matrices cambio a : "+ Max_matrix.ToString());
                Max_matrix = pullN + 1;
            }
            //ESTABLECER CARACT DE SALAS
            int NumInicios = 1;
            int NumPeleas = 1;
            int NumTesoros = 1;
            int NumBosses = 0;
            int NumPuertas = 1;
            int NumPuertasEx = 0;
            int NumPuertasBoss = 0;

            int salasTotales = NumInicios + NumPeleas + NumTesoros + NumBosses + NumPuertas + NumPuertasEx + NumPuertasBoss;
            //SELECCIONAR NUEVO SUJETO
            int Cand = 20;
            Grupo_matrices[] mejores = getMejorGroup(individuos, pullN, Accion,salasTotales);

            for(int newpull = 0; newpull < mejores.Length; newpull++)
            {
                MapsPull[newpull] = mejores[newpull];
            }
            Max_matrix = mejores.Length;
            //LLENAR VARIABLES
            //MapsPull[0].rooms = detectRegions(MapsPull[0].mapa, MapsPull[0].regionSize, MapsPull[0].regiones, MapsPull[0].regionCategorie, height, width);
            Debug.Log("Revisando candidatos");
            for (int ii = 0; ii < Cand; ii++)
            {

                mapa = mejores[ii].mapa;
                regiones = mejores[ii].regiones;
                regionSize = mejores[ii].regionSize;
                regionCategorie = mejores[ii].regionCategorie;
                //GENERAR OBJETOS
                mejores[ii].salas = ProcessMap(regiones, mapa, true);
                rooms = mejores[ii].salas;

                try
                {
                    
                    if (mejores[ii].salas.Count < salasTotales)
                    {
                        maxIT++;
                        cumplido = false;
                        repeticiones--;
                        continue;
                    }
                    if (SetRoomTYPES(NumInicios, NumPeleas, NumTesoros, NumBosses, NumPuertas, NumPuertasEx, NumPuertasBoss, mejores[ii].salas))
                    {

                        for (int ij = 0; ij < mejores.Length; ij++)
                        {
                            MapsPull[ij] = mejores[ij];
                        }

                        cumplido = true;
                    }
                    else
                    {
                        maxIT++;
                        cumplido = false;
                        if (maxIT == repeticiones)
                        {
                            cumplido = true;
                        }
                        Debug.Log("========El mapa " + maxIT + " no cumple con los parametros, Creando nuevo mapa======== ");
                        continue;
                    }
                }
                catch (Exception e)
                {
                    maxIT++;
                    cumplido = false;
                    if (maxIT == repeticiones)
                    {
                        cumplido = true;
                    }
                    Debug.Log("========Error interno en creacion, Creando nuevo mapa======== ");
                    continue;
                }
                //PONER SIMBOLOS
                SetSymbols(mejores[ii].salas);
                SpawCubes(mejores[ii].salas);
                Detecct_start(mejores[ii].salas);
                //printRegiones(mapa);
                termino = false;
                start_timer();
                break;
            }
            if (cumplido == true)
            {
                Debug.Log("Candidato encontrado");
            }
            else
            {
                Debug.Log("Candidato NO encontrado");
            }
        }
        
    }

    //------------------------------------------------------------Genera el mapa al principio-----------
    Grupo_matrices newMap(int[,] map,int[,] reg, int[] regSize, int[] regCat)
    {
        Grupo_matrices GP;
        GP = GenerarMapa(map,reg,regSize,regCat);
        for (int i = 0; i < 10; i++)
        {
            CambiarMapa(GP.mapa);
            GP.salas = ProcessMap(GP.regiones,GP.mapa, false);
        }
        CambiarMapa(GP.mapa);
        return GP;
    }
    Grupo_matrices GenerarMapa(int[,] map, int[,] reg, int[] regSize, int[] regCat)
    {
        Grupo_matrices GP = new Grupo_matrices(width,height,100);
        map = LlenarRandom(GP.mapa, GP.formaPrima, UsarSeed);
        //GP.formaPrima = map;
        //procesarMapa();
        GP.salas=ProcessMap(GP.regiones,GP.mapa);

        return GP;

    }

    //----------------------------------------------Llenar la matriz de manera psudo-aleatoria----------
    int[,] LlenarRandom(int[,] Mp, int[,] Fp, bool Useed = false)
    {
        if (!Useed)
        {
            seed = System.DateTime.Now.ToString();
            seed = seed + Random.Range(-100.0f, 100.0f).ToString() ;
            //Debug.Log("Esta es la semilla -> "+seed);
        }

        System.Random Prng = new System.Random(seed.GetHashCode());

        //Debug.Log(Prng.ToString());

        for(int x=0; x< width; x++)
        {
            for(int y=0;y< height; y++)
            {
                if(x==0 || x==width-1 || y==0 || y == height - 1)
                {
                    Mp[x, y] = 1;
                    Fp[x, y] = 1;
                }
                else
                {
                    Mp[x, y] = (Prng.Next(0, 100) < PorcLlenado) ? 1 : 0;
                    Fp[x, y] = Mp[x, y];
                }
                
            }
        }
        return Mp;
    }

    //--------------------------------------------------------Aplicar regla de Automata Celular---------
    void CambiarMapa(int[,] map)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int numVecinos = GetVecinos(x, y, map);
                if (numVecinos > 4)
                {
                    map[x, y] = 1;
                }
                else if(numVecinos < 4)
                {
                    map[x, y] = 0;
                }
                else
                {
                    //Debug.Log(x.ToString() + " " + y.ToString());
                }
            }
        }
    }

    //---------------------------------------------------------Obtener vecinos de uan posicion----------
    int GetVecinos(int x,int y, int[,] map)
    {
        int paredes = 0;
        for(int vecinox = x-1; vecinox <= x+1; vecinox++)
        {
            for(int vecinoy=y-1 ; vecinoy<=y+1; vecinoy++)
            {
                if (vecinox >= 0 && vecinox<width && vecinoy>=0 && vecinoy < height)
                {
                    if(vecinox!=x || vecinoy != y)
                    {
                        paredes += map[vecinox, vecinoy];
                    }
                }
                else
                {
                    paredes++;
                }
            }
        }
        return paredes;
    }
    int GetBordes(int x, int y, int[,] map)
    {
        int paredes = 0;
        for (int vecinox = x - 1; vecinox <= x + 1; vecinox++)
        {
            for (int vecinoy = y - 1; vecinoy <= y + 1; vecinoy++)
            {
                if (vecinox > 0 && vecinox < width-1 && vecinoy > 0 && vecinoy < height-1)
                {
                    if (vecinox != x || vecinoy != y)
                    {
                        paredes += map[vecinox, vecinoy];
                    }
                }
                else
                {
                    paredes++;
                }
            }
        }
        return paredes;
    }

    //--------------------------------------------------------------Generacion de txt-------------------
    void GenerateResult(string name, string content)
    {
        string path = Application.dataPath + "/" + name + ".txt";

        //if (!File.Exists(path){
        //    File.WriteAllText(path, content);
        //}
        File.AppendAllText(path, content);

    }

    //--------------------------------------------------------------------------------------------------
    //--------------------------------------------------------------Creacion de mapa como objeto--------
    //--------------------------------------------------------------------------------------------------
    int sameM(int[,] m1, int[,] m2)
    {
        int difs = 0;
        for(int x = 0; x < width; x++)
        {
            for (int y = 0; y < width; y++)
            {
                if (m1[x, y] != m2[x, y])
                {
                    difs++;
                }
            }
        }
        return difs;
    }
    bool Detecct_start(List<Room> rm)
    {
        foreach(Room r in rm)
        {
            if(r.TYPE=="SStart")
            {
                try
                {
                    posInix = r.tiles[r.tiles.Count/2].tileX;
                    posIniy = r.tiles[r.tiles.Count/2].tileY;
                    return true;
                }
                catch (Exception e)
                {
                    return false;
                }

            }        
        }
        return false;
    }
    public int[] arregla()
    {
        int[] pair = new int[2];
        pair[0] = 0;
        pair[1] = 0;
        foreach (Room r in rooms)
        {
            if (r.TYPE == "SStart")
            {
                try
                {
                    pair[0] = r.tiles[r.tiles.Count / 2].tileX;
                    pair[1] = r.tiles[r.tiles.Count / 2].tileY;
                    break;
                }
                catch (Exception e)
                {
                    continue;
                }

            }
        }
        return pair;
    }
    public int[] fin()
    {
        int[] pair = new int[2];
        pair[0] = RealFinalx;
        pair[1] = RealFinaly;
        return pair;
    }
    int maxList(int[] lsta,int tm,int ex)
    {
        int pos = 0;
        int max = 0;
        for(int i = 0; i < tm; i++)
        {
            if (lsta[i] > max && i!=ex)
            {
                pos = i;
                max = lsta[i];
            }
        }
        return pos;
    }

    //--------------------------------------------------------------------------------------------------
    //--------------------------------------------------------------Evolucion---------------------------
    //--------------------------------------------------------------------------------------------------
    // UserTimeComplete;       Tiempo en el que completo el juego
    // UserExploration;        Porcentaje del mapa explorado
    // UserScore;              Puntaje del personaje
    // UserOpinion;            Opinion del usuario
    // UserClear;              El usuario temino todo el jueg

    Grupo_matrices[] GenerarIndividuos(int Wd,int Hg, int evo, int indi,Grupo_matrices padre)
    {
        Grupo_matrices[] individuos = new Grupo_matrices[indi];
        int indAct = 0;
        Caract = new Caracteristicas(UserTimeComplete, UserExploration, UserScore, UserOpinion, UserClear, height, width, ActualRooms, regionCategorie);

        int[] spacemin = new int[indi];
        int[] roommax = new int[indi];

        for (int ind = 0; ind < indi; ind++)
        {
            int[,] mapaInd = new int[Wd, Hg];
            int[,] regionesInd = new int[Wd, Hg];
            int[] regionSizeInd = new int[100];
            int[] regionCategorieInd = new int[100];
            Grupo_matrices GPInd;
            GPInd = newMapEVO(mapaInd, regionesInd, regionSizeInd, regionCategorieInd, padre, evo);
            mapaInd = GPInd.mapa;
            regionesInd = GPInd.regiones;
            regionSizeInd = GPInd.regionSize;
            regionCategorieInd = GPInd.regionCategorie;
            int TRooms = detectRegions(mapaInd, regionSizeInd, regionesInd, regionCategorieInd, Hg, Wd);
            GPInd.rooms = TRooms;
            GPInd.TYPE = "E";
            //GPInd.swap2();
            individuos[indAct] = GPInd;


            roommax[indAct]=individuos[indAct].rooms;
            int spa = 0;
            for(int kk=0;kk< individuos[indAct].rooms; kk++)
            {
                spa = spa + individuos[indAct].regionSize[kk];
            }
            spacemin[indAct] = spa;

            indAct++;
        }

        //------------
        int ma1 = maxList(roommax, indi, 100);
        int ma2 = maxList(roommax, indi, ma1);
        int elMax;
        if (spacemin[ma1] > spacemin[ma2])
        {
            elMax = ma1;
        }
        else
        {
            elMax = ma2;
        }
        //------------

        int[,] matvirus = new int[width, height];
        for(int ix = 0; ix < width; ix++)
        {
            for (int iy = 0; iy < height; iy++)
            {
                matvirus[ix, iy] = 1;
            }
        }
        int pos;
        Grupo_matrices[] individuosF = new Grupo_matrices[indi];
        for (int h = 0; h < indi; h++)
        {
            
            pos = Random.Range(0, 2);
            if (pos == 1)
            {
                int[,] Madre = new int[width, height];
                Madre = individuos[h].mapa;
                individuosF[h] = mutacion(individuos[h].formaPrima, individuos[h].regiones, individuos[h].regionCategorie, individuos[h].rooms);
                individuosF[h].mama = Madre;
            }
            else
            {
            
                pos = Random.Range(0, 2);
                if (pos == 1)
                {
                    int[,] Madre = new int[width, height];
                    int[,] Padre = new int[width, height];
                    Madre = individuos[h].mapa;
                    Padre = individuos[elMax].mapa;
                    individuosF[h] = Cruzamiento2(individuos[h].formaPrima, individuos[elMax].formaPrima, individuos[h].regiones, individuos[elMax].regiones, individuos[h].regionCategorie, individuos[elMax].regionCategorie, individuos[h].rooms);
                    individuosF[h].mama = Madre;
                    individuosF[h].papa = Padre;
                }
                else
                {
                    pos = Random.Range(0, 2);
            
                    if (pos == 1)
                    {
                        int[,] Madre = new int[width, height];
                        int[,] Padre = new int[width, height];
                        Madre = individuos[h].mapa;
                        Padre = individuos[elMax].mapa;
                        individuosF[h] = Cruzamiento(individuos[h].formaPrima, individuos[elMax].formaPrima);
                        individuosF[h].mama = Madre;
                        individuosF[h].papa = Padre;
                    }
                    else
                    {
                        individuosF[h] = individuos[h];
                    }
                }
            }
        }
        
        return individuosF;
    }
    int getMejor(Grupo_matrices[] individuosF,int indi,int action)
    {
        int[] roommax = new int[indi];
        int[] spacemin = new int[indi];

        for (int jj = 0; jj < indi; jj++)
        {
            roommax[jj] = individuosF[jj].rooms;
            int spa = 0;
            for (int kk = 0; kk < individuosF[jj].rooms; kk++)
            {
                spa = spa + individuosF[jj].regionSize[kk];
            }
            spacemin[jj] = spa;
        }

        int ma1 = maxList(roommax, indi, 100);
        int ma2 = maxList(roommax, indi, ma1);
        int Maxim;
        int Maxim2;
        if (spacemin[ma1] > spacemin[ma2])
        {
            Maxim = ma1;
            Maxim2 = ma2;
        }
        else
        {
            Maxim = ma2;
            Maxim2 = ma1;
        }

        if (action == 1)
        {
            return Maxim2;
        }
        return Maxim;
    }
    bool evitar(List<int> lsti, int ev)
    {
        for(int i =0; i < lsti.Count; i++)
        {
            if (lsti[i] == ev)
            {
                return false;
            }
        }
        return true;
    }
    List<int> SortBySize(Grupo_matrices[] gru, int ord, int prom)
    {
        List<int> listaN = new List<int>();
        List<int> evitarL = new List<int>();
        int max = 0;
        int maxPos = 0;
        if(ord ==0 || ord == 1)
        {
            for (int i = 0; i < gru.Length; i++)
            {
                max = gru[i].rooms;
                maxPos = i;
                for (int j = i; j < gru.Length; j++)
                {
                    if (max < gru[j].rooms && evitar(evitarL,j))
                    {
                        max = gru[j].rooms;
                        maxPos = j;
                    }
                }
                listaN.Add(maxPos);
                evitarL.Add(maxPos);
            }
            if (ord == 1)
            {
                listaN.Reverse();
            }
        }
        else if (ord == 2)
        {
            List<int> tempo = new List<int>();
            for (int i = 0; i < gru.Length; i++)
            {
                max = width*height;
                maxPos = i;
                for (int j = i; j < gru.Length; j++)
                {
                    if (max > gru[j].rooms && gru[j].rooms >= prom && evitar(evitarL, j))
                    {
                        max = gru[j].rooms;
                        maxPos = j;
                    }
                }
                listaN.Add(maxPos);

                evitarL.Add(maxPos);
                if (gru[i].rooms < prom)
                {
                    tempo.Add(i);
                }
            }
            for(int k = 0;k < tempo.Count ;k++)
            {
                listaN.Add(tempo[k]);
            }
        }
        
        return listaN;
    }
    Grupo_matrices[] getMejorGroup(Grupo_matrices[] individuosF, int indi, int action,int cant)
    {
        List<int> pos = SortBySize(individuosF,action, cant);
        Grupo_matrices[] roommaxRET = new Grupo_matrices[individuosF.Length];
        for (int i = 0; i < individuosF.Length; i++)
        {
            roommaxRET[i] = individuosF[pos[i]];
        }
        return roommaxRET;
    }
    Grupo_matrices newMapEVO(int[,] map, int[,] reg, int[] regSize, int[] regCat, Grupo_matrices padre, int evo)

    {
        Grupo_matrices GP;
        GP = GenerarMapaEVO(map, reg, regSize, regCat, padre,evo);
        for (int i = 0; i < 10; i++)
        {
            CambiarMapa(GP.mapa);
            GP.salas=ProcessMap(GP.regiones,GP.mapa, false);
        }
        CambiarMapa(GP.mapa);
        return GP;
    }
    Grupo_matrices GenerarMapaEVO(int[,] map, int[,] reg, int[] regSize, int[] regCat, Grupo_matrices padre,int evo)
    {
        Grupo_matrices GP = new Grupo_matrices(width, height, 100);
        map = LlenarRandom(GP.mapa, GP.formaPrima);
        if (evo == 0)
        {
            GP.mapa = basadoEvo(GP.mapa, padre.formaPrima);
        }
        if (evo == 1)
        {
            GP.mapa = basadoDes(GP.mapa, padre.formaPrima);
        }
        if (evo == 2)
        {
            GP.mapa = basadoMan(GP.mapa, padre.formaPrima);
        }
        //GP.formaPrima = map;
        //procesarMapa();
        GP.salas=ProcessMap(GP.regiones,GP.mapa);

        return GP;
    }

    int[,] basadoEvo(int[,] Nueva, int[,] padre)
    {
        int dim = width-padre.GetLength(0);
        //Debug.Log(dim);
        //Debug.Log(width);

        int dimx = Random.Range(0, dim);
        int dimy = Random.Range(0, dim);

        for (int x = dimx;x < padre.GetLength(0)+dimx; x++)
        {
            for (int y = dimy; y < padre.GetLength(0)+dimy; y++)
            {
                //Debug.Log(Nueva[x,y].ToString());
                //Debug.Log(padre[x-dimx, y-dimy].ToString());
                Nueva[x, y] = padre[x-dimx, y-dimy];
                //Debug.Log(x.ToString() + " " + y.ToString());
            }
        }
        return Nueva;
    }
    int[,] basadoDes(int[,] Nueva, int[,] padre)
    {
        int dim = padre.GetLength(0)-width;

        if(width != 60)
        {
            int dimx = Random.Range(width-1, dim);
            int dimy = Random.Range(height-1, dim);

            for (int x = dimx; x < width; x++)
            {
                for (int y = dimy; y < height; y++)
                {
                    Nueva[x-dimx, y-dimy] = padre[x, y];
                }
            }
        }
        
        return Nueva;
    }
    int[,] basadoMan(int[,] Nueva, int[,] padre)
    {
        int dim = width - padre.GetLength(0);

        int dimx = Random.Range(0, width-1);
        int dimy = Random.Range(0, height-1);

        for (int x = dimx; x < width; x++)
        {
            for (int y = dimy; y < height; y++)
            {
                Nueva[x, y] = padre[x, y];
            }
        }
        return Nueva;
    }

    //------------------------------------------------------------------------------------CRUZAMIENTO-------------------------------
    //------------------------------------------------------------------------------------CRUZAMIENTO-------------------------------
    //------------------------------------------------------------------------------------CRUZAMIENTO-------------------------------
    Grupo_matrices Cruzamiento(int[,] M1, int[,] M2)
    {
        int[,] Cross = new int[width, height];
        for(int i = 0; i < width / 2; i++)
        {
            for (int j = 0; j < height; j++)
            {
                Cross[i, j] = M1[i, j];
            }
        }
        for (int i = width/2; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                Cross[i, j] = M2[i, j];
            }
        }

        Grupo_matrices GPI;
        GPI = new Grupo_matrices(width, height, 100);
        GPI.mapa = Cross;
        GPI.salas=ProcessMap(GPI.regiones,GPI.mapa);

        for (int i = 0; i < 10; i++)
        {
            CambiarMapa(GPI.mapa);
            GPI.salas=ProcessMap(GPI.regiones,GPI.mapa, false);
        }
        CambiarMapa(GPI.mapa);

        int Trooms = detectRegions(GPI.mapa, GPI.regionSize, GPI.regiones, GPI.regionCategorie, height, width);
        GPI.rooms = Trooms;
        GPI.TYPE = "C1";
        return GPI;
    }
    //------------------------------------------------------------------------------------CRUZAMIENTO 2-------------------------------
    //------------------------------------------------------------------------------------CRUZAMIENTO 2-------------------------------
    //------------------------------------------------------------------------------------CRUZAMIENTO 2-------------------------------
    Grupo_matrices Cruzamiento2(int[,] M1, int[,] M2, int[,] R1, int[,] R2, int[] RC1, int[] RC2, int rms)
    {
        int max1 = 0;
        int max2 = 0;
        for (int i = 1; i < rms; i++)
        {
            if (RC1[i] >= max1)
            {
                max1 = RC1[i];
            }
            if (RC2[i] >= max2)
            {
                max2 = RC2[i];
            }

        }
        int[,] nM1 = new int[width, height];
        int[,] tmp = new int[width, height];
        nM1 = DeleteRegions(M1, R1,max1);
        tmp = DeleteRegions(M2, R2,max2);

        int[,] nM2 = new int[width, height];

        for(int x = width-1; x >= 0; x=x-1)
        {
            for (int y = height - 1; y >= 0; y=y-1)
            {
                nM2[width-x-1,height-y-1]= tmp[x, y];
            }
        }
        int[,] FM = new int[width,height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y <height; y++)
            {
                if(nM1[x,y]==1 && nM2[x, y] == 1)
                {
                    FM[x, y] = 1;
                }
                if (nM1[x, y] == 0 && nM2[x, y] == 0)
                {
                    FM[x, y] = 0;
                }
                if (nM1[x, y] == 1 && nM2[x, y] == 0)
                {
                    FM[x, y] = 1;
                }
                if (nM1[x, y] == 0 && nM2[x, y] ==1)
                {
                    FM[x, y] = 0;
                }
            }
        }

        Grupo_matrices GPI;
        GPI = new Grupo_matrices(width, height, 100);
        GPI.mapa = FM;
        GPI.salas=ProcessMap(GPI.regiones,GPI.mapa);

        for (int i = 0; i < 10; i++)
        {
            CambiarMapa(GPI.mapa);
            GPI.salas=ProcessMap(GPI.regiones,GPI.mapa, false);
        }
        CambiarMapa(GPI.mapa);

        int Trooms = detectRegions(GPI.mapa, GPI.regionSize, GPI.regiones, GPI.regionCategorie, height, width);
        GPI.rooms = Trooms;
        GPI.TYPE = "C2";
        return GPI;
    }
    //------------------------------------------------------------------------------------LA MUTACION-------------------------------
    //------------------------------------------------------------------------------------LA MUTACION-------------------------------
    //------------------------------------------------------------------------------------LA MUTACION-------------------------------
    Grupo_matrices mutacion(int[,] map, int[,] Regi, int[] RC, int rms)
    {
        int id = 0;
        for (int i = 1; i < rms; i++)
        {
            if (RC[i] >= id)
            {
                id = RC[i];
            }
        }
        int[] pair = new int[4];
        if (id > 3)
        {
            pair = MuteRegion(Regi, id);
            //Debug.Log("Se muto una region de " + pair[0] + " " + pair[1] + " " + pair[2] + " " + pair[3] + " con -> " + id.ToString());
        }
        else
        {
            //Debug.Log("Sin regiones Eliminables");
            pair[0] = (width / 4) * 3;
            pair[1] = (height / 4) * 3;
            pair[2] = width / 4;
            pair[3] = height / 4;
        }

        int[,] newMap = new int[pair[0] - pair[2], pair[1] - pair[3]];
        int[,] FM = new int[width, height];

        FM = map;

        newMap = getRandomMute(newMap, pair[0] - pair[2], pair[1] - pair[3]);

        for(int x = pair[2]; x < pair[0]; x++)
        {
            for (int y = pair[3]; y < pair[1]; y++)
            {
                FM[x, y] = newMap[x - pair[2], y - pair[3]];
                //FM[x, y] = 1;
            }
        }

        Grupo_matrices GPI;
        GPI = new Grupo_matrices(width, height, 100);
        GPI.mapa = FM;
        GPI.salas=ProcessMap(GPI.regiones,GPI.mapa);

        for (int i = 0; i < 10; i++)
        {
            CambiarMapa(GPI.mapa);
            GPI.salas=ProcessMap(GPI.regiones,GPI.mapa, false);
        }
        CambiarMapa(GPI.mapa);

        int Trooms = detectRegions(GPI.mapa, GPI.regionSize, GPI.regiones, GPI.regionCategorie, height, width);
        GPI.rooms = Trooms;
        GPI.TYPE = "M";
        return GPI;

    }
    int[,] DeleteRegions(int[,] map, int[,] reg, int id)
    {
        for(int x=0;x < width; x++)
        {
            for (int y = 0; y < height ; y++)
            {
                if (reg[x, y] == id)
                {
                    map[x, y] = 1;
                }
            }
        }
        return map;
    }
    int[] MuteRegion(int[,] reg, int id)
    {
        int maxX = 0;
        int maxY = 0;
        int minX = 180; //son solo un valor alto
        int minY = 180;//son solo un valor alto
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (reg[x, y] == id)
                {
                    if (x >= maxX)
                    {
                        maxX = x;
                    }
                    if (x < minX)
                    {
                        minX = x;
                    }

                    if (y >= maxY)
                    {
                        maxY = y;
                    }
                    if (y < minY)
                    {
                        minY = y;
                    }
                }
            }
        }
        int[] pair = new int[4];
        pair[0] = maxX;
        pair[1] = maxY;
        pair[2] = minX;
        pair[3] = minY;
        return pair;
    }
    int[,] getRandomMute(int[,] Mp,int w, int h)
    {

        seed = System.DateTime.Now.ToString();
        seed = seed + Random.Range(-100.0f, 100.0f).ToString() + Random.Range(-100.0f, 100.0f).ToString();
    
        System.Random Prng = new System.Random(seed.GetHashCode());

        
        for (int x = 0; x < w; x++)
        {
            for (int y = 0; y < h; y++)
            {
                if (x == 0 || x == w - 1 || y == 0 || y == h - 1)
                {
                    Mp[x, y] = 0;
                }
                else
                {
                    Mp[x, y] = (Prng.Next(0, 100) < PorcLlenado) ? 1 : 0;
                }

            }
        }
        return Mp;
    }
    //int UserTimeComplete = 0;       // Tiempo en el que completo el juego
    //int UserExploration = 0;        // Porcentaje del mapa explorado
    //int UserScore;              // Puntaje del personaje
    //int UserOpinion;            // Opinion del usuario
    //bool UserClear;             // El usuario temino todo el jueg
    // int racha
    //negativas (dificil, no le gusto, tiempo alto, menos del 50% completado, )

    //neutrales (le gusto, mas del 505 completado)

    //positivas (facil, tiempo bajo )

    int Evolucionar()
    {
        int tinyRoom = (height * width * 2) / 100;
        int smallRoom = (height * width * 6) / 100;
        int mediumRoom = (height * width * 14) / 100;
        int largeRoom = (height * width * 20) / 100;
        int bigRoom = (height * width * 50) / 100;

        float TimeMinTiny = ((tinyRoom/3)/10);
        float TimeMinSmall = ((smallRoom / 3) / 10);
        float TimeMinMedium = ((mediumRoom / 3) / 10); ;
        float TimeMinLarge = ((largeRoom / 3) / 10); ;
        float TimeMinBig = ((bigRoom / 3) / 10); ;

        //Debug.Log("Valores estimados: " + TimeMinTiny.ToString() + " " + TimeMinSmall.ToString() + " " + TimeMinMedium.ToString() + " " + TimeMinLarge.ToString() + " " + TimeMinBig.ToString());

        float TimeExpected = 0;
        for(int cate = 1;cate < MapsPull[0].rooms; cate++)
        {
            if(MapsPull[0].regionCategorie[cate] == 1)
            {
                TimeExpected = TimeExpected + TimeMinTiny;
            }
            if (MapsPull[0].regionCategorie[cate] == 2)
            {
                TimeExpected = TimeExpected + TimeMinSmall;
            }
            if (MapsPull[0].regionCategorie[cate] == 3)
            {
                TimeExpected = TimeExpected + TimeMinMedium;
            }
            if (MapsPull[0].regionCategorie[cate] == 4)
            {
                TimeExpected = TimeExpected + TimeMinLarge;
            }
            if (MapsPull[0].regionCategorie[cate] == 5)
            {
                TimeExpected = TimeExpected + TimeMinBig;
            }
        }
        TimeExpected = TimeExpected + MapsPull[0].rooms - 1;
        //Debug.Log("Tiempo estimado: " + TimeExpected.ToString());



        int avanza=0;
        int retrocede = 0;
        int mantiene = 0;

        if (UserClear)
        {
            avanza++;
        }
        else{
            retrocede++;
        }
        if (UserOpinion==0)
        {
            avanza++;
            mantiene++;
        }
        else
        {
            retrocede++;
            retrocede++;
        }

        if(UserTimeComplete <= TimeExpected)
        {
            avanza++;
            if (UserExploration > (exploration / 4) * 3)
            {
                avanza++;
            }
            else
            {
                avanza++;
            }
        }
        else
        {
            if (UserExploration < (exploration / 4) * 2)
            {
                retrocede++;
            }
            else
            {
                mantiene++;
            }
            
        }
        if (UserScore == 0)
        {
            avanza++;
            avanza++;
        }
        else if (UserScore == 1)
        {
            avanza++;
        }
        else if (UserScore==2)
        {
            mantiene++;
            mantiene++;
        }
        else if (UserScore == 3)
        {
            retrocede++;
        }
        else if (UserScore==4)
        {
            retrocede++;
            retrocede++;
        }
        if (racha>=2)
        {
            avanza++;
            avanza++;
        }

        int decicion=2;

        Debug.Log("avanza "+avanza.ToString());
        Debug.Log("retrocede" + retrocede.ToString());
        Debug.Log("mantiene"+mantiene.ToString());

        if (avanza > retrocede)
        {
            if (avanza == mantiene)
            {
                //mantiene
                Debug.Log("=|=|=|=Se mantiene=|=|=|=");
                decicion = 2;
            }
            else if (avanza > mantiene)
            {
                //avanza
                Debug.Log("=|=|=|=Avanza=|=|=|=");
                decicion = 0;
            }
        }
        else if (retrocede > avanza)
        {
            if (retrocede == mantiene)
            {
                //mantiene
                Debug.Log("=|=|=|=Se mantiene=|=|=|=");
                decicion = 2;
            }
            else if (retrocede > mantiene)
            {
                //retrocede
                Debug.Log("=|=|=|=Retrocede=|=|=|=");
                decicion = 1;
            }
        }
        else if (avanza == retrocede)
        {
            //mantiene
            Debug.Log("=|=|=|=Se mantiene=|=|=|=");
            decicion = 2;
        }

        return decicion;
    }
    public void ExploUser()
    {
        completado.text = exploration.ToString()+"/"+UserExploration.ToString();
        //Debug.Log("GAAAAAAAA");
        UserExploration++;
    }

    //--------------------------------------------------------------------------------------------------
    //--------------------------------------------------------------Creacion de mapa como objeto--------
    //--------------------------------------------------------------------------------------------------
    void SpawCubes(List<Room> rm)
    {
        //scale hieig and large, pos las mitad -0.5

        BorraObject(obj);
        obj = obj + version.ToString();
        CreaObject(obj);
        if (GameObject.Find(obj) != null)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (mapa[x, y] == 1)
                    {
                        
                        var NewObj = GameObject.Instantiate(Objeto, new Vector3(x, 0, y), Quaternion.identity);
                        //NewObj.AddComponent<Rigidbody>();
                        int col = GetBordes(x, y, mapa);
                        if (col < 8 && col > 0)
                        {
                            NewObj.AddComponent<BoxCollider>();
                            exploration++;
                            //Debug.Log(exploration.ToString());
                        }
                        NewObj.transform.parent = GameObject.Find(obj).transform;
                        
                    }
                }
            }
        }
        version++;

        var plane = GameObject.Instantiate(Plane, new Vector3( (width/2)-0.5f , -1f, (height / 2) - 0.5f), Quaternion.identity);
        //NewObj.AddComponent<Rigidbody>();
        plane.AddComponent<BoxCollider>();
        plane.transform.localScale = new Vector3(width, 1, height);
        plane.transform.parent = GameObject.Find(obj).transform;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (mapa[x, y] == 0)
                {
                    posFinx = x;
                    posFiny = y;
                }
            }
        }

        foreach (Room r in rm)
        {
            if (r.TYPE == "SFinal")
            {
                try
                {
                    posFinx = r.tiles[r.tiles.Count / 2].tileX;
                    posFiny = r.tiles[r.tiles.Count / 2].tileY;
                    break;
                }
                catch (Exception e)
                {
                    Debug.Log("no se pudo colocar el final");
                    break;
                }

            }
        }

        for (int finx = posFinx - 1; finx < posFinx + 1; finx++)
        {
            for (int finy = posFiny - 1; finy < posFiny + 1; finy++)
            {
                //Debug.Log(finx.ToString() + " " + finy.ToString());
                var NewObj = GameObject.Instantiate(Llegada, new Vector3(finx, 0, finy), Quaternion.identity);
                //NewObj.AddComponent<Rigidbody>();
                //NewObj.AddComponent<BoxCollider>();
                NewObj.transform.parent = GameObject.Find(obj).transform;

                if(finx==posFinx && finy == posFiny)
                {
                    RealFinalx = (int) NewObj.transform.localPosition.x;
                    RealFinaly = (int) NewObj.transform.localPosition.z;
                }
            }
        }

    }
    public bool Inicio()
    {
        return termino;
    }
    public bool Llego()
    {
        //Debug.Log(RealFinalx.ToString() + " " + RealFinaly.ToString() + "  -> " + Player.transform.position.x.ToString() + " " + Player.transform.position.z.ToString());
        if (Player.transform.position.x <= RealFinalx + 1 && Player.transform.position.x >= RealFinalx - 1)
        {
            if (Player.transform.position.z <= RealFinaly + 1 && Player.transform.position.z >= RealFinaly - 1)
            {
                //Debug.Log(RealFinalx.ToString() + " " + RealFinaly.ToString() + "  -> " + Player.transform.position.x.ToString() + " " + Player.transform.position.z.ToString());
                //Debug.Log(">Acabo");
                termino = true;
                End_time();
                return true;
            }
        }
        return false;
    }
    public int[] tam()
    {
        int[] pair = new int[2];
        pair[0] = width;
        pair[1] = height;
        return pair;
    }
    public bool piso(int x, int y)
    {
        if (mapa[x, y] == 1)
        {
            return false;
        }
        return true;
    }
    void BorraObject(string obj)
    {
        if (GameObject.Find(obj) != null)
        {
            Debug.Log("se borra");
            borra = GameObject.Find(obj);
            Destroy(borra);
        }
    }
    void CreaObject(string obj)
    {
        container = new GameObject();
        container.name = obj;
        container.AddComponent<MeshCombiner>();
    }

    //--------------------------------------------------------------------------------------------------
    //-------------------------------------------------------------------------Dibujoo de mapa----------
    //--------------------------------------------------------------------------------------------------
    void OnDrawGizmos()
    {
        if (MapsPull!=null)
        {
            int[,] MapImp = MapsPull[Act_matrix].mapa;
            if (MapImp != null)
            {
                for (int x = 0; x < MapImp.GetLength(0); x++)
                {
                    for (int y = 0; y < MapImp.GetLength(0); y++)
                    {
                        //Gizmos.color = (mapa[x, y] == 1) ? Color.black : Color.white;
                        if (MapImp[x, y] == -2)
                        {
                            Gizmos.color = Color.magenta;

                        }
                        if (MapImp[x, y] == 1)
                        {
                            Gizmos.color = Color.black;

                        }
                        else if (MapImp[x, y] == 0)
                        {
                            Gizmos.color = Color.white;
                        }
                        else if (MapImp[x, y] == 2)
                        {
                            Gizmos.color = Color.green;
                        }
                        else if (MapImp[x, y] == 3)
                        {
                            Gizmos.color = Color.blue;
                        }
                        else if (MapImp[x, y] == 4)
                        {
                            Gizmos.color = Color.yellow;
                        }
                        else if (MapImp[x, y] == 5)
                        {
                            Gizmos.color = Color.red;
                        }
                        else if (MapImp[x, y] == 6)
                        {
                            Gizmos.color = Color.grey;
                        }
                        //Vector3 pos = new Vector3(-MapImp.GetLength(0)/ 2 + x + .5f, 0, -MapImp.GetLength(0) / 2 + y + .5f);
                        Vector3 pos = new Vector3(-160 + x + .5f, 0, y + .5f);
                        Gizmos.DrawCube(pos, Vector3.one);
                    }
                }
                if (MapsPull[Act_matrix].TYPE == "C1" || MapsPull[Act_matrix].TYPE == "C2")
                {
                    int[,] papa = MapsPull[Act_matrix].papa;
                    int[,] mama = MapsPull[Act_matrix].mama;
                    for (int x = 0; x < papa.GetLength(0); x++)
                    {
                        for (int y = 0; y < papa.GetLength(0); y++)
                        {
                            if (papa[x, y] == 1)
                            {
                                Gizmos.color = Color.black;

                            }
                            else if (papa[x, y] == 0)
                            {
                                Gizmos.color = Color.white;
                            }

                            Vector3 pos = new Vector3(-240 + x + .5f, 0, 160 + y + .5f);
                            Gizmos.DrawCube(pos, Vector3.one);
                        }
                    }
                    for (int x = 0; x < mama.GetLength(0); x++)
                    {
                        for (int y = 0; y < mama.GetLength(0); y++)
                        {
                            if (mama[x, y] == 1)
                            {
                                Gizmos.color = Color.black;

                            }
                            else if (mama[x, y] == 0)
                            {
                                Gizmos.color = Color.white;
                            }

                            Vector3 pos = new Vector3(-80 + x + .5f, 0, 160 + y + .5f);
                            Gizmos.DrawCube(pos, Vector3.one);
                        }
                    }
                }
                if (MapsPull[Act_matrix].TYPE == "M")
                {
                    int[,] mama = MapsPull[Act_matrix].mama;
                    for (int x = 0; x < mama.GetLength(0); x++)
                    {
                        for (int y = 0; y < mama.GetLength(0); y++)
                        {
                            if (mama[x, y] == 1)
                            {
                                Gizmos.color = Color.black;

                            }
                            else if (mama[x, y] == 0)
                            {
                                Gizmos.color = Color.white;
                            }

                            Vector3 pos = new Vector3(-160 + x + .5f, 0, 160 + y + .5f);
                            Gizmos.DrawCube(pos, Vector3.one);
                        }
                    }
                }
            }
        }
    }
    void cloneMatrix(int[,] M1, int[,] M2)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                M2[x, y] = M1[x, y];
            }
        }
    }

    //--------------------------------------------------------------------------------------------------
    //------------------------------------------------------------------Obtencion de regiones-----------
    //--------------------------------------------------------------------------------------------------
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "final")
        {
            Debug.Log("Do something here");
        }
    }

    //--------------------------------------------------------------------------------------------------
    //------------------------------------------------------------------Obtencion de regiones-----------
    //--------------------------------------------------------------------------------------------------
    int RecursiveSearch(int RegValue, int Dx, int Dy, int[,] map)
    {
        List<coordenedas> Posiciones = new List<coordenedas>();
        int[,] Marcador = map;
        int ValorBuscado = map[Dx, Dy];

        Queue<coordenedas> queue = new Queue<coordenedas>();
        queue.Enqueue(new coordenedas(Dx, Dy));
        Marcador[Dx, Dy] = RegValue;

        while (queue.Count > 0)
        {
            coordenedas corde = queue.Dequeue();
            Posiciones.Add(corde);

            for (int x = corde.CordX - 1; x <= corde.CordX + 1; x++)
            {
                for (int y = corde.CordY - 1; y <= corde.CordY + 1; y++)
                {
                    if (IsInMapRange(x, y) && (y == corde.CordY || x == corde.CordX))
                    {
                        if (Marcador[x, y] == 0 && map[x, y] == ValorBuscado)
                        {
                            Marcador[x, y] = RegValue;
                            queue.Enqueue(new coordenedas(x, y));
                        }
                    }
                }
            }
        }
        map = Marcador;
        return Posiciones.Count;
        /*
        int members = 1;
        setValue(Dx, Dy, RegValue, map);

        for (int vecinox = Dx - 1; vecinox <= Dx + 1; vecinox++)
        {
            for (int vecinoy = Dy - 1; vecinoy <= Dy + 1; vecinoy++)
            {
                if (vecinox >= 0 && vecinox < Wd && vecinoy >= 0 && vecinoy < Hg)
                {
                    if (vecinox != Dx || vecinoy != Dy)
                    {
                        if (mapaTemp[vecinox, vecinoy] == 0)
                        {
                            if (map[vecinox, vecinoy] == 0)
                            {
                                mapaTemp[Dx, Dy] = 1;
                                Debug.Log("Tamo en : " + Dx.ToString() + ", " + Dy.ToString() + " con valor : " + map[Dx, Dy].ToString()+",  vamos a: "+vecinox.ToString()+", "+vecinoy.ToString());
                                members = members + RecursiveSearch(RegValue, vecinox, vecinoy, Hg, Wd, map, mapaTemp);
                            }
                        }
                    }
                }
            }
        }
        return members;*/
    }
    string printCateg(int[] cat)
    {
        string txt = "";
        for(int x=0;x< 100; x++)
        {
            //Debug.Log("entre");
            txt = txt + cat[x].ToString() + ", ";
        }
        txt = txt + "\n";
        return txt;
    }
    void printRegiones(int[,] mapaI)
    {
        

        string txts = "";
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                txts = txts + mapaI[x, y].ToString();
            }
            txts = txts + "\n";
        }
        Debug.Log(txts);
    }
    void setValue(int x, int y, int val, int[,] map)
    {
        map[x, y] = val;
    }
    int GetVecinoRegion(int x, int y)
    {
        string subM = "";
        int region = 0;
        for (int vecinox = x - 1; vecinox <= x + 1; vecinox++)
        {
            for (int vecinoy = y - 1; vecinoy <= y + 1; vecinoy++)
            {
                subM = subM + regiones[vecinox, vecinoy].ToString();
                if (vecinox >= 0 && vecinox < width && vecinoy >= 0 && vecinoy < height)
                {
                    if (vecinox != x || vecinoy != y)
                    {
                        if(mapa[vecinox, vecinoy] == 0)
                        {
                            if (regiones[vecinox, vecinoy] != 0)
                            {
                                region = regiones[vecinox, vecinoy];
                                Debug.Log("subMatrix \n " + subM);
                                return region;
                            }
                        }
                        //else
                        //{
                        //   Debug.Log("es Negro " + x.ToString() + " " + y.ToString());
                        //}
                    }
                }
            }
            subM = subM + "\n";
        }
        //Debug.Log("No tiene vecinos " + x.ToString() + " " + y.ToString());
        //Debug.Log("subMatrix \n "+subM);
        return region;
    }
    int detectRegions(int[,] map,int[] RegSize,int[,] Regs, int[] RegsCat, int Hg, int Wd)
    {
        int MaxactReg = 0;
        //int Reg = 0;
        int MembReg = 0;
        
        int[,] mapaTemp;
        mapaTemp = new int[Hg, Wd];
        cloneMatrix(map, mapaTemp);

        if (mapaTemp != null)
        {
            //Regs = new int[Hg, Wd];
            cloneMatrix(map, Regs);

            for (int x = 0; x < Wd; x++)
            {
                for (int y = 0; y < Hg; y++)
                {
                    if (Regs[x, y] == 1)
                    {
                        Regs[x, y] = -1;
                    }
                }
            }
            //RegSize = new int[100];
            for (int x = 0; x < Wd; x++)
            {
                for (int y = 0; y < Hg; y++)
                {
                    if (Regs[x, y] == 0)
                    {
                        MaxactReg= MaxactReg+1;
                        MembReg = RecursiveSearch(MaxactReg ,x, y, Regs);
                        //Debug.Log(MembReg.ToString());
                        RegSize[MaxactReg] = (int) MembReg;
                        //Debug.Log("RegTam -> "+RegSize[MaxactReg].ToString());
                    }
                }
            }

            for (int x = 0; x < Wd; x++)
            {
                for (int y = 0; y < Hg; y++)
                {
                    if (Regs[x, y] == 0)
                    {
                        Regs[x, y] = -2;
                    }
                    if (Regs[x, y] == -1)
                    {
                        Regs[x, y] = 0;
                    }
                }
            }

            //printRegiones(map);

            int tinyRoom = (Hg * Wd * 2) / 100;
            int smallRoom = (Hg * Wd * 6) / 100;
            int mediumRoom = (Hg * Wd * 14) / 100;
            int largeRoom = (Hg * Wd * 20) / 100;
            int bigRoom = (Hg * Wd * 50) / 100;

            //string tmp = "";

            //RegsCat = new int[100];
            for (int e = 0; e < MaxactReg; e++)
            {
                int valt = RegSize[e];
                
                if ((0 < valt) && (valt <= tinyRoom))
                {
                    RegsCat[e] = 1;
                } 
                else if ((tinyRoom < valt) && (valt <= smallRoom))
                {
                    RegsCat[e] = 2;
                } 
                else if ((smallRoom < valt) && (valt <= mediumRoom))
                {
                    RegsCat[e] = 3;
                } 
                else if ((mediumRoom < valt) && (valt <= largeRoom))
                {
                    RegsCat[e] = 4;
                } 
                else if ((largeRoom < valt) && (valt <= bigRoom))
                {
                    RegsCat[e] = 5;
                }
                else
                {
                    RegsCat[e] = 6;
                }

                //tmp = tmp + e.ToString() + " -> " + valt.ToString() + " <------ "+ RegsCat[e].ToString() + "\n";

            }
            //Debug.Log(tmp);

            //Debug.Log(MaxactReg.ToString());

            for (int x = 0; x < Wd; x++)
            {
                for (int y = 0; y < Hg; y++)
                {
                    //Debug.Log(x.ToString() + " " + y.ToString() + " - " + regiones[x, y].ToString());
                    if(Regs[x,y] != 0 && Regs[x, y] != -2)
                    {
                        Regs[x, y] = RegsCat[Regs[x, y]];
                    }
                }
            }
            //printRegiones(Regs);
        }
        return MaxactReg;
    }
    int GetTYPEReg(int valt)
    {
        int tinyRoom = (height * width * 2) / 100;
        int smallRoom = (height * width * 6) / 100;
        int mediumRoom = (height * width * 14) / 100;
        int largeRoom = (height * width * 20) / 100;
        int bigRoom = (height * width * 50) / 100;

        if ((0 < valt) && (valt <= tinyRoom))
        {
            return 1;
        }
        else if ((tinyRoom < valt) && (valt <= smallRoom))
        {
            return 2;
        }
        else if ((smallRoom < valt) && (valt <= mediumRoom))
        {
            return 3;
        }
        else if ((mediumRoom < valt) && (valt <= largeRoom))
        {
            return 4;
        }
        else if ((largeRoom < valt) && (valt <= bigRoom))
        {
            return 5;
        }
        else
        {
            return 6;
        }
        return 0;
    }
    void printSalas(List<Room> Conecciones)
    {
        string printo = "";
        Debug.Log("Room printer");
        foreach (Room room in Conecciones)
        {
            printo = room.roomSize.ToString() + "\n ( "+ room.TYPE+" : ID "+ room.ID.ToString()+" )conecciones con:";
            foreach (Room roomTMP in room.connectedRooms)
            {
                printo = printo + roomTMP.roomSize.ToString() + "\n";
            }
            Debug.Log(printo);
        }
        Debug.Log("END Room printer");
    }
    int CalculateDistance(Room roomA, Room roomB, Room ignore, bool ig)
    {
        int dist_actual = 1;
        if (roomA.ID == roomB.ID)
        {
            return 1;
        }
        foreach (Room room in roomA.connectedRooms)
        {
            if (ig && room.ID == ignore.ID)
            {
                continue;
            }
            else
            {
                Debug.Log("entro a " + room.ID.ToString() + " : " + room.TYPE);
                int val = CalculateDistance(room, roomB, roomA, true);
                if(val > 0)
                {
                    return val + 1;
                }
                else
                {
                    continue;
                }
            }
        }
        Debug.Log("Rama equivocada");
        return 0;
    }
    int FindKey(Room roomA, Room roomB, Room ignore, bool ig, List<Room> Rompedores)
    {
        int dist_actual = 1;
        if (roomA.ID == roomB.ID)
        {
            return 0;
        }
        if (roomA.ID != roomB.ID && roomB.TYPE == "SLock")
        {
            Rompedores.Add(roomB);
            return FindKey(roomB, roomB.DependenceID, roomB, true, Rompedores);
        }
        foreach (Room room in roomA.connectedRooms)
        {
            foreach (Room cerraduras in Rompedores)
            {
                if (room.ID == cerraduras.ID)
                {
                    continue;
                }
            }
            if (ig && room.ID == ignore.ID)
            {
                continue;
            }
            else
            {
                int res = FindKey(room, roomB, roomA, true, Rompedores);
                if (res == 1)
                {
                    continue;
                }
                else if (res == 0)
                {
                    return 0;
                }
            }
        }
        return 1;
    }
    int InsureWay(Room roomA, Room roomB, Room ignore, bool ig, Room find)
    {
        int dist_actual = 1;
        if (roomA.ID == roomB.ID)
        {
            return 1;
        }
        if (roomA.ID == find.ID)
        {
            return 0;
        }
        foreach (Room room in roomA.connectedRooms)
        {
            if (ig && room.ID == ignore.ID)
            {
                continue;
            }
            else
            {
                int res = InsureWay(room, roomB, roomA, true, find);
                if (res==1)
                {
                    continue;
                }
                else if (res==0)
                {
                    return 0;
                }
            }
        }
        return 1;
    }
    int InsureDoors(Room roomA, Room roomB, Room ignore, bool ig, Room find,List<Room> Rompedores)
    {
        int dist_actual = 1;
        if (roomA.ID == roomB.ID)
        {
            return 1;
        }
        if (roomA.ID == find.ID)
        {
            return 0;
        }
        if (roomA.ID != roomB.ID && roomB.TYPE=="SLock")
        {
            Rompedores.Add(roomB);
            return FindKey(roomB, roomB.DependenceID, roomB, false, Rompedores);
        }
        foreach (Room room in roomA.connectedRooms)
        {
            foreach (Room cerraduras in Rompedores)
            {
                if(room.ID == cerraduras.ID)
                {
                    return 1;
                }
            }
            if (ig && room.ID == ignore.ID)
            {
                continue;
            }
            else
            {
                int res = InsureDoors(room, roomB, roomA, true, find, Rompedores);
                if (res == 1)
                {
                    continue;
                }
                else if (res == 0)
                {
                    return 0;
                }
            }
        }
        return 1;
    }
    int InsureBossDoors(Room roomA, Room roomB, Room ignore, bool ig, Room find, List<Room> Rompedores)
    {
        int dist_actual = 1;
        if (roomA.ID == roomB.ID)
        {
            return 1;
        }
        if (roomA.ID == find.ID)
        {
            return 0;
        }
        if (roomA.ID != roomB.ID && roomB.TYPE == "SLock")
        {
            Rompedores.Add(roomB);
            return FindKey(roomB, roomB.DependenceID, roomB, false, Rompedores);
        }
        foreach (Room room in roomA.connectedRooms)
        {
            foreach (Room cerraduras in Rompedores)
            {
                if (room.ID == cerraduras.ID)
                {
                    return 1;
                }
            }
            if (ig && room.ID == ignore.ID)
            {
                continue;
            }
            else
            {
                int res = InsureDoors(room, roomB, roomA, true, find, Rompedores);
                if (res == 1)
                {
                    continue;
                }
                else if (res == 0)
                {
                    return 0;
                }
            }
        }
        return 1;
    }
    void SetSymbols(List<Room> room)
    {
        
        foreach (Room r in room)
        {

            if (r.TYPE == "SBoss")
            {
                r.Symbol = SBoss;
            }
            else if (r.TYPE == "SEvent")
            {
                r.Symbol = SEvent;
            }
            else if (r.TYPE == "SFight")
            {
                r.Symbol = SFight;
            }
            else if (r.TYPE == "SFinal")
            {
                r.Symbol = SFinal;
            }
            else if (r.TYPE == "SKey")
            {
                r.Symbol = SKey;
            }
            else if (r.TYPE == "SLock")
            {
                r.Symbol = SLock;
            }
            else if (r.TYPE == "SStart")
            {
                r.Symbol = SStart;
            }
            else if (r.TYPE == "STreasure")
            {
                r.Symbol = STreasure;
            }
        }
    }
    void ShowSymbols(List<Room> room)
    {
        foreach (Room r in room)
        {
            try
            {
                r.ShowSymbol();
                if (r.isDependent)
                {
                    Debug.DrawLine(CoordToWorldPoint2(r.tiles[0]), CoordToWorldPoint2(r.DependenceID.tiles[0]), Color.green, 100);
                }
            }
            catch(Exception e)
            {
                Debug.Log("Asignacion no encontrada");
            }
            
        }
    }
    void DelSymbols(List<Room> room)
    {
        foreach (Room r in room)
        {
            r.DelSymbol();
        }
    }
    bool IsEdge(Room room)
    {
        if (room.connectedRooms.Count == 1)
        {
            return true;
        }
        return false;
    }

    //--------------------------------------------------------------------------------------------------
    //-------------------------------------------EVALUAR SALAS------------------------------------------
    //--------------------------------------------------------------------------------------------------

    bool EvaluarSalas(int[] RegsCat,int[] RegsSize)
    {
        int[] salas1;
        int s1 = 0;
        int[] salas2;
        int s2 = 0;
        int[] salas3;
        int s3 = 0;
        int[] salas4;
        int s4 = 0;
        int[] salas5;
        int s5 = 0;

        for (int i =0; i < RegsCat.Length; i++)
        {
            if (RegsCat[i] == 1)
            {
                s1++;
            }
            if (RegsCat[i] == 2)
            {
                s2++;
            }
            if (RegsCat[i] == 3)
            {
                s3++;
            }
            if (RegsCat[i] == 4)
            {
                s4++;
            }
            if (RegsCat[i] == 5)
            {
                s5++;
            }
        }
        salas1 = new int[s1];
        salas2 = new int[s2];
        salas3 = new int[s3];
        salas4 = new int[s4];
        salas5 = new int[s5];
        s1 = 0;
        s2 = 0;
        s3 = 0;
        s4 = 0;
        s5 = 0;
        string clasi1 = "T1 : ";
        string clasi2 = "T2 : ";
        string clasi3 = "T3 : ";
        string clasi4 = "T4 : ";
        string clasi5 = "T5 : ";
        string clasi;
        Debug.Log("llego");
        for (int i = 0; i < RegsCat.Length; i++)
        {
            if (RegsCat[i] == 1)
            {
                salas1[s1] = i;
                s1++;
                clasi1 = clasi1 + " ," + i;
            }
            if (RegsCat[i] == 2)
            {
                salas2[s2] = i;
                s2++;
                clasi2 = clasi2 + " ," + i;
            }
            if (RegsCat[i] == 3)
            {
                salas3[s3] = i;
                s3++;
                clasi3 = clasi3 + " ," + i;
            }
            if (RegsCat[i] == 4)
            {
                salas4[s4] = i;
                s4++;
                clasi4 = clasi4 + " ," + i;
            }
            if (RegsCat[i] == 5)
            {
                salas5[s5] = i;
                s5++;
                clasi5 = clasi5 + " ," + i;
            }
        }
        clasi = clasi1 + " \n" + clasi2 + " \n" + clasi3 + " \n" + clasi4 + " \n" + clasi5 + " \n";
        Debug.Log("-------------");
        Debug.Log(clasi);
        Debug.Log("-------------");
        return true;//cambiar
    }

    bool AreConected(habitacion A,habitacion B)
    {
        for(int i = 0; 1 < A.coneciones.Length; i++)
        {
            if (A.coneciones[i].ID == B.ID)
            {
                return true;
            }
        }
        return false;
    }

    bool Condicion1(habitacion[] S1, habitacion[] S2, habitacion[] S3, habitacion[] S4, habitacion[] S5)
    {
        if (S1.Length >= 2) // and S
        {

            float maxLen = 0;
            int room1=0;
            int room2=0;
            for (int i = 0;i<S1.Length; i++)
            {
                for (int j = 0;j<S1.Length; j++)
                {
                    float dis = Vector3.Distance(new Vector3(S1[i].px, 0, S1[i].py), new Vector3(S1[j].px, 0, S1[j].py));
                    if (maxLen < dis)
                    {
                        maxLen = dis;
                        room1 = i;
                        room2 = j;
                    }
                }
            }
            if (maxLen > width / 2)
            {
                if (!AreConected(S1[room1], S1[room2]))
                {
                    S1[room1].name = "inicio";
                    S1[room2].name = "final";
                    return true;
                }
            }
        }
        return false;
    }
    bool Condicion2(int[] S1, int[] S2, int[] S3, int[] S4, int[] S5)
    {

        return true;
    }


    bool SetStart(List<Room> salas)
    {
        List<Room> candidatos = new List<Room>();
        foreach (Room room in salas)
        {
            //room.sizeTYPE = GetTYPEReg(room.roomSize);
            if((room.sizeTYPE==1 ))
            {
                candidatos.Add(room);
            }
            if(room.TYPE=="SStart")
            {
                return true;
            }
        }
        int bestDistance = 0;
        Coord bestTileA = new Coord();
        Coord bestTileB = new Coord();
        Room bestRoomA = new Room();
        Room bestRoomB = new Room();
        bool possibleConnectionFound = false;
        foreach (Room roomA in candidatos)
        {
            foreach (Room roomB in candidatos)
            {
                if (!roomA.IsConnected(roomB))
                {
                    for (int tileIndexA = 0; tileIndexA < roomA.edgeTiles.Count; tileIndexA++)
                    {
                        for (int tileIndexB = 0; tileIndexB < roomB.edgeTiles.Count; tileIndexB++)
                        {
                            Coord tileA = roomA.edgeTiles[tileIndexA];
                            Coord tileB = roomB.edgeTiles[tileIndexB];
                            int distanceBetweenRooms = (int)(Mathf.Pow(tileA.tileX - tileB.tileX, 2) + Mathf.Pow(tileA.tileY - tileB.tileY, 2));

                            if (distanceBetweenRooms > bestDistance || !possibleConnectionFound)
                            {
                                bestDistance = distanceBetweenRooms;
                                possibleConnectionFound = true;
                                bestTileA = tileA;
                                bestTileB = tileB;
                                bestRoomA = roomA;
                                bestRoomB = roomB;
                            }
                        }
                    }
                }
            }
        }
        if (possibleConnectionFound == false)
        {
            return false;
        }
        bestRoomA.TYPE = "SStart";
        bestRoomB.TYPE = "SFinal";
        return true;
    }
    bool SetFight(List<Room> salas)
    {
        List<Room> candidatos = new List<Room>();
        bool possible = false;
        foreach (Room room in salas)
        {
            if ((room.sizeTYPE == 3 || room.sizeTYPE == 4)&& room.TYPE=="SEvent")
            {
                candidatos.Add(room);
                possible = true;
            }
        }
        if (!possible)
        {
            return false;
        }

        Coord bestTileA = new Coord();
        Room bestRoomA = new Room();
        bool possibleConnectionFound = false;

        foreach (Room roomA in candidatos)
        {
            if(1==Random.Range(1, 2))
            {
                roomA.TYPE = "SFight";
                possibleConnectionFound = true;
                break;
            }
        }
        if (!possibleConnectionFound)
        {
            candidatos[0].TYPE = "SFight";
        }
        return true;
        
    }
    bool SetTreasure(List<Room> salas)
    {
        List<Room> candidatos = new List<Room>();
        bool possible = false;
        foreach (Room room in salas)
        {
            if ((room.sizeTYPE == 2) && room.TYPE == "SEvent")
            {
                candidatos.Add(room);
                possible = true;
            }
        }
        if (!possible)
        {
            return false;
        }

        Coord bestTileA = new Coord();
        Room bestRoomA = new Room();
        bool possibleConnectionFound = false;

        foreach (Room roomA in candidatos)
        {
            if (1 == Random.Range(1, 2))
            {
                roomA.TYPE = "STreasure";
                possibleConnectionFound = true;
                break;
            }
        }
        if (!possibleConnectionFound)
        {
            candidatos[0].TYPE = "STreasure";
        }
        return true;

    }
    bool SetBoss(List<Room> salas)
    {
        List<Room> candidatos = new List<Room>();
        bool possible = false;
        foreach (Room room in salas)
        {
            if ((room.sizeTYPE == 3 || room.sizeTYPE == 4 || room.sizeTYPE == 5) && room.TYPE == "SEvent")
            {
                candidatos.Add(room);
                possible = true;
            }
        }
        if (!possible)
        {
            return false;
        }

        Coord bestTileA = new Coord();
        Room bestRoomA = new Room();
        bool possibleConnectionFound = false;

        foreach (Room roomA in candidatos)
        {
            if (1 == Random.Range(1, 2))
            {
                roomA.TYPE = "SBoss";
                possibleConnectionFound = true;
                break;
            }
        }
        if (!possibleConnectionFound)
        {
            candidatos[0].TYPE = "SBoss";
        }
        return true;

    }
    bool SetDoor(List<Room> salas)
    {
        List<Room> candidatosDoor = new List<Room>();
        List<Room> candidatosKey = new List<Room>();
        Room In = new Room();
        Room Fi = new Room();
        bool possible1 = false;
        bool possible2 = false;
        bool possible3 = false;
        bool possible4 = false;
        foreach (Room room in salas)
        {
            if ((room.sizeTYPE == 1) && room.TYPE == "SEvent" && !IsEdge(room))
            {
                candidatosDoor.Add(room);
                possible1 = true;
            }
            if ((room.sizeTYPE == 1 || room.sizeTYPE == 2 || room.sizeTYPE == 3 || room.sizeTYPE == 4) && room.TYPE == "SEvent")
            {
                candidatosKey.Add(room);
                possible2 = true;
            }
            if (room.TYPE == "SStart")
            {
                In = room;
                possible3 = true;
            }
            if (room.TYPE == "SFinal")
            {
                Fi = room;
                possible4 = true;
            }
        }
        if (!possible1 || !possible2 || !possible3 || !possible4)
        {
            return false;
        }


        Coord bestTileA = new Coord();
        Room bestRoomA = new Room();
        bool possibleConnectionFound = false;

        for (int i = 0; i < candidatosDoor.Count; i++)
        {
            Room temp = new Room();
            temp = candidatosDoor[i];
            int randomIndex = Random.Range(i, candidatosDoor.Count);
            candidatosDoor[i] = candidatosDoor[randomIndex];
            candidatosDoor[randomIndex] = temp;
        }
        for (int i = 0; i < candidatosKey.Count; i++)
        {
            Room temp = new Room();
            temp = candidatosKey[i];
            int randomIndex = Random.Range(i, candidatosKey.Count);
            candidatosKey[i] = candidatosKey[randomIndex];
            candidatosKey[randomIndex] = temp;
        }
        
        foreach (Room roomD in candidatosDoor)
        {
            foreach(Room roomK in candidatosKey)
            {
                if (0 == InsureWay(In, roomD, In, false, roomK) && 1 == InsureWay(In, roomD, In, false, Fi))
                {
                    roomK.TYPE = "SKey";
                    roomD.TYPE = "SLock";
                    roomK.DependenceID = roomD;
                    roomD.DependenceID = roomK;
                    roomD.isDependent = true;
                    roomK.isDependent = true;
                    possibleConnectionFound = true;
                    return true;
                }
            }
        }
        if (!possibleConnectionFound)
        {
            return false;
        }
        return true;

    }
    bool SetDoorEx(List<Room> salas)
    {
        List<Room> candidatosDoor = new List<Room>();
        List<Room> candidatosKey = new List<Room>();
        List<Room> Rompedores = new List<Room>();
        Room In = new Room();
        Room Fi = new Room();
        bool possible1 = false;
        bool possible2 = false;
        bool possible3 = false;
        bool possible4 = false;
        foreach (Room room in salas)
        {
            if ((room.sizeTYPE == 1) && room.TYPE == "SEvent" && !IsEdge(room))
            {
                candidatosDoor.Add(room);
                possible1 = true;
            }
            if ((room.sizeTYPE == 1 || room.sizeTYPE == 2 || room.sizeTYPE == 3 || room.sizeTYPE == 4) && room.TYPE == "SEvent")
            {
                candidatosKey.Add(room);
                possible2 = true;
            }
            if (room.TYPE == "SStart")
            {
                In = room;
                possible3 = true;
            }
            if (room.TYPE == "SFinal")
            {
                Fi = room;
                possible4 = true;
            }
        }
        if (!possible1 || !possible2 || !possible3 || !possible4)
        {
            return false;
        }


        Coord bestTileA = new Coord();
        Room bestRoomA = new Room();
        bool possibleConnectionFound = false;

        for (int i = 0; i < candidatosDoor.Count; i++)
        {
            Room temp = new Room();
            temp = candidatosDoor[i];
            int randomIndex = Random.Range(i, candidatosDoor.Count);
            candidatosDoor[i] = candidatosDoor[randomIndex];
            candidatosDoor[randomIndex] = temp;
        }
        for (int i = 0; i < candidatosKey.Count; i++)
        {
            Room temp = new Room();
            temp = candidatosKey[i];
            int randomIndex = Random.Range(i, candidatosKey.Count);
            candidatosKey[i] = candidatosKey[randomIndex];
            candidatosKey[randomIndex] = temp;
        }

        foreach (Room roomD in candidatosDoor)
        {
            foreach (Room roomK in candidatosKey)
            {

                if (0 == InsureDoors(In, roomD, In, false, roomK, Rompedores))
                {
                    roomK.TYPE = "SKey";
                    roomD.TYPE = "SLock";
                    roomK.DependenceID = roomD;
                    roomD.DependenceID = roomK;
                    roomK.isDependent = true;
                    roomD.isDependent = true;
                    possibleConnectionFound = true;
                    return true;
                }
            }
        }
        if (!possibleConnectionFound)
        {
            return false;
        }
        return true;

    }
    bool SetBossDoor(List<Room> salas)
    {
        List<Room> candidatosDoor = new List<Room>();
        List<Room> candidatosKey = new List<Room>();
        List<Room> Rompedores = new List<Room>();
        Room In = new Room();
        Room Fi = new Room();
        bool possible1 = false;
        bool possible2 = false;
        bool possible3 = false;
        bool possible4 = false;
        foreach (Room room in salas)
        {
            if ((room.sizeTYPE == 1) && room.TYPE == "SEvent" && !IsEdge(room))
            {
                candidatosDoor.Add(room);
                possible1 = true;
            }
            if ((room.sizeTYPE == 1 || room.sizeTYPE == 2 || room.sizeTYPE == 3 || room.sizeTYPE == 4) && room.TYPE == "SEvent")
            {
                candidatosKey.Add(room);
                possible2 = true;
            }
            if (room.TYPE == "SStart")
            {
                In = room;
                possible3 = true;
            }
            if (room.TYPE == "SFinal")
            {
                Fi = room;
                possible4 = true;
            }
        }
        if (!possible1 || !possible2 || !possible3 || !possible4)
        {
            return false;
        }


        Coord bestTileA = new Coord();
        Room bestRoomA = new Room();
        bool possibleConnectionFound = false;

        for (int i = 0; i < candidatosDoor.Count; i++)
        {
            Room temp = new Room();
            temp = candidatosDoor[i];
            int randomIndex = Random.Range(i, candidatosDoor.Count);
            candidatosDoor[i] = candidatosDoor[randomIndex];
            candidatosDoor[randomIndex] = temp;
        }
        for (int i = 0; i < candidatosKey.Count; i++)
        {
            Room temp = new Room();
            temp = candidatosKey[i];
            int randomIndex = Random.Range(i, candidatosKey.Count);
            candidatosKey[i] = candidatosKey[randomIndex];
            candidatosKey[randomIndex] = temp;
        }

        foreach (Room roomD in candidatosDoor)
        {
            foreach (Room roomK in candidatosKey)
            {

                if (0 == InsureDoors(In, roomD, In, false, roomK, Rompedores))
                {

                    foreach (Room roomDc in roomD.connectedRooms)
                    {
                        if ((roomDc.sizeTYPE == 3 || roomDc.sizeTYPE == 4 || roomDc.sizeTYPE == 5) && roomDc.TYPE == "SEvent")
                        {
                            if (0 == InsureWay(In, roomDc, In, false, roomD))
                            {
                                roomK.TYPE = "SKey";
                                roomD.TYPE = "SLock";
                                roomDc.TYPE = "SBoss";
                                roomK.DependenceID = roomD;
                                roomD.DependenceID = roomK;
                                roomK.isDependent = true;
                                roomD.isDependent = true;
                                possibleConnectionFound = true;
                                return true;
                            }
                        }
                    }
                    
                }
            }
        }
        if (!possibleConnectionFound)
        {
            return false;
        }
        return true;

    }

    bool SetRoomTYPES(int inicio, int pelea, int tesoro, int jefe, int puerta, int puerta_extra, int puerta_boss,List<Room> room)
    {
        bool exito = true;
        exito = exito && SetStart(room);

        if (!exito)
        {
            return exito;
        }

        if (inicio!=1)
        {
            Debug.Log("Solo se puede colocar un inicio y un final"); ;
        }
        
        if (puerta == 1)
        {
            exito = exito && SetDoor(room);
            if (!exito)
            {
                return exito;
            }
        }
        else if(puerta > 1)
        {
            Debug.Log("Solo se puede colocar una puerta basica"); ;
        }

        for (int i = 0; i < puerta_boss; i++)
        {
            exito = exito && SetBossDoor(room);
            if (!exito)
            {
                return exito;
            }
        }

        for (int i = 0; i < puerta_extra; i++)
        {
            exito = exito & SetDoorEx(room);
            if (!exito)
            {
                return exito;
            }
        }

        for (int i = 0; i < pelea; i++)
        {
            exito = exito && SetFight(room);
            if (!exito)
            {
                return exito;
            }
        }

        for (int i = 0; i < tesoro; i++)
        {
            exito = exito && SetTreasure(room);
            if (!exito)
            {
                return exito;
            }
        }

        for (int i = 0; i < jefe; i++)
        {
            exito = exito && SetBoss(room);
            if (!exito)
            {
                return exito;
            }
        }

        return exito;
    }

    //--------------------------------------------------------------------------------------------------
    //------------------------------------------------Habitacion---------------------------------------
    //--------------------------------------------------------------------------------------------------

    struct habitacion_t
    {
        string name;
        int reqMin;
        int reqMax;
        string depende;
        bool necesario;

        public habitacion_t(string n, int min, int max, bool ne)
        {
            name = n;
            reqMax = max;
            reqMin = min;
            necesario = ne;
            depende = null;
        }
        public void dependencia(string h)
        {
            depende = h;
        }
    }
    public class habitacion
    {
        public int ID;
        public string name;
        public habitacion[] coneciones;
        public int actConec=0;
        public int px;
        public int py;
        int size;
        string dep;
        public habitacion(int x,int y,int s,int id)
        {
            ID = id;
            px = x;
            py = y;
            size = s;
            dep = null;
        }
        public void dependencia(string d)
        {
            dep = d;
        }
        public void IniConexion(int c)
        {
            coneciones = new habitacion[c];
        }
        public void NewConexion(habitacion C)
        {
            coneciones[actConec] = C;
            actConec++;
        }
        public void SetName(string N)
        {
            name = N;
        }

    }
    //--------------------------------------------------------------------------------------------------
    //----------------------------------------------------------------------Conectividad y pasajes------
    //--------------------------------------------------------------------------------------------------
    List<Room> ProcessMap(int[,] reg,int[,] map, bool Bl=false)
    {
        
        List<List<Coord>> wallRegions = GetRegions(map,1);
        int wallThresholdSize = MinReg;
        
        foreach (List<Coord> wallRegion in wallRegions)
        {
            if (wallRegion.Count < wallThresholdSize)
            {
                foreach (Coord tile in wallRegion)
                {
                    map[tile.tileX, tile.tileY] = 0;
                }
            }
        }

        List<List<Coord>> roomRegions = GetRegions(map,0);
        int roomThresholdSize = MinRoom;
        List<Room> survivingRooms = new List<Room>();

        foreach (List<Coord> roomRegion in roomRegions)
        {
            if (roomRegion.Count < roomThresholdSize)
            {
                foreach (Coord tile in roomRegion)
                {
                    map[tile.tileX, tile.tileY] = 1;
                }
            }
            else
            {
                survivingRooms.Add(new Room(roomRegion, map));
            }
        }
        survivingRooms.Sort();
        survivingRooms[0].isMainRoom = true;
        survivingRooms[0].isAccessibleFromMainRoom = true;

        if (Bl)
        {
            int IDtmp = 0;
            foreach (Room room in survivingRooms)
            {
                room.sizeTYPE = GetTYPEReg(room.roomSize);
                room.ID = IDtmp;
                IDtmp++;
            }
            return ConnectClosestRooms(reg, survivingRooms);
        }
        return survivingRooms;
    }
    List<List<Coord>> GetRegions(int[,] map,int tileType)
    {
        List<List<Coord>> regions = new List<List<Coord>>();
        int[,] mapFlags = new int[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (mapFlags[x, y] == 0 && map[x, y] == tileType)
                {
                    List<Coord> newRegion = GetRegionTiles(map,x, y);
                    regions.Add(newRegion);

                    foreach (Coord tile in newRegion)
                    {
                        mapFlags[tile.tileX, tile.tileY] = 1;
                    }
                }
            }
        }

        return regions;
    }
    List<Coord> GetRegionTiles(int[,] map,int startX, int startY)
    {
        List<Coord> tiles = new List<Coord>();
        int[,] mapFlags = new int[width, height];
        int tileType = map[startX, startY];

        Queue<Coord> queue = new Queue<Coord>();
        queue.Enqueue(new Coord(startX, startY));
        mapFlags[startX, startY] = 1;

        while (queue.Count > 0)
        {
            Coord tile = queue.Dequeue();
            tiles.Add(tile);

            for (int x = tile.tileX - 1; x <= tile.tileX + 1; x++)
            {
                for (int y = tile.tileY - 1; y <= tile.tileY + 1; y++)
                {
                    if (IsInMapRange(x, y) && (y == tile.tileY || x == tile.tileX))
                    {
                        if (mapFlags[x, y] == 0 && map[x, y] == tileType)
                        {
                            mapFlags[x, y] = 1;
                            queue.Enqueue(new Coord(x, y));
                        }
                    }
                }
            }
        }

        return tiles;
    }
    bool IsInMapRange(int x, int y)
    {
        return x >= 0 && x < width && y >= 0 && y < height;
    }
    List<Room> ConnectClosestRooms(int[,] reg,List<Room> allRooms, bool forceAccessibilityFromMainRoom = false)
    {

        List<Room> roomListA = new List<Room>();
        List<Room> roomListB = new List<Room>();

        if (forceAccessibilityFromMainRoom)
        {
            foreach (Room room in allRooms)
            {
                if (room.isAccessibleFromMainRoom)
                {
                    roomListB.Add(room);
                }
                else
                {
                    roomListA.Add(room);
                }
            }
        }
        else
        {
            roomListA = allRooms;
            roomListB = allRooms;
        }

        int bestDistance = 0;
        Coord bestTileA = new Coord();
        Coord bestTileB = new Coord();
        Room bestRoomA = new Room();
        Room bestRoomB = new Room();
        bool possibleConnectionFound = false;

        foreach (Room roomA in roomListA)
        {
            if (!forceAccessibilityFromMainRoom)
            {
                possibleConnectionFound = false;
                if (roomA.connectedRooms.Count > 0)
                {
                    continue;
                }
            }

            foreach (Room roomB in roomListB)
            {
                if (roomA == roomB || roomA.IsConnected(roomB))
                {
                    continue;
                }

                for (int tileIndexA = 0; tileIndexA < roomA.edgeTiles.Count; tileIndexA++)
                {
                    for (int tileIndexB = 0; tileIndexB < roomB.edgeTiles.Count; tileIndexB++)
                    {
                        
                        Coord tileA = roomA.edgeTiles[tileIndexA];
                        Coord tileB = roomB.edgeTiles[tileIndexB];

                        reg[tileA.tileX, tileA.tileY] = roomA.sizeTYPE;
                        reg[tileB.tileX, tileB.tileY] = roomB.sizeTYPE;

                        int distanceBetweenRooms = (int)(Mathf.Pow(tileA.tileX - tileB.tileX, 2) + Mathf.Pow(tileA.tileY - tileB.tileY, 2));

                        if (distanceBetweenRooms < bestDistance || !possibleConnectionFound)
                        {
                            bestDistance = distanceBetweenRooms;
                            possibleConnectionFound = true;
                            bestTileA = tileA;
                            bestTileB = tileB;
                            bestRoomA = roomA;
                            bestRoomB = roomB;
                        }
                    }
                }
            }
            if (possibleConnectionFound && !forceAccessibilityFromMainRoom)
            {
                CreatePassage(bestRoomA, bestRoomB, bestTileA, bestTileB);
            }
        }

        if (possibleConnectionFound && forceAccessibilityFromMainRoom)
        {
            CreatePassage(bestRoomA, bestRoomB, bestTileA, bestTileB);
            ConnectClosestRooms(reg,allRooms, true);
        }

        if (!forceAccessibilityFromMainRoom)
        {
            ConnectClosestRooms(reg,allRooms, true);
        }
        return allRooms;
    }
    void CreatePassage(Room roomA, Room roomB, Coord tileA, Coord tileB)
    {
        Room.ConnectRooms(roomA, roomB);
        //Debug.DrawLine(CoordToWorldPoint(tileA), CoordToWorldPoint(tileB), Color.green, 100);

        List<Coord> line = GetLine(tileA, tileB);
        foreach (Coord c in line)
        {
            DrawCircle(c, PassageRadius);
        }
    }
    void DrawCircle(Coord c, int r)
    {
        for (int x = -r; x <= r; x++)
        {
            for (int y = -r; y <= r; y++)
            {
                if (x * x + y * y <= r * r)
                {
                    int drawX = c.tileX + x;
                    int drawY = c.tileY + y;
                    if (IsInMapRange(drawX, drawY))
                    {
                        mapa[drawX, drawY] = 0;
                    }
                }
            }
        }
    }
    List<Coord> GetLine(Coord from, Coord to)
    {
        List<Coord> line = new List<Coord>();

        int x = from.tileX;
        int y = from.tileY;

        int dx = to.tileX - from.tileX;
        int dy = to.tileY - from.tileY;

        bool inverted = false;
        int step = Math.Sign(dx);
        int gradientStep = Math.Sign(dy);

        int longest = Mathf.Abs(dx);
        int shortest = Mathf.Abs(dy);

        if (longest < shortest)
        {
            inverted = true;
            longest = Mathf.Abs(dy);
            shortest = Mathf.Abs(dx);

            step = Math.Sign(dy);
            gradientStep = Math.Sign(dx);
        }

        int gradientAccumulation = longest / 2;
        for (int i = 0; i < longest; i++)
        {
            line.Add(new Coord(x, y));

            if (inverted)
            {
                y += step;
            }
            else
            {
                x += step;
            }

            gradientAccumulation += shortest;
            if (gradientAccumulation >= longest)
            {
                if (inverted)
                {
                    x += gradientStep;
                }
                else
                {
                    y += gradientStep;
                }
                gradientAccumulation -= longest;
            }
        }

        return line;
    }
    Vector3 CoordToWorldPoint(Coord tile)
    {
        return new Vector3(-width / 2 + .5f + tile.tileX, 2, -height / 2 + .5f + tile.tileY);
    }
    Vector3 CoordToWorldPoint2(Coord tile)
    {
        return new Vector3((tile.tileX)-160, 2,(tile.tileY) );
    }

    class Room : IComparable<Room>
    {

        public List<Coord> tiles;
        public List<Coord> edgeTiles;
        public List<Room> connectedRooms;
        public GameObject Symbol;
        public GameObject Objt;
        public int roomSize;
        public bool isAccessibleFromMainRoom;
        public bool isMainRoom;

        public int ID;
        public string TYPE;
        public int sizeTYPE;
        public Room DependenceID;
        public bool isDependent;

        public Room()
        {
        }

        public Room(List<Coord> roomTiles, int[,] map)
        {
            isDependent = false;
            TYPE = "SEvent";
            tiles = roomTiles;
            roomSize = tiles.Count;
            connectedRooms = new List<Room>();

            edgeTiles = new List<Coord>();
            foreach (Coord tile in tiles)
            {
                for (int x = tile.tileX - 1; x <= tile.tileX + 1; x++)
                {
                    for (int y = tile.tileY - 1; y <= tile.tileY + 1; y++)
                    {
                        if (x == tile.tileX || y == tile.tileY)
                        {
                            try
                            {
                                if (map[x, y] > 0)
                                {
                                    edgeTiles.Add(tile);
                                }
                            }
                            catch (Exception e)
                            {
                                //Debug.Log("error X=" + x.ToString() + " Y=" + y.ToString());
                            }
                            
                        }
                    }
                }
            }
        }
        public void SetAccessibleFromMainRoom()
        {
            if (!isAccessibleFromMainRoom)
            {
                isAccessibleFromMainRoom = true;
                foreach (Room connectedRoom in connectedRooms)
                {
                    connectedRoom.SetAccessibleFromMainRoom();
                }
            }
        }

        public static void ConnectRooms(Room roomA, Room roomB)
        {
            if (roomA.isAccessibleFromMainRoom)
            {
                roomB.SetAccessibleFromMainRoom();
            }
            else if (roomB.isAccessibleFromMainRoom)
            {
                roomA.SetAccessibleFromMainRoom();
            }
            roomA.connectedRooms.Add(roomB);
            roomB.connectedRooms.Add(roomA);
        }

        public bool IsConnected(Room otherRoom)
        {
            return connectedRooms.Contains(otherRoom);
        }

        public int CompareTo(Room otherRoom)
        {
            return otherRoom.roomSize.CompareTo(roomSize);
        }
        public void ShowSymbol()
        {
            Objt = GameObject.Instantiate(Symbol, new Vector3(tiles[0].tileX-160,1, tiles[0].tileY), Quaternion.identity);
            Objt.name = "ID" + ID.ToString();
        }
        public void DelSymbol()
        {
            GameObject borrar;
            borrar = GameObject.Find("ID"+ID.ToString());
            Destroy(borrar);
        }
    }
    //--------------------------------------------------------------------------------------------------
    //--------------------------------------------------------------------------------Estructuras-------
    //--------------------------------------------------------------------------------------------------
    struct coordenedas
    {
        public int CordX;
        public int CordY;
        public coordenedas(int x, int y)
        {
            CordX = x;
            CordY = y;
        }
    }
    struct Coord
    {
        public int tileX;
        public int tileY;

        public Coord(int x, int y)
        {
            tileX = x;
            tileY = y;
        }
    }
    struct Poblacion
    {

        public int[,] Ind_1;                       // Individuo 1
        public int[,] Ind_2;                       // Individuo 2
        public int[,] Ind_3;                       // Individuo 3
        public int[,] Ind_4;                       // Individuo 4
        public int[,] Ind_5;                       // Individuo 5
        int Wd;                             // weight
        int Hg;                             // Height
        bool initialized;           // verifica si las matrices estan inicializadas
        
        public Poblacion(int W, int H)     //inicializar las matrices
        {
            Wd = W;
            Hg = H;

            Ind_1 = new int[Wd, Hg];
            Ind_2 = new int[Wd, Hg];
            Ind_3 = new int[Wd, Hg];
            Ind_4 = new int[Wd, Hg];
            Ind_5 = new int[Wd, Hg];

            initialized = true;
        }
        
    }

    //--------------------------------------REGLAS DE GENERACION --------BETA
    struct salaT
    {
        public int SIZE;
        int p;
        public int[] Px;
        public int[] Py;

        public salaT(int s,int sz)
        {
            p = 0;
            SIZE = s;
            Px = new int[sz];
            Py = new int[sz];
        }

        public void setPos(int x, int y)
        {
            Px[p] = x;
            Py[p] = y;
            p = p + 1;
        }

    }
    /*
    bool TieneEspacio(Grupo_matrices mp)
    {
        int cont = 0;
        for(int i = 0; i < mp.rooms; i++)
        {
            if (mp.regionCategorie[i] == 1 || mp.regionCategorie[i] == 2)
            {
                cont++;
            }
            if (cont>=2)
            {
                break;
            }
            if (i == mp.rooms - 1)
            {
                return false;
            }
        }
        for (int i = 0; i < mp.rooms; i++)
        {
            if (mp.regionCategorie[i] == 3 || mp.regionCategorie[i] == 4)
            {
                cont++;
            }
            if (cont >= 3)
            {
                break;
            }
            if (i == mp.rooms - 1)
            {
                return false;
            }
        }
        return true;
        
    }

    void RoomFitness(Grupo_matrices mp)
    {
        if (TieneEspacio(mp))
        {

        }
    }
    */
    //--------------------------------------REGLAS DE GENERACION --------BETA
    //--------------------------------------REGLAS DE GENERACION --------BETA
    //--------------------------------------REGLAS DE GENERACION --------BETA

    class Grupo_matrices
    {
        public int[,] mapa;
        public int[,] regiones;
        public int[] regionSize;
        public int[] regionCategorie;
        public List<Room> salas;
        //public salaT[] salas;

        public int rooms;                   // numero de salas
        public int[,] formaPrima;           // froma prima del mapa- primera version generada aleatoriamente

        public int[,] papa;                 // mapa padre - en caso de evolucion
        public int[,] mama;                 // mapa madre - en caso de evolucion
        public string TYPE;                 //N = natural           E = evolucion           C = cruze       M = mutacion

        public Grupo_matrices(int w, int h, int t)
        {
            mapa = new int[w, h];
            formaPrima = new int[w, h];
            regiones = new int[w, h];
            regionSize = new int[t];
            regionCategorie = new int[t];
            rooms = 0;
            TYPE = "N";
            papa = new int[w, h];
            mama = new int[w, h];
        }
        public void swap()
        {
            int[,] tmp = mapa;
            mapa = regiones;
            regiones = tmp;
        }
        public void swap2()
        {
            int[,] tmp = mapa;
            mapa = formaPrima;
            formaPrima = tmp;
        }
        public List<Room> getSalasf()
        {
            return salas;
        }


    }
    struct Caracteristicas
    {
        //caracteristicas de mapa
        public int MapWidth;
        public int MapHeight;
        public int MapRooms;
        public int[] MapCategories;

        //caracterstigas post game
        public float UserTimeComplete;       //Tiempo en el que completo el juego
        public int UserExploration;        //Porcentaje del mapa explorado
        public int UserScore;              //Puntaje del personaje
        public int UserOpinion;            //Opinion del usuario
        public bool UserClear;             //El usuario temino todo el jueg

        public Caracteristicas(float c1, int c2, int c3, int c4, bool c5, int Hg, int Wd, int Rm, int[] Rmc)
        {
            UserTimeComplete = c1;
            UserExploration = c2;
            UserScore = c3;
            UserOpinion = c4;
            UserClear = c5;
            MapHeight = Hg;
            MapWidth = Wd;
            MapRooms = Rm;
            MapCategories = Rmc;
        }
        public int get_H()
        {
            return MapHeight;
        }
        public int get_W()
        {
            return MapWidth;
        }
        public int get_rooms()
        {
            return MapRooms;
        }
        public int[] get_categ()
        {
            return MapCategories;
        }
        public float get_timeCom()
        {
            return UserTimeComplete;
        }
        public int get_explo()
        {
            return UserExploration;
        }
        public int get_score()
        {
            return UserScore;
        }
        public int get_opinion()
        {
            return UserOpinion;
        }
        public bool get_clear()
        {
            return UserClear;
        }
    }
    class Reglas
    {
        int NumInicios = 1;
        int NumPeleas = 1;
        int NumTesoros = 1;
        int NumBosses = 0;
        int NumPuertas = 1;
        int NumPuertasEx = 0;
        int NumPuertasBoss = 0;
    }
}
/*
if (Input.GetKeyUp("c"))
{
    for(int veces=0;veces < 100; veces++)
    {
        int[,] mapaInd = new int[width, height];
        int[,] regionesInd = new int[width, height];
        int[] regionSizeInd = new int[100];
        int[] regionCategorieInd = new int[100];
        Grupo_matrices GPInd;
        GPInd = newMap(mapaInd, regionesInd, regionSizeInd, regionCategorieInd);
        mapaInd = GPInd.mapa;
        regionesInd = GPInd.regiones;
        regionSizeInd = GPInd.regionSize;
        regionCategorieInd = GPInd.regionCategorie;


        int totalRooms;
        totalRooms = detectRegions(mapaInd, regionSizeInd, regionesInd, regionCategorieInd, height, width);
        GPInd.rooms = totalRooms;
        string msg = printCateg(regionCategorieInd);
        msg = totalRooms.ToString() + ", " + msg;
        GenerateResult("reultados", msg);

        GPInd.swap();
        //GPInd.swap2();

        MapsPull[Max_matrix] = GPInd;
        Max_matrix++;
        Debug.Log(veces.ToString());
    }
    Debug.Log("Se termino");
}*/