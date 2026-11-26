using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Newtonsoft.Json;

class Expediente
{
    public string Codigo { get; set; }
    public string DNI { get; set; }
    public string Nombre { get; set; }
    public string Asunto { get; set; }
    public string TipoDocumento { get; set; }
    public DateTime FechaRegistro { get; set; }
}

class Program
{
    static List<Expediente> expedientes = new List<Expediente>();
    static string archivo = "expedientes.json";

    static void Main()
    {
        CargarDatos();

        int opcion;

        do
        {
            Console.Clear();
            Console.WriteLine("================================");
            Console.WriteLine("      MESA DE PARTES DIGITAL    ");
            Console.WriteLine("================================");
            Console.WriteLine("1. Registrar expediente");
            Console.WriteLine("2. Mostrar expedientes");
            Console.WriteLine("3. Buscar expediente");
            Console.WriteLine("4. Ordenar expedientes");
            Console.WriteLine("5. Guardar expedientes");
            Console.WriteLine("6. Salir");
            Console.Write("Seleccione una opción: ");

            if (!int.TryParse(Console.ReadLine(), out opcion))
                opcion = 0;

            switch (opcion)
            {
                case 1:
                    Registrar();
                    break;

                case 2:
                    Mostrar();
                    break;

                case 3:
                    Buscar();
                    break;

                case 4:
                    Ordenar();
                    break;

                case 5:
                    GuardarDatos();
                    break;

                case 6:
                    GuardarDatos();
                    Console.WriteLine("Saliendo del sistema...");
                    break;

                default:
                    Console.WriteLine("Opción no válida.");
                    break;
            }

            if (opcion != 6)
            {
                Console.WriteLine("\nPresione una tecla para continuar...");
                Console.ReadKey();
            }

        } while (opcion != 6);
    }

    static void Registrar()
    {
        Console.Clear();
        Console.WriteLine("=== REGISTRO DE EXPEDIENTE ===");

        string dni;

        do
        {
            Console.Write("Ingrese DNI (8 dígitos): ");
            dni = Console.ReadLine() ?? "";

            if (dni.Length != 8 || !dni.All(char.IsDigit))
                Console.WriteLine("Error: el DNI debe tener 8 números.");

        } while (dni.Length != 8 || !dni.All(char.IsDigit));

        Console.Write("Ingrese nombres y apellidos: ");
        string nombre = Console.ReadLine() ?? "";

        while (string.IsNullOrWhiteSpace(nombre))
        {
            Console.Write("El nombre es obligatorio: ");
            nombre = Console.ReadLine() ?? "";
        }

        Console.Write("Ingrese el asunto: ");
        string asunto = Console.ReadLine() ?? "";

        while (string.IsNullOrWhiteSpace(asunto))
        {
            Console.Write("El asunto es obligatorio: ");
            asunto = Console.ReadLine() ?? "";
        }

        Console.Write("Tipo de documento: ");
        string tipo = Console.ReadLine() ?? "";

        while (string.IsNullOrWhiteSpace(tipo))
        {
            Console.Write("Ingrese el tipo de documento: ");
            tipo = Console.ReadLine() ?? "";
        }

        string codigo = "EXP-" +
            DateTime.Now.ToString("yyyyMMddHHmmss") +
            "-" + (expedientes.Count + 1);

        Expediente nuevo = new Expediente
        {
            Codigo = codigo,
            DNI = dni,
            Nombre = nombre,
            Asunto = asunto,
            TipoDocumento = tipo,
            FechaRegistro = DateTime.Now
        };

        expedientes.Add(nuevo);
        GuardarDatos();

        Console.WriteLine("\n¡Expediente registrado correctamente!");
        Console.WriteLine("Código asignado: " + codigo);
    }

    static void Mostrar()
    {
        Console.Clear();
        Console.WriteLine("=== EXPEDIENTES REGISTRADOS ===");

        if (expedientes.Count == 0)
        {
            Console.WriteLine("No existen expedientes registrados.");
            return;
        }

        foreach (Expediente e in expedientes)
        {
            Console.WriteLine("--------------------------------");
            Console.WriteLine("Código: " + e.Codigo);
            Console.WriteLine("DNI: " + e.DNI);
            Console.WriteLine("Solicitante: " + e.Nombre);
            Console.WriteLine("Asunto: " + e.Asunto);
            Console.WriteLine("Documento: " + e.TipoDocumento);
            Console.WriteLine("Fecha: " + e.FechaRegistro);
        }
    }

    static void Buscar()
    {
        Console.Clear();
        Console.WriteLine("=== BUSCAR EXPEDIENTE ===");

        Console.Write("Ingrese código o DNI: ");
        string dato = Console.ReadLine() ?? "";

        Expediente encontrado = expedientes.FirstOrDefault(e =>
            e.Codigo.Equals(dato, StringComparison.OrdinalIgnoreCase)
            || e.DNI == dato);

        if (encontrado != null)
        {
            Console.WriteLine("\nExpediente encontrado:");
            Console.WriteLine("Código: " + encontrado.Codigo);
            Console.WriteLine("DNI: " + encontrado.DNI);
            Console.WriteLine("Nombre: " + encontrado.Nombre);
            Console.WriteLine("Asunto: " + encontrado.Asunto);
            Console.WriteLine("Documento: " + encontrado.TipoDocumento);
            Console.WriteLine("Fecha: " + encontrado.FechaRegistro);
        }
        else
        {
            Console.WriteLine("No se encontró el expediente.");
        }
    }

    static void Ordenar()
    {
        Console.Clear();
        Console.WriteLine("=== ORDENAR EXPEDIENTES ===");
        Console.WriteLine("1. Por nombre del solicitante");
        Console.WriteLine("2. Por fecha de registro");
        Console.Write("Seleccione: ");

        if (!int.TryParse(Console.ReadLine(), out int opcion))
        {
            Console.WriteLine("Opción inválida.");
            return;
        }

        List<Expediente> ordenados;

        switch (opcion)
        {
            case 1:
                ordenados = expedientes
                    .OrderBy(e => e.Nombre)
                    .ToList();
                break;

            case 2:
                ordenados = expedientes
                    .OrderBy(e => e.FechaRegistro)
                    .ToList();
                break;

            default:
                Console.WriteLine("Opción no válida.");
                return;
        }

        Console.WriteLine("\n=== EXPEDIENTES ORDENADOS ===");

        foreach (Expediente e in ordenados)
        {
            Console.WriteLine(
                e.Codigo + " | " +
                e.Nombre + " | " +
                e.FechaRegistro.ToString("dd/MM/yyyy HH:mm")
            );
        }
    }

    static void GuardarDatos()
    {
        string json = JsonConvert.SerializeObject(expedientes, 
            Newtonsoft.Json.Formatting.Indented);

        File.WriteAllText(archivo, json);
    }

    static void CargarDatos()
    {
        if (File.Exists(archivo))
        {
            try
            {
                string json = File.ReadAllText(archivo);

                expedientes = JsonConvert.DeserializeObject<List<Expediente>>(json)
                    ?? new List<Expediente>();
            }
            catch
            {
                expedientes = new List<Expediente>();
                Console.WriteLine("No se pudieron cargar los datos.");
            }
        }
    }
}