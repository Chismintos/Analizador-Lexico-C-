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
        if (_indice < _tokens.Length && _tokens[_indice] == "while")
        {
            return AnalizarWhile();
        }

        if (_indice < _tokens.Length && _tokens[_indice] == "if")
        {
            return AnalizarIfElse();
        }

        if (_indice < _tokens.Length && _tokens.Contains("="))
        {
            return AnalizarAsignacion();
        }

        return AnalizarExpresion();
    }

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

            NodoExpresion nodoIf = new NodoExpresion("if")
            {
                Izquierda = condicion,
                Derecha = bloqueIf
            };

            if (_indice < _tokens.Length && _tokens[_indice] == "else")
            {
                _indice++;
                NodoExpresion bloqueElse = AnalizarBloque();
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
            NodoExpresion bloqueWhile = AnalizarBloque();

            NodoExpresion nodoWhile = new NodoExpresion("while")
            {
                Izquierda = condicion,
                Derecha = bloqueWhile
            };

            return nodoWhile;
        }

        return null;
    }

    // Actualización del método para analizar bloques de instrucciones
    private NodoExpresion AnalizarBloque()
    {
        if (_tokens[_indice] != "{") throw new Exception("Se esperaba '{'.");
        _indice++;

        NodoExpresion? instrucciones = null;
        NodoExpresion? ultimaInstruccion = null;

        while (_indice < _tokens.Length && _tokens[_indice] != "}")
        {
            // Analizar una instrucción individual
            NodoExpresion instruccion = Analizar(); 

            // Comprobar si es el final de la instrucción (indicado por ';')
            if (_indice < _tokens.Length && _tokens[_indice] == ";")
            {
                _indice++;  // Consumir el punto y coma
            }

            // Encadenar las instrucciones
            if (instrucciones == null)
            {
                instrucciones = instruccion;  // Primera instrucción
            }
            else
            {
                ultimaInstruccion.Derecha = instruccion;  // Encadenar la última instrucción
            }
            ultimaInstruccion = instruccion;  // Guardar la última instrucción
        }

        _indice++;  // Consumir '}'

        return instrucciones;
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
            NodoExpresion nodoLogico = new NodoExpresion(operador)
            {
                Izquierda = izquierda,
                Derecha = derecha
            };
            izquierda = nodoLogico;
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
            NodoExpresion nodoAsignacion = new NodoExpresion("=")
            {
                Izquierda = izquierda,
                Derecha = derecha
            };
            return nodoAsignacion;
        }
        return izquierda;
    }

    private NodoExpresion AnalizarComparacion()
    {
        NodoExpresion izquierda = AnalizarSumaResta();
        while (_indice < _tokens.Length && (_tokens[_indice] == ">" || _tokens[_indice] == "<" || _tokens[_indice] == ">=" || _tokens[_indice] == "<=" || _tokens[_indice] == "==" || _tokens[_indice] == "!="))
        {
            string operador = _tokens[_indice];
            _indice++;
            NodoExpresion derecha = AnalizarSumaResta();
            NodoExpresion nodoComparacion = new NodoExpresion(operador)
            {
                Izquierda = izquierda,
                Derecha = derecha
            };
            izquierda = nodoComparacion;
        }
        return izquierda;
    }

    private NodoExpresion AnalizarSumaResta()
    {
        NodoExpresion izquierda = AnalizarMultiplicacionDivision();
        while (_indice < _tokens.Length && (_tokens[_indice] == "+" || _tokens[_indice] == "-"))
        {
            string operador = _tokens[_indice];
            _indice++;
            NodoExpresion derecha = AnalizarMultiplicacionDivision();
            NodoExpresion nodoSumaResta = new NodoExpresion(operador)
            {
                Izquierda = izquierda,
                Derecha = derecha
            };
            izquierda = nodoSumaResta;
        }
        return izquierda;
    }

    private NodoExpresion AnalizarMultiplicacionDivision()
    {
        NodoExpresion izquierda = AnalizarPotencia();
        while (_indice < _tokens.Length && (_tokens[_indice] == "*" || _tokens[_indice] == "/"))
        {
            string operador = _tokens[_indice];
            _indice++;
            NodoExpresion derecha = AnalizarPotencia();
            NodoExpresion nodoMultiplicacionDivision = new NodoExpresion(operador)
            {
                Izquierda = izquierda,
                Derecha = derecha
            };
            izquierda = nodoMultiplicacionDivision;
        }
        return izquierda;
    }

    private NodoExpresion AnalizarPotencia()
    {
        NodoExpresion izquierda = AnalizarFactor();
        while (_indice < _tokens.Length && _tokens[_indice] == "^")
        {
            string operador = _tokens[_indice];
            _indice++;
            NodoExpresion derecha = AnalizarFactor();
            NodoExpresion nodoPotencia = new NodoExpresion(operador)
            {
                Izquierda = izquierda,
                Derecha = derecha
            };
            izquierda = nodoPotencia;
        }
        return izquierda;
    }

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
}
