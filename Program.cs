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

        // Convertir la lista de tokens a un arreglo de cadenas con solo los valores
        string[] tokens = listaTokens.Select(token => token.Valor).ToArray();

        // Imprimir los tokens generados por el lexer
        Console.WriteLine("Tokens: " + string.Join(", ", tokens));

        // Crear una instancia del analizador sintáctico con los tokens generados
        AnalizadorSintactico parser = new AnalizadorSintactico(tokens);

        // Intentar analizar los tokens
        NodoExpresion? arbolSintactico = parser.Analizar();

        // Si el análisis devuelve un árbol válido, imprimirlo
        if (arbolSintactico != null)
        {
            Console.WriteLine("Árbol Sintáctico:");
            AnalizadorSintactico.ImprimirArbol(arbolSintactico);
             
        }
        else
        {
            Console.WriteLine("No se pudo generar un árbol sintáctico válido.");
        }
    }
}
