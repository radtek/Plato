﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using PlatoCore.Layout.Views.Abstractions;

namespace PlatoCore.Layout.Views
{

    public interface IViewTable
    {

        ViewDescriptor Add(ViewDescriptor view);

        IReadOnlyDictionary<Type, IList<ViewDescriptor>> Views { get; }

    }

    public class ViewTable : IViewTable
    {

        private readonly ConcurrentDictionary<Type, IList<ViewDescriptor>> _views;

        public IReadOnlyDictionary<Type, IList<ViewDescriptor>> Views => _views;

        public ViewTable()
        {
            _views = new ConcurrentDictionary<Type, IList<ViewDescriptor>>();
        }

        public ViewDescriptor Add(ViewDescriptor descriptor)
        {

            if (descriptor.View == null)
            {
                return descriptor;
            }

            if (descriptor.View.Model == null)
            {
                return descriptor;
            }

            _views.AddOrUpdate(descriptor.View.Model.GetType(), new List<ViewDescriptor>()
            {
                descriptor
            }, (k, v) =>
            {
                v.Add(descriptor);
                return v;
            });

            return descriptor;
        }
       
    }

    public static class ViewTableManagerExtensions
    {
        public static T FirstModelOfType<T>(this IViewTable viewTableManager) where T : class
        {

            if (viewTableManager.Views.ContainsKey(typeof(T)))
            {
                if (viewTableManager.Views[typeof(T)][0].View.Model is T)
                {
                    return (T)viewTableManager.Views[typeof(T)][0].View.Model;
                }
            }

            return null;

        }

    }

}