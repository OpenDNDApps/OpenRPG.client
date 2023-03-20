using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Assertions;

namespace VGDevs
{
    public static class DataServiceLocator
    {
        private static readonly Dictionary<Type, Type> m_serviceDefinitions = new Dictionary<Type, Type>(); 
        private static readonly Dictionary<Type, IDataService> m_serviceInstances = new Dictionary<Type, IDataService>(); 
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        public static void DefineServices()
        {
            Register<IPersistenceService, LocalPersistenceService>();
        }
        
        private static void Register<TInterface, TInstance>(bool immediateInit = false) where TInterface : IDataService where TInstance : class, TInterface
        {
            Type interfaceType = typeof(TInterface);
            Type instanceClass = typeof(TInstance);
            
            Assert.IsFalse(m_serviceDefinitions.ContainsKey(interfaceType), $"Service {interfaceType} is already registered");
            m_serviceDefinitions.Add(interfaceType, instanceClass);

            if (immediateInit)
            {
                InitializeService<TInterface>();
            }
        }
        
        public static void Unregister<T>() where T : IDataService 
        { 
            Type type = typeof(T);
            Assert.IsFalse(!m_serviceDefinitions.ContainsKey(type), $"Service {type} is not registered"); 
            m_serviceDefinitions.Remove(type); 
        }
        
        public static T Get<T>() where T : IDataService
        { 
            Type type = typeof(T);
            return (T) Get(type);
        }
        
        private static IDataService Get(Type type)
        {
            if (m_serviceInstances.ContainsKey(type))
            {
                return m_serviceInstances[type];
            }

            return InitializeService(type);
        }

        private static T InitializeService<T>() where T : IDataService
        {
            Type type = typeof(T);
            return (T)InitializeService(type);
        }

        private static IDataService InitializeService(Type serviceType)
        {
            if (!m_serviceDefinitions.ContainsKey(serviceType))
            {
                throw new Exception($"Service {serviceType} not found"); 
            }
            
            Type concreteType = m_serviceDefinitions[serviceType];

            IDataService newInstance = CreateInstance(concreteType);
            m_serviceInstances.Add(serviceType, newInstance);
            
            newInstance.Initialize();

            return newInstance;
        }

        private static bool AreParametersValid(ConstructorInfo constructorInfo)
        {
            return constructorInfo.GetParameters().All(p => typeof(IDataService).IsAssignableFrom(p.ParameterType));
        }

        private static IDataService CreateInstance(Type concreteType)
        {
            ConstructorInfo[] constructors = concreteType.GetConstructors();

            if (constructors.Length == 0 || constructors[0].GetParameters().Length == 0)
            {
                return (IDataService)Activator.CreateInstance(concreteType);
            }

            if (constructors.Length > 1 || !AreParametersValid(constructors[0]))
            {
                throw new Exception($"The Service {concreteType} can't be created. It should define only one constructor with IGameService parameters or an empty constructor.");
            }

            ParameterInfo[] constructorParameters = constructors[0].GetParameters();
            object[] instanceParameters = new object[constructorParameters.Length];

            for (int i = 0; i < constructorParameters.Length; i++)
            {
                instanceParameters[i] = Get(constructorParameters[i].ParameterType);
            }

            return (IDataService) Activator.CreateInstance(concreteType, instanceParameters);
        }
    }

    public interface IDataService
    { 
        void Initialize();
    }
}