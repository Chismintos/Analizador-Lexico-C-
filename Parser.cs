
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
    private NodoExpresion Asignacion(){

    // Procesa la variable (en este caso asumimos que solo puede ser un identificador).
    string variable = tokenActual ?? throw new Exception("Token inesperado");

    // Verifica que el token actual sea un identificador y no un número.
    if (!EsIdentificador(variable))
    {
        Error("Se esperaba un identificador válido para la asignación");
    }

    Avanzar(); // Avanza al siguiente token.

    // Verifica si el siguiente token es un operador de asignación.
    if (tokenActual == "=")
    {
        Avanzar(); // Avanza al siguiente token.
        // Procesa la expresión del lado derecho de la asignación.
        NodoExpresion nodoDerecho = Expresion();

        // Crea un nodo para la asignación con la variable y la expresión.
        NodoExpresion nodoAsignacion = new NodoExpresion("=")
        {
            Izquierda = new NodoExpresion(variable), // Nodo para la variable.
            Derecha = nodoDerecho // Nodo para la expresión.
        };

        return nodoAsignacion; // Retorna el nodo de asignación.
    }
    else
    {
        Error("Se esperaba '=' para la asignación"); // Manejo de error si no se encuentra '='.
    }

    throw new Exception("Se alcanzó un estado inesperado");
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

            // // Muestra en consola el árbol parcial construido.
            // Console.WriteLine("Nueva expresion: ");
            // Console.WriteLine("       " + nodoDerecho.Valor);
            // Console.WriteLine("     /");
            // Console.WriteLine(operador);
            // Console.WriteLine("     \\");
            // Console.WriteLine("       " + nodo.Valor);

            // El nodo resultante se convierte en el nuevo nodo para continuar la construcción.
            nodo = nuevoNodo;
        }

        return nodo; // Retorna el nodo raíz de la expresión procesada.
    }

    // Método que procesa un término (maneja multiplicación y división).
    private NodoExpresion Termino()
    {
        // Comienza evaluando una potencia (la operación con ^ tiene mayor precedencia).
        NodoExpresion nodo = Potencia();

        // Mientras el token actual sea un operador de multiplicación o división.
        while (tokenActual == "*" || tokenActual == "/")
        {
            string operador = tokenActual; // Guarda el operador actual.
            Avanzar(); // Avanza al siguiente token.

            // Procesa el siguiente factor (parte derecha de la operación).
            NodoExpresion nodoDerecho = Potencia(); // Cambiado de Factor a Potencia.

            // Crea un nuevo nodo con el operador como valor y conecta los nodos izquierdo y derecho.
            NodoExpresion nuevoNodo = new NodoExpresion(operador)
            {
                Izquierda = nodo, // Nodo izquierdo.
                Derecha = nodoDerecho // Nodo derecho.
            };

            // // Muestra en consola el árbol parcial construido.
            // Console.WriteLine("Nuevo termino: ");
            // Console.WriteLine("       " + nodoDerecho.Valor);
            // Console.WriteLine("     /");
            // Console.WriteLine(operador);
            // Console.WriteLine("     \\");
            // Console.WriteLine("       " + nodo.Valor);

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
            // Console.WriteLine("       " + nodo.Valor);

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

