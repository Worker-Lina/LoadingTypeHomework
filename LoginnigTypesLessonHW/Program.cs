using LoginnigTypesLessonHW.Data;
using LoginnigTypesLessonHW.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace LoginnigTypesLessonHW
{
    class Program
    {
        static void Main(string[] args)
        {
            int choose = new int();

            var actionsOnFilesList = Enum.GetNames(typeof(Actions));
            var fileTypesList = Enum.GetNames(typeof(FileTypes));
            var userActionsList = new List<string>()
            {
                "Выбрать файл", "Выбрать действие над файлом", "Выполнить действие"
            };

            using (CloudContext context = new CloudContext())
            {
                if (context.CloudFiles.ToList().Count == 0)
                {
                    CloudFile rootFile = new CloudFile()
                    {
                        IsFolder = true,
                        Parent = Guid.Empty,
                        FileName = "Root"
                    };
                    context.Add(rootFile);
                    context.SaveChanges();
                }
                var root = context.CloudFiles.Where(file => file.Parent == Guid.Empty && file.FileName == "Root").ToList().First();
                var currentFile = root;
                CloudFile selectedFile = null;
                int selectedAction = new int();

                do
                {
                    Console.Clear();
                    Console.WriteLine("----ОБЛАЧНОЕ ХРАНИЛИЩЕ----\n");

                    ShowAllElements(userActionsList);

                    try { choose = ChooseIndex(userActionsList.Count); }
                    catch (ArgumentOutOfRangeException exception) { Console.WriteLine(exception.ParamName); continue; }

                    switch (choose)
                    {
                        case (int)UserActions.SelectFile:

                            var files = context.CloudFiles.Where(file => file.Parent == currentFile.Id).ToList();
                            int iterator = new int();
                            foreach (var file in files)
                            {
                                Console.WriteLine($"{++iterator}. {GetFileName(file.FileName)}");
                            }

                            try { choose = ChooseIndex(files.Count); }
                            catch (ArgumentOutOfRangeException exception) { Console.WriteLine(exception.ParamName); continue; }

                            selectedFile = files[choose - 1];

                            break;
                        case (int)UserActions.SelectActionOnFile:

                            ShowAllElements(actionsOnFilesList);

                            try { choose = ChooseIndex(actionsOnFilesList.Length); }
                            catch (ArgumentOutOfRangeException exception) { Console.WriteLine(exception.ParamName); continue; }

                            selectedAction = choose;

                            break;
                        case (int)UserActions.ExecuteAction:


                            switch ((Actions)selectedAction)
                            {
                                case Actions.Enter:
                                    if (!selectedFile.IsFolder)
                                        Console.WriteLine("Это Файл - вхождение не удалось!");
                                    currentFile = selectedFile;
                                    break;

                                case Actions.Exit:

                                    if (currentFile.Id != Guid.Empty)
                                    {
                                        var papentFile = context.CloudFiles.Where(file => file.Id == currentFile.Parent).ToList()[0];
                                        currentFile = papentFile;
                                    }

                                    break;

                                case Actions.Add:

                                    ShowAllElements(fileTypesList);

                                    try { choose = ChooseIndex(fileTypesList.Length); }
                                    catch (ArgumentOutOfRangeException exception) { Console.WriteLine(exception.ParamName); continue; }

                                    bool IsFolder = true;
                                    if (choose == (int)FileTypes.File)
                                        IsFolder = false;


                                    CloudFile NewFile = new CloudFile();

                                    if (IsFolder)
                                    {

                                        Console.Write("Введите имя папки: ");
                                        NewFile.FileName = Console.ReadLine();
                                        NewFile.Parent = currentFile.Id;
                                        NewFile.IsFolder = true;

                                    }
                                    else
                                    {
                                        Console.WriteLine("Введите путь файла на локальном компьютере: ");
                                        string path = Console.ReadLine();

                                        if (File.Exists(@path))
                                        {
                                            NewFile.FullPath = path;
                                            NewFile.Parent = currentFile.Id;
                                            NewFile.FileName = GetFileName(path);
                                            NewFile.IsFolder = false;
                                        }
                                        else
                                        {
                                            Console.WriteLine("Данный файл не найден!");
                                            break;
                                        }

                                    }
                                  
                                    int count = context.CloudFiles
                                        .Where(file => file.Parent == NewFile.Parent && file.FileName == NewFile.FileName)
                                        .ToList().Count;

                                    if (count == 0)
                                    {
                                        context.Add(NewFile);
                                        context.SaveChanges();
                                    }

                                    break;
                                case Actions.Delete:
                                    context.Remove(selectedFile);
                                    context.SaveChanges();
                                    break;
                            }

                            break;
                    }
                } while (true);
            }


        }

        public static int ChooseIndex(int collectionLength)
        {
            Console.Write("\nВыберите индекс: ");
            int.TryParse(Console.ReadLine(), out int choose);
            if (!IsRightIndex(choose, collectionLength))
            {
                throw new ArgumentOutOfRangeException("Вы выбрали неправильный индекс!!!");
            }
            return choose;
        }

        public static string GetFileName(string fullname)
        {
            var list = fullname.Split(@"\");
            return list.Last();
        }


        public static bool IsFolder(string path)
        {
            if (File.Exists(path))
                return false;
            else if (Directory.Exists(path))
                return true;
            throw new Exception();
        }

        public static void ShowAllElements(ICollection<string> elements)
        {
            int iterator = new int();
            foreach (var element in elements)
            {
                Console.WriteLine($"{++iterator}. {element}");
            }
        }

        public static bool IsRightIndex(int index, int length)
        { 
            if (index == 0 || index > length)
                return false;
            return true;
        }


    }
}
