using ProyectoDocumentales.backend.Modelo;
using ProyectoDocumentales.MVVM.Base;
using ProyectoDocumentales.Servicios;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ProyectoDocumentales.MVVM
{
    public class MvEmpresa : MVBaseCRUD<Empresa>
    {
        //Contexto
        private Documentales2Context _contexto;
        private EmpresaServicio _empresaServicio;
        private ResponsableServicio _responsableServicio;
        private CentroTrabajoServicio _centroTrabajoServicio;

      

        private Empresa _empresa;
        public Empresa empresa
        {
            get { return _empresa; }
            set
            {
                _empresa = value;
                OnPropertyChanged(nameof(empresa));
            }
        }



        private Responsable _responsable;
        public Responsable responsable
        {
            get { return _responsable; }
            set
            {
                _responsable = value;
                OnPropertyChanged(nameof(responsable));
            }
        }

        private CentrosTrabajo _centroTrabajo;

        public CentrosTrabajo centroTrabajo
        {
            get { return _centroTrabajo; }
            set
            {
                _centroTrabajo = value;
                OnPropertyChanged(nameof(centroTrabajo));
            }
        }

        //Lista obserbable de centros de trabajo sin empresa asignada
        private ObservableCollection<CentrosTrabajo> _centrosLibres;
        public ObservableCollection<CentrosTrabajo> CentrosLibres
        {

            get { return _centrosLibres; }
            set
            {
                _centrosLibres = value;
                OnPropertyChanged(nameof(CentrosLibres));
            }
        }
        //Lista de centros seleccionados para la empresa
        private ObservableCollection<CentrosTrabajo> _centrosSeleccionados;
        public ObservableCollection<CentrosTrabajo> CentrosSeleccionados
        {
            get { return _centrosSeleccionados; }
            set
            {
                _centrosSeleccionados = value;
                OnPropertyChanged(nameof(CentrosSeleccionados));
            }
        }

        //Lista de responsables sin empresa asignada
        private ObservableCollection<Responsable> _responsablesLibres;
        public ObservableCollection<Responsable> ResponsablesLibres
        {
            get { return _responsablesLibres; }
            set
            {
                _responsablesLibres = value;
                OnPropertyChanged(nameof(ResponsablesLibres));
            }
        }

       
        //Para controlar visibilidad de las tablas
        public bool HasCentrosLibres => CentrosLibres?.Count > 0;

        private void OnCentrosLibresChanged()
        {
            OnPropertyChanged(nameof(CentrosLibres));
            OnPropertyChanged(nameof(HasCentrosLibres));
        }

        public MvEmpresa(Documentales2Context contexto)
        {
            _contexto = contexto;
            _empresaServicio = new EmpresaServicio(contexto);
            _responsableServicio = new ResponsableServicio(contexto);
            _centroTrabajoServicio = new CentroTrabajoServicio(contexto);

            empresa = new Empresa();
            responsable = new Responsable();
            centroTrabajo = new CentrosTrabajo();

            //Centros de trabajo libres y seleccionados
            CentrosLibres = new ObservableCollection<CentrosTrabajo>();
            CentrosSeleccionados = new ObservableCollection<CentrosTrabajo>();

            //Responsables Libres
            ResponsablesLibres = new ObservableCollection<Responsable>();

        
            _ = InicializarDatosAsync();
        }

      

        public async Task InicializarDatosAsync()
        {
            await CargarResponsablesLibres();
            await CargarCentrosLibres();
        }

        public async Task CargarCentrosLibres()
        {
            try
            {
                var centrosLibres = await _centroTrabajoServicio.GetCentrosLibresAsync();

                //Actualizar la lista en el hilo principal
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    CentrosLibres.Clear();
                    foreach (var centro in centrosLibres)
                    {
                        CentrosLibres.Add(centro);
                    }
                });
            }
            catch (Exception ex)
            {
                //Manejar error de carga - En el hilo principal
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    MessageBox.Show($"Error al cargar centros libres: {ex.Message}");
                });
            }
        }

        public async Task CargarResponsablesLibres()
        {
            try
            {
                var responsablesLibres = await _responsableServicio.GetResponsablesLibresAsync();

                //Actualizar la colección en el hilo principal
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    ResponsablesLibres.Clear();
                    foreach (var responsable in responsablesLibres)
                    {
                        ResponsablesLibres.Add(responsable);
                    }
                });
            }
            catch (Exception ex)
            {
                // Manejo de errores al cargar responsables libres - En el hilo principal
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    MessageBox.Show($"Error al cargar responsables libres: {ex.Message}");
                });
            }
        }

        public void AnadirCentroAEmpresa(CentrosTrabajo centro)
        {
            if (centro != null && !CentrosSeleccionados.Contains(centro))
            {
                CentrosSeleccionados.Add(centro);
                CentrosLibres.Remove(centro);
            }
        }
        public void QuitarCentroDeLaEmpresa(CentrosTrabajo centro)
        {
            if (centro != null && CentrosSeleccionados.Contains(centro))
            {
                CentrosSeleccionados.Remove(centro);
                CentrosLibres.Add(centro);
            }
        }
        public async Task<bool> GuardarEmpresa()
        {
            try
            {
               
                // Primero guardamos la empresa

                await _empresaServicio.AddAsync(empresa);

                //Luego asignamos los centros de trabajo seleccionados a la empresa
                foreach (var centro in CentrosSeleccionados)
                {
                    await _centroTrabajoServicio.AsignarCentroAEmpresaAsync(centro.IdCentroTrabajo, empresa.IdEmpresa);
                }

                //Limpiar despues de guardar lo ejecuta el hilo principal
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    empresa = new Empresa();
                    CentrosSeleccionados.Clear();
                });

                //Recargar centros libres
                await CargarCentrosLibres();

                //Recargar Responsables Libres
                await CargarResponsablesLibres();

                return true;
            }
            catch (Exception ex)
            {
                //Manejo de errores en el hilo principal
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    MessageBox.Show($"Error al guardar empresa: {ex.Message}");
                });
                return false;
            }
        }


        public async Task<bool> GuardaResponsable()
        {
            try
            {
                await _responsableServicio.AddAsync(responsable);
                //Limpiar el responsable despues de guardarlo
                responsable = new Responsable();

                //Recargar la lista de responsables libres
                await CargarResponsablesLibres();

                return true;
            }
            catch (Exception ex)
            {
                // Manejo de errores en el hilo principal
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    MessageBox.Show($"Error al guardar responsable: {ex.Message}");
                });
                return false;
            }
        }
        public async Task<bool> GuardarCentroTrabajo()
        {
            try
            {
                await _centroTrabajoServicio.AddAsync(centroTrabajo);
                //Limpiar el centro de trabajo despues de guardarlo
                centroTrabajo = new CentrosTrabajo();

                //Recargar la lista de centros libres
                await CargarCentrosLibres();


                return true;
            }
            catch (Exception ex)
            {
                // Manejo de errores en el hilo principal
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    MessageBox.Show($"Error al guardar centro de trabajo: {ex.Message}");
                });
                return false;
            }
        }



    }
}
