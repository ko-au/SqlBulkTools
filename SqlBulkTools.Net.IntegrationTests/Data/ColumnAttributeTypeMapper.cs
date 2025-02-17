﻿using Dapper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;

namespace SqlBulkTools.Net.IntegrationTests.Data
{
    /// <summary>
    /// Uses the Name value of the ColumnAttribute specified, otherwise maps as usual.
    /// </summary>
    /// <typeparam name="T">The type of the object that this mapper applies to.</typeparam>
    public class ColumnAttributeTypeMapper<T> : FallbackTypeMapper
    {
        public ColumnAttributeTypeMapper()
            : base(new SqlMapper.ITypeMap[]
                  {
                  new CustomPropertyTypeMap(
                     typeof(T),
                     (type, columnName) =>
                         type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).FirstOrDefault(prop =>
                             prop.GetCustomAttributes(false)
                                 .OfType<ColumnAttribute>()
                                 .Any(attr => attr.Name == columnName)
                             )
                     ),
                  new DefaultTypeMap(typeof(T))
                  })
        {
        }
    }

    public class FallbackTypeMapper : SqlMapper.ITypeMap
    {
        private readonly IEnumerable<SqlMapper.ITypeMap> _mappers;

        public FallbackTypeMapper(IEnumerable<SqlMapper.ITypeMap> mappers)
        {
            _mappers = mappers;
        }

        public ConstructorInfo FindConstructor(string[] names, Type[] types)
        {
            foreach (var mapper in _mappers)
            {
                try
                {
                    ConstructorInfo result = mapper.FindConstructor(names, types);
                    if (result != null)
                    {
                        return result;
                    }
                }
                catch
                {
                }
            }
            return null;
        }

        public ConstructorInfo FindExplicitConstructor()
        {
            return _mappers.Select(m => m.FindExplicitConstructor()).FirstOrDefault(result => result != null);
        }

        public SqlMapper.IMemberMap GetConstructorParameter(ConstructorInfo constructor, string columnName)
        {
            foreach (var mapper in _mappers)
            {
                try
                {
                    var result = mapper.GetConstructorParameter(constructor, columnName);
                    if (result != null)
                    {
                        return result;
                    }
                }
                catch
                {
                }
            }
            return null;
        }

        public SqlMapper.IMemberMap GetMember(string columnName)
        {
            foreach (var mapper in _mappers)
            {
                try
                {
                    var result = mapper.GetMember(columnName);
                    if (result != null)
                    {
                        return result;
                    }
                }
                catch
                {
                }
            }
            return null;
        }
    }
}
