using System;

namespace ProductsManagement.Domain.Exceptions;

/// <summary>
/// Excepción de dominio para conflictos de concurrencia optimista.
/// </summary>
/// <remarks>
/// Se lanza cuando la versión esperada de una entidad no coincide
/// con la versión actual persistida.
///
/// Este mecanismo evita sobrescrituras concurrentes y permite que
/// la capa de API traduzca el error a un HTTP 409 (Conflict).
/// </remarks>
public sealed class ConcurrencyException : Exception
{
    public ConcurrencyException(string message) : base(message) { }
}
