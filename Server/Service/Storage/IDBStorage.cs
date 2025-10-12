namespace LabServer.Server.Service;

using LabServer.Server.Models;

public interface IDBStorage<D> where D : DBObjectModel
{
    IEnumerable<D> GetAll();
    Task<D?> GetById(System.Int64 Id);
    Task Add(D dbModel);
    Task ApplyChangesAsync();
}