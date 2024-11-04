// Clase NodoExpresion representa un nodo de un árbol de expresiones.
using System;

public class NodoExpresion
{
    public string Valor { get; set; }
    public NodoExpresion? Izquierda { get; set; }
    public NodoExpresion? Derecha { get; set; }

    public NodoExpresion(string valor)
    {
        Valor = valor;
        Izquierda = null;
        Derecha = null;
    }
}

public class AnalizadorSintactico
{
    private string[] _tokens;
    private int _indice;

    public AnalizadorSintactico(string[] tokens)
    {
        _tokens = tokens;
        _indice = 0;
    }

public NodoExpresion? Analizar()
{
    NodoExpresion raiz = new NodoExpresion("main");

    // Procesa la primera instrucción y la enlaza al nodo izquierdo de `main`
    NodoExpresion primeraInstruccion = AnalizarInstruccionCompleta();
    raiz.Izquierda = primeraInstruccion;

    // Procesa el resto de las instrucciones en secuencia
    NodoExpresion? nodoActual = raiz;

    while (_indice < _tokens.Length)
    {
        // Avanza el índice si hay otro `;`
        if (_indice < _tokens.Length && _tokens[_indice] == ";")
        {
            _indice++; // Saltar el punto y coma en el índice
        }

        // Si hay más instrucciones, crea un nuevo nodo `;` y conecta a la derecha
        if (_indice < _tokens.Length)
        {
            NodoExpresion nuevoPuntoYComa = new NodoExpresion(";");
            nodoActual.Derecha = nuevoPuntoYComa;
            nodoActual = nuevoPuntoYComa; // Actualiza `nodoActual` para la siguiente instrucción

            // Crear una instrucción de asignación y conectar en el nodo `;` actual
            NodoExpresion instruccion = AnalizarInstruccionCompleta();
            nodoActual.Izquierda = instruccion;
        }
    }

    return raiz;
}

    private NodoExpresion AnalizarInstruccionCompleta()
    {
        if (_indice < _tokens.Length && _tokens[_indice] == "for") return AnalizarFor();
        if (_indice < _tokens.Length && _tokens[_indice] == "while") return AnalizarWhile();
        if (_indice < _tokens.Length && _tokens[_indice] == "if") return AnalizarIfElse();
        if (_indice < _tokens.Length && _tokens.Contains("=")) return AnalizarAsignacion();

        return AnalizarExpresion();
    }
private NodoExpresion? AnalizarIfElse()
{
    if (_indice < _tokens.Length && _tokens[_indice] == "if")
    {
        _indice++;
        if (_indice >= _tokens.Length || _tokens[_indice] != "(") throw new Exception("Se esperaba '(' después de 'if'.");
        _indice++;
        NodoExpresion condicion = AnalizarExpresion();
        if (_indice >= _tokens.Length || _tokens[_indice] != ")") throw new Exception("Se esperaba ')'.");
        _indice++;
        NodoExpresion bloqueIf = AnalizarBloque();

        NodoExpresion nodoIf = new NodoExpresion("if") { Izquierda = condicion, Derecha = bloqueIf };

        if (_indice < _tokens.Length && _tokens[_indice] == "else")
        {
            _indice++;
            NodoExpresion bloqueElse = AnalizarBloque();
            NodoExpresion nodoElse = new NodoExpresion("else") { Izquierda = bloqueIf, Derecha = bloqueElse };
            nodoIf.Derecha = nodoElse;
        }

        return nodoIf;
    }

    return null;
}


    private NodoExpresion? AnalizarWhile()
{
    if (_indice < _tokens.Length && _tokens[_indice] == "while")
    {
        _indice++;
        if (_tokens[_indice] != "(") throw new Exception("Se esperaba '(' después de 'while'.");
        _indice++;
        NodoExpresion condicion = AnalizarExpresion();
        if (_tokens[_indice] != ")") throw new Exception("Se esperaba ')'.");
        _indice++;

        // Cambié el análisis de bloque aquí
        NodoExpresion bloqueWhile = AnalizarBloque();

        return new NodoExpresion("while") { Izquierda = condicion, Derecha = bloqueWhile };
    }

    return null;
}

    private NodoExpresion? AnalizarFor()
{
    if (_indice < _tokens.Length && _tokens[_indice] == "for")
    {
        _indice++;
        if (_tokens[_indice] != "(") throw new Exception("Se esperaba '(' después de 'for'.");
        _indice++;

        // Inicialización
        NodoExpresion inicializacion = AnalizarAsignacion();
        if (_tokens[_indice] != ";") throw new Exception("Se esperaba ';' después de la inicialización en 'for'.");
        _indice++;

        // Condición
        NodoExpresion condicion = AnalizarExpresion();
        if (_tokens[_indice] != ";") throw new Exception("Se esperaba ';' después de la condición en 'for'.");
        _indice++;

        // Incremento
        NodoExpresion incremento = AnalizarIncremento();
        if (_tokens[_indice] != ")") throw new Exception("Se esperaba ')' al final de la expresión 'for'.");
        _indice++;

        // Bloque de instrucciones
        NodoExpresion bloqueFor = AnalizarBloque();

        // Creación del nodo 'for' con la estructura adecuada
        NodoExpresion forNode = new NodoExpresion("for")
        {
            Izquierda = inicializacion, // Directamente la inicialización
            Derecha = new NodoExpresion(";")
            {
                Izquierda = condicion, // Condición
                Derecha = new NodoExpresion(";")
                {
                    Izquierda = incremento, // Incremento
                    Derecha = bloqueFor // El bloque de instrucciones va al final
                }
            }
        };

        return forNode;
    }

    return null;
}




// Nuevo método para manejar el incremento
private NodoExpresion? AnalizarIncremento()
{
    if (_indice < _tokens.Length)
    {
        string valor = _tokens[_indice];
        _indice++;

        // Verificar si el siguiente token es un incremento
        if (_indice < _tokens.Length && (_tokens[_indice] == "++" || _tokens[_indice] == "--"))
        {
            string operador = _tokens[_indice];
            _indice++;

            return new NodoExpresion(operador) { Izquierda = new NodoExpresion(valor) };
        }
        // Si no hay un incremento explícito, retornar solo la variable
        return new NodoExpresion(valor);
    }

    throw new Exception("Se esperaba un incremento.");
}



   private NodoExpresion AnalizarBloque()
{
    if (_tokens[_indice] != "{") throw new Exception("Se esperaba '{'.");
    _indice++;

    NodoExpresion? instrucciones = null;
    NodoExpresion? ultimaInstruccion = null;

    while (_indice < _tokens.Length && _tokens[_indice] != "}")
    {
        // Crear un nodo de punto y coma antes de cada instrucción
        NodoExpresion puntoYComa = new NodoExpresion(";");
        
        // Analizar la instrucción
        NodoExpresion instruccion = AnalizarInstruccionCompleta(); 
        if (_indice < _tokens.Length && _tokens[_indice] == ";") _indice++;

        // Conectar la instrucción al nodo de punto y coma
        puntoYComa.Izquierda = instruccion;

        // Si es la primera instrucción, la asignamos directamente
        if (instrucciones == null)
        {
            instrucciones = puntoYComa;
        }
        else
        {
            // Conectar la instrucción al bloque de instrucciones
            ultimaInstruccion.Derecha = puntoYComa;
        }

        // Actualizamos la última instrucción
        ultimaInstruccion = puntoYComa;
    }

    if (_indice < _tokens.Length && _tokens[_indice] == "}") _indice++;
    return instrucciones ?? new NodoExpresion("empty"); // Retornar un nodo vacío si no hay instrucciones
}


    private NodoExpresion AnalizarExpresion()
    {
        NodoExpresion izquierda = AnalizarComparacion();
        return AnalizarLogicos(izquierda);
    }

    private NodoExpresion AnalizarLogicos(NodoExpresion izquierda)
    {
        while (_indice < _tokens.Length && (_tokens[_indice] == "&&" || _tokens[_indice] == "||"))
        {
            string operador = _tokens[_indice];
            _indice++;
            NodoExpresion derecha = AnalizarComparacion();
            izquierda = new NodoExpresion(operador) { Izquierda = izquierda, Derecha = derecha };
        }
        return izquierda;
    }

    private NodoExpresion AnalizarAsignacion()
    {
        NodoExpresion izquierda = AnalizarComparacion();
        if (_indice < _tokens.Length && _tokens[_indice] == "=")
        {
            _indice++;
            NodoExpresion derecha = AnalizarExpresion();
            return new NodoExpresion("=") { Izquierda = izquierda, Derecha = derecha };
        }
        return izquierda;
    }

    private NodoExpresion AnalizarComparacion()
    {
        NodoExpresion izquierda = AnalizarSumaResta();
        while (_indice < _tokens.Length && (_tokens[_indice] == "==" || _tokens[_indice] == "!=" || _tokens[_indice] == "<" || _tokens[_indice] == ">" || _tokens[_indice] == "<=" || _tokens[_indice] == ">="))
        {
            string operador = _tokens[_indice];
            _indice++;
            NodoExpresion derecha = AnalizarSumaResta();
            izquierda = new NodoExpresion(operador) { Izquierda = izquierda, Derecha = derecha };
        }
        return izquierda;
    }

    private NodoExpresion AnalizarSumaResta()
    {
        NodoExpresion izquierda = AnalizarTermino();
        while (_indice < _tokens.Length && (_tokens[_indice] == "+" || _tokens[_indice] == "-"))
        {
            string operador = _tokens[_indice];
            _indice++;
            NodoExpresion derecha = AnalizarTermino();
            izquierda = new NodoExpresion(operador) { Izquierda = izquierda, Derecha = derecha };
        }
        return izquierda;
    }

    private NodoExpresion AnalizarTermino()
    {
        NodoExpresion izquierda = AnalizarFactor();
        while (_indice < _tokens.Length && (_tokens[_indice] == "*" || _tokens[_indice] == "/"))
        {
            string operador = _tokens[_indice];
            _indice++;
            NodoExpresion derecha = AnalizarFactor();
            izquierda = new NodoExpresion(operador) { Izquierda = izquierda, Derecha = derecha };
        }
        return izquierda;
    }

   private NodoExpresion AnalizarFactor()
{
    if (_indice < _tokens.Length && _tokens[_indice] == "(")
    {
        _indice++;
        NodoExpresion expresion = AnalizarExpresion();
        if (_tokens[_indice] != ")") throw new Exception("Se esperaba ')'.");
        _indice++;
        return expresion;
    }

    if (_indice < _tokens.Length)
    {
        string valor = _tokens[_indice];
        _indice++;

        // Verificar si es un post-incremento
        if (_indice < _tokens.Length && _tokens[_indice] == "++")
        {
            _indice++;
            return new NodoExpresion("++") { Izquierda = new NodoExpresion(valor) };
        }

        // Verificar si es un post-decremento
        if (_indice < _tokens.Length && _tokens[_indice] == "--")
        {
            _indice++;
            return new NodoExpresion("--") { Izquierda = new NodoExpresion(valor) };
        }

        return new NodoExpresion(valor);
    }

    throw new Exception("Se esperaba un factor.");
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

         public static void ImprimirArbolHorizontal(NodoExpresion? nodo, int nivel = 0)
{
    if (nodo == null)
        return;

    // Primero imprime el subárbol derecho
    ImprimirArbolHorizontal(nodo.Derecha, nivel + 1);

    // Imprime el nodo actual con sangría
    Console.WriteLine(new string(' ', nivel * 4) + nodo.Valor);

    // Luego imprime el subárbol izquierdo
    ImprimirArbolHorizontal(nodo.Izquierda, nivel + 1);
}

}


    

    
