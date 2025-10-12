namespace LabServer.Server.Service;

using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

using LabServer.Server.Data;
using LabServer.Server.Hubs;
using LabServer.Server.Models;

public class DBStorage<D> : IDBStorage<D> where D : DBObjectModel
{
    private LabsContext _context;
    private DbSet<D> _dataSet;
    public DBStorage(LabsContext context)
    {
        _context = context;
        _dataSet = _context.GetStorage<D>();
    }

    public IEnumerable<D> GetAll() => _dataSet.AsEnumerable().ToList();
    public async Task<D?> GetById(System.Int64 Id) => await _dataSet.SingleOrDefaultAsync(dbo => dbo.Id == Id);
    public async Task Add(D dbModel)
    {
        var res = await _dataSet.AddAsync(dbModel);
        if (dbModel is IGitIntegrated gitIntegrated)
            gitIntegrated.InjectGitLab(_context.GitLabService); // do EF's work (it didn't inject IGitLab service into an entity created outside of the scope)
    }
    public async Task ApplyChangesAsync() => await _context.SaveChangesAsync();
}