﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using WikiClientLibrary.Sites;

namespace WikiClientLibrary.Cargo.Linq
{

    internal class CargoQueryProvider : IQueryProvider
    {
        private int _PaginationSize = 10;
        private ICargoRecordConverter _RecordConverter = new CargoRecordConverter();

        public CargoQueryProvider(WikiSite wikiSite)
        {
            WikiSite = wikiSite ?? throw new ArgumentNullException(nameof(wikiSite));
        }

        public WikiSite WikiSite { get; }

        public int PaginationSize
        {
            get => _PaginationSize;
            set
            {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException(nameof(value));
                _PaginationSize = value;
            }
        }

        public ICargoRecordConverter RecordConverter
        {
            get => _RecordConverter;
            set => _RecordConverter = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <inheritdoc />
        public IQueryable CreateQuery(Expression expression)
        {
            var queryableType = expression.Type.GetInterfaces()
                .First(i => i.IsConstructedGenericType && i.GetGenericTypeDefinition() == typeof(IQueryable<>));
            var elementType = queryableType.GenericTypeArguments[0];
            return (IQueryable)Activator.CreateInstance(typeof(CargoRecordQueryable<>).MakeGenericType(elementType), this, expression);
        }

        /// <inheritdoc />
        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            return new CargoRecordQueryable<TElement>(this, expression);
        }

        /// <inheritdoc />
        public object Execute(Expression expression)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public TResult Execute<TResult>(Expression expression)
        {
            throw new NotSupportedException();
        }

    }
}
