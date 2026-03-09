//using System.Globalization;
//using System.Text.Json;
//using System.Text.Json.Serialization;
//using Snebur.Core.Utils;

//namespace Snebur.Application.Common.JsonConverters;

//internal class EntityJsonConverter<TEntity> : JsonConverter<TEntity>
//    where TEntity : EntityBase
//{
//    public override TEntity? Read(
//        ref Utf8JsonReader reader,
//        Type typeToConvert,
//        JsonSerializerOptions options)
//    {
//        var jsonDoc = JsonDocument.ParseValue(ref reader);
//        var jsonObject = jsonDoc.RootElement;

//        var jsonSerializerOptions = new JsonSerializerOptions(options);
//        jsonSerializerOptions.Converters.Remove(this);
//        jsonSerializerOptions.Converters.Clear();

//        // Deserialize the object normally
//        var entityBase = JsonUtils.Deserialize<TEntity>(jsonObject.GetRawText(), jsonSerializerOptions);

//        if (entityBase is null)
//            return null;

//        // Manually extract and set the Id property (from EntityBase)

//        SetPropertyValue<Guid>(nameof(EntityBase.Id), ref jsonObject, entityBase);
//        SetPropertyValue<Guid>(nameof(EntityBase.CreatedSession_Id), ref jsonObject, entityBase);
//        SetPropertyValue<Guid>(nameof(EntityBase.LastUpdatedSession_Id), ref jsonObject, entityBase);
//        SetPropertyValue<DateTime>(nameof(EntityBase.CreatedAt), ref jsonObject, entityBase);
//        SetPropertyValue<DateTime>(nameof(EntityBase.LastUpdatedAt), ref jsonObject, entityBase);

//        return entityBase;
//    }

//    private void SetPropertyValue<TPropertyType>(string propertyName, ref JsonElement jsonObject,
//        EntityBase entity)
//    {
//        if (jsonObject.TryGetProperty(propertyName, out var property) &&
//            property.ValueKind == JsonValueKind.String)
//        {
//            var value = GetPropertyValue<TPropertyType>(property.GetString()!);
//            var propertyInfo = entity.GetType().GetProperty(propertyName);
//            propertyInfo?.SetValue(entity, value);
//        }
//    }

//    private object? GetPropertyValue<TPropertyType>(string valueSerilized)
//    {
//        if (!string.IsNullOrEmpty(valueSerilized))
//        {
//            if (typeof(TPropertyType) == typeof(Guid))
//            {
//                if (Guid.TryParse(valueSerilized, out var guid))
//                    return guid;

//                ThrowIfDebug($"DateTime value {valueSerilized} could not be parsed");
//            }
//            else if (typeof(TPropertyType) == typeof(DateTime))
//            {
//                if (DateTime.TryParse(valueSerilized, CultureInfo.InvariantCulture, out var dateTime))
//                    return dateTime;

//                ThrowIfDebug($"DateTime value {valueSerilized} could not be parsed");
//            }
//            else
//            {
//                throw new NotImplementedException($"Type {typeof(TPropertyType)} not implemented");
//            }
//        }
//        return default(TPropertyType);
//    }

//    private void ThrowIfDebug(string message)
//    {
//#if DEBUG
//        throw new InvalidOperationException(message);
//#endif
//    }

//    public override void Write(Utf8JsonWriter writer, TEntity value, JsonSerializerOptions options)
//    {
//        options.Converters.Remove(this);
//        JsonSerializer.Serialize(writer, (object)value, options);
//    }
//}
