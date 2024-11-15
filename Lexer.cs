using System;

public class Token
{
    public string Tipo { get; set; }
    public string Valor { get; set; }

    // Constructor para inicializar el tipo  y valo del token 
    public Token(string tipo, string valor)
    {
        Tipo = tipo;
        Valor = valor;
    }
}

public class Lexer
{
    private string _codigo; // Variable que contiene el código fuente
    private int _posicion; //variable que rastrea la posición actual dentro del código mientras se procesa.

    // Constructor que lee el archivo fuente y inicializa la posición
    public Lexer(string rutaArchivo)
    {
        _codigo = File.ReadAllText(rutaArchivo); // toma la ruta del archivo donde esta el codigo a analizar 
        _posicion = 0;
    }

    // Método principal para tokenizar el código fuente
    public List<Token> Tokenizar()
    {
        List<Token> tokens = new List<Token>();  // Creamos una lista donde se almacenaran los tokens identificados

        while (_posicion < _codigo.Length) // Mientras la posicion sea menor al tamaño del codigo
        {
            char actual = _codigo[_posicion]; // actual es la pocicion actual del codigo

            if (char.IsWhiteSpace(actual))
            {
                _posicion++; // Ignorar espacios en blanco
            }
            else if (char.IsLetter(actual))
            {
                tokens.Add(LeerIdentificadorOClave()); //Si es una letra lo lleva a Identificador o Palabra clave
            }
            else if (char.IsDigit(actual))
            {
                tokens.Add(LeerNumero()); // Procesa numeros 
            }
            else if (actual == '/' && Peek() == '/') // Mira el siguiente carácter sin avanzar la posición actual en el código.
            {
                OmitirComentarioLinea(); // si es un comentario //
            }
            else if (actual == '/' && Peek() == '*')
            {
                OmitirComentarioMultilinea(); // si es un comentario /*
            }
            else if (actual == '"')
            {
                tokens.Add(LeerCadena()); // si es una cadena 
            }
            else if (actual == '\'')
            {
                tokens.Add(LeerChar()); // si es un cchar
            }
            else if (actual == '0' && Peek() == 'x')
            {
                tokens.Add(LeerHexadecimal());
            }
            else if (actual == '0' && Peek() == 'b')
            {
                tokens.Add(LeerBinario());
            }
            else
            {
                tokens.Add(LeerSimbolo()); // si es otar cosa, lo lleva a identificar como simbolo en su metodo
            }
        }

        return tokens;
    }

    // La función para imprimir la tabla de tokens
    public void ImprimirTablaTokens(List<Token> tokens)
    {
        // Encabezado de la tabla
        Console.WriteLine("+-------------------------+---------------------------+----------------------+");
        Console.WriteLine("| Token                   | Tipo                      | Palabra Reservada    |");
        Console.WriteLine("+-------------------------+---------------------------+----------------------+");

        // Lista de palabras reservadas
        var palabrasReservadas = new List<string>
        {
            "if", "else", "return", "while", "for",
            "int", "float", "double", "char", "bool",
            "switch", "case", "default", "break", "continue",
            "try", "catch", "finally", "throw", "new",
            "class", "public", "private", "protected", "static",
            "void", "const", "enum", "namespace", "using"
        };

        // Recorre los tokens y los imprime en formato de tabla
        foreach (var token in tokens)
        {
            string esReservada = palabrasReservadas.Contains(token.Valor) ? "Sí" : "No";

            Console.WriteLine(
                $"| {token.Valor,-23} | {token.Tipo,-25} | {esReservada,-20} |"
            );

            // Línea divisoria para cada fila
            Console.WriteLine("+-------------------------+---------------------------+----------------------+");
        }
    }



    // Lee identificadores y palabras clave
    private Token LeerIdentificadorOClave()
    {
        string palabra = "";
        while (_posicion < _codigo.Length && (char.IsLetterOrDigit(_codigo[_posicion]) || _codigo[_posicion] == '_')) 
        {
            palabra += _codigo[_posicion];
            _posicion++;
        }// verfica que palabra tenga lo que se espera para despues compararla con las palabras claves

        // Palabras clave definidas
        switch (palabra)  // Si coincide con una palabra clave predefinida, retorna un token de ese tipo; de lo contrario, un IDENTIFICADOR.
        {
            case "if":
                return new Token("IF", palabra);
            case "else":
                return new Token("ELSE", palabra);
            case "return":
                return new Token("RETURN", palabra);
            case "while":
                return new Token("WHILE", palabra);
            case "for":
                return new Token("FOR", palabra);
            case "int":
                return new Token("INT", palabra);
            case "float":
                return new Token("FLOAT", palabra);
            case "double":
                return new Token("DOUBLE", palabra);
            case "char":
                return new Token("CHAR", palabra);
            case "bool":
                return new Token("BOOL", palabra);
            case "switch":
                return new Token("SWITCH", palabra);
            case "case":
                return new Token("CASE", palabra);
            case "default":
                return new Token("DEFAULT", palabra);
            case "break":
                return new Token("BREAK", palabra);
            case "continue":
                return new Token("CONTINUE", palabra);
            case "try":
                return new Token("TRY", palabra);
            case "catch":
                return new Token("CATCH", palabra);
            case "finally":
                return new Token("FINALLY", palabra);
            case "throw":
                return new Token("THROW", palabra);
            case "new":
                return new Token("NEW", palabra);
            case "class":
                return new Token("CLASS", palabra);
            case "public":
                return new Token("PUBLIC", palabra);
            case "private":
                return new Token("PRIVATE", palabra);
            case "protected":
                return new Token("PROTECTED", palabra);
            case "static":
                return new Token("STATIC", palabra);
            case "void":
                return new Token("VOID", palabra);
            case "const":
                return new Token("CONST", palabra);
            case "enum":
                return new Token("ENUM", palabra);
            case "namespace":
                return new Token("NAMESPACE", palabra);
            case "using":
                return new Token("USING", palabra);
            default:
                return new Token("IDENTIFICADOR", palabra);
        }
    }

    // Lee números (enteros y decimales)
    private Token LeerNumero()
    {
        string numero = ""; // cadena para contruir el numero a medida que se lee
        bool esDecimal = false; //indicador para ver si es decimal o no

        // Lee el número entero o decimal, mientras este dentro de los limtes del codigo o un punto que indica punto decimal
        while (_posicion < _codigo.Length && (char.IsDigit(_codigo[_posicion]) || _codigo[_posicion] == '.'))
        {
            if (_codigo[_posicion] == '.')
            {
                // Verifica que solo haya un punto decimal
                if (esDecimal) // verifica que no tenga dos puntos decimales si esDecimal es true y da error, de lo contrario lo cambia a true 
                {
                    throw new Exception("Error léxico: Número decimal mal formado.");
                }
                esDecimal = true;
            }
            numero += _codigo[_posicion]; 
            _posicion++; // agrega el caracter actual al string y incrementamos posicion
        }

        // Validar longitud del número decimal
        if (numero.Length > 10)
        {
            throw new Exception("Error léxico: Número demasiado largo.");
        }

        // Determinar el tipo de número
        if (esDecimal) // si el esDecimal es true lo manda el numero como FLOAT
        {
            return new Token("FLOAT", numero);
        }
        else // y si esta en false lo manda como INT
        {
            return new Token("INT", numero);
        }
    }

    // Lee cadenas de texto
    private Token LeerCadena()
    {
        _posicion++; // Saltar la comilla doble inicial
        string valorCadena = "";
        while (_posicion < _codigo.Length && _codigo[_posicion] != '"') //se ejecuta mientras posicion esté dentro de los límites de _codigo
        {
            if (_codigo[_posicion] == '\\' && Peek() == '"') // verifica que en caso de ser un comillas termina y agrega el string sin las comillas
            {
                _posicion++; // Saltar el carácter de escape
            }
            valorCadena += _codigo[_posicion];
            _posicion++;
        }

        if (valorCadena.Length > 100)
        {
            throw new Exception("Error léxico: Cadena de texto demasiado larga.");
        }

        _posicion++; // Saltar la comilla doble final
        return new Token("STRING", valorCadena);
    }

    // Lee caracteres individuales
    private Token LeerChar()
    {
        _posicion++; // Saltar la comilla simple inicial
        string valorChar = _codigo[_posicion].ToString();
        _posicion++;
        if (_codigo[_posicion] != '\'')
        {
            throw new Exception("Error léxico: Carácter no cerrado correctamente.");
        }
        _posicion++; // Saltar la comilla simple final
        return new Token("CHAR", valorChar);
    }

    // Lee números hexadecimales
    private Token LeerHexadecimal()
    {
        string valorHex = "0x";
        _posicion += 2; // Saltar "0x"
        while (_posicion < _codigo.Length && (char.IsDigit(_codigo[_posicion]) || "abcdefABCDEF".Contains(_codigo[_posicion])))
        {
            valorHex += _codigo[_posicion];
            _posicion++;
        }
        return new Token("HEX", valorHex);
    }

    // Lee números binarios
    private Token LeerBinario()
    {
        string valorBin = "0b";
        _posicion += 2; // Saltar "0b"
        while (_posicion < _codigo.Length && (_codigo[_posicion] == '0' || _codigo[_posicion] == '1'))
        {
            valorBin += _codigo[_posicion];
            _posicion++;
        }
        return new Token("BIN", valorBin);
    }

    // Omitir comentarios de una sola línea
    private void OmitirComentarioLinea()
    {
        while (_posicion < _codigo.Length && _codigo[_posicion] != '\n')
        {
            _posicion++;
        }
    }

    // Omitir comentarios multilínea
    private void OmitirComentarioMultilinea()
    {
        _posicion += 2; // Saltar "/*"
        while (_posicion < _codigo.Length && !(_codigo[_posicion] == '*' && Peek() == '/'))
        {
            _posicion++;
        }
        _posicion += 2; // Saltar "*/"
    }

    // Lee símbolos (delimitadores y operadores)
    private Token LeerSimbolo(){

    char actual = _codigo[_posicion];
    string valorSimbolo = actual.ToString();
    _posicion++;

    switch (actual)
    {
        case '+':
        if (_posicion < _codigo.Length && _codigo[_posicion] == '+') // Verificar el siguiente carácter
            {
                _posicion++; // Avanzar para incluir el segundo '+'
                return new Token("OP_INCREMENTO", "++"); // Token para el operador de incremento
            }
            return new Token("OP_SUMA", valorSimbolo); // Operador de suma
        case '-':
            return new Token("OP_RESTA", valorSimbolo); // Operador de resta
        case '*':
            return new Token("OP_MULTIPLICACION", valorSimbolo); // Operador de multiplicación
        case '/':   
            return new Token("OP_DIVISION", valorSimbolo); // Operador de división
        case '^':
            return new Token("OP_POTENCIA", valorSimbolo); // Operador de potencia
        case '=':
            if (_posicion < _codigo.Length && _codigo[_posicion] == '=')
            {
                _posicion++; // Avanzar para incluir el segundo '='
                return new Token("OP_IGUAL_IGUAL", "=="); // Operador de igualdad
            }
            return new Token("OP_IGUAL", valorSimbolo); // Operador de asignación
        case '<':
            if (_posicion < _codigo.Length && _codigo[_posicion] == '=')
            {
                _posicion++; // Avanzar para incluir el '='
                return new Token("OP_MENOR_O_IGUAL", "<="); // Operador menor o igual
            }
            return new Token("OP_MENOR_QUE", valorSimbolo); // Operador menor que
        case '>':
            if (_posicion < _codigo.Length && _codigo[_posicion] == '=')
            {
                _posicion++; // Avanzar para incluir el '='
                return new Token("OP_MAYOR_O_IGUAL", ">="); // Operador mayor o igual
            }
            return new Token("OP_MAYOR_QUE", valorSimbolo); // Operador mayor que
        case '!':
            if (_posicion < _codigo.Length && _codigo[_posicion] == '=')
            {
                _posicion++; // Avanzar para incluir el '='
                return new Token("OP_DIFERENTE", "!="); // Operador diferente
            }
            // Si solo encontramos un '!', podríamos manejarlo como un error léxico.
            throw new Exception($"Error léxico: El símbolo '{actual}' no es válido en esta posición.");
        case '&':
            if (_posicion < _codigo.Length && _codigo[_posicion] == '&')
            {
                _posicion++; // Avanzar para incluir el segundo '&'
                return new Token("OP_AND", "&&"); // Operador AND lógico
            }
            return new Token("AMPERSAND", valorSimbolo);
        case '|':
            if (_posicion < _codigo.Length && _codigo[_posicion] == '|')
            {
                _posicion++; // Avanzar para incluir el segundo '|'
                return new Token("OP_OR", "||"); // Operador OR lógico
            }
            // Si no es el operador lógico, podemos manejarlo como error o simplemente ignorarlo.
            throw new Exception($"Error léxico: El símbolo '{actual}' no es válido en esta posición.");
        case ';':
            return new Token("PUNTO_Y_COMA", valorSimbolo); // Punto y coma
        case ',':
            return new Token("COMA", valorSimbolo); // Coma
        case '(':
            return new Token("PARENTESIS_ABIERTO", valorSimbolo); // Paréntesis abierto
        case ')':
            return new Token("PARENTESIS_CERRADO", valorSimbolo); // Paréntesis cerrado
        case '{':
            return new Token("LLAVE_ABIERTA", valorSimbolo); // Llave abierta
        case '}':
            return new Token("LLAVE_CERRADA", valorSimbolo); // Llave cerrada
        case '[':
            return new Token("CORCHETE_ABIERTO", valorSimbolo); // Corchete abierto
        case ']':
            return new Token("CORCHETE_CERRADO", valorSimbolo); // Corchete cerrado
        default:
            // Si el símbolo no se reconoce, se lanza una excepción
            throw new Exception($"Error léxico: Símbolo no reconocido '{actual}' en la posición {_posicion}.");
    }
}

    // Mira el siguiente carácter sin avanzar la posición
    private char Peek()
    {
        if (_posicion + 1 < _codigo.Length)
        {
            return _codigo[_posicion + 1];
        }
        return '\0'; // Retorna un carácter nulo si no hay más caracteres
    }
}
