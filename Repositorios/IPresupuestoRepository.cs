public interface IPresupuestoRepository{
    void CrearNuevo(Presupuestos presupuesto); 
    List<Presupuestos> ListarPresupuestos();
    List<PresupuestoDetalle> ObtenerDetalle(int id); 
    Presupuestos ObtenerPresupuestoPorId(int id);
    void ModificarPresupuestoQ(Presupuestos presupuesto);
    void EliminarPresupuesto(int id); 

}