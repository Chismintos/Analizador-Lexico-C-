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
    private List<(string Operador, string Operando1, string Operando2, string Resultado)> cuadruplos;
    private int tempCount;

    public GeneradorCodigoIntermedio()
    {
        cuadruplos = new List<(string, string, string, string)>();
        tempCount = 1; // Los temporales comienzan desde t1
    }

    // Método para recorrer el árbol y generar el código intermedio
    public string GenerarCodigoIntermedio(NodoExpresion nodo)
    {
        if (nodo == null)
            return "";

        // Ignorar nodos "main"
        if (nodo.Valor == "main")
        {
            // Procesar solo los hijos de "main"
            GenerarCodigoIntermedio(nodo.Izquierda);
            GenerarCodigoIntermedio(nodo.Derecha);
            return "";
        }

        // Ignorar nodos ";"
        if (nodo.Valor == ";")
        {
            // Procesar subárboles delimitados por ";"
            GenerarCodigoIntermedio(nodo.Izquierda);
            GenerarCodigoIntermedio(nodo.Derecha);
            return "";
        }

        // Si el nodo es una hoja (constante o variable), manejarlo
        if (nodo.Izquierda == null && nodo.Derecha == null)
        {
            // Para constantes, generar un temporal
            if (int.TryParse(nodo.Valor, out _))
            {
                string temporal = $"t{tempCount++}";
                cuadruplos.Add(("=", nodo.Valor, "", temporal));
                return temporal;
            }

            // Para variables, devolver su nombre
            return nodo.Valor;
        }

        // Si el nodo es una asignación, procesarla
        if (nodo.Valor == "=")
        {
            string izquierda = GenerarCodigoIntermedio(nodo.Izquierda); // Variable
            string derecha = GenerarCodigoIntermedio(nodo.Derecha); // Valor o expresión
            cuadruplos.Add((nodo.Valor, derecha, "", izquierda));
            return izquierda; // La variable asignada
        }

        // Si el nodo es una operación aritmética, generar un temporal para el resultado
        string izquierdaOp = GenerarCodigoIntermedio(nodo.Izquierda);
        string derechaOp = GenerarCodigoIntermedio(nodo.Derecha);
        string temporalOp = $"t{tempCount++}";
        cuadruplos.Add((nodo.Valor, izquierdaOp, derechaOp, temporalOp));
        return temporalOp;
    }

public void ImprimirCodigoIntermedio()
{

    foreach (var cuadruplo in cuadruplos)
    {
        string operador = cuadruplo.Operador == "=" ? "" : cuadruplo.Operador;
        string operando1 = cuadruplo.Operando1 ?? "";
        string operando2 = cuadruplo.Operando2 ?? "";
        string resultado = cuadruplo.Resultado;

        if (operador == "")
        {
            // Caso asignación simple
            Console.WriteLine($"{resultado} = {operando1};");
        }
        else
        {
            // Caso operación con operador
            Console.WriteLine($"{resultado} = {operando1} {operador} {operando2};");
        }
    }
}

    // Método para imprimir los cuádruplos como tabla
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

