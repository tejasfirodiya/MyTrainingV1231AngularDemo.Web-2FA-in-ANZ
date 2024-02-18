using Abp;

namespace MyTrainingV1231AngularDemo.OpenIddict.Scopes
{
    public static class OpenIddictScopeExtensions
{
    public static OpenIddictScope ToEntity(this OpenIddictScopeModel model)
    {
        Check.NotNull(model, nameof(model));

        var entity = new OpenIddictScope(model.Id)
        {
            Description = model.Description,
            Descriptions = model.Descriptions,
            DisplayName = model.DisplayName,
            DisplayNames = model.DisplayNames,
            Name = model.Name,
            Properties = model.Properties,
            Resources = model.Resources
        };

        return entity;
    }

    public static OpenIddictScope ToEntity(this OpenIddictScopeModel model, OpenIddictScope entity)
    {
        Check.NotNull(model, nameof(model));
        Check.NotNull(entity, nameof(entity));

        entity.Description = model.Description;
        entity.Descriptions = model.Descriptions;
        entity.DisplayName = model.DisplayName;
        entity.DisplayNames = model.DisplayNames;
        entity.Name = model.Name;
        entity.Properties = model.Properties;
        entity.Resources = model.Resources;

        return entity;
    }

    public static OpenIddictScopeModel ToModel(this OpenIddictScope entity)
    {
        if(entity == null)
        {
            return null;
        }

        var model = new OpenIddictScopeModel
        {
            Id = entity.Id,
            Description = entity.Description,
            Descriptions = entity.Descriptions,
            DisplayName = entity.DisplayName,
            DisplayNames = entity.DisplayNames,
            Name = entity.Name,
            Properties = entity.Properties,
            Resources = entity.Resources
        };

        return model;
    }
}
}