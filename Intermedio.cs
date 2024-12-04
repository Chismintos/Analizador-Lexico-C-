public class InstruccionIntermedia
{
    public string Operador { get; set; }
    public string Operando1 { get; set; }
    public string Operando2 { get; set; }
    public string Resultado { get; set; }

    public InstruccionIntermedia(string operador, string operando1, string operando2, string resultado)
    {
        Operador = operador;
        Operando1 = operando1;
        Operando2 = operando2;
        Resultado = resultado;
    }

    public override string ToString()
    {
        return $"{Resultado} = {Operando1} {Operador} {Operando2}";
    }
}

public class GeneradorCodigoIntermedio
{
    private List<InstruccionIntermedia> cuadruplos;
    private int tempCount;

    public GeneradorCodigoIntermedio()
    {
        cuadruplos = new List<InstruccionIntermedia>();
        tempCount = 1; // Los temporales comienzan desde t1
    }

    // Método para recorrer el árbol y generar el código intermedio
    public string GenerarCodigoIntermedio(NodoExpresion nodo)
    {
        if (nodo == null)
            return "";

        // Procesar nodos especiales como "if", "while", "main", y ";"
        if (nodo.Valor == "if")
        {
            GenerarCodigoIf(nodo);
            return "";
        }

        if (nodo.Valor == "while")
        {
            GenerarCodigoWhile(nodo);
            return "";
        }

        if (nodo.Valor == "for")
        {
            GenerarCodigoFor(nodo);
            return "";
        }

        if (nodo.Valor == "main" || nodo.Valor == ";")
        {
            GenerarCodigoIntermedio(nodo.Izquierda);
            GenerarCodigoIntermedio(nodo.Derecha);
            return "";
        }

        // Si el nodo es una hoja (constante o variable)
        if (nodo.Izquierda == null && nodo.Derecha == null)
        {
            if (int.TryParse(nodo.Valor, out _)) // Si es un número
            {
                string temporal = $"t{tempCount++}";
                cuadruplos.Add(new InstruccionIntermedia("=", nodo.Valor, "", temporal));
                return temporal;
            }
            return nodo.Valor; // Es una variable
        }

        // Si el nodo es una asignación
        if (nodo.Valor == "=")
        {
            string izquierda = GenerarCodigoIntermedio(nodo.Izquierda);
            string derecha = GenerarCodigoIntermedio(nodo.Derecha);
            cuadruplos.Add(new InstruccionIntermedia(nodo.Valor, derecha, "", izquierda));
            return izquierda;
        }

        // Si el nodo es una operación aritmética
        string izquierdaOp = GenerarCodigoIntermedio(nodo.Izquierda);
        string derechaOp = GenerarCodigoIntermedio(nodo.Derecha);
        string temporalOp = $"t{tempCount++}";
        cuadruplos.Add(new InstruccionIntermedia(nodo.Valor, izquierdaOp, derechaOp, temporalOp));
        return temporalOp;
    }

    // Método para procesar un nodo "if"
    public void GenerarCodigoIf(NodoExpresion nodo)
    {
        string condicion = GenerarCodigoIntermedio(nodo.Izquierda); // Condición
        string etiquetaElse = $"L{tempCount++}";
        string etiquetaFin = $"L{tempCount++}";

        // Salto si la condición es falsa
        cuadruplos.Add(new InstruccionIntermedia("if_false", condicion, "", etiquetaElse));

        // Bloque "then"
        GenerarCodigoIntermedio(nodo.Derecha.Izquierda);

        // Salto al final
        cuadruplos.Add(new InstruccionIntermedia("goto", "", "", etiquetaFin));

        // Etiqueta "else"
        cuadruplos.Add(new InstruccionIntermedia("label", etiquetaElse, "", ""));
        if (nodo.Derecha.Derecha != null)
        {
            GenerarCodigoIntermedio(nodo.Derecha.Derecha);
        }

        // Etiqueta final
        cuadruplos.Add(new InstruccionIntermedia("label", etiquetaFin, "", ""));
    }

    // Método para procesar un nodo "while"
    public void GenerarCodigoWhile(NodoExpresion nodo)
    {
        string etiquetaInicio = $"L{tempCount++}";
        string etiquetaFin = $"L{tempCount++}";

        // Etiqueta de inicio del bucle
        cuadruplos.Add(new InstruccionIntermedia("label", etiquetaInicio, "", ""));

        // Generar código intermedio para la condición
        string condicion = GenerarCodigoIntermedio(nodo.Izquierda); // Condición del while

        // Salto al final si la condición es falsa
        cuadruplos.Add(new InstruccionIntermedia("if_false", condicion, "", etiquetaFin));

        // Bloque del bucle
        GenerarCodigoIntermedio(nodo.Derecha);

        // Salto al inicio del bucle
        cuadruplos.Add(new InstruccionIntermedia("goto", "", "", etiquetaInicio));

        // Etiqueta final
        cuadruplos.Add(new InstruccionIntermedia("label", etiquetaFin, "", ""));
    }

    // Método para procesar un nodo "for"
    public void GenerarCodigoFor(NodoExpresion nodo)
    {
        string etiquetaInicio = $"L{tempCount++}";
        string etiquetaFin = $"L{tempCount++}";

        // Generar la inicialización
        GenerarCodigoIntermedio(nodo.Izquierda.Izquierda); // La inicialización (nodo.Izquierda.Izquierda)

        // Etiqueta de inicio del ciclo
        cuadruplos.Add(new InstruccionIntermedia("label", etiquetaInicio, "", ""));

        // Generar la condición
        string condicion = GenerarCodigoIntermedio(nodo.Izquierda.Derecha); // La condición (nodo.Izquierda.Derecha)

        // Salto al final si la condición es falsa
        cuadruplos.Add(new InstruccionIntermedia("if_false", condicion, "", etiquetaFin));

        // Bloque del ciclo
        GenerarCodigoIntermedio(nodo.Derecha); // El bloque del ciclo (nodo.Derecha)

        // Generar el incremento
        GenerarCodigoIntermedio(nodo.Izquierda.Derecha); // El incremento (nodo.Izquierda.Derecha)

        // Salto al inicio del ciclo
        cuadruplos.Add(new InstruccionIntermedia("goto", "", "", etiquetaInicio));

        // Etiqueta final
        cuadruplos.Add(new InstruccionIntermedia("label", etiquetaFin, "", ""));
    }

    // Método para imprimir el código intermedio en formato texto
    public void ImprimirCodigoIntermedio()
    {
        foreach (var cuadruplo in cuadruplos)
        {
            if (cuadruplo.Operador == "label")
            {
                // Imprimir etiquetas
                Console.WriteLine($"{cuadruplo.Resultado}:");
            }
            else if (cuadruplo.Operador == "goto")
            {
                // Imprimir saltos incondicionales
                Console.WriteLine($"goto {cuadruplo.Resultado};");
            }
            else if (cuadruplo.Operador == "if_false")
            {
                // Imprimir saltos condicionales
                Console.WriteLine($"if_false {cuadruplo.Operando1} goto {cuadruplo.Resultado};");
            }
            else
            {
                // Imprimir operaciones estándar
                string operador = cuadruplo.Operador == "=" ? "" : cuadruplo.Operador;
                Console.WriteLine($"{cuadruplo.Resultado} = {cuadruplo.Operando1} {operador} {cuadruplo.Operando2};");
            }
        }
    }

    // Método para imprimir los cuádruplos en formato de tabla
    public void ImprimirCuadruplosEnTabla()
    {
        Console.WriteLine("+-----------+------------+------------+-----------+");
        Console.WriteLine("| Operador  | Operando1  | Operando2  | Resultado |");
        Console.WriteLine("+-----------+------------+------------+-----------+");

        foreach (var cuadruplo in cuadruplos)
        {
            string operador = cuadruplo.Operador.PadRight(10);
            string operando1 = (cuadruplo.Operando1 ?? "").PadRight(10);
            string operando2 = (cuadruplo.Operando2 ?? "").PadRight(10);
            string resultado = cuadruplo.Resultado.PadRight(9);

            Console.WriteLine($"| {operador} | {operando1} | {operando2} | {resultado} |");
        }

        Console.WriteLine("+-----------+------------+------------+-----------+");
    }
}
