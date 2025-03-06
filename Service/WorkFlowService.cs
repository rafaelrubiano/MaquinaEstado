using System;
using System.Collections.Generic;
using System.Linq;
using MaquinaEstado.DB.DataSet;
using MaquinaEstado.DB.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static MaquinaEstado.DB.Enum.EnumEstados;

public class WorkFlowService
{
    private readonly MaquinaEstadosDBContext _context;

    public WorkFlowService(MaquinaEstadosDBContext context)
    {
        _context = context;
    }

    #region MetodosSync

    public bool CambiarEstado(int solicitudId, EstadoSolicitud nuevoEstado, string usuario)
    {
        // Convertir el enum a su valor entero
        int nuevoEstadoId = (int)nuevoEstado;

        var solicitud = _context.Solicitudes.Include(s => s.Estado).FirstOrDefault(s => s.Id == solicitudId && s.Activo);
        if (solicitud == null) throw new Exception("Solicitud no encontrada o inactiva");

        var transicionValida = _context.Transiciones.Any(t => t.EstadoOrigenId == solicitud.EstadoId && t.EstadoDestinoId == nuevoEstadoId && t.Activo);
        if (!transicionValida) throw new Exception("Transición de estado no permitida");

        var nuevoEstadoEntity = _context.Estados.Find(nuevoEstadoId);
        if (nuevoEstadoEntity == null || !nuevoEstadoEntity.Activo) throw new Exception("Estado destino no encontrado o inactivo");

        var historial = new HistorialEstados
        {
            SolicitudId = solicitudId,
            EstadoAnteriorId = solicitud.EstadoId,
            EstadoNuevoId = nuevoEstadoId,
            Usuario = usuario,
            FechaCambio = DateTime.UtcNow,
            Creado = DateTime.UtcNow,
            CreadoPor = usuario
        };

        solicitud.EstadoId = nuevoEstadoId;
        solicitud.Modificado = DateTime.UtcNow;
        solicitud.ModificadoPor = usuario;

        _context.HistorialEstados.Add(historial);
        _context.SaveChanges();

        return true;
    }

    public EstadoSolicitud ObtenerEstadoActual(int solicitudId)
    {
        var solicitud = _context.Solicitudes
            .Include(s => s.Estado)
            .FirstOrDefault(s => s.Id == solicitudId && s.Activo);

        if (solicitud == null)
        {
            throw new Exception("Solicitud no encontrada o inactiva.");
        }

        // Convertir el ID del estado al enum correspondiente
        return (EstadoSolicitud)solicitud.EstadoId;
    }

    public List<EstadoSolicitud> ObtenerTodosLosEstados(int solicitudId)
    {
        var historial = _context.HistorialEstados
            .Include(h => h.EstadoAnterior)
            .Include(h => h.EstadoNuevo)
            .Where(h => h.SolicitudId == solicitudId)
            .OrderBy(h => h.FechaCambio)
            .ToList();

        if (!historial.Any())
        {
            throw new Exception("No se encontró historial para la solicitud especificada.");
        }

        var estados = new List<EstadoSolicitud>();

        // Agregar el estado inicial (EstadoAnterior del primer registro)
        estados.Add((EstadoSolicitud)historial.First().EstadoAnteriorId);

        // Agregar todos los estados nuevos del historial
        foreach (var registro in historial)
        {
            estados.Add((EstadoSolicitud)registro.EstadoNuevoId);
        }

        return estados;
    }

    public List<Estados> ObtenerEstados()
    {
        // Obtener la solicitud de manera asíncrona
        var solicitud = _context.Estados.ToList();

        if (solicitud == null)
        {
            throw new Exception("Solicitud no encontrada o inactiva.");
        }

        // Convertir el ID del estado al enum correspondiente
        return solicitud;
    }

    public List<Solicitudes> ObtenerTodasLasSolicitudes(bool soloActivas = true)
    {
        // Consulta base para obtener las solicitudes
        var query = _context.Solicitudes
            .Include(s => s.Estado) // Incluir el estado actual de la solicitud
            .AsQueryable();

        // Filtrar solo las solicitudes activas si se solicita
        if (soloActivas)
        {
            query = query.Where(s => s.Activo);
        }

        // Devolver la lista de solicitudes
        return query.ToList();
    }
    #endregion

    #region MetodosAsync
    public async Task<bool> CambiarEstadoAsync(int solicitudId, EstadoSolicitud nuevoEstado, string usuario)
    {
        // Convertir el enum a su valor entero
        int nuevoEstadoId = (int)nuevoEstado;

        // Obtener la solicitud de manera asíncrona
        var solicitud = await _context.Solicitudes
            .Include(s => s.Estado)
            .FirstOrDefaultAsync(s => s.Id == solicitudId && s.Activo);
        if (solicitud == null) throw new Exception("Solicitud no encontrada o inactiva");

        // Verificar si la transición es válida de manera asíncrona
        var transicionValida = await _context.Transiciones
            .AnyAsync(t => t.EstadoOrigenId == solicitud.EstadoId && t.EstadoDestinoId == nuevoEstadoId && t.Activo);
        if (!transicionValida) throw new Exception("Transición de estado no permitida");

        // Obtener el nuevo estado de manera asíncrona
        var nuevoEstadoEntity = await _context.Estados.FindAsync(nuevoEstadoId);
        if (nuevoEstadoEntity == null || !nuevoEstadoEntity.Activo) throw new Exception("Estado destino no encontrado o inactivo");

        // Crear el registro de historial
        var historial = new HistorialEstados
        {
            SolicitudId = solicitudId,
            EstadoAnteriorId = solicitud.EstadoId,
            EstadoNuevoId = nuevoEstadoId,
            Usuario = usuario,
            FechaCambio = DateTime.UtcNow,
            Creado = DateTime.UtcNow,
            CreadoPor = usuario
        };

        // Actualizar la solicitud
        solicitud.EstadoId = nuevoEstadoId;
        solicitud.Modificado = DateTime.UtcNow;
        solicitud.ModificadoPor = usuario;

        // Guardar los cambios de manera asíncrona
        _context.HistorialEstados.Add(historial);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<EstadoSolicitud> ObtenerEstadoActualAsync(int solicitudId)
    {
        // Obtener la solicitud de manera asíncrona
        var solicitud = await _context.Solicitudes
            .Include(s => s.Estado)
            .FirstOrDefaultAsync(s => s.Id == solicitudId && s.Activo);

        if (solicitud == null)
        {
            throw new Exception("Solicitud no encontrada o inactiva.");
        }

        // Convertir el ID del estado al enum correspondiente
        return (EstadoSolicitud)solicitud.EstadoId;
    }

    public async Task<List<Estados>> ObtenerEstadosAsync()
    {
        // Obtener la solicitud de manera asíncrona
        var solicitud = await _context.Estados.ToListAsync();

        if (solicitud == null)
        {
            throw new Exception("Solicitud no encontrada o inactiva.");
        }

        // Convertir el ID del estado al enum correspondiente
        return solicitud;
    }

    public async Task<List<EstadoSolicitud>> ObtenerTodosLosEstadosAsync(int solicitudId)
    {
        // Obtener el historial de manera asíncrona
        var historial = await _context.HistorialEstados
            .Include(h => h.EstadoAnterior)
            .Include(h => h.EstadoNuevo)
            .Where(h => h.SolicitudId == solicitudId)
            .OrderBy(h => h.FechaCambio)
            .ToListAsync();

        if (!historial.Any())
        {
            throw new Exception("No se encontró historial para la solicitud especificada.");
        }

        var estados = new List<EstadoSolicitud>();

        // Agregar el estado inicial (EstadoAnterior del primer registro)
        estados.Add((EstadoSolicitud)historial.First().EstadoAnteriorId);

        // Agregar todos los estados nuevos del historial
        foreach (var registro in historial)
        {
            estados.Add((EstadoSolicitud)registro.EstadoNuevoId);
        }

        return estados;
    }

    public async Task<List<Solicitudes>> ObtenerTodasLasSolicitudesAsync(bool soloActivas = true)
    {
        // Consulta base para obtener las solicitudes
        var query = _context.Solicitudes
            .Include(s => s.Estado) // Incluir el estado actual de la solicitud
            .AsQueryable();

        // Filtrar solo las solicitudes activas si se solicita
        if (soloActivas)
        {
            query = query.Where(s => s.Activo);
        }

        // Devolver la lista de solicitudes de manera asíncrona
        return await query.ToListAsync();
    }

    #endregion

}