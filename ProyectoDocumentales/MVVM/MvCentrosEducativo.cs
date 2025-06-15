using ProyectoDocumentales.backend.Modelo;
using ProyectoDocumentales.MVVM.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.Json;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using ProyectoDocumentales.Servicios;


namespace ProyectoDocumentales.MVVM
{
    public class MvCentrosEducativo : MVBaseCRUD<CentrosEducativo>
    {


        private CentrosEducativo _centro;

        private CentrosEducativo _centroOriginal; //Copia

        private readonly Documentales2Context _contexto;
        private readonly Usuario _usuario;

        private CentroEducativoServicio _centroEducativoServicio;

        public CentrosEducativo Centro
        {
            get => _centro;
            set
            {
                _centro = value;
                OnPropertyChanged(nameof(Centro));
            }
        }

        public MvCentrosEducativo(Documentales2Context contexto, Usuario usuario)
        {
            _contexto = contexto;
            _usuario = usuario;
        }
        public async Task Inicializar()
        {
            _centroEducativoServicio = new CentroEducativoServicio(_contexto);
            servicio = _centroEducativoServicio;

            await CargarCentroDelUsuario();
        }

        public async Task CargarCentroDelUsuario()
        {
            try
            {
                if (_usuario?.IdCentroEducativo != null)
                {
                    //Cargar el centro educativo del usuario desde la base de datos usando el servicio
                    var centroUsuario = await _centroEducativoServicio.GetByIdAsync(_usuario.IdCentroEducativo.Value);

                    if (centroUsuario != null)
                    {
                        Centro = centroUsuario;
                        _centroOriginal = new CentrosEducativo
                        {
                            IdCentroEducativo = centroUsuario.IdCentroEducativo,
                            Nombre = centroUsuario.Nombre,
                            Direccion = centroUsuario.Direccion,
                            Cif = centroUsuario.Cif,
                            Telefono = centroUsuario.Telefono,
                            Fax = centroUsuario.Fax,
                            CodigoPostal = centroUsuario.CodigoPostal,
                            Director = centroUsuario.Director
                        };
                        return;
                    }
                }

                //Si no se encuentra el centro del usuario, crear uno nuevo vacío
                Centro = new CentrosEducativo();
                _centroOriginal = new CentrosEducativo();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar el centro educativo: {ex.Message}",
                               "Error",
                               MessageBoxButton.OK,
                               MessageBoxImage.Error);

                //En caso de error, crear un centro vacío
                Centro = new CentrosEducativo();
                _centroOriginal = new CentrosEducativo();
            }
        }

        private async Task<bool> ComprobadorCamposUnicos()
        {
            //Verificar si otro centro tiene el mismo NºConcierto
            var cifExistente = await _centroEducativoServicio.FindAsync(r =>
                r.Cif == Centro.Cif && r.IdCentroEducativo != Centro.IdCentroEducativo);
            if (cifExistente.Any())
            {
                MessageBox.Show("El Centro no puede tener el mismo cif que otro centro. Introduce uno diferente.",
                              "Duplicado", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;

        }

     

        public async Task<bool> ActualizarCentro()
        {
            try
            {
                if (Centro != null)
                {
                    //Comprobamos campos unicos
                    if (!await ComprobadorCamposUnicos()) return false;


                    if (Centro.IdCentroEducativo > 0)
                    {
                        // ctualizar centro
                        await _centroEducativoServicio.UpdateAsync(Centro);
                    }
                   

                    //Actualizar la copia original
                    _centroOriginal = new CentrosEducativo
                    {
                        IdCentroEducativo = Centro.IdCentroEducativo,
                        Nombre = Centro.Nombre,
                        Direccion = Centro.Direccion,
                        Cif = Centro.Cif,
                        Telefono = Centro.Telefono,
                        Fax = Centro.Fax,
                        CodigoPostal = Centro.CodigoPostal,
                        Director = Centro.Director
                    };

                    MessageBox.Show("Los datos del centro educativo se han guardado correctamente.",
                                   "Éxito",
                                   MessageBoxButton.OK,
                                   MessageBoxImage.Information);

                    return true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar los datos del centro educativo: {ex.Message}",
                               "Error",
                               MessageBoxButton.OK,
                               MessageBoxImage.Error);
            }

            return false;
        }

        public void RestaurarCambios()
        {
            if (_centroOriginal != null)
            {
                Centro = new CentrosEducativo
                {
                    IdCentroEducativo = _centroOriginal.IdCentroEducativo,
                    Nombre = _centroOriginal.Nombre,
                    Direccion = _centroOriginal.Direccion,
                    Cif = _centroOriginal.Cif,
                    Telefono = _centroOriginal.Telefono,
                    Fax = _centroOriginal.Fax,
                    CodigoPostal = _centroOriginal.CodigoPostal,
                    Director = _centroOriginal.Director
                };
            }

        }

    }
}
