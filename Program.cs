class Program
{
    // Método principal (punto de entrada) para probar el parser.
    public static void Main()
    {
        // Especificar directamente la ruta del archivo fuente aquí
        string rutaArchivo = @"C:\Users\crist\OneDrive\Escritorio\AnalizadorSintactico\Analizador-Lexico-C-\test.txt";

        // Crear una instancia del lexer con la ruta del archivo
        Lexer lexer = new Lexer(rutaArchivo);

        // Tokenizar el contenido del archivo
        List<Token> listaTokens = lexer.Tokenizar();

        // Preguntar al usuario qué acción desea realizar
        Console.WriteLine("¿Qué deseas hacer?");
        Console.WriteLine("1. Imprimir tabla de tokens");
        Console.WriteLine("2. Generar árbol sintáctico");
        Console.Write("Selecciona una opción (1 o 2): ");
        string opcion = Console.ReadLine();

        // Verificar la opción ingresada por el usuario
        if (opcion == "1"){
            // Imprimir tabla de tokens
            lexer.ImprimirTablaTokens(listaTokens);
        }
        else if (opcion == "2"){
            // Convertir la lista de tokens a un arreglo de cadenas con solo los valores
            string[] tokens = listaTokens.Select(token => token.Valor).ToArray();

            // Crear una instancia del analizador sintáctico con los tokens generados
            AnalizadorSintactico parser = new AnalizadorSintactico(tokens);

            // Intentar analizar los tokens
            NodoExpresion? arbolSintactico = parser.Analizar();

            // Si el análisis devuelve un árbol válido, imprimirlo
            if (arbolSintactico != null)
            {
                Console.WriteLine("Tokens: " + string.Join(", ", tokens));
                Console.WriteLine("Árbol Sintáctico:");
                AnalizadorSintactico.ImprimirArbol(arbolSintactico);
                Console.WriteLine("Árbol Sintáctico horizontal:");
                AnalizadorSintactico.ImprimirArbolHorizontal(arbolSintactico);
                // parser.ImprimirTablaSimbolos();
                
            }
            else
            {
                Console.WriteLine("No se pudo generar un árbol sintáctico válido.");
            }
        }
        else
        {
            Console.WriteLine("Opción no válida. Por favor selecciona 1 o 2.");
        }
    }
}
