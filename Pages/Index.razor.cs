using MaquinaEstado.DB.Model;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using static MaquinaEstado.DB.Enum.EnumEstados;

namespace MaquinaEstado.Pages
{
    public partial class Index
    {

        [Inject] WorkFlowService workFlowService { get; set; }
        [Inject] ISnackbar Snackbar { get; set; }

        public List<Estados> Estados = new List<Estados>();
        public List<Solicitudes> Solicitudes { get; set; } = new();




        protected override async Task OnInitializedAsync()
        {
            Estados = await workFlowService.ObtenerEstadosAsync();
            await SolicitudesAsync();
        }

        private async Task SolicitudesAsync()
        {
            Solicitudes = await workFlowService.ObtenerTodasLasSolicitudesAsync(true);
        }


        // Variable para almacenar el último estado seleccionado
        private EstadoSolicitud? _ultimoEstadoSeleccionado = EstadoSolicitud.Creado;

        private void ActualizarDatos(Solicitudes solicitud)
        {
            try
            {
                // Obtener todos los valores del enum
                var estadosValidos = Enum.GetValues(typeof(EstadoSolicitud)).Cast<EstadoSolicitud>().ToList();

                EstadoSolicitud estadoAleatorio;

                // Generar un estado aleatorio hasta que sea diferente al último
                do
                {
                    var random = new Random();
                    int indiceAleatorio = random.Next(estadosValidos.Count);
                    estadoAleatorio = estadosValidos[indiceAleatorio];
                }
                while (estadoAleatorio == _ultimoEstadoSeleccionado && estadosValidos.Count > 1);

                // Actualizar el último estado seleccionado
                _ultimoEstadoSeleccionado = estadoAleatorio;

                // Llamar al servicio para cambiar el estado
                var items = workFlowService.CambiarEstado(solicitud.Id, estadoAleatorio, "Marco");

                // Mostrar un mensaje de éxito
                Snackbar.Add($"Estado cambiado a {estadoAleatorio}", Severity.Success);
            }
            catch (Exception ex)
            {
                // Mostrar un mensaje de error
                Snackbar.Add(ex.Message, Severity.Warning);
            }
        }


    }
}
