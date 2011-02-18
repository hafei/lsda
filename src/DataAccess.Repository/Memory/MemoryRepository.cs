// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MemoryRepository.cs" company="Logic Software">
//   (c) Logic Software
// </copyright>
// <summary>
//   In-Memory Entities Repository
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LogicSoftware.DataAccess.Repository.Memory
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Linq;
    using System.Data.Linq.Mapping;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Text;

    using Basic;

    using Infrastructure.Extensions;
    using Infrastructure.Helpers;
    using Infrastructure.Linq;

    using Mapping;

    /// <summary>
    /// In-Memory Entities Repository
    /// </summary>
    [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "To be refactored :(")]
    public class MemoryRepository : IRepository
    {
        #region Constants and Fields

        /// <summary>
        ///   The all method.
        /// </summary>
        private static readonly MethodInfo AllMethod = typeof(MemoryRepository).GetMethod("All", new[] { typeof(LoadOptions) });

        /// <summary>
        ///   The data lock object.
        /// </summary>
        private readonly object DataLockObject = new object();

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MemoryRepository"/> class.
        /// </summary>
        /// <param name="mappingSourceManager">
        /// The mapping source manager.
        /// </param>
        public MemoryRepository(IMappingSourceManager mappingSourceManager)
            : this()
        {
            if (mappingSourceManager == null)
            {
                throw new ArgumentNullException("mappingSourceManager");
            }

            this.MappingSource = mappingSourceManager.MappingSource;
        }

        /// <summary>
        ///   Prevents a default instance of the <see cref = "MemoryRepository" /> class from being created. 
        ///   Initializes a new instance of the <see cref = "MemoryRepository" /> class.
        /// </summary>
        private MemoryRepository()
        {
            this.Data = new DataSet();
            this.Data.Locale = CultureInfo.InvariantCulture;
        }

        #endregion

        #region Events

        /// <summary>
        ///   Occurs when [deleted].
        /// </summary>
        public event EventHandler<MemoryRepositoryStatusEventArgs> Deleted;

        /// <summary>
        ///   Occurs when [deleting].
        /// </summary>
        public event EventHandler<MemoryRepositoryMethodEventArgs> Deleting;

        /// <summary>
        ///   Occurs when [inserted].
        /// </summary>
        public event EventHandler<MemoryRepositoryStatusEventArgs> Inserted;

        /// <summary>
        ///   Occurs when [inserting].
        /// </summary>
        public event EventHandler<MemoryRepositoryMethodEventArgs> Inserting;

        /// <summary>
        ///   Occurs when [selected].
        /// </summary>
        public event EventHandler<MemoryRepositoryStatusEventArgs> Selected;

        /// <summary>
        ///   Occurs when [selecting].
        /// </summary>
        public event EventHandler<MemoryRepositorySelectingEventArgs> Selecting;

        /// <summary>
        ///   Occurs when [updated].
        /// </summary>
        public event EventHandler<MemoryRepositoryStatusEventArgs> Updated;

        /// <summary>
        ///   Occurs when [updating].
        /// </summary>
        public event EventHandler<MemoryRepositoryMethodEventArgs> Updating;

        #endregion

        #region Properties

        /// <summary>
        ///   Gets or sets the data.
        /// </summary>
        /// <value>The dataset with repository data.</value>
        public DataSet Data { get; set; }

        /// <summary>
        ///   Gets MappingSource.
        /// </summary>
        /// <value>The mapping source.</value>
        public MappingSource MappingSource { get; private set; }

        #endregion

        #region Implemented Interfaces (Methods)

        #region IRepository methods

        /// <summary>
        /// Returns query for all entities of type T.
        /// </summary>
        /// <typeparam name="T">
        /// Type of the entity.
        /// </typeparam>
        /// <returns>
        /// Query for all entities of type T.
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "By design, to allow type-safe calls.")]
        public IQueryable<T> All<T>() where T : class
        {
            return this.All<T>(new LoadOptions());
        }

        /// <summary>
        /// Returns query for all entities of type T.
        ///   Non-generic overload.
        /// </summary>
        /// <param name="entityType">
        /// Type of the entity.
        /// </param>
        /// <returns>
        /// Query for all entities of type T.
        /// </returns>
        public IQueryable All(Type entityType)
        {
            return this.All(entityType, new LoadOptions());
        }

        /// <summary>
        /// Returns query for all entities of type T with specified load options.
        /// </summary>
        /// <typeparam name="T">
        /// Type of the entity.
        /// </typeparam>
        /// <param name="loadOptions">
        /// The load options.
        /// </param>
        /// <returns>
        /// Query for all entities of type T.
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "By design, to allow type-safe calls.")]
        public IQueryable<T> All<T>(LoadOptions loadOptions) where T : class
        {
            this.OnSelecting(new MemoryRepositorySelectingEventArgs());

            DataTable entitiesTable = this.FindTable<T>();

            List<T> result = entitiesTable.AsEnumerable().Select<DataRow, T>(this.MapRowToObject<T>).ToList();

            this.LoadAssociationsForEach(result, loadOptions);

            var provider = new MemoryQueryProvider<T>(result.AsQueryable(), this);

            var wrappedResult = new MemoryQueryable<T>(provider);

            this.OnSelected(new MemoryRepositoryStatusEventArgs(wrappedResult, null));

            return wrappedResult;
        }

        /// <summary>
        /// Returns query for all entities of type T with specified load options.
        ///   Non-generic overload.
        /// </summary>
        /// <param name="entityType">
        /// Type of the entity.
        /// </param>
        /// <param name="loadOptions">
        /// The load options.
        /// </param>
        /// <returns>
        /// Query for all entities of type T.
        /// </returns>
        public IQueryable All(Type entityType, LoadOptions loadOptions)
        {
            var allMethodGeneric = AllMethod.MakeGenericMethod(entityType);

            return (IQueryable)allMethodGeneric.Invoke(this, new[] { loadOptions });
        }

        /// <summary>
        /// Deletes the specified entity.
        /// </summary>
        /// <typeparam name="T">
        /// Type of the entity.
        /// </typeparam>
        /// <param name="entity">
        /// The entity.
        /// </param>
        public void Delete<T>(T entity) where T : class
        {
            this.Delete(typeof(T), entity);
        }

        /// <summary>
        /// Executes the specified query.
        /// </summary>
        /// <typeparam name="TResult">
        /// The type of the result.
        /// </typeparam>
        /// <param name="query">
        /// The query expression.
        /// </param>
        /// <returns>
        /// The result of the query execution.
        /// </returns>
        public TResult Execute<TResult>(Expression query)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Inserts the specified entity.
        /// </summary>
        /// <typeparam name="T">
        /// Type of the entity.
        /// </typeparam>
        /// <param name="entity">
        /// The entity.
        /// </param>
        public void Insert<T>(T entity) where T : class
        {
            this.OnInserting(new MemoryRepositoryMethodEventArgs(entity));

            DataTable entitiesTable = this.FindTable<T>();

            DataRow newRow = entitiesTable.NewRow();

            this.FillGeneratedMembers(newRow, entity);
            this.MapObjectToRow<T>(entity, newRow);

            entitiesTable.Rows.Add(newRow);
            this.Data.AcceptChanges();

            this.OnInserted(new MemoryRepositoryStatusEventArgs(entity, null));
        }

        /// <summary>
        /// Updates the specified entity.
        /// </summary>
        /// <typeparam name="T">
        /// Type of the entity.
        /// </typeparam>
        /// <param name="entity">
        /// The entity.
        /// </param>
        public void Update<T>(T entity) where T : class
        {
            this.OnUpdating(new MemoryRepositoryMethodEventArgs(entity));

            DataRow row = this.GetEntityRow<T>(entity);

            T oldEntity = this.MapRowToObject<T>(row);

            this.MapObjectToRow<T>(entity, row);

            this.Data.AcceptChanges();

            this.OnUpdated(new MemoryRepositoryStatusEventArgs(entity, oldEntity));
        }

        #endregion

        #endregion

        #region Methods

        /// <summary>
        /// Returns all entities with specified property value.
        /// </summary>
        /// <typeparam name="T">
        /// Entity type
        /// </typeparam>
        /// <param name="property">
        /// The property.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// All entities with specified property value.
        /// </returns>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "By design, invoked with reflection call.")]
        internal List<T> GetAllByPropertyValue<T>(PropertyInfo property, object value) where T : class
        {
            IQueryable<T> all = this.All<T>();
            return FilterByPropertyValue(all, property, value).ToList();
        }

        /// <summary>
        /// Gets the model.
        /// </summary>
        /// <returns>
        /// Meta model.
        /// </returns>
        internal MetaModel GetModel()
        {
            return this.MappingSource.GetModel(typeof(DataContext));
        }

        /// <summary>
        /// Gets the single entity with specified property value.
        /// </summary>
        /// <typeparam name="T">
        /// Entity type
        /// </typeparam>
        /// <param name="property">
        /// The property.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The single entity with specified property value.
        /// </returns>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "By design, invoked with reflection call.")]
        internal T GetSingleByPropertyValue<T>(PropertyInfo property, object value) where T : class
        {
            IQueryable<T> all = this.All<T>();
            return FilterByPropertyValue(all, property, value).SingleOrDefault();
        }

        /// <summary>
        /// Converts from db value.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="type">
        /// The type to convert to.
        /// </param>
        /// <returns>
        /// converted value
        /// </returns>
        private static object ConvertFromDbValue(object value, Type type)
        {
            object rawValue = DBNull.Value.Equals(value) ? null : value;
            object convertedValue;
            if (type.IsEnum)
            {
                convertedValue = Enum.Parse(type, rawValue.ToString());
            }
            else if (type == typeof(Binary))
            {
                convertedValue = new Binary((byte[])rawValue);
            }
            else
            {
                if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    if (rawValue == null)
                    {
                        convertedValue = null;
                    }
                    else
                    {
                        Type actualType = type.GetGenericArguments()[0];
                        convertedValue = Convert.ChangeType(rawValue, actualType, CultureInfo.InvariantCulture);
                    }
                }
                else
                {
                    convertedValue = Convert.ChangeType(rawValue, type, CultureInfo.InvariantCulture);
                }
            }

            return convertedValue;
        }

        /// <summary>
        /// Converts to db value.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// Value converted to database-formatted value
        /// </returns>
        private static object ConvertToDbValue(object value)
        {
            if (value == null)
            {
                return DBNull.Value;
            }

            Type valueType = value.GetType();

            if (valueType == typeof(Binary))
            {
                return ((Binary)value).ToArray();
            }
            else
            {
                return value;
            }
        }

        /// <summary>
        /// Filters entities the by property value.
        /// </summary>
        /// <typeparam name="T">
        /// Entity type
        /// </typeparam>
        /// <param name="entities">
        /// The entities.
        /// </param>
        /// <param name="property">
        /// The property.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// Entites with specific value of specified propery.
        /// </returns>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "By design, invoked with reflection call.")]
        private static IQueryable<T> FilterByPropertyValue<T>(IQueryable<T> entities, PropertyInfo property, object value)
        {
            // build where expression
            ParameterExpression objectParameter = Expression.Parameter(typeof(T), "a");
            MemberExpression keyPropertyExpression = Expression.Property(objectParameter, property);
            ConstantExpression constToCompare = Expression.Constant(value);

            if (keyPropertyExpression.Type.IsValueType && value == null)
            {
                // null FK Case, return empty list
                return new List<T>().AsQueryable();
            }

            LambdaExpression filterLambda;

            // todo: add support for nullable == null fiters
            if (keyPropertyExpression.Type.IsGenericType &&
                keyPropertyExpression.Type.GetGenericTypeDefinition() == typeof(Nullable<>) &&
                keyPropertyExpression.Type.GetGenericArguments().First() == value.GetType())
            {
                var notNullPropertyExpression = keyPropertyExpression.Property("HasValue");
                var keyPropertyValueExpression = keyPropertyExpression.Property("Value");

                BinaryExpression equalityExpression = Expression.Equal(keyPropertyValueExpression, constToCompare);

                BinaryExpression notNullCheckExpression = Expression.AndAlso(notNullPropertyExpression, equalityExpression);

                filterLambda = Expression.Lambda(notNullCheckExpression, objectParameter);
            }
            else
            {
                BinaryExpression equalityExpression = Expression.Equal(keyPropertyExpression, constToCompare);

                filterLambda = Expression.Lambda(equalityExpression, objectParameter);
            }

            var predicate = (Expression<Func<T, bool>>)filterLambda;

            return entities.Where(predicate);
        }

        /// <summary>
        /// Gets the storage type of the column.
        /// </summary>
        /// <param name="type">
        /// The original column type.
        /// </param>
        /// <returns>
        /// The storage type of a column
        /// </returns>
        private static Type GetColumnStorageType(Type type)
        {
            if (type == typeof(Binary))
            {
                return typeof(byte[]);
            }
            else
            {
                return type;
            }
        }

        /// <summary>
        /// Creates the table to store entities data.
        /// </summary>
        /// <typeparam name="T">
        /// Entity type
        /// </typeparam>
        /// <param name="tableName">
        /// Name of the table.
        /// </param>
        /// <returns>
        /// The table to store entities data
        /// </returns>
        private DataTable CreateTable<T>(string tableName)
        {
            return this.CreateTable(typeof(T), tableName);
        }

        /// <summary>
        /// Creates the table to store entities data.
        /// </summary>
        /// <param name="entityType">
        /// Entity type
        /// </param>
        /// <param name="tableName">
        /// Name of the table.
        /// </param>
        /// <returns>
        /// The table to store entities data
        /// </returns>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Factory Method, TODO: dispose later")]
        private DataTable CreateTable(Type entityType, string tableName)
        {
            var table = new DataTable(tableName);
            table.Locale = CultureInfo.InvariantCulture;

            MetaType type = this.GetMetaType(entityType);

            foreach (MetaDataMember member in type.DataMembers)
            {
                Type columnType = GetColumnStorageType(member.Type);
                if (columnType.IsGenericType && columnType.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    columnType = columnType.GetGenericArguments()[0];
                }

                table.Columns.Add(member.MappedName, columnType);

                if (member.IsDbGenerated)
                {
                    if (member.Type == typeof(int))
                    {
                        table.Columns[member.MappedName].AutoIncrementSeed = 1;
                        table.Columns[member.MappedName].AutoIncrement = true;
                    }
                }
            }

            return table;
        }

        /// <summary>
        /// Deletes the entity
        /// </summary>
        /// <param name="entityType">
        /// The entity type.
        /// </param>
        /// <param name="entity">
        /// The entity.
        /// </param>
        private void Delete(Type entityType, object entity)
        {
            this.OnDeleting(new MemoryRepositoryMethodEventArgs(entity));

            DataRow row = this.GetEntityRow(entityType, entity);

            this.DeleteAssociations(entityType, row);

            row.Delete();
            this.Data.AcceptChanges();

            this.OnDeleted(new MemoryRepositoryStatusEventArgs(null, entity));
        }

        /// <summary>
        /// Processes all associated data on "main" entity deletion
        /// </summary>
        /// <param name="entityType">
        /// The entity Type.
        /// </param>
        /// <param name="entityRow">
        /// Entity being deleted
        /// </param>
        private void DeleteAssociations(Type entityType, DataRow entityRow)
        {
            MetaType type = this.GetMetaType(entityType);

            // todo: IsMany or !IsForeignKey?
            List<MetaAssociation> associations = type.Associations.Where(a => a.IsMany).ToList();

            // order so that CASCADE go first
            foreach (var association in associations.OrderBy(a => a.DeleteRule))
            {
                if (!this.Data.Tables.Contains(association.OtherType.Type.FullName))
                {
                    // no associated data was inserted at all - nothing to do with this association
                    continue;
                }

                StringBuilder rowFilter = new StringBuilder();
                int keyIndex = 0;
                foreach (var keyMember in association.ThisKey)
                {
                    object thisValue = entityRow[keyMember.Name];
                    if (rowFilter.Length != 0)
                    {
                        rowFilter.Append(" AND ");
                    }

                    rowFilter.AppendFormat("{0} = {1}", association.OtherKey[keyIndex].MappedName, thisValue);

                    keyIndex++;
                }

                DataView associatedEntityData = this.Data.Tables[association.OtherType.Type.FullName].DefaultView;
                associatedEntityData.RowFilter = rowFilter.ToString();
                switch (association.DeleteRule != null ? association.DeleteRule.ToUpperInvariant() : null)
                {
                    case "CASCADE":
                        foreach (DataRowView row in associatedEntityData)
                        {
                            this.DeleteAssociations(association.OtherType.Type, row.Row);
                            row.Delete();
                        }

                        associatedEntityData.Table.AcceptChanges();
                        break;

                    default:
                        if (associatedEntityData.Count > 0)
                        {
                            throw new InvalidOperationException(String.Format(
                                CultureInfo.CurrentCulture, 
                                "An attempt was made to delete a record from {0}, but {1} items are present in {2} with {3}. This is (yet) invalid with {4} delete rule", 
                                association.OtherType.Name, 
                                associatedEntityData.Count, 
                                association.ThisMember.Name, 
                                rowFilter, 
                                association.DeleteRule));
                        }

                        break;
                }
            }
        }

        /// <summary>
        /// Fills the db-generated members.
        /// </summary>
        /// <typeparam name="T">
        /// Entity type
        /// </typeparam>
        /// <param name="row">
        /// The entity data row.
        /// </param>
        /// <param name="entity">
        /// The entity.
        /// </param>
        private void FillGeneratedMembers<T>(DataRow row, T entity)
        {
            MetaType type = this.GetMetaType<T>();

            object obj = entity;

            foreach (MetaDataMember member in type.DataMembers)
            {
                if (member.IsDbGenerated)
                {
                    bool valueGenerated = false;
                    if (member.Type == typeof(Guid))
                    {
                        // GUID generation should keep existing values
                        Guid existingValue = (Guid)member.StorageAccessor.GetBoxedValue(obj);

                        if (existingValue == Guid.Empty)
                        {
                            row[member.MappedName] = Guid.NewGuid();
                            valueGenerated = true;
                        }
                    }
                    else if (row.Table.Columns[member.MappedName].AutoIncrement)
                    {
                        valueGenerated = true;
                    }

                    if (valueGenerated)
                    {
                        if (member.AutoSync == AutoSync.Always || member.AutoSync == AutoSync.Default || member.AutoSync == AutoSync.OnInsert)
                        {
                            object value = DBNull.Value.Equals(row[member.MappedName]) ? null : row[member.MappedName];

                            // sync
                            member.StorageAccessor.SetBoxedValue(ref obj, value);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Finds the storage table for entity type.
        /// </summary>
        /// <typeparam name="T">
        /// Entity type
        /// </typeparam>
        /// <returns>
        /// Storage table for entity type.
        /// </returns>
        private DataTable FindTable<T>()
        {
            return this.FindTable(typeof(T));
        }

        /// <summary>
        /// Finds the storage table for entity type.
        /// </summary>
        /// <param name="entityType">
        /// The type of the entity
        /// </param>
        /// <returns>
        /// Storage table for entity type.
        /// </returns>
        private DataTable FindTable(Type entityType)
        {
            string tableName = entityType.FullName;

            DataTable table;

            lock (this.DataLockObject)
            {
                table = this.Data.Tables[tableName];

                if (table == null)
                {
                    table = this.CreateTable(entityType, tableName);

                    this.Data.Tables.Add(table);
                }
            }

            return table;
        }

        /// <summary>
        /// Gets the entity data row.
        /// </summary>
        /// <typeparam name="T">
        /// Entity type
        /// </typeparam>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <returns>
        /// Entity data row.
        /// </returns>
        private DataRow GetEntityRow<T>(T entity) where T : class
        {
            return this.GetEntityRow(typeof(T), entity);
        }

        /// <summary>
        /// Gets the entity data row.
        /// </summary>
        /// <param name="entityType">
        /// The entity Type.
        /// </param>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <returns>
        /// Entity data row.
        /// </returns>
        private DataRow GetEntityRow(Type entityType, object entity)
        {
            MetaType type = this.GetMetaType(entityType);
            DataTable entitiesTable = this.FindTable(entityType);

            // selecting row by primary key
            var query = entitiesTable.AsEnumerable();

            foreach (MetaDataMember key in type.IdentityMembers)
            {
                string keyName = key.MappedName;
                object keyValue = key.StorageAccessor.GetBoxedValue(entity);

                query = query.Where(r => r[keyName].Equals(keyValue));
            }

            DataRow row = query.SingleOrDefault();

            return row;
        }

        /// <summary>
        /// Gets the metatype for entity type.
        /// </summary>
        /// <typeparam name="T">
        /// Entity type
        /// </typeparam>
        /// <returns>
        /// Metatype for entity type.
        /// </returns>
        private MetaType GetMetaType<T>()
        {
            return this.GetMetaType(typeof(T));
        }

        /// <summary>
        /// Gets the metatype for entity type
        /// </summary>
        /// <param name="sourceType">
        /// Entity type
        /// </param>
        /// <returns>
        /// Metatype for entity type.
        /// </returns>
        private MetaType GetMetaType(Type sourceType)
        {
            MetaModel model = this.GetModel();
            MetaType type = model.GetMetaType(sourceType);
            return type;
        }

        /// <summary>
        /// Loads the associations for entities.
        /// </summary>
        /// <typeparam name="T">
        /// Entity type
        /// </typeparam>
        /// <param name="entities">
        /// The entities.
        /// </param>
        /// <param name="loadOptions">
        /// The load options.
        /// </param>
        [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Somebody pls refactor this")]
        private void LoadAssociationsForEach<T>(List<T> entities, LoadOptions loadOptions)
        {
            var typeTLoadOptions = loadOptions.LoadWithOptions
                .Where(lo => lo.Member.Parameters.Single().Type == typeof(T))
                .Select(lo => new { LoadOption = lo, Member = ((MemberExpression)lo.Member.Body).Member });

            foreach (var loadOption in typeTLoadOptions)
            {
                var thisSideAssociationProperty = loadOption.Member as PropertyInfo;
                if (thisSideAssociationProperty == null)
                {
                    throw new NotSupportedException("Only properties are supported");
                }

                MetaType type = this.GetMetaType<T>();
                MetaAssociation association = type.Associations.Where(assoc => assoc.ThisMember.Name == thisSideAssociationProperty.Name).SingleOrDefault();

                if (association == null)
                {
                    throw new InvalidOperationException(String.Format(CultureInfo.InvariantCulture, "Association mapping for property {0} of class {1} is not defined.", thisSideAssociationProperty.Name, typeof(T).FullName));
                }

                // find all possible associated entities                        
                Type associatedType = thisSideAssociationProperty.PropertyType;

                if (!association.IsForeignKey)
                {
                    // there is a collection on this side, get collection item type
                    Type listInterface = associatedType.GetInterfaces().Where(i => (i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IList<>))).SingleOrDefault();

                    if (listInterface == null)
                    {
                        throw new InvalidOperationException("Non-FK association property type must implement IList<T> interface");
                    }

                    associatedType = listInterface.GetGenericArguments().Single();

                    // fill with empty lists
                    foreach (T entity in entities)
                    {
                        object localEntity = entity;
                        association.ThisMember.StorageAccessor.SetBoxedValue(ref localEntity, Activator.CreateInstance(association.ThisMember.Type));
                    }
                }

                // first list to join
                Expression allTsConst = Expression.Constant(entities.AsQueryable());

                // second list to join
                var allAssociatedEntitiesQuery = this.All(associatedType, loadOptions);

                // appying Filter expressions if present
                if (loadOption.LoadOption.Association != null)
                {
                    var allAssociatedEntitiesFilteredEnumerableExpression = new ExpressionReplacer(loadOption.LoadOption.Member.Body, allAssociatedEntitiesQuery.Expression).Visit(loadOption.LoadOption.Association.Body);

                    var asQueryableGenericMethod = TypeSystem.FindQueryableMethod("AsQueryable", new[] { typeof(IEnumerable<>).MakeGenericType(associatedType) }, new[] { associatedType });

                    var allAssociatedEntitiesFilteredQueryableExpression = Expression.Call(asQueryableGenericMethod, allAssociatedEntitiesFilteredEnumerableExpression);

                    allAssociatedEntitiesQuery = allAssociatedEntitiesQuery.Provider.CreateQuery(allAssociatedEntitiesFilteredQueryableExpression);
                }

                Expression allAssociatedEntitiesExpression = allAssociatedEntitiesQuery.Expression;

                // outerKeySelector
                var paramA1 = Expression.Parameter(typeof(T), "a");
                MemberExpression thisKeyPropertyExpression = paramA1.Property(association.ThisKey.First().Member as PropertyInfo);

                // innerKeySelector
                var paramB1 = Expression.Parameter(associatedType, "b");
                MemberExpression otherKeyPropertyExpression = paramB1.Property(association.OtherKey.First().Member as PropertyInfo);

                if (thisKeyPropertyExpression.Type.IsGenericType && thisKeyPropertyExpression.Type.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    var nullableFilterParamA1 = Expression.Parameter(typeof(T), "a");

                    MemberExpression nullableFilterPropertyExpression =
                        nullableFilterParamA1
                            .Property(association.ThisKey.First().Member as PropertyInfo)
                            .Property("HasValue");

                    LambdaExpression whereHasValueFilter = nullableFilterParamA1.ToLambda(nullableFilterPropertyExpression);

                    allTsConst = allTsConst.Where(whereHasValueFilter);

                    thisKeyPropertyExpression = thisKeyPropertyExpression.Property("Value");
                }

                if (otherKeyPropertyExpression.Type.IsGenericType && otherKeyPropertyExpression.Type.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    var nullableFilterParamB1 = Expression.Parameter(associatedType, "b");

                    MemberExpression nullableFilterPropertyExpression =
                        nullableFilterParamB1
                            .Property(association.OtherKey.First().Member as PropertyInfo)
                            .Property("HasValue");

                    LambdaExpression whereHasValueFilter = nullableFilterParamB1.ToLambda(nullableFilterPropertyExpression);

                    allAssociatedEntitiesExpression = allAssociatedEntitiesExpression.Where(whereHasValueFilter);

                    otherKeyPropertyExpression = otherKeyPropertyExpression.Property("Value");
                }

                var outerKeySelector = Expression.Quote(paramA1.ToLambda(thisKeyPropertyExpression));
                var innerKeySelector = Expression.Quote(paramB1.ToLambda(otherKeyPropertyExpression));

                var keyValueType = typeof(KeyValuePair<,>).MakeGenericType(typeof(T), associatedType);

                // resultSelector
                var paramA2 = Expression.Parameter(typeof(T), "a");
                var paramB2 = Expression.Parameter(associatedType, "b");
                var resultSelector = Expression.Quote(
                    Expression.Lambda(
                        Expression.New(
                            keyValueType.GetConstructors().Single(), 
                            paramA2, 
                            paramB2), 
                        paramA2, 
                        paramB2));

                var joinMethod = typeof(Queryable).GetMethods().Where(m => m.Name == "Join" && m.GetParameters().Count() == 5).Single();

                var joinMethodGeneric = joinMethod.MakeGenericMethod(typeof(T), associatedType, thisKeyPropertyExpression.Type, keyValueType);

                // join!
                var joinExpression = Expression.Call(null, joinMethodGeneric, allTsConst, allAssociatedEntitiesExpression, outerKeySelector, innerKeySelector, resultSelector);

                var joinResult = allAssociatedEntitiesQuery.Provider.Execute(joinExpression) as IEnumerable;

                var keyProperty = keyValueType.GetProperty("Key");
                var valueProperty = keyValueType.GetProperty("Value");

                foreach (var pair in joinResult)
                {
                    var thisObject = keyProperty.GetValue(pair, new object[0]);
                    var otherObject = valueProperty.GetValue(pair, new object[0]);

                    if (association.IsForeignKey)
                    {
                        association.ThisMember.StorageAccessor.SetBoxedValue(
                            ref thisObject, 
                            otherObject);
                    }
                    else
                    {
                        var list = association.ThisMember.StorageAccessor.GetBoxedValue(thisObject);
                        list.GetType().GetMethod("Add").Invoke(list, new[] { otherObject });
                    }
                }
            }
        }

        /// <summary>
        /// Maps the object to entity data row.
        /// </summary>
        /// <typeparam name="T">
        /// Entity type
        /// </typeparam>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <param name="row">
        /// The entity data row.
        /// </param>
        private void MapObjectToRow<T>(object entity, DataRow row)
        {
            MetaType type = this.GetMetaType<T>();

            foreach (MetaDataMember member in type.DataMembers)
            {
                if (!member.IsDbGenerated || DBNull.Value.Equals(row[member.MappedName]))
                {
                    row[member.MappedName] = ConvertToDbValue(member.StorageAccessor.GetBoxedValue(entity));
                }
            }
        }

        /// <summary>
        /// Maps the entity data row row to object.
        /// </summary>
        /// <typeparam name="T">
        /// Entity type
        /// </typeparam>
        /// <param name="row">
        /// The entity data row.
        /// </param>
        /// <returns>
        /// Entity object.
        /// </returns>
        private T MapRowToObject<T>(DataRow row)
        {
            MetaType type = this.GetMetaType<T>();

            object obj = Activator.CreateInstance<T>();

            foreach (MetaDataMember member in type.DataMembers.Where(m => !m.IsAssociation))
            {
                object value = row[member.MappedName];
                object convertedValue = ConvertFromDbValue(value, member.Type);
                member.StorageAccessor.SetBoxedValue(ref obj, convertedValue);
            }

            return (T)obj;
        }

        /// <summary>
        /// Raises the <see cref="E:Deleted"/> event.
        /// </summary>
        /// <param name="e">
        /// The <see cref="LogicSoftware.DataAccess.Repository.Memory.MemoryRepositoryStatusEventArgs"/> instance containing the event data.
        /// </param>
        private void OnDeleted(MemoryRepositoryStatusEventArgs e)
        {
            if (this.Deleted != null)
            {
                this.Deleted(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:Deleting"/> event.
        /// </summary>
        /// <param name="e">
        /// The <see cref="LogicSoftware.DataAccess.Repository.Memory.MemoryRepositoryMethodEventArgs"/> instance containing the event data.
        /// </param>
        private void OnDeleting(MemoryRepositoryMethodEventArgs e)
        {
            if (this.Deleting != null)
            {
                this.Deleting(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:Inserted"/> event.
        /// </summary>
        /// <param name="e">
        /// The <see cref="LogicSoftware.DataAccess.Repository.Memory.MemoryRepositoryStatusEventArgs"/> instance containing the event data.
        /// </param>
        private void OnInserted(MemoryRepositoryStatusEventArgs e)
        {
            if (this.Inserted != null)
            {
                this.Inserted(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:Inserting"/> event.
        /// </summary>
        /// <param name="e">
        /// The <see cref="LogicSoftware.DataAccess.Repository.Memory.MemoryRepositoryMethodEventArgs"/> instance containing the event data.
        /// </param>
        private void OnInserting(MemoryRepositoryMethodEventArgs e)
        {
            if (this.Inserting != null)
            {
                this.Inserting(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:Selected"/> event.
        /// </summary>
        /// <param name="e">
        /// The <see cref="LogicSoftware.DataAccess.Repository.Memory.MemoryRepositoryStatusEventArgs"/> instance containing the event data.
        /// </param>
        private void OnSelected(MemoryRepositoryStatusEventArgs e)
        {
            if (this.Selected != null)
            {
                this.Selected(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:Selecting"/> event.
        /// </summary>
        /// <param name="e">
        /// The <see cref="LogicSoftware.DataAccess.Repository.Memory.MemoryRepositorySelectingEventArgs"/> instance containing the event data.
        /// </param>
        private void OnSelecting(MemoryRepositorySelectingEventArgs e)
        {
            if (this.Selecting != null)
            {
                this.Selecting(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:Updated"/> event.
        /// </summary>
        /// <param name="e">
        /// The <see cref="LogicSoftware.DataAccess.Repository.Memory.MemoryRepositoryStatusEventArgs"/> instance containing the event data.
        /// </param>
        private void OnUpdated(MemoryRepositoryStatusEventArgs e)
        {
            if (this.Updated != null)
            {
                this.Updated(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:Updating"/> event.
        /// </summary>
        /// <param name="e">
        /// The <see cref="LogicSoftware.DataAccess.Repository.Memory.MemoryRepositoryMethodEventArgs"/> instance containing the event data.
        /// </param>
        private void OnUpdating(MemoryRepositoryMethodEventArgs e)
        {
            if (this.Updating != null)
            {
                this.Updating(this, e);
            }
        }

        #endregion
    }
}