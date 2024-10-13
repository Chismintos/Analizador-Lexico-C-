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
        return AnalizarIfElse() ?? AnalizarExpresion();
    }

    // Método para analizar una estructura de if-else
    private NodoExpresion? AnalizarIfElse()
    {
        if (_indice < _tokens.Length && _tokens[_indice] == "if")
        {
            _indice++;
            if (_tokens[_indice] != "(") throw new Exception("Se esperaba '(' después de 'if'.");
            _indice++;
            NodoExpresion condicion = AnalizarExpresion();
            if (_tokens[_indice] != ")") throw new Exception("Se esperaba ')'.");
            _indice++;
            NodoExpresion bloqueIf = AnalizarBloque();
            
            NodoExpresion? bloqueElse = null;
            if (_indice < _tokens.Length && _tokens[_indice] == "else")
            {
                _indice++;
                bloqueElse = AnalizarBloque();
            }

            NodoExpresion nodoIfElse = new NodoExpresion("if");
            nodoIfElse.Izquierda = condicion;
            nodoIfElse.Derecha = new NodoExpresion("else")
            {
                Izquierda = bloqueIf,
                Derecha = bloqueElse
            };
            return nodoIfElse;
        }

        return null;
    }

    // Método para analizar bloques de código, por ejemplo, el cuerpo de un if o else
    private NodoExpresion AnalizarBloque()
    {
        if (_tokens[_indice] != "{") throw new Exception("Se esperaba '{'.");
        _indice++;
        NodoExpresion bloque = new NodoExpresion("bloque");
        while (_tokens[_indice] != "}")
        {
            bloque.Izquierda = AnalizarExpresion(); // Analizar una instrucción dentro del bloque
        }
        _indice++; // Consumir '}'
        return bloque;
    }

    // Método para analizar expresiones aritméticas y lógicas
    private NodoExpresion AnalizarExpresion()
    {
        return AnalizarAsignacion();
    }

    // Analiza una asignación como "a = 5"
    private NodoExpresion AnalizarAsignacion()
    {
        NodoExpresion izquierda = AnalizarComparacion();
        if (_indice < _tokens.Length && _tokens[_indice] == "=")
        {
            _indice++;
            NodoExpresion derecha = AnalizarAsignacion();
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
