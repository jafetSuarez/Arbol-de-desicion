using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArbolDecision
{
    class Persona
    {
        public String nombre;
        public int genero; // 1 masculino, 2 femenino
        public int edad;
        public int asalariado; // 1 sí, 2 no
        public ArrayList compras = new ArrayList();  // 1 joyeria, 2 cosmeticos, 3 cocina, 4 videojuegos, 5 deportes, 6 herramientas; 
        public Persona(int semilla)
        {
            var rnd = new Random(semilla + DateTime.Now.Millisecond);

            nombre = "nombre";

            genero = rnd.Next(1,3);

            edad = rnd.Next (18, 61);

            asalariado = rnd.Next(1,3);

            //Genera las compras de las personas.
            for (int i = 0; i < 3; i++)
            {
                if (genero == 2)
                {

                    if (edad < 25 && asalariado == 2) { compras.Add(rnd.Next(2, 5)); }

                    else if (edad >= 25 && asalariado == 1) { compras.Add(rnd.Next(1, 4)); }

                    else if (edad < 25 && asalariado == 1) { compras.Add(rnd.Next(1, 6)); }

                    else { compras.Add(rnd.Next(1, 4)); };
                }

                else {

                    if (edad < 25 && asalariado == 2) { compras.Add(rnd.Next(3, 6)); }

                    else if (edad >= 25 && asalariado == 1) { compras.Add(rnd.Next(3, 7)); }

                    else if (edad < 25 && asalariado == 1) { compras.Add(rnd.Next(1, 6)); }

                    else { compras.Add(rnd.Next(3, 7)); };
                }
            }
        }

        public Persona(String nombre, int genero, int edad, int asalariado)
        {
            this.nombre = nombre;
            this.genero = genero;
            this.edad = edad;
            this.asalariado = asalariado;
        }
    }

    class Arbol
    {
        public string nombre = "";
        public ArrayList personasHojaAnterior = new ArrayList();
        internal ArrayList si = new ArrayList();
        public ArrayList no = new ArrayList();
        public Arbol der;
        public Arbol izq;
        public Arbol(string nombre, ArrayList personasHojaAnterior)
        {
            switch (nombre)
            {
                case "Genero":
                    foreach (Persona p in personasHojaAnterior)
                    {
                        //Pregunta si la persona es de genero masculino;
                        if (p.genero == 1) 
                            si.Add(p);
                        else no.Add(p);
                    }
                    break;
                case "Edad":
                    foreach (Persona p in personasHojaAnterior)
                    {
                        //Pregunta si la edad de la persona es mayor o igual a 25 años;
                        if (p.edad >= 25)
                            si.Add(p);
                        else no.Add(p);
                    }
                    break;
                case "Asalariado":
                    foreach (Persona p in personasHojaAnterior)
                    {
                        //Pregunta la persona es asalariada;
                        if (p.asalariado == 1)
                            si.Add(p);
                        else no.Add(p);
                    }
                    break;
                default:
                    break;
            }
        }

    }

    class KMeans
    {
        static int size = 0;
        static int assignments = 0;
        static int comparisons = 0;
        static int executedLines = 0;
        static int numClusters = 3;


        internal void init(int numSize, string sort)
        {
            Console.WriteLine("\nComenzar agrupación k-means \n");

            size = numSize;
            double[][] rawData = new double[numSize][];
            Random random = new Random();//declaracion del random para llenar la matriz

            // Genera Matriz de datos Ascendentes
            if (sort == "A" || sort == "a")
                for (int i = 0; i < numSize; i++) { rawData[i] = new double[] { random.Next(0, 70), random.Next(0, 600) }; Array.Sort(rawData[i]); }

            // Genera Matriz de datos Descendentes
            if (sort == "B" || sort == "b")
            {
                for (int i = 0; i < numSize; i++) { rawData[i] = new double[] { random.Next(0, 70), random.Next(0, 600) }; Array.Sort(rawData[i]); }
                Array.Reverse(rawData);
            }

            // Genera Matriz de datos Aleatorios
            if (sort == "C" || sort == "c")

                for (int i = 0; i < numSize; i++) rawData[i] = new double[] { random.Next(0, 70), random.Next(0, 600) };




            Console.WriteLine("Datos sin agrupar:\n");
            Console.WriteLine(" -- Altura & Peso");
            Console.WriteLine("-------------------");
            ShowData(rawData, 1, true, true);

            DateTime Time1 = DateTime.Now;
            int[] clustering = Cluster(rawData, numClusters);
            DateTime Time2 = DateTime.Now;
            Console.WriteLine("\n Fin de agrupación k-means \n");
            TimeSpan total = new TimeSpan(Time2.Ticks - Time1.Ticks);
            Console.Write("Tiempo de ejecución:  " + total.ToString() + " \n");
            Console.WriteLine("\n Asignaciones: " + assignments + " \n");
            Console.WriteLine("\n Comparaciones: " + comparisons + " \n");
            Console.WriteLine("\n Lineas ejecutadas: " + executedLines + " \n");
        }

        // ============================================================================

        public static int[] Cluster(double[][] rawData, int numClusters)
        {
            // k-means clustering
            // index   return  es tuple ID, cell es cluster ID
            // ex: [2 1 0 0 2 2] significa que la tupla 0 es el clúster 2, la tupla 1 es el clúster 1, la tupla 2 es el clúster 0, la tupla 3 es el clúster 0, etc.
            // Una agrupación alternativa DS para ahorrar espacio es usar la clase .NET BitArray

            double[][] data = Normalized(rawData);
            bool changed = true; // ¿hubo algún cambio en al menos una asignación de grupo?
            bool success = true; //¿fueron todos los medios capaces de ser calculados? (sin clústeres de conteo cero)

            // init clustering [] para que todo comience
            // una alternativa es inicializar, a tuplas seleccionadas al azar
            // entonces el loop de procesamiento es
            // loop
            //      clustering
            //    update means
            // end loop

            int[] clustering = InitClustering(data.Length, numClusters, 0); // inicialización semi-random
            double[][] means = Allocate(numClusters, data[0].Length); // pequeña conveniencia

            int maxCount = data.Length * size; // comprobación de validez
            int ct = 0;
            comparisons++;
            executedLines++;
            while (changed == true && success == true && ct < maxCount)
            {
                ++ct; assignments++; // k-means típicamente converge muy rápidamente
                success = UpdateMeans(data, clustering, means); assignments++; executedLines++; // calcular nuevos cluster, si es posible. No hay efecto si falla
                changed = UpdateClustering(data, clustering, means); assignments++; executedLines++; // (re) asignar tuplas a los clusters. No hay efecto si falla
                comparisons++;
            }

            // Considere agregar medias [] [] como un parámetro de salida - el medio final podría ser calculado
            // los medios finales son útiles en algunos escenarios (por ejemplo, discretización y centroides RBF)
            // y aunque puede calcular los medios finales de la agrupación final, en algunos casos
            // tiene sentido devolver los medios (a expensas de algunas fealdades de firma de métodos)
            //
            // otra alternativa es devolver, como un parámetro de salida, alguna medida de bondad del clúster
            // como la distancia promedio entre los clusters, o la distancia promedio entre las tuplas en
            // un cluster, o una combinación ponderada de ambos
            return clustering;
        }

        private static double[][] Normalized(double[][] rawData)
        {
            // normaliza los datos sin procesar mediante el calculo (x - mean) / stddev
            // la alternativa principal es min-max:
            // v' = (v - min) / (max - min)

            // hace una copia de los datos de entrada
            double[][] result = new double[rawData.Length][];

            for (int i = 0; i < rawData.Length; ++i)
            {
                result[i] = new double[rawData[i].Length];
                Array.Copy(rawData[i], result[i], rawData[i].Length);
            }

            for (int j = 0; j < result[0].Length; ++j) // cada col
            {
                double colSum = 0.0;
                for (int i = 0; i < result.Length; ++i)
                    colSum += result[i][j];
                double mean = colSum / result.Length;
                double sum = 0.0;
                for (int i = 0; i < result.Length; ++i)
                    sum += (result[i][j] - mean) * (result[i][j] - mean);
                double sd = sum / result.Length;
                for (int i = 0; i < result.Length; ++i)
                    result[i][j] = (result[i][j] - mean) / sd;
            }
            return result;
        }

        private static int[] InitClustering(int numTuples, int numClusters, int randomSeed)
        {
            // inicialización del agrupamiento semi-aleatorio (al menos una tupla en cada grupo)
            // considerar alternativas, especialmente la inicialización de k-means ++,
            // o en lugar de asignar aleatoriamente cada tupla a un clúster, selecciona
            // numClusters de las tuplas como centroides iniciales / means luego usa
            // esos medios para asignar cada tupla a un grupo inicial.
            Random random = new Random(randomSeed);
            int[] clustering = new int[numTuples];
            //-
            comparisons++;
            executedLines++;
            for (int i = 0; i < numClusters; ++i) // asegura de que cada grupo tenga al menos una tupla
                clustering[i] = i; assignments++; comparisons++; executedLines++;

            comparisons++;
            executedLines++;
            for (int i = numClusters; i < clustering.Length; ++i)
                clustering[i] = random.Next(0, numClusters); assignments++; comparisons++; executedLines++; // otras asignaciones al azar
            return clustering;
        }

        private static double[][] Allocate(int numClusters, int numColumns)
        {
            // asignador de matriz de conveniencia para Cluster()
            double[][] result = new double[numClusters][];
            comparisons++;
            executedLines++;
            for (int k = 0; k < numClusters; ++k)
                result[k] = new double[numColumns]; assignments++; comparisons++; executedLines++;
            return result;
        }

        private static bool UpdateMeans(double[][] data, int[] clustering, double[][] means)
        {
            // Devuelve returns false si hay un cluster que no tiene tuplas asignadas
            // parameter significa que [] [] es realmente un parámetro ref

            // verifica los conteos de clúster
            // puede omitir esta comprobación si InitClustering y UpdateClustering
            // ambos garantizan al menos una tupla en cada clúster (generalmente verdadero)
            int numClusters = means.Length;
            int[] clusterCounts = new int[numClusters];
            //-
            comparisons++;
            executedLines++;
            for (int i = 0; i < data.Length; ++i)
            {
                int cluster = clustering[i]; assignments++; executedLines++;
                ++clusterCounts[cluster]; assignments++; executedLines++;
                comparisons++;
            }

            comparisons++;
            executedLines++;
            for (int k = 0; k < numClusters; ++k)
            {
                comparisons++;
                executedLines++;
                if (clusterCounts[k] == 0)
                {
                    return false; // mal agrupamiento. no hay cambio en los medios [] []
                }
                comparisons++;
            }


            // actualización, cero significa que puede usarse como matriz de cero
            comparisons++;
            executedLines++;
            for (int k = 0; k < means.Length; ++k)
            {
                comparisons++;
                executedLines++;
                for (int j = 0; j < means[k].Length; ++j)
                    means[k][j] = 0.0; assignments++; comparisons++; executedLines++;
                comparisons++;
            }

            comparisons++;
            executedLines++;
            for (int i = 0; i < data.Length; ++i)
            {
                int cluster = clustering[i]; assignments++; executedLines++;
                comparisons++;
                executedLines++;
                for (int j = 0; j < data[i].Length; ++j)
                    means[cluster][j] += data[i][j]; assignments++; comparisons++; executedLines++; // acumular sum
                comparisons++;
            }

            comparisons++;
            executedLines++;
            for (int k = 0; k < means.Length; ++k)
            {
                comparisons++;
                executedLines++;
                for (int j = 0; j < means[k].Length; ++j)
                    means[k][j] /= clusterCounts[k]; assignments++; comparisons++; executedLines++;// peligro de div por 0
                comparisons++;
            }
            return true;
        }

        private static bool UpdateClustering(double[][] data, int[] clustering, double[][] means)
        {
            // (re) asignar cada tupla a un cluster (media más cercana)
            // devuelve falso si no cambian las asignaciones de tupla O
            // si la reasignación daría lugar a un agrupamiento donde
            // uno o más clusters no tienen tuplas.

            int numClusters = means.Length;
            bool changed = false;

            int[] newClustering = new int[clustering.Length]; // resultado propuesto
            Array.Copy(clustering, newClustering, clustering.Length);

            double[] distances = new double[numClusters]; // distancias desde curr tuple a cada media

            comparisons++;
            executedLines++;
            for (int i = 0; i < data.Length; ++i) // pasa a través de cada tupla
            {
                comparisons++;
                executedLines++;
                for (int k = 0; k < numClusters; ++k)
                    distances[k] = Distance(data[i], means[k]); assignments++; comparisons++; executedLines++; // calcular distancias desde curr tuple a todos k significa

                int newClusterID = MinIndex(distances); assignments++; executedLines++; // encontrar ID de media más cercano
                comparisons++;
                executedLines++;
                if (newClusterID != newClustering[i])
                {
                    changed = true; assignments++; executedLines++;
                    newClustering[i] = newClusterID; assignments++; executedLines++; // actualización 
                }
                comparisons++;
            }
            comparisons++;
            executedLines++;
            if (changed == false)
                return false; // no hay cambio, así que rescatar y no actualizar el clustering[][]

            // comprobar los recuentos propuestos del clustering[]

            int[] clusterCounts = new int[numClusters];

            comparisons++; executedLines++;
            for (int i = 0; i < data.Length; ++i)
            {
                int cluster = newClustering[i]; assignments++; executedLines++;
                ++clusterCounts[cluster]; assignments++; executedLines++;
                comparisons++;
            }

            comparisons++; executedLines++;
            for (int k = 0; k < numClusters; ++k)
            {
                comparisons++; executedLines++;
                if (clusterCounts[k] == 0)
                    return false; // mal agrupamiento. no hay cambio en el clustering[][]
                comparisons++;
            }
            Array.Copy(newClustering, clustering, newClustering.Length); // actualización
            return true; // buen agrupamiento y al menos un cambio
        }

        private static double Distance(double[] tuple, double[] mean)
        {
            // Distancia euclidiana entre dos vectores para UpdateClustering ()
            // considerar alternativas como la distancia de Manhattan
            double sumSquaredDiffs = 0.0;
            comparisons++; executedLines++;
            for (int j = 0; j < tuple.Length; ++j)
                sumSquaredDiffs += Math.Pow((tuple[j] - mean[j]), 2); assignments++; comparisons++; executedLines++;
            return Math.Sqrt(sumSquaredDiffs);
        }

        private static int MinIndex(double[] distances)
        {
            // índice de menor valor en matriz
            // helper para UpdateClustering ()
            int indexOfMin = 0;
            double smallDist = distances[0];
            for (int k = 0; k < distances.Length; ++k)
            {
                if (distances[k] < smallDist)
                {
                    smallDist = distances[k];
                    indexOfMin = k;
                }
            }
            return indexOfMin;
        }

        // ============================================================================

        // misc display helpers para demostración  

        static void ShowData(double[][] data, int decimals, bool indices, bool newLine)
        {
            for (int i = 0; i < data.Length; ++i)
            {
                if (indices) Console.Write(i.ToString().PadLeft(3) + " ");
                for (int j = 0; j < data[i].Length; ++j)
                {
                    if (data[i][j] >= 0.0) Console.Write(" ");
                    Console.Write(data[i][j].ToString("F" + decimals) + " ");
                }
                Console.WriteLine("");
            }
            if (newLine) Console.WriteLine("");
        } // ShowData

        static void ShowVector(int[] vector, bool newLine)
        {
            for (int i = 0; i < vector.Length; ++i)
                Console.Write(vector[i] + " ");
            if (newLine) Console.WriteLine("\n");
        }

        static void ShowClustered(double[][] data, int[] clustering, int numClusters, int decimals)
        {
            for (int k = 0; k < numClusters; ++k)
            {
                Console.WriteLine("===================");
                for (int i = 0; i < data.Length; ++i)
                {
                    int clusterID = clustering[i];
                    if (clusterID != k) continue;
                    Console.Write(i.ToString().PadLeft(3) + " ");
                    for (int j = 0; j < data[i].Length; ++j)
                    {
                        if (data[i][j] >= 0.0) Console.Write(" ");
                        Console.Write(data[i][j].ToString("F" + decimals) + " ");
                    }
                    Console.WriteLine("");
                }
                Console.WriteLine("===================");
            } // k
        }
    }

    class ComparaPersonasEdadMayorAMenor : IComparer
    {
        public int Compare(object x, object y)
        {
            return ((Persona)y).edad - ((Persona)x).edad;
        }
    }

    class ComparaPersonasEdadMenorAMayor : IComparer
    {
        public int Compare(object y, object x)
        {
            return ((Persona)y).edad - ((Persona)x).edad;
        }
    }

    class Program
    {
        public static int comparaciones, asignaciones, lineasEjecutadas;
        public static DateTime arbolTime1, arbolTime2;

        //Instancia raiz del arbol con arreglo aleatorio.
        public static Arbol raizArregloAleatorio;
        //Instancia raiz del arbol con arreglo descendente (Mayor a menor por criterio de edad).
        public static Arbol raizArregloDescendente;
        //Instancia raiz del arbol con arreglo ascendente (Menor a mayor por criterio de edad).
        public static Arbol raizArregloAscendente;
        static void Main(string[] args)
        {
            //DECLARACIÓN DE VARIABLES Y ARREGLOS, TAMBIEN INCLUYE CICLO DE MENU PRICIPAL:
            ArrayList personas = new ArrayList();

            //Imprime en consola el menú.
            int menu = 0;
            while (menu == 0)
            {
                Console.Clear();
                Console.WriteLine("Digite 1 para el algoritmo supervisado Arbol de Decisión \nDigite 2 para el algorimo no supervisado K-means");
                String inpt = Console.ReadLine();
                if (inpt.All(char.IsDigit))
                {
                    menu = Convert.ToInt32(inpt);
                    if (menu == 1)
                    {
                        //Variable tam para la representacion del menú del algoritmo supervisado Arbol de Decisión y/o tamaño del arreglo.
                        int tam = 0;
                        int orde = 0;
                        while (tam == 0)
                        {
                            Console.Clear();
                            Console.Write("ARBOL DE DECISIÓN \nDigite el tamaño del arreglo: ");
                            String input = Console.ReadLine();
                            if (input.All(char.IsDigit))
                            {
                                tam = Convert.ToInt32(input);
                                if (tam > 0)
                                {
                                    personas = TamArreglo(tam);
                                    Console.WriteLine("1 para que el arrglo sea aleatorio. \n2 para que el arrglo sea descendente (Mayor a menor por criterio de edad). \n3 para que el arreglo sea ascendente (Menor a mayor por criterio de edad).");
                                    input = Console.ReadLine();
                                    arbolTime1 = DateTime.Now;
                                    if (input.All(char.IsDigit))
                                    {
                                        orde = Convert.ToInt32(input);
                                        switch (orde) {

                                            case 1:
                                                raizArregloAleatorio = Aprendizaje(personas);
                                                break;
                                            case 2:
                                                personas.Sort(new ComparaPersonasEdadMayorAMenor());
                                                raizArregloDescendente = Aprendizaje(personas);
                                                break;
                                            case 3:
                                                personas.Sort(new ComparaPersonasEdadMenorAMayor());
                                                raizArregloAscendente = Aprendizaje(personas);
                                                break;
                                        }
                                        arbolTime2 = DateTime.Now;
                                    }
                                    else tam = 0;
                                }
                                else tam = 0;
                            }
                            else tam = 0;
                        }

                        String e = "";
                        while (e.Equals(""))
                        {
                            Console.Clear();
                            Console.Write("NOTA:\nCrear una persona como modelo para predecir sus posibles compras: \nPara este ejemplo los intems son:  Joyeria, Cosmeticos, Cocina, Videojuegos, Deportes y Herramientas. ");
                            
                            Console.Write("\n\nDigite la edad de la persona, debe ser entre 18 y 60 años de edad: ");
                            e = Console.ReadLine();
                            if (e.All(char.IsDigit))
                            {
                                int ed = Convert.ToInt32(e);
                                if (ed >= 18 && ed <= 60)
                                {
                                    String g = "";
                                    int ge = 0;
                                    while (g.Equals(""))
                                    {
                                        Console.Write("\nDigite 1 para genero masculino o 2 para femenino: ");
                                        g = Console.ReadLine();
                                        if (g.All(char.IsDigit))
                                        {
                                            if (g.Equals("1") || g.Equals('1') || g.Equals("2") || g.Equals('2'))
                                            {
                                                ge = Convert.ToInt32(g);
                                            }
                                            else g = "";
                                        }
                                        else g = "";
                                    }
                                    int asa = 0;
                                    String a = "";
                                    while (a.Equals(""))
                                    {
                                        Console.Write("\nDigite 1 para asalariado o 2 para NO asalariado: ");
                                        a = Console.ReadLine();
                                        if (a.All(char.IsDigit))
                                        {
                                            if (a.Equals("1") || a.Equals('1') || a.Equals("2") || a.Equals('2'))
                                            {
                                                asa = Convert.ToInt32(a);
                                            }
                                            else a = "";
                                        }
                                        else a = "";
                                    }
                                    Persona personaModelo = new Persona("Modelo", ge, ed, asa);

                                    Console.WriteLine("\n\nPosibles compras para el MODELO: ");

                                    Arbol r = null;

                                    switch (orde)
                                    {
                                        case 1:
                                            r = raizArregloAleatorio;
                                            break;
                                        case 2:
                                            r = raizArregloDescendente;
                                            break;
                                        case 3:
                                            r = raizArregloAscendente;
                                            break;
                                    }

                                    foreach (int i in ModeloPredictivo(personaModelo, r ))
                                    {
                                        
                                        switch (i)
                                        {
                                            case 1:
                                                Console.WriteLine(" -> Joyeria  ");
                                                break;
                                            case 2:
                                                Console.WriteLine(" -> Cosmeticos  ");
                                                break;
                                            case 3:
                                                Console.WriteLine(" -> Cocina  ");
                                                break;
                                            case 4:
                                                Console.WriteLine(" -> Videojuegos");
                                                break;
                                            case 5:
                                                Console.WriteLine(" -> Deportes  ");
                                                break;
                                            case 6:
                                                Console.WriteLine(" -> Herramientas  ");
                                                break;

                                        }


                                    }
                                    Console.WriteLine("\n");

                                    TimeSpan total = new TimeSpan(arbolTime2.Ticks - arbolTime1.Ticks);
                                    Console.Write("Tiempo de ejecución:  " + total.ToString() + " \n");
                                    Console.WriteLine("\n Asignaciones: " + asignaciones + " \n");
                                    Console.WriteLine("\n Comparaciones: " + comparaciones + " \n");
                                    Console.WriteLine("\n Lineas ejecutadas: " + (asignaciones+comparaciones) + " \n");
                                }
                                else e = "";
                            }
                            else e = "";
                        }
                    }
                    else if (menu == 2)
                    {
                        KMeans kMeans = new KMeans();
                        //Variable tam para la representacion del menú del algoritmo supervisado Arbol de Decisión y/o tamaño del arreglo.
                        int tam = 0;
                        while (tam == 0)
                        {
                            Console.Clear();
                            Console.Write("K-MEANS \nDigite el tamaño del arreglo: ");
                            String input = Console.ReadLine();
                            if (input.All(char.IsDigit))
                            {
                                tam = Convert.ToInt32(input);
                                if (tam > 0)
                                {
                                    personas = TamArreglo(tam);
                                }
                                else tam = 0;
                            }
                            else tam = 0;
                        }
                        
                        String orden = "";
                        Console.WriteLine(orden);
                        while (orden.Equals(""))
                        {
                            Console.Clear();
                            Console.WriteLine("\rDigite: \na para orden ascendente \nb para orden descendente \nc para orden aleatorio");
                            orden = Console.ReadLine();
                            if (orden.Equals("a") || orden.Equals("A") || orden.Equals("b")  || orden.Equals("B") || orden.Equals("c") || orden.Equals("C"))
                            {
                                kMeans.init(tam, orden);
                            }
                            else orden = "";
                        }                        
                    }
                    else menu = 0;
                }
                else menu = 0;
            }

            //METODOS:
            //Método generador del ArrayList inicial de personas.
            ArrayList TamArreglo(int t)
            {
                ArrayList per = new ArrayList();
                if (t > 0)
                {
                    for (int i = 0; i < t; i++)
                    {
                        Persona p = new Persona(i);
                        p.nombre = "Persona " + i;
                        per.Add(p);
                    }
                }
                return per;
            }

            //Método generador de la estructura del arbol de decisión.
            Arbol Aprendizaje(ArrayList personasHojaAnterior)
            {
                                                                                                                    Arbol raiz = new Arbol("Genero", personasHojaAnterior);
                                                                                                                                             /**/
                                                                                                                                            /**/
                                                     raiz.izq = new Arbol("Edad", raiz.no);/*<---------------------------------------------------------------------------------------->*/raiz.der = new Arbol("Edad", raiz.si);
                                                                           /**/                                                                                                                                /**/
                                                                          /**/                                                                                                                                /**/
                raiz.izq.izq = new Arbol("Asalariado", raiz.izq.no);/*<------->*/raiz.izq.der = new Arbol("Asalariado", raiz.izq.si);/*           */raiz.der.izq = new Arbol("Asalariado", raiz.der.no);/*<------->*/raiz.der.der = new Arbol("Asalariado", raiz.der.si);

                asignaciones = asignaciones + 7;
                return raiz;
            }

            //Método que recibe una persona y predice cuales podrian ser sus compras basado en el arbol de decisión.
            ArrayList ModeloPredictivo(Persona persona, Arbol raiz)
            {
                ArrayList posiblesCompras = new ArrayList();
                comparaciones++;
                if (persona.genero == 1) 
                {
                    comparaciones++;
                    if (persona.edad >= 25)
                    {
                        comparaciones++;
                        if (persona.asalariado == 1)
                        {
                            comparaciones++;
                            for (int i = 0; i < (raiz.der.der.si).Count - 1; i++ )
                            {
                                
                                Persona p = (Persona) (raiz.der.der.si)[i];asignaciones++;

                                comparaciones++;
                                for (int j = 0; j < p.compras.Count - 1; j++)
                                {
                                    comparaciones++;
                                    if (posiblesCompras.Contains(p.compras[j]) == false ){
                                        posiblesCompras.Add(p.compras[j]); asignaciones++;
                                    }
                                    comparaciones++;
                                }
                                comparaciones++;
                            }
                        }
                        else
                        {
                            comparaciones++;
                            for (int i = 0; i < (raiz.der.der.no).Count - 1; i++)
                            {
                                Persona p = (Persona)(raiz.der.der.no)[i]; asignaciones++;
                                comparaciones++;
                                for (int j = 0; j < p.compras.Count - 1; j++)
                                {
                                    comparaciones++;
                                    if (posiblesCompras.Contains(p.compras[j]) == false)
                                    {
                                        posiblesCompras.Add(p.compras[j]); asignaciones++;
                                    }
                                    comparaciones++;
                                }
                                comparaciones++;
                            }
                        }
                    }
                    else
                    {
                        comparaciones++;
                        if (persona.asalariado == 1)
                        {
                            comparaciones++;
                            for (int i = 0; i < (raiz.der.izq.si).Count - 1; i++)
                            {
                                Persona p = (Persona)(raiz.der.izq.si)[i]; asignaciones++;
                                comparaciones++;
                                for (int j = 0; j < p.compras.Count - 1; j++)
                                {
                                    comparaciones++;
                                    if (posiblesCompras.Contains(p.compras[j]) == false)
                                    {
                                        posiblesCompras.Add(p.compras[j]); asignaciones++;
                                    }
                                    comparaciones++;
                                }
                                comparaciones++;
                            }
                        }
                        else
                        {
                            comparaciones++;
                            for (int i = 0; i < (raiz.der.izq.no).Count - 1; i++)
                            {
                                Persona p = (Persona)(raiz.der.izq.no)[i]; asignaciones++;
                                comparaciones++;
                                for (int j = 0; j < p.compras.Count - 1; j++)
                                {
                                    comparaciones++;
                                    if (posiblesCompras.Contains(p.compras[j]) == false)
                                    {
                                        posiblesCompras.Add(p.compras[j]); asignaciones++;
                                    }
                                    comparaciones++;
                                }
                                comparaciones++;
                            }
                        }
                    }
                }
                else
                {
                    comparaciones++;
                    if (persona.edad >= 25)
                    {
                        comparaciones++;
                        if (persona.asalariado == 1)
                        {
                            comparaciones++;
                            for (int i = 0; i < (raiz.izq.der.si).Count - 1; i++)
                            {
                                Persona p = (Persona)(raiz.izq.der.si)[i]; asignaciones++;
                                comparaciones++;
                                for (int j = 0; j < p.compras.Count - 1; j++)
                                {
                                    comparaciones++;
                                    if (posiblesCompras.Contains(p.compras[j]) == false)
                                    {
                                        posiblesCompras.Add(p.compras[j]); asignaciones++;
                                    }
                                    comparaciones++;
                                }
                                comparaciones++;
                            }
                        }
                        else
                        {
                            comparaciones++;
                            for (int i = 0; i < (raiz.izq.der.no).Count - 1; i++)
                            {
                                Persona p = (Persona)(raiz.izq.der.no)[i]; asignaciones++;
                                comparaciones++;
                                for (int j = 0; j < p.compras.Count - 1; j++)
                                {
                                    comparaciones++;
                                    if (posiblesCompras.Contains(p.compras[j]) == false)
                                    {
                                        posiblesCompras.Add(p.compras[j]); asignaciones++;
                                    }
                                    comparaciones++;
                                }
                                comparaciones++;
                            }
                        }
                    }
                    else
                    {
                        comparaciones++;
                        if (persona.asalariado == 1)
                        {
                            comparaciones++;
                            for (int i = 0; i < (raiz.izq.izq.si).Count-1; i++)
                            {
                                Persona p = (Persona)(raiz.izq.izq.si)[i]; asignaciones++;
                                comparaciones++;
                                for (int j = 0; j < p.compras.Count-1; j++)
                                {
                                    comparaciones++;
                                    if (posiblesCompras.Contains(p.compras[j]) == false)
                                    {
                                        posiblesCompras.Add(p.compras[j]); asignaciones++;
                                    }
                                    comparaciones++;
                                }
                                comparaciones++;
                            }
                        }
                        else
                        {
                            comparaciones++;
                            for (int i = 0; i < (raiz.izq.izq.no).Count - 1; i++)
                            {
                                Persona p = (Persona)(raiz.izq.izq.no)[i]; asignaciones++;
                                comparaciones++;
                                for (int j = 0; j < p.compras.Count - 1; j++)
                                {
                                    comparaciones++;
                                    if (posiblesCompras.Contains(p.compras[j]) == false)
                                    {
                                        posiblesCompras.Add(p.compras[j]); asignaciones++;
                                    }
                                    comparaciones++;
                                }
                                comparaciones++;
                            }
                        }
                    }
                }

                return posiblesCompras;
            }
            
            Console.WriteLine("Presione una tecla para terminar");
            Console.ReadKey();
        }
    }
}
