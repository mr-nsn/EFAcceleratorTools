using Apparatus.AOT.Reflection;
using EFAcceleratorTools.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Reflection;

namespace EFAcceleratorTools.DataTables
{
    public static class DataTableExtensions
    {
        public static DataTable ToDataTable<T>(this ICollection<T> data, Dictionary<string, int> columnsOrder, DbContext dbContext) where T : Entity
        {
            var dataTable = new DataTable(typeof(T).Name);
            var order = new Dictionary<(string propName, string colName), int>();
            var entityType = dbContext.Model.FindEntityType(typeof(T));

            if (entityType == null) throw new ArgumentException("The entity is not configured in the DbContext.");

            var propertyMappings = entityType.GetProperties()
                                             .ToDictionary(p => p.Name, p => p.GetColumnName(StoreObjectIdentifier.Table(entityType.GetTableName()!)));

            var properties = typeof(T).GetProperties()
                .Where(p => (p.PropertyType.IsPrimitive ||
                            p.PropertyType.IsValueType ||
                            p.PropertyType == typeof(string))
                            && (p.CanWrite &&
                            p.GetCustomAttribute<NotMappedAttribute>() == null));

            foreach (var prop in properties)
            {
                if (propertyMappings.TryGetValue(prop.Name, out var columnName))
                {
                    var column = new DataColumn(columnName, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
                    column.AllowDBNull = true;
                    dataTable.Columns.Add(column);

                    if (columnsOrder.TryGetValue(prop.Name, out var columnIndex))
                        order.Add((prop.Name!, columnName!), columnIndex);
                }
                else
                    throw new KeyNotFoundException($"The property '{prop.Name}' does not have a column mapping in the database.");
            }

            foreach (var item in data)
            {
                var values = new object[properties.Count()];
                int i = 0;

                foreach (var prop in properties)
                {
                    values[i] = prop.GetValue(item) ?? DBNull.Value;
                    i++;
                }

                dataTable.Rows.Add(values);
            }

            dataTable.ReorderDataTableColumns(order);
            return dataTable;
        }

        public static void ReorderDataTableColumns(this DataTable table, Dictionary<(string propName, string colName), int> columnOrder)
        {
            if (columnOrder is null)
                throw new ArgumentException($"Column order must be defined.");

            foreach (var order in columnOrder)
                if (!table.Columns.Contains(order.Key.colName))
                    throw new ArgumentException($"Column '{order.Key.colName}' does not exist in the DataTable.");

            foreach (var order in columnOrder)
                table.Columns[order.Key.colName]!.SetOrdinal(order.Value);
        }
    }
}
