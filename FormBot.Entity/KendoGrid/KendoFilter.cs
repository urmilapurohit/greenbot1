using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Reflection;
using System.Runtime.Serialization;
using FormBot.Helper.Helper;

namespace FormBot.Entity.KendoGrid
{
    [DataContract]
    public class KendoFilter
    {
        public static readonly IDictionary<string, string> operators = new Dictionary<string, string>
        {
            {
                "eq",
                "="
            },
            {
                "neq",
                "!="
            },
            {
                "lt",
                "<"
            },
            {
                "lte",
                "<="
            },
            {
                "gt",
                ">"
            },
            {
                "gte",
                ">="
            },
            {
                "startswith",
                "StartsWith"
            },
            {
                "endswith",
                "EndsWith"
            },
            {
                "contains",
                "Contains"
            },
            {
                "doesnotcontain",
                "Contains"
            },
            {
                "inarray",
                "inarray"
            }

        };

        [DataMember(Name = "field")]
        public string Field
        {
            get;
            set;
        }

        [DataMember(Name = "operator")]
        public string Operator
        {
            get;
            set;
        }

        [DataMember(Name = "value")]
        public object Value
        {
            get;
            set;
        }

        [DataMember(Name = "logic")]
        public string Logic
        {
            get;
            set;
        }

        [DataMember(Name = "filters")]
        public IEnumerable<KendoFilter> Filters
        {
            get;
            set;
        }

        public IList<KendoFilter> All()
        {
            List<KendoFilter> list = new List<KendoFilter>();
            Collect(list);
            return list;
        }

        private void Collect(IList<KendoFilter> filters)
        {
            if (Filters != null && Filters.Any())
            {
                foreach (KendoFilter filter in Filters)
                {
                    filters.Add(filter);
                    filter.Collect(filters);
                }
            }
            else
            {
                filters.Add(this);
            }
        }

        public string ToExpression(IList<KendoFilter> filters)
        {
            if (Filters != null && Filters.Any())
            {
                return "(" + string.Join(" " + Logic + " ", Filters.Select((KendoFilter filter) => filter.ToExpression(filters)).ToArray()) + ")";
            }

            int num = filters.IndexOf(this);
            string text = operators[Operator];
            if (Operator == "doesnotcontain")
            {
                return $"!{Field}.{text}(@{num})";
            }

            switch (text)
            {
                case "StartsWith":
                case "EndsWith":
                case "Contains":
                    return $"{Field}.{text}(@{num})";
                default:
                    return $"{Field} {text} @{num}";
            }
        }
    }

    [DataContract]
    public class KendoSort
    {
        [DataMember(Name = "field")]
        public string Field
        {
            get;
            set;
        }

        [DataMember(Name = "dir")]
        public string Dir
        {
            get;
            set;
        }

        public string ToExpression()
        {
            return Field + " " + Dir;
        }
    }

    public enum KendoFilterLogic
    {
        and = 1,
        or = 2
    }

    public enum KendoFilterOperator
    {
        eq, neq, lt, lte, gt, gte, startswith, endswith, contains, doesnotcontain, inarray
    }

    [KnownType("GetKnownTypes")]
    public class DataSourceResult
    {
        public IEnumerable Data
        {
            get;
            set;
        }

        public int Total
        {
            get;
            set;
        }

        public object Aggregates
        {
            get;
            set;
        }

        private static Type[] GetKnownTypes()
        {
            Assembly assembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault((Assembly a) => a.FullName.StartsWith("DynamicClasses"));
            if (assembly == null)
            {
                return new Type[0];
            }

            return (from t in assembly.GetTypes()
                    where t.Name.StartsWith("DynamicClass")
                    select t).ToArray();
        }
    }

    public static class QueryableExtensions
    {
        /// <summary>
        /// Applies data processing (paging, sorting and filtering) over IQueryable using Dynamic Linq.
        /// </summary>
        /// <typeparam name="T">The type of the IQueryable</typeparam>
        /// <param name="queryable">The IQueryable which should be processed.</param>
        /// <param name="take">Specifies how many items to take. Configurable via the pageSize setting of the Kendo DataSource.</param>
        /// <param name="skip">Specifies how many items to skip.</param>
        /// <param name="sort">Specifies the current sort order.</param>
        /// <param name="filter">Specifies the current filter.</param>
        /// <returns>A DataSourceResult object populated from the processed IQueryable.</returns>
        public static DataSourceResult ToDataSourceResult<T>(this IQueryable<T> queryable, int take, int skip, IEnumerable<KendoSort> sort, KendoFilter filter)
        {
            // Filter the data first
            queryable = Filter(queryable, filter);

            // Calculate the total number of records (needed for paging)
            var total = queryable.Count();

            // Sort the data
            queryable = Sort(queryable, sort);

            // Finally page the data
            queryable = Page(queryable, take, skip);

            return new DataSourceResult
            {
                Data = queryable.ToList(),
                Total = total
            };
        }
        public static List<T> ToDataSourceResultSortFilter<T>(this IQueryable<T> queryable, IEnumerable<KendoSort> sort, KendoFilter filter) where T : class, new()
        {

            FilterResetValue<T>(ref filter);
            // Filter the data first
            queryable = Filter(queryable, filter);

            // Calculate the total number of records (needed for paging)
            //total = queryable.Count();

            //if (!string.IsNullOrEmpty(searchQuery))
            //    queryable = queryable.Where(searchQuery);
            // Sort the datah
            queryable = Sort(queryable, sort);

            // Finally page the data
            //queryable = Page(queryable, take, skip);

            return queryable.ToList<T>();
        }


        public static void FilterResetValue<T>(ref KendoFilter filter) where T : class, new()
        {
            if (filter == null)
                return;

            T obj = new T();
            foreach (var item in filter.Filters)
            {
                if (item.Filters == null)
                {
                    PropertyInfo propertyInfo = obj.GetType().GetProperty(item.Field);
                    item.Value = Common.ResetValue(item.Value, propertyInfo.PropertyType);
                }
                else
                {
                    foreach (var subItem in item.Filters)
                    {
                        if (subItem.Filters == null)
                        {
                            PropertyInfo propertyInfo = obj.GetType().GetProperty(subItem.Field);
                            subItem.Value = Common.ResetValue(subItem.Value, propertyInfo.PropertyType);
                        }
                    }
                }
            }
        }

        public static object ResetValueCheck<T>(string field, object value) where T : class, new()
        {
            T obj = new T();
            PropertyInfo propertyInfo = obj.GetType().GetProperty(field);
            return Common.ResetValue(value, propertyInfo.PropertyType);
        }

        private static IQueryable<T> Filter<T>(IQueryable<T> queryable, KendoFilter filter)
        {

            if (filter != null && filter.Logic != null)
            {
                // Collect a flat list of all filters
                var filters = filter.All();

                // Get all filter values as array (needed by the Where method of Dynamic Linq)
                var values = filters.Select(f => f.Value).ToArray();

                // Create a predicate expression e.g. Field1 = @0 And Field2 > @1
                string predicate = filter.ToFilterExpression(filters);

                // Use the Where method of Dynamic Linq to filter the data
                queryable = queryable.Where(predicate, values);
            }
            return queryable;
        }

        private static IQueryable<T> Sort<T>(IQueryable<T> queryable, IEnumerable<KendoSort> sort)
        {
            if (sort != null && sort.Any())
            {
                // Create ordering expression e.g. Field1 asc, Field2 desc
                var ordering = String.Join(",", sort.Select(s => s.ToExpression()));

                // Use the OrderBy method of Dynamic Linq to sort the data
                return queryable.OrderBy(ordering);
            }

            return queryable;
        }

        private static IQueryable<T> Page<T>(IQueryable<T> queryable, int take, int skip)
        {
            return queryable.Skip(skip).Take(take);
        }

        public static IEnumerable<T> AddItem<T>(this IEnumerable<T> e, T value)
        {
            if (e != null)
            {
                foreach (var cur in e)
                {
                    yield return cur;
                }
            }
            yield return value;
        }

        public static string ToFilterExpression(this KendoFilter _filters, IList<KendoFilter> filterList)
        {
            if (_filters.Filters != null && _filters.Filters.Any())
            {
                return "(" + string.Join(" " + _filters.Logic + " ", _filters.Filters.Select((KendoFilter filter) => filter.ToFilterExpression(filterList)).ToArray()) + ")";
            }

            int num = filterList.IndexOf(_filters);
            string text = KendoFilter.operators[_filters.Operator];
            if (_filters.Operator == "doesnotcontain")
            {
                return $"!{_filters.Field}.{text}(@{num})";
            }

            if (_filters.Operator == "inarray")
            {
                return $"{_filters.Field} != null && @{num}.Contains({_filters.Field})";
            }

            switch (text)
            {
                case "StartsWith":
                case "EndsWith":
                case "Contains":
                    return $"{_filters.Field} != null && {_filters.Field}.ToLower().{text}(@{num}.ToLower().Trim())";
                default:
                    return $"{_filters.Field} {text} @{num}";
            }
        }
        public static bool CheckFilterContains(this KendoFilter filter, string[] FieldsForInstallerDesignerSolarCompany)
        {
            if (filter.Filters != null && filter.Filters.Any())
            {
                return filter.Filters.Any((KendoFilter _filter) => _filter.CheckFilterContains(FieldsForInstallerDesignerSolarCompany));
            }
            return FieldsForInstallerDesignerSolarCompany.Contains(filter.Field);
        }

        public static void AddFilterItem(this KendoFilter filter, string field, string operatorType, object value)
        {
            filter.Filters = filter.Filters.AddItem(new KendoFilter() { Field = field, Operator = operatorType, Value = value }).ToList();
        }
    }

}
