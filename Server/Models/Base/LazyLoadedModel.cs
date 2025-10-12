namespace LabServer.Server.Models;

using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore.Infrastructure;

public class LazyLoadedModel : DBObjectModel
{
    protected LazyLoadedModel() { }
    protected ILazyLoader LazyLoader { get; set; }
    [NotMapped]
    [JsonIgnore]
    protected ILazyLoader Loader => LazyLoader ?? throw new NotImplementedException("shouldn't be here");
    protected LazyLoadedModel(ILazyLoader lazyLoader) => LazyLoader = lazyLoader;
    // TODO : doesn't work for some reason (returns null)
    protected Model LazyLoad<Model>(System.Object entity, ref Model? navigationField) where Model : class
        => Loader.Load(entity, ref navigationField) ?? throw new NotImplementedException("error in lazyloading");
}