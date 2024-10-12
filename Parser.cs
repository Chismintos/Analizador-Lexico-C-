
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

         // Llama automáticamente a Parsear() cuando se cree el objeto
        NodoExpresion arbol = Parsear();
        
        // Llama a ImprimirArbol después de generar el árbol
        ImprimirArbol(arbol);
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
        return Asignacion(); // Comienza el análisis sintáctico desde el nivel de expresión.
    }

    public NodoExpresion ExpresionCompleta(){
    // Procesa la parte izquierda de la expresión.
    NodoExpresion izquierda = Expresion();

    // Verifica si hay un signo de igual.
    if (tokenActual == "=")
    {
        Avanzar(); // Avanza al siguiente token.

        // Procesa la parte derecha de la expresión (la variable).
        string variableDerecha = tokenActual ?? throw new Exception("Token inesperado");
        Avanzar(); // Avanza al siguiente token.

        // Crea un nodo para la asignación con la expresión a la izquierda y la variable a la derecha.
        NodoExpresion nodoAsignacion = new NodoExpresion("=")
        {
            Izquierda = izquierda, // Nodo para la expresión.
            Derecha = new NodoExpresion(variableDerecha) // Nodo para la variable.
        };

        return nodoAsignacion; // Retorna el nodo de asignación.
    }
    else
    {
        Error("Se esperaba '=' para la asignación");
    }

    throw new Exception("Se alcanzó un estado inesperado");
}

//Aqui comienza la logica de los operadores
    private NodoExpresion Asignacion()
{
    // Primero procesamos el lado izquierdo: puede ser un identificador o un número.
    NodoExpresion nodoIzquierda = Expresion(); // Cambié el nombre del nodo para reflejar mejor el flujo

    // Verifica si el siguiente token es un operador de asignación.
    if (tokenActual == "=")
    {
        Avanzar(); // Avanzamos al siguiente token, que debería ser la expresión del lado derecho.

        // Procesa la expresión del lado derecho de la asignación.
        NodoExpresion nodoDerecho = Expresion();

        // Crea un nodo de asignación donde el izquierdo es el identificador y el derecho es la expresión.
        NodoExpresion nodoAsignacion = new NodoExpresion("=")
        {
            Izquierda = nodoIzquierda, // El identificador o número en el lado izquierdo.
            Derecha = nodoDerecho // La expresión en el lado derecho.
        };

        return nodoAsignacion; // Retorna el nodo de asignación.
    }
    else
    {
        // Si no es una asignación (no hay un '='), simplemente devolvemos el nodo izquierdo.
        return nodoIzquierda;
    }
}


    // Método auxiliar para verificar si un token es un identificador válido.

    private HashSet<string> palabrasReservadas = new HashSet<string>
    {
    "if", "else", "for", "while", "return", "int", "float", "string", // Agrega más según tu lenguaje
    };
    private bool EsIdentificador(string token)
{
    // Regla 1: Longitud mínima y máxima.
    if (token.Length < 1 || token.Length > 255)
    {
        return false; // Fuera de rango
    }

    // Regla 2: Verifica si el primer carácter es una letra o un guion bajo.
    if (!char.IsLetter(token[0]) && token[0] != '_')
    {
        return false; // El identificador debe comenzar con una letra o un guion bajo.
    }

    // Reglas 3 y 4: Verifica cada carácter.
    foreach (char c in token)
    {
        // Debe ser una letra, un dígito o un guion bajo.
        if (!char.IsLetterOrDigit(c) && c != '_')
        {
            return false; // Caracter no válido
        }
    }

    // Regla 5: Verifica si el identificador es una palabra reservada.
    if (palabrasReservadas.Contains(token))
    {
        return false; // Identificador no puede ser una palabra reservada.
    }

    return true; // Es un identificador válido.
}
    // Método que procesa una expresión (maneja las operaciones de suma y resta).
    private NodoExpresion Expresion()
{
    NodoExpresion nodo = Termino(); // Aquí ahora se procesa la multiplicación y división.

    while (tokenActual == "+" || tokenActual == "-")
    {
        string operador = tokenActual;
        Avanzar();
        NodoExpresion nodoDerecho = Termino();
        NodoExpresion nuevoNodo = new NodoExpresion(operador)
        {
            Izquierda = nodo,
            Derecha = nodoDerecho
        };
        nodo = nuevoNodo;
    }

    return nodo;
}

    // Método que procesa un término (maneja multiplicación y división).
    private NodoExpresion Termino()
{
    NodoExpresion nodo = Potencia(); // Cambiado para que ahora procese la potencia antes.

    while (tokenActual == "*" || tokenActual == "/")
    {
        string operador = tokenActual;
        Avanzar();
        NodoExpresion nodoDerecho = Potencia(); // Usamos Potencia en lugar de Factor aquí.
        NodoExpresion nuevoNodo = new NodoExpresion(operador)
        {
            Izquierda = nodo,
            Derecha = nodoDerecho
        };
        nodo = nuevoNodo;
    }

    return nodo;
}


    
    // Método que maneja los factores (números y expresiones entre paréntesis).
    private NodoExpresion Factor()
{
    if (tokenActual == "(") // Manejo de paréntesis
    {
        Avanzar(); // Avanzamos al siguiente token.
        NodoExpresion expresionDentroDeParentesis = Expresion(); // Procesamos lo que está dentro de los paréntesis.
        if (tokenActual != ")") Error("Se esperaba un paréntesis de cierre.");
        Avanzar(); // Avanzamos al siguiente token después del paréntesis de cierre.
        return expresionDentroDeParentesis;
    }
    else if (int.TryParse(tokenActual, out int numero)) // Si el token actual es un número
    {
        NodoExpresion nodoNumero = new NodoExpresion(numero.ToString()); // Crea un nodo para el número.
        Avanzar(); // Avanzamos al siguiente token.
        return nodoNumero;
    }
    else if (EsIdentificador(tokenActual)) // Si el token actual es un identificador (como "num")
    {
        NodoExpresion nodoIdentificador = new NodoExpresion(tokenActual); // Crea un nodo para el identificador.
        Avanzar(); // Avanzamos al siguiente token.
        return nodoIdentificador;
    }
    else
    {
        Error("Token inesperado en el factor");
        return null;
    }
}

    // Método que procesa una potencia (maneja el operador ^).
    private NodoExpresion Potencia()
{
    // Comienza evaluando un factor (un número o una expresión entre paréntesis).
    NodoExpresion nodo = Factor();

    // Mientras el token actual sea un operador de exponenciación (^).
    while (tokenActual == "^")
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

        // // Muestra en consola el árbol parcial construido.
        // Console.WriteLine("Nuevo potencia: ");
        // Console.WriteLine("       " + nodoDerecho.Valor);
        // Console.WriteLine("     /");
        // Console.WriteLine(operador);
        // Console.WriteLine("     \\");

        // El nuevo nodo se convierte en el nodo actual.
        nodo = nuevoNodo;
    }

    return nodo; // Retorna el nodo raíz de la potencia procesada.
}

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

}
    // Método para imprimir el árbol sintáctico de manera jerárquica.
