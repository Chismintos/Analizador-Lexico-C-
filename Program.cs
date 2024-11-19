class Program
{
    // Método principal (punto de entrada) para probar el parser.
    public static void Main()
    {
        // Especificar directamente la ruta del archivo fuente aquí
        string rutaArchivo = @"C:\Users\crist\OneDrive\Escritorio\AnalizadorSintactico\Analizador-Lexico-C-\test.txt";

        //------------------------------------------------------------------------------------------------------------

        // Crear una instancia del lexer con la ruta del archivo
        Lexer lexer = new Lexer(rutaArchivo);

        // Tokenizar el contenido del archivo
        List<Token> listaTokens = lexer.Tokenizar();

        //------------------------------------------------------------------------------------------------------------

        // Preguntar al usuario qué acción desea realizar
        Console.WriteLine("¿Qué deseas hacer?");
        Console.WriteLine("1. Imprimir tabla de tokens");
        Console.WriteLine("2. Generar árbol sintáctico");
        Console.WriteLine("3. Generar código intermedio (cuádruplos)");
        Console.Write("Selecciona una opción (1, 2 o 3): ");
        string opcion = Console.ReadLine();

        //------------------------------------------------------------------------------------------------------------

        // Crear una instancia del analizador sintáctico
        string[] tokens = listaTokens.Select(token => token.Valor).ToArray();
        AnalizadorSintactico parser = new AnalizadorSintactico(tokens);

        // Variable para almacenar el árbol sintáctico (fuera del bloque)
        NodoExpresion? arbolSintactico = null;

        // Intentar analizar los tokens
        arbolSintactico = parser.Analizar();

        //------------------------------------------------------------------------------------------------------------


 
        //------------------------------------------------------------------------------------------------------------
        // Verificar la opción ingresada por el usuario
        if (opcion == "1")
        {
            // Imprimir tabla de tokens
            lexer.ImprimirTablaTokens(listaTokens);
        }
        else if (opcion == "2")
        {
            
            // Si el análisis devuelve un árbol válido, imprimirlo
            if (arbolSintactico != null)
            {
                Console.WriteLine("Tokens: " + string.Join(", ", tokens));
                Console.WriteLine("Árbol Sintáctico:");
                AnalizadorSintactico.ImprimirArbol(arbolSintactico);
                Console.WriteLine("Árbol Sintáctico horizontal:");
                AnalizadorSintactico.ImprimirArbolHorizontal(arbolSintactico);
            }
            else
            {
                Console.WriteLine("No se pudo generar un árbol sintáctico válido.");
            }
        }
        else if (opcion == "3")
        {
            // Si ya se generó un árbol sintáctico en el paso 2, lo usamos
            if (arbolSintactico != null)
            {
                // Crear el generador de código intermedio
                GeneradorCodigoIntermedio generador = new GeneradorCodigoIntermedio();

                // Generar el código intermedio a partir del árbol sintáctico
                generador.GenerarCodigoIntermedio(arbolSintactico);

                // Imprimir 
                Console.WriteLine("Código Intermedio No Optimizado:");
                generador.ImprimirCuadruplosEnTabla();
                Console.WriteLine("Código Intermedio Final:");
                generador.ImprimirCodigoIntermedio();
            }
            else
            {
                Console.WriteLine("Primero debes generar el árbol sintáctico (opción 2).");
            }
        }
        else
        {
            Console.WriteLine("Opción no válida. Por favor selecciona 1, 2 o 3.");
        }
    }
}
