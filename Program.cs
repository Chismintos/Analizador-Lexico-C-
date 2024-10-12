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

    Console.WriteLine("Tokens: " + string.Join(", ", tokens)); // Imprime los tokens.
    // Crea una instancia del analizador sintáctico con los tokens.
    AnalizadorSintactico parser = new AnalizadorSintactico(tokens);
  
}
}