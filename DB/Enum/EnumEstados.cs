namespace MaquinaEstado.DB.Enum
{
    public class EnumEstados
    {
        public enum EstadoSolicitud
        {
            Creado = 1,         // ID 1 en la base de datos
            EnRevision = 2,     // ID 2 en la base de datos
            Aprobado = 3,       // ID 3 en la base de datos
            Rechazado = 4,      // ID 4 en la base de datos
            Finalizado = 5,     // ID 5 en la base de datos
            PendienteDocumentacion = 6, // ID 6 en la base de datos
            DocumentacionCompleta = 7,  // ID 7 en la base de datos
            EnProceso = 8,      // ID 8 en la base de datos
            Cancelado = 9,      // ID 9 en la base de datos
            Archivado = 10      // ID 10 en la base de datos
        }
    }
}
