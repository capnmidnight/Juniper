using Juniper.Cedrus.Entities;
using Juniper.Data;
using Juniper.Units;

using Microsoft.EntityFrameworkCore;

using System.Text.Json;

namespace Juniper.Cedrus.Models;

public partial class CedrusContextSecure
{
    private static readonly Dictionary<DataType, Type[]> ExpectedTypes = new()
    {
        [DataType.Boolean] = [typeof(bool)],
        [DataType.Integer] = [typeof(ulong), typeof(long), typeof(uint), typeof(int), typeof(ushort), typeof(short), typeof(byte), typeof(sbyte)],
        [DataType.Decimal] = [typeof(decimal), typeof(double), typeof(float), typeof(ulong), typeof(long), typeof(uint), typeof(int), typeof(ushort), typeof(short), typeof(byte), typeof(sbyte)],
        [DataType.Currency] = [typeof(decimal), typeof(double), typeof(float), typeof(ulong), typeof(long), typeof(uint), typeof(int), typeof(ushort), typeof(short), typeof(byte), typeof(sbyte)],
        [DataType.String] = [typeof(string)],
        [DataType.Enumeration] = [typeof(string)],
        [DataType.LongText] = [typeof(string)],
        [DataType.Date] = [typeof(DateTime)],
        [DataType.Link] = [typeof(string)],
        [DataType.File] = [typeof(string)]
    };

    private static T ValidateDataType<T>(PropertyType type, T? value)
        where T : notnull
    {
        if (value is null)
        {
            throw new ArgumentNullException(nameof(value), $"PropertyType {type.Name} expects a value.");
        }

        var actual = value.GetType();
        if (type.Storage == StorageType.Single && actual.IsArray)
        {
            throw new ArgumentOutOfRangeException(nameof(value), $"PropertyType {type.Name} expects a single value, but an array of values was given");
        }

        if (!ExpectedTypes.TryGetValue(type.Type, out var expected)
            || expected is null)
        {
            throw new ArgumentOutOfRangeException(nameof(value), $"PropertyType {type.Name} has a type {type.Type} that cannot be processed.");
        }

        if (actual.IsArray)
        {
            actual = actual.GetElementType()!;
        }

        if (actual.BaseType is not null
            && actual.BaseType.IsGenericType
            && actual.BaseType.GetGenericTypeDefinition() == typeof(TimeSeries<>))
        {
            if (type.Storage != StorageType.TimeSeries)
            {
                throw new ArgumentOutOfRangeException(nameof(value), $"PropertyType {type.Name} was not expected to be a Time Series collection.");
            }

            actual = actual.BaseType.GetGenericArguments()[0];
        }

        if (!expected.Contains(actual))
        {
            var validTypes = expected
                .Select(v => v.Name)
                .ToArray()
                .Join(", ");
            throw new ArgumentOutOfRangeException(nameof(value), value, $"PropertyType {type.Name} expects value in types of {validTypes}");
        }

        return value;
    }

    private static UnitOfMeasure ValidateUnitType(PropertyType type, UnitOfMeasure unitOfMeasure)
    {
        if ((type.Type == DataType.Integer || type.Type == DataType.Decimal)
            && !Converter.IsInCategory(type.UnitsCategory, unitOfMeasure))
        {
            var validUnits = Converter
                .GetUnitsInCategory(type.UnitsCategory)
                .Select(v => v.ToString())
                .ToArray()
                .Join(", ");
            throw new ArgumentOutOfRangeException(nameof(unitOfMeasure), unitOfMeasure, $"PropertyType {type.Name} expects values in units of {validUnits}");
        }

        return unitOfMeasure;
    }

    public IQueryable<Property> GetProperties(CedrusUser user, NameOrId[]? entityTypes = null, NameOrId[]? entities = null, NameOrId[]? propertyTypes = null)
    {
        entityTypes.CheckTypeStamp("entityType");
        entities.CheckTypeStamp("entity");
        propertyTypes.CheckTypeStamp("propertyType");
        var entityIds = entities.IDs();
        var entityNames = entities.Names();
        var entityTypeIds = entityTypes.IDs();
        var entityTypeNames = entityTypes.Names();
        var propertyTypeIds = propertyTypes.IDs();
        var propertyTypeNames = propertyTypes.Names();
        return insecure.Properties
            .Where(p => (entityIds.Length == 0 || entityIds.Contains(p.EntityId))
                && (entityNames.Length == 0 || entityNames.Contains(p.Entity.Name))
                && (entityTypeIds.Length == 0 || entityTypeIds.Contains(p.Entity.TypeId))
                && (entityTypeNames.Length == 0 || entityTypeNames.Contains(p.Entity.Type.Name))
                && (propertyTypeIds.Length == 0 || propertyTypeIds.Contains(p.TypeId))
                && (propertyTypeNames.Length == 0 || propertyTypeNames.Contains(p.Type.Name)));
    }

    public Task<Property?> GetPropertyInsecureAsync(Entity entity, PropertyType propertyType) =>
        insecure.Properties
        .Where(p => p.Entity == entity
            && p.Type == propertyType)
        .SingleOrDefaultAsync();

    public IQueryable<Property> GetPropertyInsecure(params EntityType[] entityTypes) =>
        insecure.Properties
        .Where(p => entityTypes.Contains(p.Entity.Type));

    public async Task<Property> GetPropertyAsync(int propertyId, CedrusUser user) =>
        await GetProperties(user).SingleOrDefaultAsync(p => p.Id == propertyId)
            ?? throw new FileNotFoundException($"Property:{propertyId}");

    public void DeleteProperty(Property property) =>
        insecure.Properties.Remove(property);

    /// <summary>
    /// Set a boolean value on an entity.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="entity"></param>
    /// <param name="value"></param>
    /// <param name="user"></param>
    /// <returns></returns>
    public Task<Property> SetPropertyAsync(PropertyType type, Entity entity, bool value, CedrusUser user, Entity? reference = null) =>
        SetPropertyAsync_Internal(type, entity, value, UnitOfMeasure.None, user, reference);

    /// <summary>
    /// Set a boolean array value on an entity.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="entity"></param>
    /// <param name="value"></param>
    /// <param name="user"></param>
    /// <returns></returns>
    public Task<Property> SetPropertyAsync(PropertyType type, Entity entity, bool[] value, CedrusUser user, Entity? reference = null) =>
        SetPropertyAsync_Internal(type, entity, value, UnitOfMeasure.None, user, reference);

    /// <summary>
    /// Set a string value on an entity.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="entity"></param>
    /// <param name="value"></param>
    /// <param name="user"></param>
    /// <returns></returns>
    public Task<Property> SetPropertyAsync(PropertyType type, Entity entity, string value, CedrusUser user, Entity? reference = null) =>
        SetPropertyAsync_Internal(type, entity, value, UnitOfMeasure.None, user, reference);

    /// <summary>
    /// Set a string array value on an entity.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="entity"></param>
    /// <param name="value"></param>
    /// <param name="user"></param>
    /// <returns></returns>
    public Task<Property> SetPropertyAsync(PropertyType type, Entity entity, string[] value, CedrusUser user, Entity? reference = null) =>
        SetPropertyAsync_Internal(type, entity, value, UnitOfMeasure.None, user, reference);

    /// <summary>
    /// Set a string timeseries value on an entity.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="entity"></param>
    /// <param name="value"></param>
    /// <param name="user"></param>
    /// <returns></returns>
    public Task<Property> SetPropertyAsync(PropertyType type, Entity entity, StringTimeSeries[] value, CedrusUser user, Entity? reference = null) =>
        SetPropertyAsync_Internal(type, entity, value, UnitOfMeasure.None, user, reference);

    /// <summary>
    /// Set a DateTime value on an entity.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="entity"></param>
    /// <param name="value"></param>
    /// <param name="user"></param>
    /// <returns></returns>
    public Task<Property> SetPropertyAsync(PropertyType type, Entity entity, DateTime value, CedrusUser user, Entity? reference = null) =>
        SetPropertyAsync_Internal(type, entity, value, UnitOfMeasure.None, user, reference);

    /// <summary>
    /// Set a DateTime array value on an entity.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="entity"></param>
    /// <param name="value"></param>
    /// <param name="user"></param>
    /// <returns></returns>
    public Task<Property> SetPropertyAsync(PropertyType type, Entity entity, DateTime[] value, CedrusUser user, Entity? reference = null) =>
        SetPropertyAsync_Internal(type, entity, value, UnitOfMeasure.None, user, reference);

    /// <summary>
    /// Set a File as a property of an entity.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="entity"></param>
    /// <param name="file"></param>
    /// <param name="user"></param>
    /// <returns></returns>
    public Task<Property> SetPropertyAsync(PropertyType type, Entity entity, FileAsset file, CedrusUser user) =>
        SetPropertyAsync_Internal(type, entity, file.LinkPath, UnitOfMeasure.None, user);

    /// <summary>
    /// Set a 32-bit integer value, with units, as a property of an entity.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="entity"></param>
    /// <param name="value"></param>
    /// <param name="unitOfMeasure"></param>
    /// <param name="user"></param>
    /// <returns></returns>
    public Task<Property> SetPropertyAsync(PropertyType type, Entity entity, int value, UnitOfMeasure unitOfMeasure, CedrusUser user, Entity? reference = null) =>
        SetPropertyAsync_Internal(type, entity, value, unitOfMeasure, user, reference);

    /// <summary>
    /// Set a 32-bit integer array value, with units, as a property of an entity.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="entity"></param>
    /// <param name="value"></param>
    /// <param name="unitOfMeasure"></param>
    /// <param name="user"></param>
    /// <returns></returns>
    public Task<Property> SetPropertyAsync(PropertyType type, Entity entity, int[] value, UnitOfMeasure unitOfMeasure, CedrusUser user, Entity? reference = null) =>
        SetPropertyAsync_Internal(type, entity, value, unitOfMeasure, user, reference);

    /// <summary>
    /// Set a 32-bit integer timeseries value, with units, as a property of an entity.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="entity"></param>
    /// <param name="value"></param>
    /// <param name="unitOfMeasure"></param>
    /// <param name="user"></param>
    /// <returns></returns>
    public Task<Property> SetPropertyAsync(PropertyType type, Entity entity, IntegerTimeSeries[] value, UnitOfMeasure unitOfMeasure, CedrusUser user, Entity? reference = null) =>
        SetPropertyAsync_Internal(type, entity, value, unitOfMeasure, user, reference);

    /// <summary>
    /// Set a 64-bit float value, with units, as a property of an entity.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="entity"></param>
    /// <param name="value"></param>
    /// <param name="unitOfMeasure"></param>
    /// <param name="user"></param>
    /// <returns></returns>
    public Task<Property> SetPropertyAsync(PropertyType type, Entity entity, double value, UnitOfMeasure unitOfMeasure, CedrusUser user, Entity? reference = null) =>
        SetPropertyAsync_Internal(type, entity, value, unitOfMeasure, user, reference);

    /// <summary>
    /// Set a 64-bit float array value, with units, as a property of an entity.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="entity"></param>
    /// <param name="value"></param>
    /// <param name="unitOfMeasure"></param>
    /// <param name="user"></param>
    /// <returns></returns>
    public Task<Property> SetPropertyAsync(PropertyType type, Entity entity, double[] value, UnitOfMeasure unitOfMeasure, CedrusUser user, Entity? reference = null) =>
        SetPropertyAsync_Internal(type, entity, value, unitOfMeasure, user, reference);

    /// <summary>
    /// Set a 64-bit float timeseries value, with units, as a property of an entity.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="entity"></param>
    /// <param name="value"></param>
    /// <param name="unitOfMeasure"></param>
    /// <param name="user"></param>
    /// <returns></returns>
    public Task<Property> SetPropertyAsync(PropertyType type, Entity entity, DoubleTimeSeries[] value, UnitOfMeasure unitOfMeasure, CedrusUser user, Entity? reference = null) =>
        SetPropertyAsync_Internal(type, entity, value, unitOfMeasure, user, reference);

    /// <summary>
    /// Set a decimal value, with units, as a property of an entity.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="entity"></param>
    /// <param name="value"></param>
    /// <param name="unitOfMeasure"></param>
    /// <param name="user"></param>
    /// <returns></returns>
    public Task<Property> SetPropertyAsync(PropertyType type, Entity entity, decimal value, UnitOfMeasure unitOfMeasure, CedrusUser user, Entity? reference = null) =>
        SetPropertyAsync_Internal(type, entity, value, unitOfMeasure, user, reference);

    /// <summary>
    /// Set a decimal array value, with units, as a property of an entity.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="entity"></param>
    /// <param name="value"></param>
    /// <param name="unitOfMeasure"></param>
    /// <param name="user"></param>
    /// <returns></returns>
    public Task<Property> SetPropertyAsync(PropertyType type, Entity entity, decimal[] value, UnitOfMeasure unitOfMeasure, CedrusUser user, Entity? reference = null) =>
        SetPropertyAsync_Internal(type, entity, value, unitOfMeasure, user, reference);

    /// <summary>
    /// Set a decimal timeseries value, with units, as a property of an entity.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="entity"></param>
    /// <param name="value"></param>
    /// <param name="unitOfMeasure"></param>
    /// <param name="user"></param>
    /// <returns></returns>
    public Task<Property> SetPropertyAsync(PropertyType type, Entity entity, DecimalTimeSeries[] value, UnitOfMeasure unitOfMeasure, CedrusUser user, Entity? reference = null) =>
        SetPropertyAsync_Internal(type, entity, value, unitOfMeasure, user, reference);

    /// <summary>
    /// Set a boolean value on an entity.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="entity"></param>
    /// <param name="value"></param>
    /// <param name="user"></param>
    /// <returns></returns>
    public Task<Property> SetPropertyAsync<EnumT>(PropertyType type, Entity entity, EnumT value, CedrusUser user, Entity? reference = null)
        where EnumT : struct, Enum =>
        SetPropertyAsync_Internal(type, entity, value.ToString(), UnitOfMeasure.None, user, reference);

    /// <summary>
    /// Set a generic, unitless value, as a property of an entity
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="type"></param>
    /// <param name="entity"></param>
    /// <param name="value"></param>
    /// <param name="user"></param>
    /// <returns></returns>
    public Task<Property> SetPropertyAsync(PropertyType type, Entity entity, JsonElement value, UnitOfMeasure unitOfMeasure, CedrusUser user, Entity? reference = null) =>
        (type.Type, type.Storage) switch
        {
            (DataType.Boolean, StorageType.Single) => SetPropertyAsync(type, entity, value.GetBoolean(), user, reference),
            (DataType.Decimal, StorageType.Single) => SetPropertyAsync(type, entity, value.GetDouble(), unitOfMeasure, user, reference),
            (DataType.Decimal, StorageType.Array) => SetPropertyAsync(type, entity, value.GetDoubleArray(), unitOfMeasure, user, reference),
            (DataType.Decimal, StorageType.TimeSeries) => SetPropertyAsync(type, entity, value.GetDoubleTimeSeries(), unitOfMeasure, user, reference),
            (DataType.Currency, StorageType.Single) => SetPropertyAsync(type, entity, value.GetDecimal(), unitOfMeasure, user, reference),
            (DataType.Currency, StorageType.Array) => SetPropertyAsync(type, entity, value.GetDecimalArray(), unitOfMeasure, user, reference),
            (DataType.Currency, StorageType.TimeSeries) => SetPropertyAsync(type, entity, value.GetDecimalTimeSeries(), unitOfMeasure, user, reference),
            (DataType.Integer, StorageType.Single) => SetPropertyAsync(type, entity, value.GetInt32(), unitOfMeasure, user, reference),
            (DataType.Integer, StorageType.Array) => SetPropertyAsync(type, entity, value.GetIntegerArray(), unitOfMeasure, user, reference),
            (DataType.Integer, StorageType.TimeSeries) => SetPropertyAsync(type, entity, value.GetIntegerTimeSeries(), unitOfMeasure, user, reference),
            (DataType.Enumeration, StorageType.Single) => SetPropertyAsync(type, entity, value.GetString()!, user, reference),
            (DataType.Enumeration, StorageType.Array) => SetPropertyAsync(type, entity, value.GetStringArray()!, user, reference),
            (DataType.Date, StorageType.Single) => SetPropertyAsync(type, entity, value.GetDateTime(), user, reference),
            (DataType.Date, StorageType.Array) => SetPropertyAsync(type, entity, value.GetDateTimeArray(), user, reference),
            (DataType.File or DataType.String, StorageType.Single) => SetPropertyAsync(type, entity, value.GetString()!, user, reference),
            (DataType.File or DataType.String, StorageType.Array) => SetPropertyAsync(type, entity, value.GetStringArray()!, user, reference),
            (DataType.String, StorageType.TimeSeries) => SetPropertyAsync(type, entity, value.GetStringTimeSeries(), user, reference),
            (DataType.Link or DataType.LongText, StorageType.Single) => SetPropertyAsync(type, entity, value.GetString()!, user, reference),
            _ => throw new NotImplementedException()
        };

    /// <summary>
    /// Set a generic value, with units, as a property of an entity.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="type"></param>
    /// <param name="entity"></param>
    /// <param name="value"></param>
    /// <param name="unitOfMeasure"></param>
    /// <param name="user"></param>
    /// <returns></returns>
    public Task<Property> SetPropertyAsync_Internal<T>(PropertyType type, Entity entity, T value, UnitOfMeasure unitOfMeasure, CedrusUser user, Entity? reference = null)
        where T : notnull
    {
        var property = insecure.Properties.UpsertAsync(
            p => p.Entity.Name == entity.Name && p.Type.Name == type.Name,
            () => new Property
            {
                Entity = entity,
                Type = type,
                Value = ValidateDataType(type, value),
                Units = ValidateUnitType(type, unitOfMeasure),
                Reference = reference,
                User = user,
                UpdatedByUser = user
            },
            (here) =>
            {
                here.Value = ValidateDataType(type, value);
                here.Units = ValidateUnitType(type, unitOfMeasure);
                here.Reference= reference;
                here.UpdatedByUser = user;
                here.UpdatedOn = DateTime.Now;
            }
        );

        return property;
    }
}
