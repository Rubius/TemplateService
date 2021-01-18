using System;
using System.Linq;
using System.Reflection;

namespace Common.Utils
{
    /// <summary>
    /// Читает переменные окружения.
    /// </summary>
    public static class EnvironmentVariableReader
    {
        private static readonly string _separator = "_";

        /// <summary>
        /// Заполняет публичные строковые свойства (на любом уровне вложенности) объекта obj из переменных окружения.
        /// Если вложенные свойства не инициализированы, то для них будет вызван дефолтный конструктор.
        /// Пример именования переменной окружения: ServiceName_UserConnection_Auth_Login, где:
        /// ServiceName - префикс, UserConnection/Auth/Login - название вложенных полей объекта obj.
        /// </summary>
        /// <param name="obj">Объект, строковые свойства которого могут задаваться через переменные окружения</param>
        /// <param name="envPrefixes">Префиксы, которые будут добавлены при поиске имен переменных окружения</param>
        public static void SetProperies<T>(T obj, params string[] envPrefixes) where T : class, new()
        {
            if (obj == null)
                throw new NullReferenceException("Parameter: obj can`t be null.");

            SetProperiesFromEnvironment(obj, Join(envPrefixes));
        }

        
        // Проходит дерево свойств объекта obj вглубь и заполняет строковые свойства для вложенных объектов.
        private static void SetProperiesFromEnvironment<T>(T obj, string prefix) where T : class, new()
        {
            SetStringProperiesFromEnvironment(obj, prefix);

            foreach (var properpty in typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public)
                // Проверка того что свойство имеет тип не относящийся к System types.
                .Where(p => !p.PropertyType.Namespace.StartsWith("System")))
            {
                var newPrefix = Join(prefix, properpty.Name);
                var innerObject = properpty.GetValue(obj);

                // Если вложенное поле не инициализировано, то вызываем для него простой констрктор.
                if (innerObject == null)
                {
                    innerObject = Activator.CreateInstance(properpty.PropertyType);
                    properpty.SetValue(obj, innerObject);
                }

                SetProperiesFromEnvironment(innerObject, newPrefix);
            }
        }

        // Заполняет внутренние строковые свойства объекта значениями из переменных окружения, если они были найдены.
        private static void SetStringProperiesFromEnvironment<T>(T obj, string prefix) where T : class, new()
        {
            var stringProperties = obj.GetType().GetProperties()
                .Where(p => p.PropertyType == typeof(string)).ToList();

            foreach (var properpty in stringProperties)
            {
                var newPrefix = Join(prefix, properpty.Name);
                string value = Environment.GetEnvironmentVariable(newPrefix);

                if (value != null)
                    properpty.SetValue(obj, value);
            }
        }

        private static string Join(params string[] prefixes)
        {
            return string.Join(_separator, prefixes.Where(p => !string.IsNullOrWhiteSpace(p)));
        }
    }
}
