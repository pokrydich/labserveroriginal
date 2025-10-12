namespace LabServer.Server.Models;

using Microsoft.EntityFrameworkCore.Infrastructure;

public class AccessMapping<E> : LazyLoadedModel where E : DBObjectModel
{
    public AccessMapping() { }
    protected AccessMapping(ILazyLoader lazyLoader) : base(lazyLoader) { }

    private E? _entity;
    public System.Int64 EntityId;
    public E Entity
    {
        get => Loader.Load(this, ref _entity) ?? throw new NotImplementedException();
        set => _entity = value;
    }

    private UserModel? _user;
    public System.Int64 UserId;
    public UserModel User
    {
        get => Loader.Load(this, ref _user) ?? throw new NotImplementedException();
        set => _user = value;
    }

    public System.Boolean Owner { get; set; }
}