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
    // Si comienza con 'if', analizamos la estructura condicional
    if (_indice < _tokens.Length && _tokens[_indice] == "if")
    {
        return AnalizarIfElse();
    }

    // Si encontramos un '=', analizamos la asignación
    if (_indice < _tokens.Length && _tokens.Contains("="))
    {
        return AnalizarAsignacion();
    }

    // Si no es una asignación ni un if, lo tratamos como una expresión
    return AnalizarExpresion();
}


    // Método para analizar una estructura de if-else
    // Método para analizar una estructura de if-else
private NodoExpresion? AnalizarIfElse()
{
    if (_indice < _tokens.Length && _tokens[_indice] == "if")
    {
        _indice++;
        if (_tokens[_indice] != "(") throw new Exception("Se esperaba '(' después de 'if'.");
        _indice++;
        NodoExpresion condicion = AnalizarExpresion(); // Condición del if
        if (_tokens[_indice] != ")") throw new Exception("Se esperaba ')'.");

        _indice++;
        NodoExpresion bloqueIf = AnalizarBloque();  // Bloque del if

        NodoExpresion nodoIf = new NodoExpresion("if")
        {
            Izquierda = condicion,
            Derecha = bloqueIf
        };

        if (_indice < _tokens.Length && _tokens[_indice] == "else")
        {
            _indice++;
            NodoExpresion bloqueElse = AnalizarBloque();  // Bloque del else
            NodoExpresion nodoElse = new NodoExpresion("else")
            {
                Izquierda = bloqueIf,
                Derecha = bloqueElse
            };

            nodoIf.Derecha = nodoElse;
        }

        return nodoIf;
    }

    return null;
}



    // Método para analizar bloques de código, por ejemplo, el cuerpo de un if o else
private NodoExpresion AnalizarBloque()
{
    if (_tokens[_indice] != "{") throw new Exception("Se esperaba '{'.");
    _indice++;

    NodoExpresion? instrucciones = null;
    NodoExpresion? ultimaInstruccion = null;
    
    while (_indice < _tokens.Length && _tokens[_indice] != "}")
    {
        NodoExpresion instruccion = Analizar(); // Analizar cualquier instrucción dentro del bloque
        if (instrucciones == null)
        {
            instrucciones = instruccion; // La primera instrucción
        }
        else
        {
            ultimaInstruccion.Derecha = instruccion;  // Encadenar las instrucciones
        }
        ultimaInstruccion = instruccion; // Guardar la última instrucción
    }
    _indice++; // Consumir el '}'
    
    return instrucciones;  // Retornar el bloque de instrucciones
}



    // Método para analizar expresiones aritméticas y lógicas
    // Método para analizar expresiones aritméticas y lógicas
private NodoExpresion AnalizarExpresion()
{
    NodoExpresion izquierda = AnalizarComparacion(); // Empezamos con comparaciones
    return AnalizarLogicos(izquierda);  // Luego manejamos los operadores lógicos
}


private NodoExpresion AnalizarLogicos(NodoExpresion izquierda)
{
    while (_indice < _tokens.Length && (_tokens[_indice] == "&&" || _tokens[_indice] == "||"))
    {
        string operador = _tokens[_indice];
        _indice++;
        NodoExpresion derecha = AnalizarComparacion();  // Los operadores lógicos usan comparaciones
        NodoExpresion nodoLogico = new NodoExpresion(operador);
        nodoLogico.Izquierda = izquierda;
        nodoLogico.Derecha = derecha;
        izquierda = nodoLogico;  // Continuamos con el nodo lógico actualizado
    }
    return izquierda;
}

    // Analiza una asignación como "a = 5"
    private NodoExpresion AnalizarAsignacion()
{
    NodoExpresion izquierda = AnalizarComparacion();  // Puede ser una variable a la izquierda
    if (_indice < _tokens.Length && _tokens[_indice] == "=")
    {
        _indice++;
        NodoExpresion derecha = AnalizarExpresion();  // Se analiza una expresión a la derecha
        NodoExpresion nodoAsignacion = new NodoExpresion("=");
        nodoAsignacion.Izquierda = izquierda;
        nodoAsignacion.Derecha = derecha;
        return nodoAsignacion;
    }
    return izquierda;
}

    // Analiza expresiones de comparación (>, <, >=, <=, ==, !=)
    private NodoExpresion AnalizarComparacion()
    {
        NodoExpresion izquierda = AnalizarSumaResta();
        while (_indice < _tokens.Length && (_tokens[_indice] == ">" || _tokens[_indice] == "<" || _tokens[_indice] == ">=" || _tokens[_indice] == "<=" || _tokens[_indice] == "==" || _tokens[_indice] == "!="))
        {
            string operador = _tokens[_indice];
            _indice++;
            NodoExpresion derecha = AnalizarSumaResta();
            NodoExpresion nodoComparacion = new NodoExpresion(operador);
            nodoComparacion.Izquierda = izquierda;
            nodoComparacion.Derecha = derecha;
            izquierda = nodoComparacion;
        }
        return izquierda;
    }

    // Analiza suma y resta
    private NodoExpresion AnalizarSumaResta()
    {
        NodoExpresion izquierda = AnalizarMultiplicacionDivision();
        while (_indice < _tokens.Length && (_tokens[_indice] == "+" || _tokens[_indice] == "-"))
        {
            string operador = _tokens[_indice];
            _indice++;
            NodoExpresion derecha = AnalizarMultiplicacionDivision();
            NodoExpresion nodoSumaResta = new NodoExpresion(operador);
            nodoSumaResta.Izquierda = izquierda;
            nodoSumaResta.Derecha = derecha;
            izquierda = nodoSumaResta;
        }
        return izquierda;
    }

    // Analiza multiplicación y división
    private NodoExpresion AnalizarMultiplicacionDivision()
    {
        NodoExpresion izquierda = AnalizarPotencia();
        while (_indice < _tokens.Length && (_tokens[_indice] == "*" || _tokens[_indice] == "/"))
        {
            string operador = _tokens[_indice];
            _indice++;
            NodoExpresion derecha = AnalizarPotencia();
            NodoExpresion nodoMultiplicacionDivision = new NodoExpresion(operador);
            nodoMultiplicacionDivision.Izquierda = izquierda;
            nodoMultiplicacionDivision.Derecha = derecha;
            izquierda = nodoMultiplicacionDivision;
        }
        return izquierda;
    }

    // Analiza la operación de potencia
    private NodoExpresion AnalizarPotencia()
    {
        NodoExpresion izquierda = AnalizarFactor();
        while (_indice < _tokens.Length && _tokens[_indice] == "^")
        {
            string operador = _tokens[_indice];
            _indice++;
            NodoExpresion derecha = AnalizarFactor();
            NodoExpresion nodoPotencia = new NodoExpresion(operador);
            nodoPotencia.Izquierda = izquierda;
            nodoPotencia.Derecha = derecha;
            izquierda = nodoPotencia;
        }
        return izquierda;
    }

    // Analiza factores (números, variables, o expresiones entre paréntesis)
    private NodoExpresion AnalizarFactor()
    {
        if (_tokens[_indice] == "(")
        {
            _indice++;
            NodoExpresion expresion = AnalizarExpresion();
            if (_tokens[_indice] != ")") throw new Exception("Se esperaba ')'.");
            _indice++;
            return expresion;
        }
        else
        {
            NodoExpresion nodo = new NodoExpresion(_tokens[_indice]);
            _indice++;
            return nodo;
        }
    }

    // Método para imprimir el árbol sintáctico de manera recursiva.
    public static void ImprimirArbol(NodoExpresion nodo, string indentacion = "", bool esUltimo = true)
    {
        if (nodo == null)
            return;

        // Imprimir la indentación y si es el último nodo o no
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

        // Imprimir el valor del nodo actual
        Console.WriteLine(nodo.Valor);

        // Recursivamente imprimir los nodos hijos
        var tieneHijoIzquierdo = nodo.Izquierda != null;
        var tieneHijoDerecho = nodo.Derecha != null;

        if (tieneHijoIzquierdo)
            ImprimirArbol(nodo.Izquierda, indentacion, !tieneHijoDerecho);

        if (tieneHijoDerecho)
            ImprimirArbol(nodo.Derecha, indentacion, true);
    }
}
