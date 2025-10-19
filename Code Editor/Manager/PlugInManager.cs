using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Examination_SP
{
    class PlugInItem
    {
        // Название плагина
        public String Name { get; set; }

        // Информация о классе плагина, полученная из Reflection
        public Type Type { get; set; }

        // Информация о методе плагина, полученная из Reflection, который нужен для активации плагина
        public MethodInfo Method { get; set; }
    }

    class PlugInManager
    {
        // Ссылка на основное окно программы, которое будет изменяться
        public Window MainWindow { get; }
        public PlugInManager(Window mainWindow)
        {
            MainWindow = mainWindow;
        }

        /// <summary>
        /// Метод проверят, авляется ли файл, заданный полным путём, плагином нашей системы
        /// </summary>
        /// <param name="fullName">Полный путь к файлу</param>
        /// <param name="plugInType">Выходной параметр, возвращает описание класса плагина</param>
        /// <returns>Возвращает описание метода активации плагина</returns>
        public MethodInfo CheckPlugIn(String fullName, out Type plugInType)
        {
            plugInType = null;

            // Попытка загрузки сборки (DLL) в программу
            Assembly assembly = null;
            try
            {
                // Удачная загрузка сборки, возможно это наш плагин
                assembly = Assembly.LoadFrom(fullName);
            }
            catch (Exception ex)
            {
                // Сборку загрузить не удалось, это не плагин
                return null;
            }

            // Сборку загрузить не удалось, это не плагин
            if (assembly == null)
                return null;

            // Попытка найти нужный класс для активации плагина (PlugIn)
            foreach (Type currentType in assembly.GetTypes())
            {
                if (currentType.Name == "PlugIn")
                {
                    plugInType = currentType;
                    break;
                }
            }

            // Если класс с именем PlugIn найти не удалось - это не наш плагин
            if (plugInType == null || !plugInType.IsClass)
                return null;

            // Попытка найти нужный метод для активации плагина (ChangeWindow)
            MethodInfo plugInMethod = null;
            foreach (MethodInfo currentMethod in plugInType.GetMethods())
            {
                if (currentMethod.Name == "ChangeWindow")
                {
                    plugInMethod = currentMethod;
                    break;
                }
            }

            // Если класс с именем ChangeWindow найти не удалось - это не наш плагин
            if (plugInMethod == null || !plugInMethod.IsPublic)
                return null;

            return plugInMethod;
        }

        // Словарь, содержащий список обнаруженных плагинов (ключ - название плагина, значение - информация о плагине)
        Dictionary<String, PlugInItem> plugIns = new Dictionary<string, PlugInItem>();

        /// <summary>
        /// Сканирует папку Plug-ins, создаёт БД плагинов и возвращает список обнаруженных плагинов
        /// </summary>
        /// <returns>Список обнаруженных плагинов</returns>
        public List<String> GetPlugInsNames()
        {
            // Результирующий список плагинов
            List<String> names = new List<string>();
            plugIns.Clear();

            // Получить полный путь к папке, с запускным файлом
            String baseDirectoryPath = System.AppDomain.CurrentDomain.BaseDirectory;

            // Получить объект DirectoryInfo для папки Plug-ins
            DirectoryInfo dinfo = new DirectoryInfo(baseDirectoryPath + @"Plug-ins");

            // Получить массив файлов в текущей папке
            FileInfo[] files = dinfo.GetFiles("*.dll");

            // Перебрать все обнаруженные файлы
            foreach (FileInfo currentFile in files)
            {
                // Проверить, является ли обнаруженный DLL-файл нашим плагином
                Type plugInType = null;
                MethodInfo plugInMethod = CheckPlugIn(currentFile.FullName, out plugInType);

                // Эсли это плагин
                if (plugInMethod != null)
                {
                    // Добавить его в БД плагинов и в список имён
                    names.Add(currentFile.Name);
                    plugIns.Add(currentFile.Name, new PlugInItem() { Name = currentFile.Name, Method = plugInMethod, Type = plugInType });
                }
            }

            // Вернуть список имён найденных плагинов
            return names;
        }

        /// <summary>
        /// Активация плагина по имени
        /// </summary>
        /// <param name="name">Имя плагина для активации</param>
        public void ActivatePlugIn(String name)
        {
            PlugInItem item = plugIns[name];

            // Если плагин с таким именем существует
            if (item != null)
            {
                // Запуск метода ChangeWindow в указанном плагине
                Object obj = Activator.CreateInstance(item.Type);
                object[] arg = new object[1];

                // Передать туда ссылку на основное окно
                arg[0] = MainWindow;

                // активация плагина (запуск целевого метода)
                item.Method.Invoke(obj, arg);
            }
        }
    }
}
