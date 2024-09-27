// PARTE DE ANALIZADOR SINTACTICO (PARSER)



class NodoExpresion
{
    // Nodo que representa un valor en la expresión (número u operador) y sus hijos izquierdo y derecho.
    public string Valor { get; set; } // Valor del nodo (operador o número).
    public NodoExpresion? Izquierda { get; set; } // Referencia al nodo hijo izquierdo.
    public NodoExpresion? Derecha { get; set; } // Referencia al nodo hijo derecho.

    // Constructor del nodo, toma el valor del operador o número.
    public NodoExpresion(string valor)
    {
        Valor = valor;
        Izquierda = null; // Inicialización explícita de los hijos como null.
        Derecha = null;
    }
}

class AnalizadorSintactico
{
    // Atributos para manejar la lista de tokens y la posición actual en el análisis.
    private string[] tokens; // Array de tokens (cada elemento es un operador, número o paréntesis).
    private int pos; // Posición actual del token en el array.
    private string? tokenActual; // El token que se está procesando actualmente.

    // Constructor del analizador, inicializa los tokens y la posición.
    public AnalizadorSintactico(string[] tokens)
    {
        this.tokens = tokens;
        this.pos = 0; // Comienza en la primera posición.
        // Si hay tokens, inicializa el token actual, de lo contrario lo deja en null.
        this.tokenActual = tokens.Length > 0 ? tokens[pos] : null; 
    }

    // Avanza al siguiente token en el array.
    private void Avanzar()
    {
        pos++; // Incrementa la posición.
        // Si la nueva posición es válida, actualiza el token actual, de lo contrario, lo deja en null.
        tokenActual = pos < tokens.Length ? tokens[pos] : null;
    }

    // Genera un error de sintaxis con un mensaje personalizado.
    private void Error(string mensaje)
    {
        throw new Exception("Error de sintaxis: " + mensaje); // Lanza una excepción con el mensaje de error.
    }

    // Método principal que inicia el proceso de análisis sintáctico.
    public NodoExpresion Parsear()
    {
        return Expresion(); // Comienza el análisis sintáctico desde el nivel de expresión.
    }

    // Método que procesa una expresión (maneja las operaciones de suma y resta).
    private NodoExpresion Expresion()
    {
        // Comienza evaluando un término (multiplicación/división o un número).
        NodoExpresion nodo = Termino();

        // Mientras el token actual sea un operador de suma o resta.
        while (tokenActual == "+" || tokenActual == "-")
        {
            string operador = tokenActual; // Guarda el operador actual.
            Avanzar(); // Avanza al siguiente token.

            // Procesa el siguiente término (parte derecha de la expresión).
            NodoExpresion nodoDerecho = Termino();

            // Crea un nuevo nodo con el operador como valor, y conecta el nodo izquierdo y derecho.
            NodoExpresion nuevoNodo = new NodoExpresion(operador)
            {
                Izquierda = nodo, // El nodo actual (izquierdo).
                Derecha = nodoDerecho // El nuevo término (derecho).
            };

            // Muestra en consola el árbol parcial construido.
            Console.WriteLine("Nueva expresion: ");
            Console.WriteLine("       " + nodoDerecho.Valor);
            Console.WriteLine("     /");
            Console.WriteLine(operador);
            Console.WriteLine("     \\");
            Console.WriteLine("       " + nodo.Valor);

            // El nodo resultante se convierte en el nuevo nodo para continuar la construcción.
            nodo = nuevoNodo;
        }

        return nodo; // Retorna el nodo raíz de la expresión procesada.
    }

    // Método que procesa un término (maneja multiplicación y división).
    private NodoExpresion Termino()
    {
        // Comienza evaluando un factor (un número o una expresión entre paréntesis).
        NodoExpresion nodo = Factor();

        // Mientras el token actual sea un operador de multiplicación o división.
        while (tokenActual == "*" || tokenActual == "/")
        {
            string operador = tokenActual; // Guarda el operador actual.
            Avanzar(); // Avanza al siguiente token.

            // Procesa el siguiente factor (parte derecha de la operación).
            NodoExpresion nodoDerecho = Factor();

            // Crea un nuevo nodo con el operador como valor y conecta los nodos izquierdo y derecho.
            NodoExpresion nuevoNodo = new NodoExpresion(operador)
            {
                Izquierda = nodo, // Nodo izquierdo.
                Derecha = nodoDerecho // Nodo derecho.
            };

            // Muestra en consola el árbol parcial construido.
            Console.WriteLine("Nuevo termino: ");
            Console.WriteLine("       " + nodoDerecho.Valor);
            Console.WriteLine("     /");
            Console.WriteLine(operador);
            Console.WriteLine("     \\");
            Console.WriteLine("       " + nodo.Valor);

            // El nuevo nodo se convierte en el nodo actual.
            nodo = nuevoNodo;
        }

        return nodo; // Retorna el nodo raíz del término procesado.
    }

    // Método que procesa un factor (número o expresión entre paréntesis).
    private NodoExpresion Factor()
    {
        string token = tokenActual ?? throw new Exception("Token inesperado"); // Verifica si el token es válido.

        // Si el token actual es un número.
        if (int.TryParse(token, out _))
        {
            Avanzar(); // Avanza al siguiente token.
            return new NodoExpresion(token); // Crea y retorna un nodo con el número.
        }
        // Si el token actual es un paréntesis izquierdo.
        else if (token == "(")
        {
            Avanzar(); // Avanza después del '('.
            NodoExpresion nodo = Expresion(); // Procesa la expresión dentro de los paréntesis.
            
            // Verifica que haya un paréntesis derecho para cerrar la expresión.
            if (tokenActual == ")")
            {
                Avanzar(); // Avanza después del ')'.
                return nodo; // Retorna el nodo procesado dentro de los paréntesis.
            }
            else
            {
                Error("Se esperaba ')'"); // Si no hay ')', lanza un error de sintaxis.
            }
        }
        else
        {
            Error("Token inesperado"); // Si el token no es un número o paréntesis, lanza un error.
        }

        throw new Exception("Se alcanzó un estado inesperado"); // Manejo de error genérico para evitar advertencias.
    }

    // Método para imprimir el árbol sintáctico de manera jerárquica.
    
}

class Program
{
    public static void ImprimirArbol(NodoExpresion nodo, int nivel = 0, int lado = 0)
    {
        if (nodo != null)
        {
            // Imprime el subárbol derecho primero.
            ImprimirArbol(nodo.Derecha, nivel + 1, 2);
            // Imprime el valor del nodo actual con sangría según el nivel.
            Console.WriteLine(new string(' ', 4 * nivel) + nodo.Valor);
            // Imprime el subárbol izquierdo.
            ImprimirArbol(nodo.Izquierda, nivel + 1, 1);
        }
    }

    // Método principal (punto de entrada) para probar el parser.
    public static void Main()
    {

        // Expresión a evaluar.
        string expresion = "3 + 5 * ( 10 - 2 ) / 4";
        // Tokeniza la expresión separando por espacios.
        string[] tokens = expresion.Split(' '); // Tokenización básica.

        // Crea una instancia del analizador sintáctico con los tokens.
        AnalizadorSintactico parser = new AnalizadorSintactico(tokens);
        Console.WriteLine("Tokens: " + string.Join(", ", tokens)); // Imprime los tokens.

        // Llama al método de análisis y obtiene el árbol sintáctico.
        NodoExpresion arbol = parser.Parsear();

        // Muestra el árbol sintáctico en consola.
        Console.WriteLine("Árbol Sintáctico:");
        ImprimirArbol(arbol);
    }
}