using Juniper.Units;

using Microsoft.EntityFrameworkCore;

using Juniper.Cedrus.Entities;
using Juniper.Cedrus.Controllers.V1;
using System.Text.Json;
using System.Text.Json.Nodes;
using Newtonsoft.Json.Linq;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Juniper.Cedrus.Models;

public partial class CedrusContextSecure
{
    private static readonly Dictionary<DataType, Type[]> ExpectedTypes = new()
    {
        [DataType.Boolean] = new[] { typeof(bool) },
        [DataType.BooleanArray] = new[] { typeof(bool[]) },
        [DataType.Integer] = new[] { typeof(ulong), typeof(long), typeof(uint), typeof(int), typeof(ushort), typeof(short), typeof(byte), typeof(sbyte) },
        [DataType.IntegerArray] = new[] { typeof(ulong[]), typeof(long[]), typeof(uint[]), typeof(int[]), typeof(ushort[]), typeof(short[]), typeof(byte[]), typeof(sbyte[]) },
        [DataType.Decimal] = new[] { typeof(double), typeof(float), typeof(ulong), typeof(long), typeof(uint), typeof(int), typeof(ushort), typeof(short), typeof(byte), typeof(sbyte) },
        [DataType.DecimalArray] = new[] { typeof(double[]), typeof(float[]), typeof(ulong[]), typeof(long[]), typeof(uint[]), typeof(int[]), typeof(ushort[]), typeof(short[]), typeof(byte[]), typeof(sbyte[]) },
        [DataType.String] = new[] { typeof(string) },
        [DataType.StringArray] = new[] { typeof(string[]) },
        [DataType.Date] = new[] { typeof(DateTime) },
        [DataType.DateArray] = new[] { typeof(DateTime[]) },
        [DataType.Link] = new[] { typeof(string) },
        [DataType.File] = new[] { typeof(string) }
    };

    private static T ValidateDataType<T>(PropertyType type, T value)
        where T : notnull
    {
        var actual = value.GetType();
        if (ExpectedTypes.TryGetValue(type.Type, out var expected)
            && expected is not null
            && !expected.Contains(actual))
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

    public IQueryable<Property> GetProperties(CedrusUser user)
    {
        var now = DateTime.Now;
        var parts = GetClassificationParts(user);
        return insecure.Properties.Current<PropertyType, Property>(parts)
            .Include(p => p.ReferenceEntity)
                .ThenInclude(e => e!.Properties
                    .Where(rp => rp.Start <= now && now < rp.End
                            && parts.Levels.Contains(rp.Classification.LevelId)
                            && rp.Classification.Caveats.All(cv => parts.Caveats.Contains(cv.Id))));
    }

    public async Task<Property> GetPropertyAsync(int propertyId, CedrusUser user) =>
        await GetProperties(user).SingleOrDefaultAsync(p => p.Id == propertyId)
            ?? throw new FileNotFoundException();

    public void EndProperty(Property property) =>
        property.End = DateTime.Now;

    public Property? FindPropertyAtDate(PropertyType type, Entity entity, DateTime context) => FindPropertiesAtDate(entity.Id, type.Id, context).SingleOrDefault();
    public Task<Property?> FindPropertyAtDateAsync(PropertyType type, Entity entity, DateTime context) => FindPropertiesAtDate(entity.Id, type.Id, context).SingleOrDefaultAsync();
    private IQueryable<Property> FindPropertiesAtDate(int entityId, int typeId, DateTime context) =>
        from ev in insecure.Properties
        where ev.TypeId == typeId
            && ev.EntityId == entityId
            && ev.Start <= context && context < ev.End
        select ev;

    /// <summary>
    /// Set a boolean value on an entity.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="entity"></param>
    /// <param name="value"></param>
    /// <param name="user"></param>
    /// <param name="classification"></param>
    /// <param name="startDate"></param>
    /// <param name="endDate"></param>
    /// <returns></returns>
    public Property SetProperty(PropertyType type, Entity entity, bool value, CedrusUser user, Classification? classification = null, DateTime? startDate = null, DateTime? endDate = null) =>
        SetProperty(type, entity, value, UnitOfMeasure.None, user, classification, startDate, endDate);

    /// <summary>
    /// Set a boolean array value on an entity.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="entity"></param>
    /// <param name="value"></param>
    /// <param name="user"></param>
    /// <param name="classification"></param>
    /// <param name="startDate"></param>
    /// <param name="endDate"></param>
    /// <returns></returns>
    public Property SetProperty(PropertyType type, Entity entity, bool[] value, CedrusUser user, Classification? classification = null, DateTime? startDate = null, DateTime? endDate = null) =>
        SetProperty(type, entity, value, UnitOfMeasure.None, user, classification, startDate, endDate);

    /// <summary>
    /// Set a string value on an entity.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="entity"></param>
    /// <param name="value"></param>
    /// <param name="user"></param>
    /// <param name="classification"></param>
    /// <param name="startDate"></param>
    /// <param name="endDate"></param>
    /// <returns></returns>
    public Property SetProperty(PropertyType type, Entity entity, string value, CedrusUser user, Classification? classification = null, DateTime? startDate = null, DateTime? endDate = null) =>
        SetProperty(type, entity, value, UnitOfMeasure.None, user, classification, startDate, endDate);

    /// <summary>
    /// Set a string array value on an entity.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="entity"></param>
    /// <param name="value"></param>
    /// <param name="user"></param>
    /// <param name="classification"></param>
    /// <param name="startDate"></param>
    /// <param name="endDate"></param>
    /// <returns></returns>
    public Property SetProperty(PropertyType type, Entity entity, string[] value, CedrusUser user, Classification? classification = null, DateTime? startDate = null, DateTime? endDate = null) =>
        SetProperty(type, entity, value, UnitOfMeasure.None, user, classification, startDate, endDate);

    /// <summary>
    /// Set a DateTime value on an entity.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="entity"></param>
    /// <param name="value"></param>
    /// <param name="user"></param>
    /// <param name="classification"></param>
    /// <param name="startDate"></param>
    /// <param name="endDate"></param>
    /// <returns></returns>
    public Property SetProperty(PropertyType type, Entity entity, DateTime value, CedrusUser user, Classification? classification = null, DateTime? startDate = null, DateTime? endDate = null) =>
        SetProperty(type, entity, value, UnitOfMeasure.None, user, classification, startDate, endDate);

    /// <summary>
    /// Set a DateTime array value on an entity.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="entity"></param>
    /// <param name="value"></param>
    /// <param name="user"></param>
    /// <param name="classification"></param>
    /// <param name="startDate"></param>
    /// <param name="endDate"></param>
    /// <returns></returns>
    public Property SetProperty(PropertyType type, Entity entity, DateTime[] value, CedrusUser user, Classification? classification = null, DateTime? startDate = null, DateTime? endDate = null) =>
        SetProperty(type, entity, value, UnitOfMeasure.None, user, classification, startDate, endDate);

    /// <summary>
    /// Set a File as a property of an entity.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="entity"></param>
    /// <param name="file"></param>
    /// <param name="user"></param>
    /// <param name="classification"></param>
    /// <param name="startDate"></param>
    /// <param name="endDate"></param>
    /// <returns></returns>
    public Property SetProperty(PropertyType type, Entity entity, FileAsset file, CedrusUser user, Classification? classification = null, DateTime? startDate = null, DateTime? endDate = null) =>
        SetProperty(type, entity, file.LinkPath, UnitOfMeasure.None, user, classification, startDate, endDate);

    /// <summary>
    /// Set a 32-bit integer value, with units, as a property of an entity.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="entity"></param>
    /// <param name="value"></param>
    /// <param name="unitOfMeasure"></param>
    /// <param name="user"></param>
    /// <param name="classification"></param>
    /// <param name="startDate"></param>
    /// <param name="endDate"></param>
    /// <returns></returns>
    public Property SetProperty(PropertyType type, Entity entity, int value, UnitOfMeasure unitOfMeasure, CedrusUser user, Classification? classification = null, DateTime? startDate = null, DateTime? endDate = null) =>
        SetProperty<int>(type, entity, value, unitOfMeasure, user, classification, startDate, endDate);

    /// <summary>
    /// Set a 32-bit integer array value, with units, as a property of an entity.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="entity"></param>
    /// <param name="value"></param>
    /// <param name="unitOfMeasure"></param>
    /// <param name="user"></param>
    /// <param name="classification"></param>
    /// <param name="startDate"></param>
    /// <param name="endDate"></param>
    /// <returns></returns>
    public Property SetProperty(PropertyType type, Entity entity, int[] value, UnitOfMeasure unitOfMeasure, CedrusUser user, Classification? classification = null, DateTime? startDate = null, DateTime? endDate = null) =>
        SetProperty<int[]>(type, entity, value, unitOfMeasure, user, classification, startDate, endDate);

    /// <summary>
    /// Set a 32-bit float value, with units, as a property of an entity.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="entity"></param>
    /// <param name="value"></param>
    /// <param name="unitOfMeasure"></param>
    /// <param name="user"></param>
    /// <param name="classification"></param>
    /// <param name="startDate"></param>
    /// <param name="endDate"></param>
    /// <returns></returns>
    public Property SetProperty(PropertyType type, Entity entity, float value, UnitOfMeasure unitOfMeasure, CedrusUser user, Classification? classification = null, DateTime? startDate = null, DateTime? endDate = null) =>
        SetProperty<float>(type, entity, value, unitOfMeasure, user, classification, startDate, endDate);

    /// <summary>
    /// Set a 32-bit float array value, with units, as a property of an entity.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="entity"></param>
    /// <param name="value"></param>
    /// <param name="unitOfMeasure"></param>
    /// <param name="user"></param>
    /// <param name="classification"></param>
    /// <param name="startDate"></param>
    /// <param name="endDate"></param>
    /// <returns></returns>
    public Property SetProperty(PropertyType type, Entity entity, float[] value, UnitOfMeasure unitOfMeasure, CedrusUser user, Classification? classification = null, DateTime? startDate = null, DateTime? endDate = null) =>
        SetProperty<float[]>(type, entity, value, unitOfMeasure, user, classification, startDate, endDate);

    /// <summary>
    /// Set a 64-bit float value, with units, as a property of an entity.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="entity"></param>
    /// <param name="value"></param>
    /// <param name="unitOfMeasure"></param>
    /// <param name="user"></param>
    /// <param name="classification"></param>
    /// <param name="startDate"></param>
    /// <param name="endDate"></param>
    /// <returns></returns>
    public Property SetProperty(PropertyType type, Entity entity, double value, UnitOfMeasure unitOfMeasure, CedrusUser user, Classification? classification = null, DateTime? startDate = null, DateTime? endDate = null) =>
        SetProperty<double>(type, entity, value, unitOfMeasure, user, classification, startDate, endDate);

    /// <summary>
    /// Set a 64-bit float array value, with units, as a property of an entity.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="entity"></param>
    /// <param name="value"></param>
    /// <param name="unitOfMeasure"></param>
    /// <param name="user"></param>
    /// <param name="classification"></param>
    /// <param name="startDate"></param>
    /// <param name="endDate"></param>
    /// <returns></returns>
    public Property SetProperty(PropertyType type, Entity entity, double[] value, UnitOfMeasure unitOfMeasure, CedrusUser user, Classification? classification = null, DateTime? startDate = null, DateTime? endDate = null) =>
        SetProperty<double[]>(type, entity, value, unitOfMeasure, user, classification, startDate, endDate);

    /// <summary>
    /// Set a generic, unitless value, as a property of an entity
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="type"></param>
    /// <param name="entity"></param>
    /// <param name="value"></param>
    /// <param name="user"></param>
    /// <param name="classification"></param>
    /// <param name="startDate"></param>
    /// <param name="endDate"></param>
    /// <returns></returns>
    public Property SetProperty<T>(PropertyType type, Entity entity, T value, CedrusUser user, Classification? classification = null, DateTime? startDate = null, DateTime? endDate = null)
        where T : notnull =>
        SetProperty(type, entity, value, UnitOfMeasure.None, user, classification, startDate, endDate);

    /// <summary>
    /// Set a generic, unitless value, as a property of an entity
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="type"></param>
    /// <param name="entity"></param>
    /// <param name="value"></param>
    /// <param name="classification"></param>
    /// <param name="user"></param>
    /// <param name="startDate"></param>
    /// <param name="endDate"></param>
    /// <returns></returns>
    public Property SetProperty(PropertyType type, Entity entity, JsonElement value, UnitOfMeasure unitOfMeasure, CedrusUser user, Classification? classification = null, DateTime? startDate = null, DateTime? endDate = null)
    {
        if(value.ValueKind == JsonValueKind.Array)
        {
            return SetPropertyArray(type, entity, value.EnumerateArray(), unitOfMeasure, user, classification, startDate, endDate);
        }

        return type.Type switch
        {
            DataType.Boolean => SetProperty(type, entity, value.GetBoolean(), user, classification, startDate, endDate),
            DataType.Date => SetProperty(type, entity, value.GetDateTime(), user, classification, startDate, endDate),
            DataType.Decimal => SetProperty(type, entity, value.GetDouble(), unitOfMeasure, user, classification, startDate, endDate),
            DataType.File or DataType.Link or DataType.String => SetProperty(type, entity, value.GetString()!, user, classification, startDate, endDate),
            DataType.Integer => SetProperty(type, entity, value.GetInt32(), unitOfMeasure, user, classification, startDate, endDate),
            _ => throw new NotImplementedException()
        };
    }

    private Property SetPropertyArray(PropertyType type, Entity entity, JsonElement.ArrayEnumerator array, UnitOfMeasure unitOfMeasure, CedrusUser user, Classification? classification = null, DateTime? startDate = null, DateTime? endDate = null)
    {
        var elements = array.ToArray();
        if(elements.Length == 0)
        {
            throw new ArgumentException("Cannot store empty array.");
        }

        var kinds = elements
            .Select(e => e.ValueKind)
            .Distinct()
            .ToArray();

        if(kinds.Length > 1)
        {
            throw new ArgumentException("Cannot store dynamically typed array.");
        }

        return type.Type switch
        {
            DataType.BooleanArray => SetProperty(type, entity, elements.Select(value => value.GetBoolean()).ToArray(), user, classification, startDate, endDate),
            DataType.DateArray => SetProperty(type, entity, elements.Select(value => value.GetDateTime()).ToArray(), user, classification, startDate, endDate),
            DataType.DecimalArray => SetProperty(type, entity, elements.Select(value => value.GetDouble()).ToArray(), unitOfMeasure, user, classification, startDate, endDate),
            DataType.StringArray => SetProperty(type, entity, elements.Select(value => value.GetString()!).ToArray(), user, classification, startDate, endDate),
            DataType.IntegerArray => SetProperty(type, entity, elements.Select(value => value.GetInt32()).ToArray(), unitOfMeasure, user, classification, startDate, endDate),
            _ => throw new NotImplementedException()
        };
    }

    /// <summary>
    /// Set a generic value, with units, as a property of an entity.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="type"></param>
    /// <param name="entity"></param>
    /// <param name="value"></param>
    /// <param name="unitOfMeasure"></param>
    /// <param name="user"></param>
    /// <param name="classification"></param>
    /// <param name="startDate"></param>
    /// <param name="endDate"></param>
    /// <returns></returns>
    public Property SetProperty<T>(PropertyType type, Entity entity, T value, UnitOfMeasure unitOfMeasure, CedrusUser user, Classification? classification = null, DateTime? startDate = null, DateTime? endDate = null)
        where T : notnull =>
        insecure.Properties.TimeSeriesSplit(
            type,
            p => p.EntityId == entity.Id && p.TypeId == type.Id,
            p => p.Entity == entity && p.Type.Name == type.Name,
            (a, b) => a.Value.Equals(b.Value)
                && a.Units == b.Units
                && a.Classification.Id == b.Classification.Id,
            (type, start, end) => new Property
            {
                Entity = entity,
                Type = type,
                Value = ValidateDataType(type, value),
                Units = ValidateUnitType(type, unitOfMeasure),
                Classification = classification ?? U,
                User = user,
                Start = start,
                End = end
            },
            (type, here, start, end) => new Property
            {
                Entity = entity,
                Type = type,
                Value = ValidateDataType(type, value),
                Units = ValidateUnitType(type, unitOfMeasure),
                Classification = classification ?? here.Classification,
                User = user,
                Start = start,
                End = end
            },
            startDate,
            endDate
        );
}
