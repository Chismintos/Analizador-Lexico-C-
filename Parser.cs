// Clase NodoExpresion representa un nodo de un árbol de expresiones.
public class NodoExpresion
{
    // Propiedad que almacena el valor (operador o operando) del nodo.
    public string Valor { get; set; }
    
    // Puntero al nodo hijo izquierdo (puede ser nulo).
    public NodoExpresion? Izquierda { get; set; }
    
    // Puntero al nodo hijo derecho (puede ser nulo).
    public NodoExpresion? Derecha { get; set; }

    // Constructor que inicializa el valor del nodo y establece los hijos como nulos.
    public NodoExpresion(string valor)
    {
        Valor = valor;
        Izquierda = null;
        Derecha = null;
    }
}
// Clase encargada de analizar sintácticamente una lista de tokens y generar un árbol de expresiones
public class AnalizadorSintactico
{
    private string[] _tokens; // Arreglo de tokens que representa la entrada a analizar
    private int _indice;      // Índice que rastrea la posición actual en los tokens


    // Constructor que inicializa el analizador con un conjunto de tokens
    public AnalizadorSintactico(string[] tokens)
    {
        _tokens = tokens;
        _indice = 0;
    }

    // Método principal que analiza la entrada y devuelve el árbol de expresión correspondiente
    public NodoExpresion? Analizar()
    {

        // Si el token actual es "for", se analiza como una expresión 'for'
        if (_indice < _tokens.Length && _tokens[_indice] == "for")
        {
            return AnalizarFor();
        }
        // Si el token actual es "while", se analiza como una expresión 'while'
        if (_indice < _tokens.Length && _tokens[_indice] == "while")
        {
            return AnalizarWhile();
        }

        // Si el token actual es "if", se analiza como una expresión 'if'
        if (_indice < _tokens.Length && _tokens[_indice] == "if")
        {
            return AnalizarIfElse();
        }

        // Si existe un token '=', se analiza como una asignación
        if (_indice < _tokens.Length && _tokens.Contains("="))
        {
            return AnalizarAsignacion();
        }

        // Si no es ninguno de los anteriores, se trata como una expresión general
        return AnalizarExpresion();
    }

     // Método que analiza una expresión 'if-else' y devuelve su nodo correspondiente
    private NodoExpresion? AnalizarIfElse()
    {
        // Verifica si el token es "if"
        if (_indice < _tokens.Length && _tokens[_indice] == "if")
        {
            _indice++; // Avanza al siguiente token
            if (_tokens[_indice] != "(") throw new Exception("Se esperaba '(' después de 'if'.");
            _indice++; // Avanza al token dentro de los paréntesis
            NodoExpresion condicion = AnalizarExpresion(); // Analiza la condición del 'if'
            if (_tokens[_indice] != ")") throw new Exception("Se esperaba ')'.");
            _indice++; // Avanza después del cierre de los paréntesis
            NodoExpresion bloqueIf = AnalizarBloque(); // Analiza el bloque de código dentro del 'if'

            // Crea el nodo 'if' y asigna la condición y el bloque de instrucciones
            NodoExpresion nodoIf = new NodoExpresion("if")
            {
                Izquierda = condicion,
                Derecha = bloqueIf
            };

            // Verifica si existe una parte 'else'
            if (_indice < _tokens.Length && _tokens[_indice] == "else")
            {
                _indice++; // Avanza al siguiente token
                NodoExpresion bloqueElse = AnalizarBloque(); // Analiza el bloque 'else'

                // Crea el nodo 'else' y lo enlaza al nodo 'if'
                NodoExpresion nodoElse = new NodoExpresion("else")
                {
                    Izquierda = bloqueIf,
                    Derecha = bloqueElse
                };
                nodoIf.Derecha = nodoElse; // Asigna el nodo 'else' al 'if'
            }

            return nodoIf; // Devuelve el nodo 'if'
        }

        return null; // Si no es un 'if', retorna null
    }

    // Método que analiza una expresión 'while' y devuelve su nodo correspondiente
    private NodoExpresion? AnalizarWhile()
    {
        // Verifica si el token es "while"
        if (_indice < _tokens.Length && _tokens[_indice] == "while")
        {
            _indice++; // Avanza al siguiente token
            if (_tokens[_indice] != "(") throw new Exception("Se esperaba '(' después de 'while'.");
            _indice++; // Avanza al token dentro de los paréntesis
            NodoExpresion condicion = AnalizarExpresion(); // Analiza la condición del 'while'
            if (_tokens[_indice] != ")") throw new Exception("Se esperaba ')'.");
            _indice++; // Avanza después del cierre de los paréntesis
            NodoExpresion bloqueWhile = AnalizarBloque(); // Analiza el bloque de código dentro del 'while'

            // Crea el nodo 'while' y asigna la condición y el bloque de instrucciones
            NodoExpresion nodoWhile = new NodoExpresion("while")
            {
                Izquierda = condicion,
                Derecha = bloqueWhile
            };

            return nodoWhile; // Devuelve el nodo 'while'
        }

        return null; // Si no es un 'while', retorna null
    }
    
private NodoExpresion? AnalizarFor()
{
    // Verifica si el token es "for"
    if (_indice < _tokens.Length && _tokens[_indice] == "for")
    {
        Console.WriteLine("Se encontró un 'for'");
        _indice++; // Avanza al siguiente token
        if (_tokens[_indice] != "(") throw new Exception("Se esperaba '(' después de 'for'.");
        _indice++; // Avanza al primer token dentro de los paréntesis

        // Analizar la inicialización (por ejemplo: i = 0)
        NodoExpresion inicializacion = AnalizarAsignacion();
        if (_tokens[_indice] != ";") throw new Exception("Se esperaba ';' después de la inicialización en 'for'.");
        _indice++; // Avanza al siguiente token

        // Analizar la condición (por ejemplo: i < 10)
        NodoExpresion condicion = AnalizarExpresion();
        if (_tokens[_indice] != ";") throw new Exception("Se esperaba ';' después de la condición en 'for'.");
        _indice++; // Avanza al siguiente token

        // Analizar el incremento (por ejemplo: i++)
        NodoExpresion incremento;
        if (_tokens[_indice + 1] == "++")  // Verificar si es un incremento postfijo
        {
            incremento = new NodoExpresion("++")
            {
                Izquierda = new NodoExpresion(_tokens[_indice])
            };
            _indice += 2; // Avanzamos dos tokens (i y ++)
        }
        else
        {
            incremento = AnalizarExpresion(); // Si no es un incremento, se trata como una expresión normal
        }

        if (_tokens[_indice] != ")") throw new Exception("Se esperaba ')' al final de la expresión 'for'.");
        _indice++; // Avanza al siguiente token después del ')'

        // Analiza el bloque de código dentro del 'for'
        if (_tokens[_indice] != "{") throw new Exception("Se esperaba '{' después de la expresión 'for'.");
        NodoExpresion bloqueFor = AnalizarBloque();  // Analizar el bloque dentro de '{}'

        // Crea el nodo 'for' y asigna la inicialización, condición, incremento y el bloque
        NodoExpresion nodoFor = new NodoExpresion("for")
        {
            Izquierda = new NodoExpresion("init")
            {
                Izquierda = inicializacion,
                Derecha = new NodoExpresion("condicion")
                {
                    Izquierda = condicion,
                    Derecha = new NodoExpresion("incremento")
                    {
                        Izquierda = incremento
                    }
                }
            },
            Derecha = bloqueFor  // Aquí se debe enlazar el bloque directamente
        };

        return nodoFor; // Devuelve el nodo 'for'
    }

    return null; // Si no es un 'for', retorna null
}



 // Método que analiza un bloque de código delimitado por llaves '{' y '}'
private NodoExpresion AnalizarBloque()
{
    if (_tokens[_indice] != "{") throw new Exception("Se esperaba '{'.");
    _indice++; // Avanza después de la llave de apertura

    NodoExpresion? instrucciones = null; // Almacena las instrucciones dentro del bloque
    NodoExpresion? ultimaInstruccion = null; // Almacena la última instrucción analizada

    // Continúa analizando instrucciones hasta encontrar la llave de cierre '}'
    while (_indice < _tokens.Length && _tokens[_indice] != "}")
    {
        NodoExpresion instruccion = Analizar(); // Analiza una instrucción individual

        // Si hay un punto y coma ';', lo consume
        if (_indice < _tokens.Length && _tokens[_indice] == ";")
        {
            _indice++; // Consumir el punto y coma
        }

        // Si es la primera instrucción, la asigna directamente
        if (instrucciones == null)
        {
            instrucciones = instruccion;
        }
        else
        {
            // Encadena la nueva instrucción a la lista de instrucciones
            NodoExpresion nuevaInstruccion = new NodoExpresion("Block");
            nuevaInstruccion.Izquierda = ultimaInstruccion; // El nodo anterior es el lado izquierdo
            nuevaInstruccion.Derecha = instruccion; // La nueva instrucción es el lado derecho
            instrucciones = nuevaInstruccion;
        }

        ultimaInstruccion = instruccion; // Actualiza la última instrucción
    }

    if (_tokens[_indice] == "}")
    {
        _indice++; // Consumir la llave de cierre '}'
    }

    return instrucciones; // Devuelve el conjunto de instrucciones como un solo nodo
}



   // Método que analiza expresiones generales
    private NodoExpresion AnalizarExpresion()
    {
        NodoExpresion izquierda = AnalizarComparacion(); // Analiza la parte izquierda de la expresión
        return AnalizarLogicos(izquierda); // Analiza operadores lógicos en la expresión
    }

    // Método que analiza operadores lógicos (&&, ||) en una expresión
    private NodoExpresion AnalizarLogicos(NodoExpresion izquierda)
    {
        // Mientras haya operadores lógicos por analizar
        while (_indice < _tokens.Length && (_tokens[_indice] == "&&" || _tokens[_indice] == "||"))
        {
            string operador = _tokens[_indice]; // Obtiene el operador lógico actual
            _indice++; // Avanza al siguiente token
            NodoExpresion derecha = AnalizarComparacion(); // Analiza la parte derecha de la expresión
            NodoExpresion nodoLogico = new NodoExpresion(operador) // Crea el nodo lógico
            {
                Izquierda = izquierda,
                Derecha = derecha
            };
            izquierda = nodoLogico; // Actualiza la expresión izquierda con el nuevo nodo lógico
        }
        return izquierda; // Devuelve el nodo lógico
    }

    // Método que analiza asignaciones (x = y)
    private NodoExpresion AnalizarAsignacion()
    {
        NodoExpresion izquierda = AnalizarComparacion(); // Analiza la parte izquierda de la asignación
        if (_indice < _tokens.Length && _tokens[_indice] == "=")
        {
            _indice++; // Avanza al siguiente token después del '='
            NodoExpresion derecha = AnalizarExpresion(); // Analiza la parte derecha de la asignación
            NodoExpresion nodoAsignacion = new NodoExpresion("=") // Crea el nodo de asignación
            {
                Izquierda = izquierda,
                Derecha = derecha
            };
            return nodoAsignacion; // Devuelve el nodo de asignación
        }
        return izquierda; // Si no hay asignación, devuelve la expresión izquierda
    }

    // Método que analiza operadores de comparación (==, !=, <, >, <=, >=)
    private NodoExpresion AnalizarComparacion()
    {
        NodoExpresion izquierda = AnalizarSumaResta(); // Analiza la parte izquierda de la comparación
        // Mientras haya operadores de comparación por analizar
        while (_indice < _tokens.Length && (_tokens[_indice] == ">" || _tokens[_indice] == "<" || _tokens[_indice] == ">=" || _tokens[_indice] == "<=" || _tokens[_indice] == "==" || _tokens[_indice] == "!="))
        {
            string operador = _tokens[_indice]; // Obtiene el operador de comparación actual
            _indice++; // Avanza al siguiente token
            NodoExpresion derecha = AnalizarSumaResta(); // Analiza la parte derecha de la comparación
            NodoExpresion nodoComparacion = new NodoExpresion(operador) // Crea el nodo de comparación
            {
                Izquierda = izquierda,
                Derecha = derecha
            };
            izquierda = nodoComparacion; // Actualiza la expresión izquierda con el nuevo nodo de comparación
        }
        return izquierda; // Devuelve el nodo de comparación
    }

    // Método que analiza operaciones de suma y resta (+, -)
    private NodoExpresion AnalizarSumaResta()
    {
        NodoExpresion izquierda = AnalizarMultiplicacionDivision(); // Analiza la parte izquierda de la suma/resta
        // Mientras haya operadores de suma/resta por analizar
        while (_indice < _tokens.Length && (_tokens[_indice] == "+" || _tokens[_indice] == "-"))
        {
            string operador = _tokens[_indice]; // Obtiene el operador de suma/resta
            _indice++; // Avanza al siguiente token
            NodoExpresion derecha = AnalizarMultiplicacionDivision(); // Analiza la parte derecha de la suma/resta
            NodoExpresion nodoSumaResta = new NodoExpresion(operador) // Crea el nodo de suma/resta
            {
                Izquierda = izquierda,
                Derecha = derecha
            };
            izquierda = nodoSumaResta; // Actualiza la expresión izquierda con el nuevo nodo
        }
        return izquierda; // Devuelve el nodo de suma/resta
    }

    // Método que analiza operaciones de multiplicación y división (*, /)
    private NodoExpresion AnalizarMultiplicacionDivision()
    {
        NodoExpresion izquierda = AnalizarPotencia(); // Analiza la parte izquierda de la multiplicación/división
        // Mientras haya operadores de multiplicación/división por analizar
        while (_indice < _tokens.Length && (_tokens[_indice] == "*" || _tokens[_indice] == "/"))
        {
            string operador = _tokens[_indice]; // Obtiene el operador de multiplicación/división
            _indice++; // Avanza al siguiente token
            NodoExpresion derecha = AnalizarPotencia(); // Analiza la parte derecha de la multiplicación/división
            NodoExpresion nodoMultiplicacionDivision = new NodoExpresion(operador) // Crea el nodo de multiplicación/división
            {
                Izquierda = izquierda,
                Derecha = derecha
            };
            izquierda = nodoMultiplicacionDivision; // Actualiza la expresión izquierda con el nuevo nodo
        }
        return izquierda; // Devuelve el nodo de multiplicación/división
    }
    private NodoExpresion AnalizarPotencia()
{
    // Se analiza el primer factor de la expresión (puede ser un número o una subexpresión).
    NodoExpresion izquierda = AnalizarFactor();
    
    // Mientras existan tokens y el token actual sea el operador de potencia (^), se analiza la potencia.
    while (_indice < _tokens.Length && _tokens[_indice] == "^")
    {
        // Se guarda el operador de potencia.
        string operador = _tokens[_indice];
        
        // Se avanza al siguiente token (después del operador ^).
        _indice++;
        
        // Se analiza el siguiente factor (a la derecha del operador ^).
        NodoExpresion derecha = AnalizarFactor();
        
        // Se crea un nuevo nodo para representar la operación de potencia.
        NodoExpresion nodoPotencia = new NodoExpresion(operador)
        {
            // El lado izquierdo del nodo es el resultado del análisis previo (factor izquierdo).
            Izquierda = izquierda,
            
            // El lado derecho del nodo es el nuevo factor (factor derecho).
            Derecha = derecha
        };
        
        // El nodo actual pasa a ser el nuevo nodo de potencia.
        izquierda = nodoPotencia;
    }
    
    // Se devuelve la expresión resultante después de procesar todas las operaciones de potencia.
    return izquierda;
}


    private NodoExpresion AnalizarFactor()
{
    // Si el token actual es un paréntesis de apertura, se trata de una subexpresión.
    if (_tokens[_indice] == "(")
    {
        // Se avanza al siguiente token después del paréntesis.
        _indice++;
        
        // Se analiza la subexpresión contenida dentro de los paréntesis.
        NodoExpresion expresion = AnalizarExpresion();
        
        // Verifica si la subexpresión está correctamente cerrada con un paréntesis.
        if (_tokens[_indice] != ")") 
            throw new Exception("Se esperaba ')'.");
        
        // Avanza al siguiente token después del paréntesis de cierre.
        _indice++;
        
        // Devuelve la subexpresión ya analizada.
        return expresion;
    }
    else
    {
        // Si no es una subexpresión, se asume que es un número o variable.
        NodoExpresion nodo = new NodoExpresion(_tokens[_indice]);
        
        // Avanza al siguiente token.
        _indice++;
        
        // Devuelve el nodo que representa el factor (número o variable).
        return nodo;
    }
}


    public static void ImprimirArbol(NodoExpresion nodo, string indentacion = "", bool esUltimo = true)
    {
        if (nodo == null)
            return;

        Console.Write(indentacion);

        if (esUltimo)
        {
            Console.Write("└─");
            indentacion += "  ";
        }
        else
        {
            Console.Write("├─");
            indentacion += "| ";
        }

        Console.WriteLine(nodo.Valor);

        ImprimirArbol(nodo.Izquierda, indentacion, false);
        ImprimirArbol(nodo.Derecha, indentacion, true);
    }

    public static void ImprimirArbol2(NodoExpresion nodo, string indentacion = "")
{
    if (nodo == null)
        return;

    Console.WriteLine(indentacion + nodo.Valor); // Imprimir el valor del nodo actual

    // Recursivamente imprimir el subárbol izquierdo
    if (nodo.Izquierda != null)
    {
        ImprimirArbol2(nodo.Izquierda, indentacion + "    ");
    }

    // Recursivamente imprimir el subárbol derecho
    if (nodo.Derecha != null)
    {
        ImprimirArbol2(nodo.Derecha, indentacion + "    ");
    }
}

}